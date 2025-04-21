using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eticaret.Core.Entities;
using Eticaret.Data;
using Microsoft.AspNetCore.Authorization;

namespace Eticaret.WebUI.Areas.Admin.Controllers
{
    [Area("Admin"),Authorize(Policy ="AdminPolicy")]
    public class OrdersController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IEmailService _emailService;
        public OrdersController(DatabaseContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        private async Task SendOrderStateChangedMail(string toEmail, EnumOrderState state)
        {
            string subject = "Sipariş Durumunuz Güncellendi";
            string stateText = state switch
            {
                EnumOrderState.Waiting => "Siparişinizi aldık. En kısa sürede işleme alınacaktır.",
                EnumOrderState.Approved => "Siparişiniz onaylandı.",
                EnumOrderState.Shipped => "Siparişiniz kargoya verildi.",
                EnumOrderState.Completed => "Siparişiniz tamamlandı.",
                EnumOrderState.Cancelled => "Siparişiniz iptal edildi.",
                EnumOrderState.Returned => "Siparişiniz iade edildi.",
                _ => "Sipariş durumunuz güncellendi."
            };

            string body = $@"
        <p>Merhaba,</p>
        <p>{stateText}</p>
        <p>Sipariş durumunuzu hesabınızdan takip edebilirsiniz.</p>
        <p><a href='https://seninsiten.com/account/orders'>Siparişlerim</a></p>
        <br/>
        <p>Teşekkür ederiz.</p>";

            await _emailService.SendEmailAsync(toEmail, subject, body);
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.Include(x=>x.AppUser).ToListAsync());
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(x=>x.AppUser).Include(o=>o.OrderLines).
                ThenInclude(p=>p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Orders/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                // Sipariş durumu "Onay Bekliyor" olarak atanır
                order.OrderState = EnumOrderState.Waiting;

                // Siparişi veritabanına kaydet
                _context.Add(order);
                await _context.SaveChangesAsync();

                // Kullanıcıya "Onay Bekliyor" durumu ile ilgili mail gönder
                await SendOrderStateChangedMail(order.AppUser.Email, EnumOrderState.Waiting);

                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(x=>x.AppUser).FirstOrDefaultAsync(x=>x.Id==id);
           
            if (order == null)
            {
                return NotFound();
            }
            ViewBag.OrderStates = new SelectList(Enum.GetValues<EnumOrderState>());
            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingOrder = await _context.Orders
                        .Include(o => o.AppUser)
                        .FirstOrDefaultAsync(o => o.Id == id);

                    if (existingOrder == null)
                        return NotFound();

                    // Durum değişikliği kontrolü: Siparişin durumu değişmişse mail gönder
                    if (existingOrder.OrderState != order.OrderState)
                    {
                        // Kullanıcıya mail gönder
                        await SendOrderStateChangedMail(existingOrder.AppUser.Email, order.OrderState);
                    }

                    // Durumu güncelle
                    existingOrder.OrderState = order.OrderState;
                    _context.Update(existingOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Bir hata oluştu.");
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.OrderStates = new SelectList(Enum.GetValues<EnumOrderState>());
            return View(order);
        }


        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.
                Include(x=>x.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}

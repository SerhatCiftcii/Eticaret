using Eticaret.Core.Entities;
using Eticaret.Service.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Eticaret.WebUI.Controllers
{
    [Authorize] //üylere özel alan yada test aşamasında admin
    public class MyAddressesController : Controller
    {
        private readonly IService<AppUser> _serviceAppUser;
        private readonly IService<Address> _serviceAddress;
        public MyAddressesController(IService<AppUser> service, IService<Address> serviceAddress)
        {
            _serviceAppUser = service;
            _serviceAddress = serviceAddress;
        }
        public async Task<IActionResult> Index()
        {
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return NotFound("kullanıcı Datası Bulunamadı! Lütfen Tekrar Giriş Yapın!");
            }
            var model = await _serviceAddress.GetAllAsync(u => u.AppUserId == appUser.Id);
            return View(model);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Address address)
        {
            
                if (ModelState.IsValid)
                {
                    try
                {
                    var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
                    if (appUser != null)
                    {
                        address.AppUserId=appUser.Id;//bu adres bu appusera aittir
                        _serviceAddress.Add(address);
                        await _serviceAddress.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }
                    catch (Exception)
                {

                    ModelState.AddModelError("", "hata oluştu adres");
                }
                
                     }
            ModelState.AddModelError("", "Kayıt Başarısız!");
            return View();
            }
        public async Task<IActionResult> Edit(string id)
        {
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return NotFound("Kullanıcı Datası Bulunamadı! Lütfen Tekrar Giriş Yapın!");
            }

            var model= await _serviceAddress.GetAsync(u=>u.AddressGuid.ToString() == id);
            return View(model);
        }
        

        }
    }


﻿using Eticaret.Core.Entities;
using Eticaret.Data;
using Eticaret.Service.Abstract;
using Eticaret.WebUI.Models;
using Microsoft.AspNetCore.Authentication;//login
using Microsoft.AspNetCore.Authorization;//login
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
///login
using System.Threading.Tasks;

namespace Eticaret.WebUI.Controllers
{
    public class AccountController : Controller
    {

        private readonly IEmailService _emailService;
        private readonly IService<AppUser> _service;
        private readonly IService<Order> _serviceOrder;

        public AccountController(IEmailService emailService, IService<AppUser> service, IService<Order> serviceOrder)
        {
            _emailService = emailService;
            _service = service;
            _serviceOrder = serviceOrder;
        }


        [Authorize]
        // Index sayfası
        public async Task<IActionResult> Index()
        {
            AppUser user = await _service.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (user is null)
            {
                return NotFound();
            }
            var model = new UserEditViewModel()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Phone = user.Phone
            };
            return View(model);
        }


        [HttpPost, Authorize]
        public async Task<IActionResult> Index(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AppUser user = await _service.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
                    if (user is not null)
                    {
                        user.Name = model.Name;
                        user.Surname = model.Surname;
                        user.Email = model.Email;
                        user.Phone = model.Phone;
                        _service.Update(user);
                        await _service.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi!";
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Bir hata oluştu. Lütfen tekrar deneyin.");
                }
            }
            return View();
            // Giriş sayfası
        }
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            AppUser user = await _service.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (user is null)
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("SignIn");
            }
            var model = _serviceOrder.GetQueryable().Where(s => s.AppUserId == user.Id).Include(o=>o.OrderLines).ThenInclude(p=>p.Product);
            return View(model);
        }
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignInAsync(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kullanıcıyı e-posta ve şifre ile veritabanından buluyoruz
                    var account = await _service.GetAsync(x => x.Email == loginViewModel.Email && x.Password == loginViewModel.Password && x.IsActive);

                    if (account == null)
                    {
                        // Eğer kullanıcı bulunamazsa hata mesajı ekliyoruz
                        ModelState.AddModelError("", "Şifre veya Email hatalı");
                    }
                    else
                    {
                        // Kullanıcı başarıyla bulunduysa, claim'leri oluşturuyoruz
                        var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, account.Name),
                    new Claim(ClaimTypes.Role, account.IsAdmin ? "Admin" : "Customer"),
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim("UserId", account.Id.ToString()),
                    new Claim("UserGuid", account.UserGuid.ToString())
                };

                        var userIdentity = new ClaimsIdentity(claims, "Login");
                        ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);

                        // Kullanıcıyı oturum açtırıyoruz
                        await HttpContext.SignInAsync(userPrincipal);

                        // Eğer ReturnUrl varsa ona yönlendiriyoruz, yoksa anasayfaya yönlendiriyoruz
                        return Redirect(string.IsNullOrEmpty(loginViewModel.ReturnUrl) ? "/" : loginViewModel.ReturnUrl);
                    }
                }
                catch (Exception)
                {
                    // Kullanıcıya bir hata mesajı gösteriyoruz
                    ModelState.AddModelError("", "Bir hata oluştu. Lütfen tekrar deneyin.");
                }
            }

            // Giriş formu geçersizse veya hata varsa tekrar formu gösteriyoruz
            return View(loginViewModel);
        }




        // Kayıt sayfası (GET)
        public IActionResult SignUp()
        {
            return View();
        }

        // Kayıt işlemi (POST)
        [HttpPost]
        public async Task<IActionResult> SignUpAsync(AppUser appUser)
        {
            appUser.IsAdmin = false;

            // Model doğrulama kontrolü
            if (!ModelState.IsValid)
            {
                return View(appUser);
            }

            // Şifre doğrulama
            if (appUser.Password != appUser.PasswordConfirm)
            {
                ModelState.AddModelError("PasswordConfirm", "Şifreler eşleşmiyor!");
                return View(appUser);
            }
            var Email= await _service.GetAsync(x=> x.Email == appUser.Email);
            if (Email != null)
            {
                ModelState.AddModelError("Email", "Bu email adresi zaten kayıtlı!");
                return View(appUser);
            }

            // 6 haneli doğrulama kodu oluşturuluyor
            var random = new Random();
            appUser.VerificationCode = random.Next(100000, 999999).ToString();

            // Kullanıcıyı veritabanına ekliyoruz
            await _service.AddAsync(appUser);
            await _service.SaveChangesAsync();

            // Doğrulama kodunu e-posta ile gönderiyoruz
            await _emailService.SendEmailAsync(appUser.Email, "Doğrulama Kodu", $"Doğrulama kodunuz: {appUser.VerificationCode}");

            TempData["SuccessMessage"] = "Kayıt başarıyla tamamlandı! Lütfen e-posta adresinizdeki doğrulama kodunu girin.";

            // Kullanıcıyı doğrulama sayfasına yönlendiriyoruz
            return RedirectToAction("VerifyEmail");
        }

        // Doğrulama sayfası (GET)
        public IActionResult VerifyEmail()
        {
            return View();
        }

        // Doğrulama kodu kontrolü (POST)
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(string verificationCode)
        {
            // Kullanıcıyı doğrulama kodu ile buluyoruz
            var appUser = await _service.GetAsync(u => u.VerificationCode == verificationCode);

            // Geçersiz doğrulama kodu durumunda hata mesajı gösteriyoruz
            if (appUser == null)
            {
                ModelState.AddModelError("VerificationCode", "Geçersiz doğrulama kodu.");
                return View();
            }

            // Doğrulama kodu doğruysa kullanıcıyı aktif yapıyoruz
            appUser.IsActive = true;
            _service.Update(appUser);
            await _service.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hesabınız başarıyla doğrulandı!";

            // Giriş sayfasına yönlendiriyoruz
            return RedirectToAction("SignIn");
        }
        public async Task<IActionResult> SignOutAsync()
        {
            // Kullanıcıyı oturumdan çıkartıyoruz
            await HttpContext.SignOutAsync();
            // Anasayfaya yönlendiriyoruz
            return RedirectToAction("SignIn");
        }
        public IActionResult PasswordRenew()
        {

            return View();
        
        }
        [HttpPost]
        public async Task<IActionResult> PasswordRenewAsync(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError("", "Email boş geçilemez");
                return View();
            }

            AppUser user = await _service.GetAsync(x => x.Email == Email);
            if (user is null)
            {
                ModelState.AddModelError("", "Girdiğiniz Email Bulunamadı");
                return View();
            }

            // 6 haneli doğrulama kodu üret
            var random = new Random();
            user.VerificationCode = random.Next(100000, 999999).ToString();

            // Kod güncellensin
            _service.Update(user);
            await _service.SaveChangesAsync();

            // Email gönder
            await _emailService.SendEmailAsync(user.Email, "Şifre Yenileme Kodu", $"Doğrulama kodunuz: {user.VerificationCode}");

            // TempData ile kullanıcıyı geçici tut
            TempData["Email"] = Email;

            return RedirectToAction("VerifyResetCode");
        }
        public IActionResult VerifyResetCode()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyResetCode(string verificationCode)
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("PasswordRenew");
            }

            var user = await _service.GetAsync(x => x.Email == email && x.VerificationCode == verificationCode);
            if (user is null)
            {
                ModelState.AddModelError("", "Kod geçersiz veya süresi doldu.");
                return View();
            }

            TempData["Email"] = email;
            return RedirectToAction("ResetPassword");
        }
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string password, string passwordConfirm)
        {
            // Şifrelerin eşleşip eşleşmediğini kontrol et
            if (password != passwordConfirm)
            {
                ModelState.AddModelError("", "Şifreler eşleşmiyor.");
                return View();
            }

            // Email bilgisini al
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("PasswordRenew");
            }

            // Kullanıcıyı veritabanından al
            var user = await _service.GetAsync(x => x.Email == email);
            if (user is null)
            {
                return RedirectToAction("PasswordRenew");
            }

            // Şifreyi güncelle
            user.Password = password; // Şifreyi düz metin olarak kaydediyoruz

            // Doğrulama kodunu null yap
            user.VerificationCode = null;

            // Kullanıcıyı güncelle ve veritabanına kaydet
            _service.Update(user);
            await _service.SaveChangesAsync();

            // Başarı mesajını ekle
            TempData["SuccessMessage"] = "Şifreniz başarıyla güncellendi!";
            return RedirectToAction("SignIn");
        }


    }
}

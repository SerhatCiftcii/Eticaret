using Eticaret.Core.Entities;
using Eticaret.Service;
using Eticaret.Service.Abstract;
using Eticaret.Service.Concrete;
using Eticaret.WebUI.ExtensionMethods;
using Eticaret.WebUI.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Drawing;

namespace Eticaret.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IService<AppUser> _serviceAppUser;
        private readonly IService<Product> _serviceProduct;
        private readonly IService<Core.Entities.Address> _seriveceAddress;
        private readonly IService<Order> _serviceOrder;
        private readonly IConfiguration _configuration; //ıyzco
        private readonly IEmailService _emailService;  // Burayı doğru şekilde tanımlıyoruz

        public CartController(IService<Product> serviceProduct,
                              IService<Core.Entities.Address> serviceAddress,
                              IService<AppUser> serviceAppUser,
                              IService<Order> seriveceOrder,
                              IEmailService emailService,
                              IConfiguration configuration)
        {
            _serviceProduct = serviceProduct;
            _seriveceAddress = serviceAddress;
            _serviceAppUser = serviceAppUser;
            _serviceOrder = seriveceOrder;
            _emailService = emailService;  // Burada _emailService doğru şekilde atanıyor
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            var model = new CartViewModel()
            {
                CartLines = cart.CartLines,
                TotalPrice = cart.TotalPrice()
            };
            return View(model);
        }


        public IActionResult Add(int ProductId, int quantity = 1)
        {
            var product = _serviceProduct.Find(ProductId);
            if (product != null)
            {
                // Stok kontrolü
                if (product.Stock < quantity)
                {
                    TempData["ErrorMessage"] = $"{product.Name} için yeterli stok bulunmamaktadır. Mevcut stok: {product.Stock}";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                var cart = GetCart();
                cart.AddProduct(product, quantity);
                HttpContext.Session.SetJson("Cart", cart); // Sepet güncelleniyor

                return Redirect(Request.Headers["Referer"].ToString()); // Kullanıcının bir önceki sayfasına dön
            }
            return RedirectToAction("Index");
        }
        public IActionResult Update(int ProductId, int quantity = 1)
        {
            var product = _serviceProduct.Find(ProductId);
            if (product != null)
            {
                var cart = GetCart();
                cart.UpdateProduct(product, quantity);
                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Remove(int ProductId)
        {
            var product = _serviceProduct.Find(ProductId);
            if (product != null)
            {
                int quantity = 1; // Varsayılan miktar 1
                // Stok kontrolü: Eğer istenilen ürün miktarı, mevcut stoktan fazla ise işlemi engelle.
                if (product.Stock < quantity)
                {
                    TempData["ErrorMessage"] = $"{product.Name} için yeterli stok bulunmamaktadır. Mevcut stok: {product.Stock}";
                    return Redirect(Request.Headers["Referer"].ToString());
                }



                var cart = GetCart();
                int removedQuantity = cart.RemoveProduct(product); // Çıkarılan adedi al

                if (removedQuantity > 0) // Ürün sepetten başarıyla çıkarıldıysa
                {
                    // Stoğu geri artır

                    _serviceProduct.Update(product); // Update metodu async değilse
                    _serviceProduct.SaveChanges();   // SaveChanges metodu async değilse
                }

                HttpContext.Session.SetJson("Cart", cart);
            }
            return RedirectToAction("Index"); // Sepet sayfasına geri dön
        }
        //satın al sayfası onun içinde authrize olması gerekir.
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return RedirectToAction("SignIn", "Account");
            }
            var addresses = await _seriveceAddress.GetAllAsync(a => a.AppUserId == appUser.Id && a.IsActive);
            var cart = GetCart();
            var model = new CheckoutViewModel()
            {
                CartProducts = cart.CartLines,
                TotalPrice = cart.TotalPrice(),
                Addresses = addresses
            };
            return View(model);
        }
        [Authorize, HttpPost]
        public async Task<IActionResult> Checkout(string CardNameSurname, string CardNumber, string CardMonth, string CardYear, string CVV, string DeliveryAddress, string BillingAddress)
        {
            var cart = GetCart();
            var appUser = await _serviceAppUser.GetAsync(x => x.UserGuid.ToString() == HttpContext.User.FindFirst("UserGuid").Value);
            if (appUser == null)
            {
                return RedirectToAction("SignIn", "Account");
            }

            var addresses = await _seriveceAddress.GetAllAsync(a => a.AppUserId == appUser.Id && a.IsActive);
            var model = new CheckoutViewModel()
            {
                CartProducts = cart.CartLines,
                TotalPrice = cart.TotalPrice(),
                Addresses = addresses
            };

            if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(CardMonth) || string.IsNullOrWhiteSpace(CardYear) || string.IsNullOrWhiteSpace(CVV) || string.IsNullOrWhiteSpace(DeliveryAddress) || string.IsNullOrWhiteSpace(BillingAddress))
            {
                return View(model);
            }

            var faturaAdresi = addresses.FirstOrDefault(a => a.AddressGuid.ToString() == BillingAddress);
            var teslimatAdresi = addresses.FirstOrDefault(a => a.AddressGuid.ToString() == DeliveryAddress);

            var siparis = new Order
            {
                AppUserId = appUser.Id,
                BillingAddress = $"{faturaAdresi.OpenAddress} {faturaAdresi.Destrict} {faturaAdresi.City}", // Fatura Adresi
                DeliveryAddress = $"{teslimatAdresi.OpenAddress} {teslimatAdresi.Destrict} {teslimatAdresi.City}", // Teslimat Adresi
                CustomerId = appUser.UserGuid.ToString(),
                OrderDate = DateTime.Now,
                TotalPrice = cart.TotalPrice(),
                OrderNumber = Guid.NewGuid().ToString(),
                OrderState = (int)EnumOrderState.Waiting, // Sipariş durumu "Onay Bekliyor"
                OrderLines = new List<OrderLine>()
            };

            // Ödeme çekme
            #region OdemeIslemi
            Options options = new Options();
            options.ApiKey = _configuration["IyzicoOptions:ApiKey"];
            options.SecretKey = _configuration["IyzicoOptions:SecretKey"];
            options.BaseUrl = _configuration["IyzicoOptions:BaseUrl"]; //"https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = HttpContext.Session.Id;
            request.Price = siparis.TotalPrice.ToString().Replace(",", ".");
            request.PaidPrice = siparis.TotalPrice.ToString().Replace(",", ".");
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B" + HttpContext.Session.Id;
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = CardNameSurname; // "John Doe";
            paymentCard.CardNumber = CardNumber; // "5528790000000008";
            paymentCard.ExpireMonth = CardMonth; // "12";
            paymentCard.ExpireYear = CardYear; // "2030";
            paymentCard.Cvc = CVV; // "123";
            paymentCard.RegisterCard = 1;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = "BY" + appUser.Id;
            buyer.Name = appUser.Name;
            buyer.Surname = appUser.Surname;
            buyer.GsmNumber = appUser.Phone;
            buyer.Email = appUser.Email;
            buyer.IdentityNumber = "11111111111";
            buyer.LastLoginDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Örn: "2025-05-01 14:30:45"
            buyer.RegistrationDate = appUser.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"); // Örn: "2024-04-21 15:12:09"
            buyer.RegistrationAddress = siparis.DeliveryAddress;
            buyer.Ip = HttpContext.Connection.RemoteIpAddress?.ToString(); // "85.34.78.112";
            buyer.City = teslimatAdresi.City;
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            var shippingAddress = new Iyzipay.Model.Address();
            shippingAddress.ContactName = appUser.Name + " " + appUser.Surname;
            shippingAddress.City = teslimatAdresi.City;
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = teslimatAdresi.OpenAddress;
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            var billingAddress = new Iyzipay.Model.Address();
            billingAddress.ContactName = appUser.Name + " " + appUser.Surname;
            billingAddress.City = faturaAdresi.City;
            billingAddress.Country = "Turkey";
            billingAddress.Description = faturaAdresi.OpenAddress;
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();

            //BasketItem firstBasketItem = new BasketItem();
            //firstBasketItem.Id = "BI101";
            //firstBasketItem.Name = "Binocular";
            //firstBasketItem.Category1 = "Collectibles";
            //firstBasketItem.Category2 = "Accessories";
            //firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            //firstBasketItem.Price = "0.3";
            //basketItems.Add(firstBasketItem);

            foreach (var item in cart.CartLines)
            {
                siparis.OrderLines.Add(new OrderLine
                {
                    ProductId = item.Product.Id,
                    OrderId = siparis.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });
                basketItems.Add(new BasketItem
                {
                    Id = item.Product.Id.ToString(),
                    Name = item.Product.Name,
                    Category1 = "Collectibles",
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (item.Product.Price * item.Quantity).ToString().Replace(",", ".")
                });
            }

            if (siparis.TotalPrice < 999) //KARGO ÜCRETİ
            {
                basketItems.Add(new BasketItem
                {
                    Id = "Kargo",
                    Name = "kargo Ücreti",
                    Category1 = "Collectibles",
                    ItemType = BasketItemType.VIRTUAL.ToString(),
                    Price = "99"
                });
                siparis.TotalPrice += 99;
                request.Price = siparis.TotalPrice.ToString().Replace(",", ".");
                request.PaidPrice = siparis.TotalPrice.ToString().Replace(",", ".");
            }


            request.BasketItems = basketItems;

            Payment payment = await Payment.Create(request, options);

            #endregion

            try
            {
                if (payment.Status == "success")
                {
                    // Siparişi veritabanına kaydet
                    await _serviceOrder.AddAsync(siparis);
                    var sonuc = await _serviceOrder.SaveChangesAsync();

                    if (sonuc > 0)
                    {
                        // Stokları güncelle
                        foreach (var item in cart.CartLines)
                        {
                            var product = await _serviceProduct.FindAsync(item.Product.Id);
                            if (product != null)
                            {
                                product.Stock -= item.Quantity; // Stok azalması burada yapılmalı
                                await _serviceProduct.UpdateAsync(product);
                            }
                        }
                        await _serviceProduct.SaveChangesAsync();

                        // Mail gönder
                        string subject = "Siparişiniz Alındı";
                        string body = $@"
        <p>Merhaba {appUser.Name},</p>
        <p>Siparişinizi aldık. En kısa sürede işleme alınacaktır.</p>
        <p>Sipariş durumunuzu hesabınızdan takip edebilirsiniz.</p>
        <br/>
        <p>Teşekkür ederiz.</p>
        ";
                        // E-posta gönderme işlemi
                        await _emailService.SendEmailAsync(appUser.Email, subject, body);

                        // Sepeti temizle
                        HttpContext.Session.Remove("Cart");

                        // Teşekkür sayfasına yönlendir
                        return RedirectToAction("Thanks");
                    }
                }

                else
                {
                    TempData["Message"] = $"<div class='alert alert-danger'>Ödeme İşlemi Başarısız! ({payment.ErrorMessage})</div>";
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "<div class='alert alert-danger'>Hata Oluştu!</div>";
            }

            return View(model);
        }



        public IActionResult Thanks()
        {
            return View();
        }
        private CartService GetCart()
        {
            return HttpContext.Session.GetJson<CartService>("Cart") ?? new CartService();
        }
    }
}
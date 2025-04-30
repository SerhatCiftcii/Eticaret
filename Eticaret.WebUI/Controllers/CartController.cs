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
        private readonly IService<Order> _seriveceOrder;
        private readonly IEmailService _emailService;  // Burayı doğru şekilde tanımlıyoruz

        public CartController(IService<Product> serviceProduct,
                              IService<Core.Entities.Address> serviceAddress,
                              IService<AppUser> serviceAppUser,
                              IService<Order> seriveceOrder,
                              IEmailService emailService)  // IEmailService parametre olarak alıyoruz
        {
            _serviceProduct = serviceProduct;
            _seriveceAddress = serviceAddress;
            _serviceAppUser = serviceAppUser;
            _seriveceOrder = seriveceOrder;
            _emailService = emailService;  // Burada _emailService doğru şekilde atanıyor
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
                HttpContext.Session.SetJson("Cart", cart);

                // Sepete eklendiği anda stoğu azalt
                product.Stock -= quantity;
                _serviceProduct.Update(product); // Update metodu async değilse
                _serviceProduct.SaveChanges();   // SaveChanges metodu async değilse

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
                var cart = GetCart();
                int removedQuantity = cart.RemoveProduct(product); // Çıkarılan adedi al

                if (removedQuantity > 0) // Ürün sepetten başarıyla çıkarıldıysa
                {
                    // Stoğu geri artır
                    product.Stock += removedQuantity;
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
        public async Task<IActionResult> Checkout(string CardNumber, string CardMonth, string CardYear, string CVV, string DeliveryAddress, string BillingAddress)
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

            if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(CardMonth) || string.IsNullOrWhiteSpace(CardYear) || string.IsNullOrWhiteSpace(CVV) || string.IsNullOrWhiteSpace(DeliveryAddress) || string.IsNullOrWhiteSpace(BillingAddress))
            {
                return View(model);
            }

            var teslimatAdresi = addresses.FirstOrDefault(a => a.AddressGuid.ToString() == DeliveryAddress); // Teslimat adresi
            var faturaAdresi = addresses.FirstOrDefault(a => a.AddressGuid.ToString() == BillingAddress); // Fatura adresi
            //Ödeme Çekme iyizco 
            // Sipariş nesnesi oluşturuluyor
            var siparis = new Order
            {
                AppUserId = appUser.Id,
                BillingAddress = $"{faturaAdresi.OpenAddress} {faturaAdresi.Destrict} {faturaAdresi.City}",
                CustomerId = appUser.UserGuid.ToString(),
                DeliveryAddress = $"{teslimatAdresi.OpenAddress} {teslimatAdresi.Destrict} {teslimatAdresi.City}",
                OrderDate = DateTime.Now,
                TotalPrice = cart.TotalPrice(),
                OrderNumber = Guid.NewGuid().ToString(),
                OrderState = (int)EnumOrderState.Waiting, // Sipariş durumu "Onay Bekliyor"
                OrderLines = new List<OrderLine>()
            };

            foreach (var item in cart.CartLines)
            {
                siparis.OrderLines.Add(new OrderLine
                {
                    ProductId = item.Product.Id,
                    OrderId = siparis.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,
                });
            }
            #region OdemIslemi
            Options options = new Options();
            options.ApiKey = "your api key";
            options.SecretKey = "your secret key";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = "123456789";
            request.Price = "1";
            request.PaidPrice = "1.2";
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = "John Doe";
            paymentCard.CardNumber = "5528790000000008";
            paymentCard.ExpireMonth = "12";
            paymentCard.ExpireYear = "2030";
            paymentCard.Cvc = "123";
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = "John";
            buyer.Surname = "Doe";
            buyer.GsmNumber = "+905350000000";
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            var shippingAddress = new Iyzipay.Model.Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            var billingAddress = new Iyzipay.Model.Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem firstBasketItem = new BasketItem();
            firstBasketItem.Id = "BI101";
            firstBasketItem.Name = "Binocular";
            firstBasketItem.Category1 = "Collectibles";
            firstBasketItem.Category2 = "Accessories";
            firstBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            firstBasketItem.Price = "0.3";
            basketItems.Add(firstBasketItem);

            BasketItem secondBasketItem = new BasketItem();
            secondBasketItem.Id = "BI102";
            secondBasketItem.Name = "Game code";
            secondBasketItem.Category1 = "Game";
            secondBasketItem.Category2 = "Online Game Items";
            secondBasketItem.ItemType = BasketItemType.VIRTUAL.ToString();
            secondBasketItem.Price = "0.5";
            basketItems.Add(secondBasketItem);

            BasketItem thirdBasketItem = new BasketItem();
            thirdBasketItem.Id = "BI103";
            thirdBasketItem.Name = "Usb";
            thirdBasketItem.Category1 = "Electronics";
            thirdBasketItem.Category2 = "Usb / Cable";
            thirdBasketItem.ItemType = BasketItemType.PHYSICAL.ToString();
            thirdBasketItem.Price = "0.2";
            basketItems.Add(thirdBasketItem);
            request.BasketItems = basketItems;

            Payment payment = await Payment.Create(request, options);
            #endregion
            try
            {
                // Siparişi veritabanına kaydediyoruz
                await _seriveceOrder.AddAsync(siparis);
                var sonuc = await _seriveceOrder.SaveChangesAsync();

                if (sonuc > 0)
                {
                    // Stok güncelleme işlemi burada yapılacak
                    foreach (var item in cart.CartLines)
                    {
                        var product = await _serviceProduct.FindAsync(item.Product.Id);
                        if (product != null)
                        {
                            product.Stock -= item.Quantity;
                            await _serviceProduct.UpdateAsync(product);
                        }
                    }
                    await _serviceProduct.SaveChangesAsync(); // Güncellenen stok bilgilerini kaydet

                    // Sipariş başarıyla kaydedildikten sonra, kullanıcıya mail gönderimi yapılacak
                    string subject = "Siparişiniz Alındı";
                    string body = $@"
    <p>Merhaba {appUser.Name},</p>
    <p>Siparişinizi aldık. En kısa sürede işleme alınacaktır.</p>
    <p>Sipariş durumunuzu hesabınızdan takip edebilirsiniz.</p>
    <br/>
    <p>Teşekkür ederiz.</p>
            ";

                    await _emailService.SendEmailAsync(appUser.Email, subject, body);

                    // Sepeti temizleyelim
                    HttpContext.Session.Remove("Cart");
                    return RedirectToAction("Thanks"); // Teşekkür sayfasına yönlendirme
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Hata Oluştu. Ödeme işlemi gerçekleştirilemedi.";
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

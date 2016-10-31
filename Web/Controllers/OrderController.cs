using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Domen.Abstract;
using Web.Domen.Infrastructure;
using Web.Domen.Models;
using Web.Helpers;

namespace Web.Controllers
{
    public class OrderController : Controller
    {
        private IHome _home;

        public OrderController(IHome home)
        {
            _home = home;
        }
        // GET: Order
        public ActionResult Payment(Order data)
        {
            var order = _home.GetOrderBuId(data.Id);
            var model = new Ykassa
            {
                Sum = order.TotalCost,
                Cpsemail = order.Customer.Email,
                Cpsphone = order.Customer.Phone,
                CustomerNumber = order.Customer.Id+":"+order.Customer.Email,
                OrderNumber = order.Id.ToString(),
                ShopArticleId = order.Paty.Id,
                ShopSuccessUrl = "www.redblackclub.ru/home/orderdetails/"+order.Id,
                Scid = "543778",
                ShopId = "80812",
                Order = order
            };
            return View(model);
        }
        [HttpPost]
        public string CheckTest(DateTime requestDatetime,
                                string action,
                                string orderSumAmount,
                                string orderSumCurrencyPaycash,
                                string orderSumBankPaycash,
                                string shopId,
                                string invoiceId,
                                string customerNumber,
                                string orderNumber,
                                string md5
                                )
        {
            var toHash = string.Join(";", new[] {"checkTest", orderSumAmount, orderSumCurrencyPaycash, orderSumBankPaycash, shopId, invoiceId, customerNumber, orderNumber});
            var myMd5 = PassAuth.EncodeMd5(toHash);

            var code = 0;
            var message = string.Empty;

            if (!string.Equals(myMd5, md5, StringComparison.CurrentCultureIgnoreCase))
            {
                code = 1;
                message = "Несовпадение значения параметра md5 с результатом расчета хэш-функции. Окончательная ошибка.";
            }
            HttpContext.Response.ContentType = "application/xml";
            var resp = $"<?xml version=\"1.0\" encoding=\"UTF - 8\"?><checkOrderResponse performedDatetime=\"{DateTime.Now}\" code=\"{code}\" invoiceId=\"{invoiceId}\" shopId=\"{shopId}\" message=\"{message}\"/>";
            _home.SeeCheck(toHash+">"+md5);

            return resp;
        }
    }

    public class Ykassa
    {
        public string ShopId { get; set; }//Идентификатор магазина, выдается при подключении к Яндекс.Кассе.
        public string Scid { get; set; }//Идентификатор витрины магазина, выдается при подключении к Яндекс.Кассе.
        public decimal Sum { get; set; }//Сумма заказа.
        public string CustomerNumber { get; set; }//Идентификатор плательщика в системе магазина. В качестве идентификатора может использоваться номер договора плательщика, логин плательщика и т. п.
        public string OrderNumber { get; set; }//Уникальный номер заказа в системе магазина. Уникальность контролируется Яндекс.Деньгами в сочетании с параметром shopId.
        public int ShopArticleId { get; set; }//Идентификатор товара, выдается при подключении к Яндекс.Кассе. Применяется, если магазин использует несколько платежных форм для разных товаров.
        public string ShopSuccessUrl { get; set; }//URL, на который будет вести ссылка Вернуться в магазин со страницы успешного платежа. В зависимости от настроек магазина:
        public string Cpsphone { get; set; }//Номер мобильного телефона плательщика. Если он передан, то соответствующее поле на странице подтверждения платежа будет предзаполнено
        public string Cpsemail { get; set; }//Адрес электронной почты плательщика. Если он передан, то соответствующее поле на странице подтверждения платежа будет предзаполнено
        public Order Order { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IHome _home;

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
        public string CheckTest(DateTime requestDatetime,string action,string orderSumAmount,string orderSumCurrencyPaycash,string orderSumBankPaycash,string shopId,string invoiceId,string customerNumber,string orderNumber,string md5)
        {
            const string shopPassword = "Kjhk&*lk%$h211KU6";
            var order = _home.GetOrderBuId(int.Parse(orderNumber));
            var price = order.TotalCost.ToString("F").Replace(",", ".");
            var customer = order.Customer.Id + ":" + order.Customer.Email;
            var toHash = string.Join(";", action, price, orderSumCurrencyPaycash, orderSumBankPaycash, shopId, invoiceId, customer, shopPassword);
            var myMd5 = PassAuth.EncodeMd5(toHash);

            var code = 0;

            if (!string.Equals(myMd5, md5, StringComparison.CurrentCultureIgnoreCase))
            {
                code = 1;
            }

            var dt = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz");
            HttpContext.Response.ContentType = "application/xml";
            var resp = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><checkOrderResponse performedDatetime=\"{dt}\" code=\"{code}\" invoiceId=\"{invoiceId}\" shopId=\"{shopId}\"/>";

            return resp;
        }

        [HttpPost]
        public string AvisoTest(DateTime requestDatetime, string action, string orderSumAmount, string orderSumCurrencyPaycash, string orderSumBankPaycash, string shopId, string invoiceId, string customerNumber, string orderNumber, string paymentPayerCode, string shopSumAmount, string md5)
        {
            const string shopPassword = "Kjhk&*lk%$h211KU6";
            var order = _home.GetOrderBuId(int.Parse(orderNumber));
            var price = order.TotalCost.ToString("F").Replace(",", ".");
            var customer = order.Customer.Id + ":" + order.Customer.Email;
            var toHash = string.Join(";", action, price, orderSumCurrencyPaycash, orderSumBankPaycash, shopId, invoiceId, customer, shopPassword);
            var myMd5 = PassAuth.EncodeMd5(toHash);

            var code = 0;
            var eq = false;
            if (!string.Equals(myMd5, md5, StringComparison.CurrentCultureIgnoreCase))
            {
                code = 1;
            }
            if (code == 0)
            {
                eq = true;
                var aviso = new PaymentAviso
                {
                    InvoiceId = invoiceId,
                    PayDate = DateTime.Now,
                    PaymentPayerCode = paymentPayerCode,
                    ShopSumAmount = shopSumAmount
                };
                _home.AcceptPayAsync(aviso, order);
                var body = System.IO.File.ReadAllText(Server.MapPath("/Views/Shared/ticket.html"));
                var emessageBody = MakeOrderBody(body, order);
                PassAuth.SendMyMail(emessageBody, order.Customer.Email, "Приглашение на мероприятие с сайта redblackclub.ru");
            }
            var dt = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffzzz");
            HttpContext.Response.ContentType = "application/xml";
            var resp = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><paymentAvisoResponse performedDatetime=\"{dt}\" code=\"{code}\" invoiceId=\"{invoiceId}\" shopId=\"{shopId}\"/>";

            //_home.SeeCheck(toHash+" MD5: "+md5+" Equals:" + eq+" RESP: > "+resp+"ORD: #"+order.Id);
            return resp;
        }

        public ActionResult SuccessTest(string orderNumber)
        {
            var order = _home.GetOrderBuId(int.Parse(orderNumber));
            return View(order);
        }

        public ActionResult FailTest(string orderNumber)
        {
            var model = string.IsNullOrEmpty(orderNumber) ? new Order() : _home.GetOrderBuId(int.Parse(orderNumber));
            return View(model);
        }
        private string MakeOrderBody(string body, Order order)
        {
            var result = body;
            var oDate = order.OrderDate.ToLongDateString().Split(' ');
            var pDate = order.Paty.PatyDate.ToLongDateString().Split(' ');
            result = result.Replace("{0}", pDate[0]);
            result = result.Replace("{1}", pDate[1] + "," + pDate[2]);
            result = result.Replace("{2}", order.Paty.PatyDate.ToShortTimeString());
            result = result.Replace("{3}", order.Paty.Title);
            result = result.Replace("{4}", order.Paty.Place);
            result = result.Replace("{5}", order.Customer.Fio);
            result = result.Replace("{6}", order.PlaceNumbers);
            result = result.Replace("{7}", order.TotalCost.ToString());
            result = result.Replace("{8}", oDate[0] + " " + oDate[1]);
            result = result.Replace("{9}", order.OrderDate.ToShortTimeString());
            result = result.Replace("{10}", order.Id.ToString());
            return result;
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
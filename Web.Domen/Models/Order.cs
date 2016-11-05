using System;

namespace Web.Domen.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalCost { get; set; }
        public int Place { get; set; }//кол-во мест
        public string PlaceNumbers { get; set; }
        public string AvisoLog { get; set; }
        public virtual Paty Paty { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual PaymentAviso Aviso { get; set; }
    }

    public class PaymentAviso
    {
        public int Id { get; set; }
        public DateTime PayDate { get; set; }//Момент регистрации оплаты заказа в Яндекс.Деньгах.
        public string InvoiceId { get; set; }//Уникальный номер транзакции в сервисе Яндекс.Денег.
        public string ShopSumAmount { get; set; }//Сумма к выплате на счет магазина (сумма заказа минус комиссия Яндекс.Денег).
        public string PaymentPayerCode { get; set; }//Номер счета в Яндекс.Деньгах, с которого производится оплата.

    }
    public class OrderViewmodel
    {
        public string Fio { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Workplace { get; set; }
        public int PatyId { get; set; }
        public int Place { get; set; }
        public Paty Paty { get; set; }
        public Customer Customer { get; set; }
    }
}
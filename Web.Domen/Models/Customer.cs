using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Domen.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string Fio { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public DateTime? Birthday { get; set; }
        public string Work { get; set; }
        public string WorkPlace { get; set; }
        public string User { get; set; }
        public int Rate { get; set; }
        public virtual CustImage Avatar { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    public class CustImage
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string ContentType { get; set; }

    }

    public class CustomerViewModel
    {
        [Required(ErrorMessage = "Имя - это обязательное поле.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Телефон - это обязательное поле.")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "Укажите все цифры номера телефона")]
        [MinLength(10,ErrorMessage = "Укажите все 10 цыфр телефонного номера")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "E-mail - обязательное поле")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Проверьте, правильно ли вы написали адрес электронной почты")]
        public string Email { get; set; }
        public string City { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Формат даты не верный. ")]
        public string Birthday { get; set; }
        public string Workplace { get; set; }
        public string UserId { get; set; }
    }
}
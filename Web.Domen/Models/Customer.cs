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
        public string Facebook { get; set; }
        public string Insta { get; set; }
        public string Vk { get; set; }
        public string Od { get; set; }
        public string Twit { get; set; }
        public string AboutMe { get; set; }
        public bool SmsNotice { get; set; }
        public bool EmailNotice { get; set; }
        public bool PhoneNotice { get; set; }
        public string ShowData { get; set; }//0-ФИО, 1-Телефон, 2-Мыло, 3-Город, 4-рождение, 5-должность,6-facebook, 7-instagram, 8-Vk, 9-Od, 10-twit,11-sex
        public virtual CustImage Avatar { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    public class Company
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }
        public virtual CustImage Image { get; set; }
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
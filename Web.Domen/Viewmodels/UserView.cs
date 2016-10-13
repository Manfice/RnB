using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Domen.Viewmodels
{
    public class UserView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } 
    }

    public class LogivVm
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class Register
    {
        [Required(ErrorMessage = "Поле \"Имя\" обязательно для заполнения")]
        public string Fio { get; set; }
        [Required(ErrorMessage = "Поле \"Телефон\" обязательно для заполнения")]
        [DataType(DataType.PhoneNumber,ErrorMessage = "Укажите телефон с кодом города")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Поле \"E-mail\" обязательно для заполнения")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail указан не корректно")]
        public string Email { get; set; }
        public string City { get; set; }
        public DateTime BirthDay { get; set; }
        public string Work { get; set; }
    }
}
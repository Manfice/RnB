using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Viewmodels
{
    public class AuthViews
    {
         
    }

    public class RoleEditView
    {
        public AppRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMember { get; set; }
    }

    public class RoleModModel
    {
        [Required]
        public string Name { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToRemove { get; set; }
    }

    public class ForgotPassword
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [DataType(DataType.EmailAddress,ErrorMessage = "Укажите корректный e-mail адрес")]
        public string Email { get; set; }
    }

    public class ResetPassVm
    {
        public string Id { get; set; }
        public string Token { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль, от 6 символов")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Введите новый пароль еще раз.")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLoginConfirmationViewmodel
    {
        [Required]
        [Display(Name = "Ваш E-mail адрес")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Имя и фамилия обязательны")]
        public string Fio { get; set; }
    }

    public class VkSecret
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string user_id { get; set; }
        public string email { get; set; }
    }

    public class VkUserInfo
    {
        public string id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string photo_200_orig { get; set; }

        public string bdate { get; set; }

        public string nickname { get; set; }

        public string screen_name { get; set; }
        public string sex { get; set; }
        [Required]
        [Display(Name = "Ваш E-mail адрес")]
        public string email { get; set; }
        public virtual City city { get; set; }
        public string connections { get; set; }

        public string domain { get; set; }

        public LoginInfo GetLoginInfo()
        {
            return new LoginInfo
            {
                LoginProvider = "Vk",
                ProviderKey = this.id
            };
        }
    }

    public class City
    {
        public int id { get; set; }
        public string title { get; set; }
    }

    public class LoginInfo
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
    }

    public class FbUserInfo
    {
        public string id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string link { get; set; }
        public string birthday { get; set; }
    }
}
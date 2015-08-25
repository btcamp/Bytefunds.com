using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class LoginViewModel
    {
        public int languageId { get; set; }
        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "请输入用户名")]
        [EmailAddress(ErrorMessage="请输入正确的邮箱")]
        public string Username { get; set; }
        public string RedirectUrl { get; set; }

        public bool RememberMe { get; set; }
    }
}
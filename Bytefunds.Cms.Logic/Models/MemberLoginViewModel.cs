using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class MemberLoginViewModel
    {
        [Display(Name = "登陆邮箱")]
        [Required(ErrorMessage = "请输入登陆邮箱")]
        public string Email { get; set; }

        [Display(Name = "登陆密码")]
        [Required(ErrorMessage = "请输入登陆密码")]
        public string Password { get; set; }
    }
}
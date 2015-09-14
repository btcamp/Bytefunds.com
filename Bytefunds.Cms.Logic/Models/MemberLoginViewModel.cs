using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class MemberLoginViewModel
    {
        [Display(Name = "手机")]
        [Required(ErrorMessage = "请输入手机")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "请输入正确的手机号码")]
        public string Phone { get; set; }

        [Display(Name = "登陆密码")]
        [Required(ErrorMessage = "请输入登陆密码")]
        public string Password { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class MemberRegisterViewModel
    {
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "请输入姓名")]
        public string Name { get; set; }
        [Display(Name = "邮箱")]
        //[Required(ErrorMessage = "请输入邮箱")]
        [RegularExpression(@"^.+@.+\..+$", ErrorMessage = "请输入正确的邮箱地址")]
        public string Email { get; set; }

        [Display(Name = "手机")]
        [Required(ErrorMessage = "请输入手机")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "请输入正确的手机号码")]
        public string Phone { get; set; }

        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入密码")]
        public string Password { get; set; }

        [Display(Name = "验证码")]
        [Required(ErrorMessage = "请输入验证码")]
        public string Code { get; set; }

        //[Display(Name = "确认密码")]
        //[Required(ErrorMessage = "请输入确认密码")]
        //[Compare("Password",ErrorMessage="两次密码不一致，请重新输入")]
        //public string ConfirmPassword { get; set; }
    }
}
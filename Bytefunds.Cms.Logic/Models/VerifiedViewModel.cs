using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class VerifiedViewModel
    {
        [Display(Name = "身份证号")]
        [Required(ErrorMessage = "请输入身份证号")]
        [RegularExpression(@"^\d{18}|\d{17}[X,x]$", ErrorMessage = "请输入正确的身份证号")]
        public string IDCard { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "请输入姓名")]
        public string Name { get; set; }


        [Display(Name = "邮箱")]
        [Required(ErrorMessage = "请输入邮箱")]
        [RegularExpression(@"^.+@.+\..+$", ErrorMessage = "请输入正确的邮箱地址")]
        public string Email { get; set; }

        [Display(Name = "身份证正面")]
        [Required(ErrorMessage = "请选择身份证正面照")]
        public HttpPostedFileWrapper Card1 { get; set; }

        [Display(Name = "身份证反面")]
        [Required(ErrorMessage = "请选择身份证反面照")]
        public HttpPostedFileWrapper Card2 { get; set; }


    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class ReservationViewModel
    {
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "请输入姓名")]
        public string Name { get; set; }

        [Display(Name = "手机号码")]
        [Required(ErrorMessage = "请输入手机号码")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "请输入正确的手机号码")]
        public string Phone { get; set; }

        [Display(Name = "邮箱")]
        [Required(ErrorMessage = "请输入邮箱")]
        [RegularExpression(@"^.+@.+\..+$", ErrorMessage = "请输入正确的邮箱地址")]
        public string Email { get; set; }

        [Display(Name = "信息")]
        [Required(ErrorMessage = "请输入预约信息")]
        public string Text { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class WithdrawViewModel
    {
        [Display(Name = "金额")]
        [Required(ErrorMessage = "请输入提现金额")]
        //[Range(100, int.MaxValue, ErrorMessage = "请输入有效的出款金额，最低出款100元")]
        [RegularExpression(@"^((\d{3,})|(\d{3,}\.\d{1,2}))$", ErrorMessage = "请输入有效的金额，最低提现100元")]
        public double Amount { get; set; }

        [Display(Name = "开户姓名")]
        [Required(ErrorMessage = "请输入开户姓名")]
        public string Name { get; set; }

        [Display(Name = "开户行")]
        [Required(ErrorMessage = "请输入开户行")]
        public string BankName { get; set; }

        [Display(Name = "开户行分行")]
        [Required(ErrorMessage = "请输入开户行分行")]
        public string BankDetail { get; set; }

        [Display(Name = "银行卡卡号")]
        [Required(ErrorMessage = "请输入银行卡卡号")]
        [RegularExpression(@"^\d+$", ErrorMessage = "请输入正确的银行卡卡号")]
        public string BankNumber { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class BankCertificationViewModel
    {
        [Display(Name = "开户行")]
        [Required(ErrorMessage = "请输入开户行")]
        public string BankName { get; set; }

        [Display(Name = "开户人")]
        [Required(ErrorMessage = "请输入开户人")]
        public string BankAccountName { get; set; }

        [Display(Name = "银行卡账号")]
        [Required(ErrorMessage = "请输入银行卡账号")]
        [RegularExpression(@"^\d+$",ErrorMessage="请输入合格的银行卡卡号")]
        public string BankCardNumber { get; set; }
    }
}
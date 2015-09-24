using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class TransferredViewModel
    {
        [Display(Name = "转入金额")]
        [Required(ErrorMessage = "请选择要转入的金额")]
        [RegularExpression(@"^((\d{3,})|(\d{3,}\.\d{1,2}))$", ErrorMessage = "请输入有效的金额，最低转入100元")]
        public double Amount { get; set; }

        [Display(Name = "定期宝")]
        [Required(ErrorMessage = "请选择要转入的定期宝")]
        public int ProductId { get; set; }

    }
}
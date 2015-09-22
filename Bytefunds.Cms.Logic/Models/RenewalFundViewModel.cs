using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class RenewalFundViewModel
    {

        public int CurrentProductId { get; set; }
        [Display(Name = "当前产品")]
        public string CurrentProductTitle { get; set; }
        [Display(Name = "金额")]
        public string Amount { get; set; }

        [Display(Name = "定投时间")]
        public string StartTime { get; set; }
        [Display(Name = "到期时间")]
        public string EndTime { get; set; }

        [Display(Name = "续约产品")]
        public int RenewalFundId { get; set; }
    }
}
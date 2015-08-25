using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class PaymentModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public double Amount { get; set; }
        public string Phone { get; set; }
        public string Billno { get; set; }
        public string Linkphone { get; set; }

        
    }
}
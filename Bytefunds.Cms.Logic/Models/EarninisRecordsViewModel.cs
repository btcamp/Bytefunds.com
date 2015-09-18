using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class EarninisRecordsViewModel
    {
        public EarninisRecordsViewModel()
        {
            Times = new List<string>();
            Datas = new List<decimal>();
        }
        public List<string> Times { get; set; }

        public List<decimal> Datas { get; set; }
    }
}
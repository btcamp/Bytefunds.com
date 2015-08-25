using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public enum TxtParamActionEnum
    {
        Insert = 0, Update = 1, Monitor = 2
    }

    public class TxtParamClass
    {
        public string Name { get; set; }
        public int Login { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Amount { get; set; }
        public double Ratio { get; set; }
        public TxtParamActionEnum Action { get; set; }
        public int MemberId { get; set; }
        public DateTime MonStart { get; set; }
        public DateTime MonEnd { get; set; }
        public int MonLots { get; set; }
        public double MonRate { get; set; }
        public double ExpectedProfit { get; set; }
        public string[] Groups { get; set; }
    }
}
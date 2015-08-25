using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Helpers
{
    public class UmbBillnoFactoryHelper
    {
        private static Random rand = new Random();
        /// <summary>
        /// 获取订单号
        /// </summary>
        public static string GetBaillno
        {
            get
            {
                return DateTime.Now.ToString("yyyyMMddhhmmss") + rand.Next(10000, 99999);
            }
        }
    }
}
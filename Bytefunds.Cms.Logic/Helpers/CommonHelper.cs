using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Bytefunds.Cms.Logic.Helpers
{
    public class CommonHelper
    {
        public static string GetRandomPassword(int pwdlength)
        {
            Random random = new Random();
            string reslut = "qwertyuipkjhgfdsazxcvbnm23654789QWERTYUPLKJHGFDSAZXCVBNM";
            string pwd = string.Empty;
            for (int i = 0; i < pwdlength; i++)
            {
                pwd += Convert.ToString(reslut[random.Next(0, reslut.Length)]);
            }
            return pwd;
        }

        public static double ConvertToUsd(double cny)
        {
            Models.UmbDictionaryModel umbmodel = Helpers.UmbracoDictionaryHelper.GetDictionaryByKey("ExchangeRate");
            double exrate;
            if (!double.TryParse(umbmodel.CnValue, out exrate))
            {
                exrate = 6.2475;
            }
            return Math.Round((cny / exrate), 2);
        }

    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using umbraco.cms.businesslogic;
using Umbraco.Core;
using Umbraco.Core.Models;
namespace Bytefunds.Cms.Logic.Helpers
{
    public class UmbracoDictionaryHelper
    {
        /// <summary>
        /// 获取Umbraco中Dictionary中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Models.UmbDictionaryModel GetDictionaryByKey(string key)
        {
            Models.UmbDictionaryModel model = new Models.UmbDictionaryModel();
            Dictionary.DictionaryItem item = Dictionary.getTopMostItems.Where(d => d.key.Equals(key)).FirstOrDefault();
            model.EnValue = item.Value(1);
            model.CnValue = item.Value(2);
            model.DicKey = item.key;
            return model;
        }

        //public static void CopyDictionaryValue()
        //{
        //    List<ILanguage> allLanuage = ApplicationContext.Current.Services.LocalizationService.GetAllLanguages().ToList();
        //    var allDictionary = Dictionary.getTopMostItems;
        //    foreach (var d in allDictionary)
        //    {
        //        var d1 = d.Value(5);
        //        if (d1.IndexOf("(请转繁体)") <= 0)
        //        {
        //            d.setValue(5, d1.Remove(0, 6));
        //        }
        //        else
        //        {
        //            d.setValue(5, "(请转繁体)" + d1);
        //        }
        //    }

        //}
           

        public static string GetLanguageIsoCodeById(int languageId)
        {
            var language = ApplicationContext.Current.Services.LocalizationService.GetAllLanguages().FirstOrDefault(l=>l.Id.Equals(languageId));
            return language.IsoCode;
           
        }

   
        public static string GetDictionaryItem(string key,int languageId)
        {
            string value = string.Empty;
            try
            {
                //int languageId=umbraco.cms.businesslogic.language.Language.GetByCultureCode(language).id;
                value = Dictionary.getTopMostItems.Where(d => d.key.Equals(key)).FirstOrDefault().Value(languageId);
            }
            catch 
            {
                value = string.Empty;
            }
            return value;
        }

        public static Dictionary<string,string> GetDictionaryItem(string key)
        {
            Dictionary<string,string>  result = new Dictionary<string,string>();
            List<ILanguage> allLanuage =ApplicationContext.Current.Services.LocalizationService.GetAllLanguages().ToList();
            foreach (ILanguage language in allLanuage)
            {
                string value = GetDictionaryItem(key,language.Id);
                result.Add(language.IsoCode, value);
            }
            return result;
        }




        public static string GetDomainName(string UrlWithDomain, string Url)
        {
            string domain = string.Empty;
            int index = UrlWithDomain.IndexOf(Url);
            int count = Url.Count();
            if (index >= 0)
            {
                domain= UrlWithDomain.Remove(index, count);
            }
            else
            {
                if (UrlWithDomain.IndexOf("v2e")>=0)
                {
                    domain= "/v2e" + Url;
                }
                else if (UrlWithDomain.IndexOf("v2") >= 0)
                {
                    domain= "/v2" + Url;
                }
            }
            return domain;
        }
    }
}
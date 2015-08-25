using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Models;

namespace Bytefunds.Cms.Logic.Models
{
    public class ResposeModel
    {
        private int _languageId = 0;
        private string _RedirectUrl =string.Empty;
        public int languageId
        {
            get { return this._languageId; }
            set { this._languageId = value; }
        }
        public bool success { get; set; }
        public string Msg { get; set; }
        public string RedirectUrl
        {
            get { return this._RedirectUrl; }
            set { this._RedirectUrl = GetRootContentName(value);}
        }

        private string GetRootContentName(string value)
        {
            return value;
            //两个语言网站时，需取消下面的注释
            //string url = string.Empty;
            //if (_languageId != 0 && value.ToLower().IndexOf("http://")<0)
            //{
            //    ILanguage currentLanguage = ApplicationContext.Current.Services.LocalizationService.GetAllLanguages().Where(l => l.Id.Equals(languageId)).FirstOrDefault();
            //    if (value.ToLower().IndexOf(currentLanguage.IsoCode.ToLower()) >= 0)
            //    {
            //        return value;
            //    }
            //    url = "/" + currentLanguage.IsoCode + value;
            //    return url;
            //}
            //else
            //{
            //    return value;
            //}
            
        }
    }
}
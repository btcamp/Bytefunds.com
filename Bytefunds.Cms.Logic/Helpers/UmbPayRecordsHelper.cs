using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core;
namespace Bytefunds.Cms.Logic.Helpers
{
    public class UmbPayRecordsHelper
    {
        public static bool IsExistsBillno(string billno)
        {
            IContentType ct = ApplicationContext.Current.Services.ContentTypeService.GetContentType("PayRecords");
            IEnumerable<IContent> list = ApplicationContext.Current.Services.ContentService.GetContentOfContentType(ct.Id);
            var model = list.Where(c => !string.IsNullOrEmpty(c.GetValue<string>("payBillno")) && c.GetValue<string>("payBillno").Equals(billno)).FirstOrDefault();
            return model == null ? true : false;
        }
    }
}
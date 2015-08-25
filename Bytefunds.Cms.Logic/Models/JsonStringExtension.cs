using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public  static class JsonStringExtension
    {
         public static string ToJsonString(this ResposeModel model)
        {
            return JsonConvert.SerializeObject(model);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            Success = false;
            RedirectUrl = "/";
        }
        public bool Success { get; set; }

        public string Msg { get; set; }

        public string RedirectUrl { get; set; }
    }
}
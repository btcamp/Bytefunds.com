using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Services;

namespace Bytefunds.Cms.Logic.Models
{
    public class ThreadParameter
    {
        public ServiceContext Services { get; set; }
        public HttpContext Context { get; set; }
        public object Parameter { get; set; }
    }
}
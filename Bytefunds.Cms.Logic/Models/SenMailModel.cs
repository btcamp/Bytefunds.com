using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Services;

namespace Bytefunds.Cms.Logic.Models
{
    public class SenMailModel
    {
        public string UserName { get; set; }
        public string ToEmail { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CurrentSendMailKey { get; set; }
        public ServiceContext Services { get; set; }
        public HttpContext Context { get; set; }
    }
}
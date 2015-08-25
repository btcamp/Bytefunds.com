using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Models;

namespace Bytefunds.Cms.Logic.Models
{
    public class ChangePasswordModel : ChangingPasswordModel
    {
        public int languageId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Models
{
    public class ImportExcelResponseModel
    {
        public ImportExcelResponseModel()
        {
            Messages = new List<string>();
        }
        public string Email { get; set; }

        public string LoginPwd { get; set; }

        public string MT4Account { get; set; }

        public string MT4Pwd { get; set; }

        public string MT4PasswordInvestor { get; set; }

        public List<string> Messages { get; private set; }
    }
}
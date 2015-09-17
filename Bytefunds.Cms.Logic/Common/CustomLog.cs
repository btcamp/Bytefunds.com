using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Bytefunds.Cms.Logic.Common
{
    public class CustomLog
    {
        public static void WriteLog(string msg)
        {

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            System.IO.File.AppendAllText(Path.Combine(path, DateTime.Now.ToString("yyyyMMdd") + ".log"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg + "\r\n");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MD.SDK
{
    public class AppAttr
    {
        public static string MDApiUrl = ConfigurationManager.AppSettings["MDApiUrl"] ?? "https://api.mingdao.com";
        public static string AppKey = ConfigurationManager.AppSettings["AppKey"] ?? "B9CC7B89CD9DF2E927793C5FFA90726F";
       public static string AppSecret = ConfigurationManager.AppSettings["AppSecret"] ?? "D35B79CA77D45CD1664DA27BA3E7376";
       public static string CallbackUrl = ConfigurationManager.AppSettings["CallbackUrl"] ?? "D35B79CA77D45CD1664DA27BA3E7376";
    }
}

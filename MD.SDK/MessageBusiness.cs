using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
namespace MD.SDK
{
    public class MessageBusiness
    {
        static string AppKey = ConfigurationManager.AppSettings["AppKey"] ?? "B9CC7B89CD9DF2E927793C5FFA90726F";
        static string AppSecret = ConfigurationManager.AppSettings["AppSecret"] ?? "D35B79CA77D45CD1664DA27BA3E7376";
        public static string CreateSys(string token, string msg, string userID, string projectID, out int errorCode)
        {
            errorCode = 0;
            var paras = new Dictionary<string, object>();
            paras.Add("access_token", token);
            paras.Add("msg", msg);
            paras.Add("u_id", userID);
            paras.Add("p_id", projectID);
            paras.Add("app_key", AppKey);
            paras.Add("app_secret", AppSecret);
            var result = HttpRequest.RequestServer(ApiOption.message_create_sys, paras);

            JObject resultObj = (JObject)JsonConvert.DeserializeObject(result);
            if (resultObj != null)
            {
                if (resultObj.Property("error_code") == null)
                    return resultObj["post"].ToString();
                else
                    errorCode = int.Parse(resultObj["error_code"].ToString());
            }

            return string.Empty;

        }
    }
}

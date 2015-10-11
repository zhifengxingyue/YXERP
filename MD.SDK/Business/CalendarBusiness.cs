using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace MD.SDK
{
    public class CalendarBusiness
    {
        public static string AddCalendar(string token, string name,string des, string startTime, string endTime, out int errorCode)
        {
            errorCode = 0;
            var paras = new Dictionary<string, object>();
            paras.Add("access_token", token);
            paras.Add("c_name", name);
            paras.Add("c_des", des);
            paras.Add("c_stime", startTime);
            paras.Add("c_etime", endTime);
            var result = HttpRequest.RequestServer(ApiOption.task_v4_addTask, paras);

            JObject resultObj = (JObject)JsonConvert.DeserializeObject(result);
            if (resultObj != null)
            {
                if (resultObj.Property("error_code") == null)
                {
                    var calendar = JsonConvert.DeserializeObject<CalendarEntity>(result);
                    return calendar.id;
                }
                else
                    errorCode = int.Parse(resultObj["error_code"].ToString());
            }

            return string.Empty;

        }
    }
}

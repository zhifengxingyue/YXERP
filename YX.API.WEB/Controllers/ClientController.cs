using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace YX.API.WEB.Controllers
{
    public class ClientController : Controller
    {
        //
        // GET: /Client/
        
        public void MDEvent(FormCollection paras)
        {
            string signature = paras["signature"];
            string timestamp = paras["timestamp"];
            string nonce = paras["nonce"];
            string content = paras["content"];
            JObject contentObj = (JObject)JsonConvert.DeserializeObject(content);
            
            CloudSalesEntity.Clients client=new CloudSalesEntity.Clients();
            client.CompanyName = contentObj["pname"].ToString();
            client.ContactName = contentObj["uname"].ToString();
            int result;
            string clientID= CloudSalesBusiness.ClientBusiness.InsertClient(client, "loginname", "pwd", string.Empty, out result);

            if (!string.IsNullOrEmpty(clientID))
            {
                //授权
                CloudSalesBusiness.ClientBusiness.ClientAuthorize(clientID, 20, 1, DateTime.Now.AddMonths(3));

                //新增云销初始用户
                string uid = contentObj["uid"].ToString();
                string pid = contentObj["pid"].ToString();
            }

        }

    }
}

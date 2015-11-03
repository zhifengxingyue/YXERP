using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class ActivityReplyEntity
    {
        [Property("Lower")]
        public string ReplyID { get; set; }

        public string ActivityID { get; set; }

        public string Msg { get; set; }

        public int Status { get; set; }

        public string CreateUserID { get; set; }

        public string AgentID { get; set; }

        public Users CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public string FromReplyID { get; set; }

        public string FromReplyUserID { get; set; }

        public string FromReplyAgentID { get; set; }

        public Users FromReplyUser { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

    }
}

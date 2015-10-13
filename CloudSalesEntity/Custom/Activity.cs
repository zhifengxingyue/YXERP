using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class Activity
    {
        public string ActivityID { get; set; }

        public string Name { get; set; }

        public string Poster { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }
        /// <summary>
        /// 状态1正常 2结束 9删除
        /// </summary>
        public int Status { get; set; }

        public string OwnerID { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime CreateUserID { get; set; }

        public string AgentID { get; set; }

        public string ClientID { get; set; }

    }
}

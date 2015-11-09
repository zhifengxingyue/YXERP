using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class CustomerLogEntity
    {
        public string CustomerID { get; set; }

        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserID { get; set; }

        public Users CreateUser { get; set; }

        public string AgentID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}

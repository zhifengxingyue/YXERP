﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class CustomerEntity
    {
        [Property("Lower")]
        public string CustomerID { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        [Property("Lower")]
        public string IndustryID { get; set; }

        public int Extent { get; set; }

        public string CityCode { get; set; }

        public CityEntity City { get; set; }

        public string Address { get; set; }

        public string ContactName { get; set; }

        public string MobilePhone { get; set; }

        public string OfficePhone { get; set; }

        public string Email { get; set; }

        public string Jobs { get; set; }

        public DateTime Birthday { get; set; }

        public int Age { get; set; }

        public int Sex { get; set; }

        public int Education { get; set; }

        public string Description { get; set; }

        [Property("Lower")]
        public string SourceID { get; set; }

        [Property("Lower")]
        public string ActivityID { get; set; }

        [Property("Lower")]
        public string StageID { get; set; }

        [Property("Lower")]
        public string OwnerID { get; set; }

        public Users Owner { get; set; }

        public int Status { get; set; }

        public DateTime AllocationTime { get; set; }

        public DateTime OrderTime { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public string CreateUserID { get; set; }

        public Users CreateUser { get; set; }

        [Property("Lower")]
        public string AgentID { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }


        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }
    }
}

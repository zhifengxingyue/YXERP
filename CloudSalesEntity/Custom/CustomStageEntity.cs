using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity
{
    public class CustomStageEntity
    {
        [Property("Lower")]
        public string StageID { get; set; }

        public string StageName { get; set; }

        public int Sort { get; set; }

        public int Mark { get; set; }

        [Property("Lower")]
        public string PID { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public DateTime CreateUserID { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

    }

    public class StageItemEntity
    {
        [Property("Lower")]
        public string ItemID { get; set; }

        public string ItemName { get; set; }

        [Property("Lower")]
        public string StageID { get; set; }

        public DateTime CreateTime { get; set; }

        [Property("Lower")]
        public DateTime CreateUserID { get; set; }

        [Property("Lower")]
        public string ClientID { get; set; }

        public void FillData(System.Data.DataRow dr)
        {
            dr.FillData(this);
        }

    }
}

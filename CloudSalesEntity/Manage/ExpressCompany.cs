using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEntity.Manage
{
    public class ExpressCompany
    {
        public int AutoID { get; set; }

        public string ExpressID { get; set; }

        public string Name { get; set; }

        public string Website { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUserID { get; set; }
    }
}

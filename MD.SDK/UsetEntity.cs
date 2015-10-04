using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.SDK
{
    public class UsetEntity
    {
        public int error_code = 0;

        public string id
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public string avatar
        {
            get;
            set;
        }

        public string email
        {
            get;
            set;
        }

        public string mobile_phone
        {
            get;
            set;
        }

        public string work_phone
        {
            get;
            set;
        }

        public ProjectEntity project
        {
            get;
            set;
        }
    }
}

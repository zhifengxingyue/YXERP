using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSalesEnum
{
    public enum ActivityStatus
    {
        All = -1,
        /// <summary>
        /// 已结束
        /// </summary>
        End = 1,
        /// <summary>
        /// 进行中
        /// </summary>
        Runing = 2,
        /// <summary>
        /// 未开始
        /// </summary>
        NoBegin = 3
    }
}

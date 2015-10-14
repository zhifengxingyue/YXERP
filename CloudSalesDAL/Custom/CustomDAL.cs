using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSalesDAL
{
    public class CustomDAL : BaseDAL
    {
        public static CustomDAL BaseProvider = new CustomDAL();

        #region 查询

        public DataTable GetCustomSources(string clientid)
        {
            SqlParameter[] paras = { 
                                       new SqlParameter("@ClientID",clientid)
                                   };

            DataTable dt = GetDataTable("select * from CustomSource where ClientID=@ClientID", paras, CommandType.Text);

            return dt;
        }

        public DataTable GetCustomSourceByID(string sourceid)
        {
            string sqlText = "select * from CustomSource where SourceID=@SourceID";
            SqlParameter[] paras = { 
                                     new SqlParameter("@SourceID",sourceid)
                                   };

            return GetDataTable(sqlText, paras, CommandType.Text);
        }

        #endregion

        #region 添加

        public bool CreateCustomSource(string sourceid, string sourcecode, string sourcename, int ischoose, string userid, string clientid)
        {
            string sqlText = @"insert into CustomSource(SourceID,SourceName,SourceCode,IsChoose,Status,CreateUserID,ClientID)
                                values(@SourceID,@SourceName,@SourceCode,@IsChoose,1,@CreateUserID,@ClientID)";
            SqlParameter[] paras = { 
                                     new SqlParameter("@SourceID",sourceid),
                                     new SqlParameter("@SourceName",sourcename),
                                     new SqlParameter("@SourceCode",sourcecode),
                                     new SqlParameter("@IsChoose",ischoose),
                                     new SqlParameter("@CreateUserID" , userid),
                                     new SqlParameter("@ClientID" , clientid)
                                   };

            return ExecuteNonQuery(sqlText, paras, CommandType.Text) > 0;
        }

        #endregion

        #region 编辑/删除

        #endregion
    }
}

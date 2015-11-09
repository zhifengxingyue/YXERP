﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using CloudSalesDAL;
using CloudSalesEntity;
using System.Data;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class CustomBusiness
    {
        public static CustomBusiness BaseBusiness = new CustomBusiness();

        #region 查询

        /// <summary>
        /// 公司规模
        /// </summary>
        /// <returns></returns>
        public static List<ExtentEntity> GetExtents()
        {
            List<ExtentEntity> list = new List<ExtentEntity>();
            list.Add(new ExtentEntity() { ExtentID = "1", ExtentName = "0-49人" });
            list.Add(new ExtentEntity() { ExtentID = "2", ExtentName = "50-99人" });
            list.Add(new ExtentEntity() { ExtentID = "3", ExtentName = "100-199人" });
            list.Add(new ExtentEntity() { ExtentID = "4", ExtentName = "200-499人" });
            list.Add(new ExtentEntity() { ExtentID = "5", ExtentName = "500-999人" });
            list.Add(new ExtentEntity() { ExtentID = "6", ExtentName = "1000人以上" });
            return list;
        }


        public List<CustomerEntity> GetCustomers(EnumSearchType searchtype, int type, string sourceid, string stageid, int status, int mark, string activityid, string searchuserid, string searchteamid, string searchagentid,
                                                 string begintime, string endtime, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string userid, string agentid, string clientid)
        {
            List<CustomerEntity> list = new List<CustomerEntity>();
            DataSet ds = CustomDAL.BaseProvider.GetCustomers((int)searchtype, type, sourceid, stageid, status, mark, activityid, searchuserid, searchteamid, searchagentid, begintime, endtime, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, userid, agentid, clientid);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CustomerEntity model = new CustomerEntity();
                model.FillData(dr);

                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Source = SystemBusiness.BaseBusiness.GetCustomSourcesByID(model.SourceID, model.AgentID, model.ClientID);
                model.Stage = SystemBusiness.BaseBusiness.GetCustomStageByID(model.StageID, model.AgentID, model.ClientID);
                list.Add(model);
            }
            return list;
        }

        public CustomerEntity GetCustomerByID(string customerid, string agentid, string clientid)
        {
            DataSet ds = CustomDAL.BaseProvider.GetCustomerByID(customerid, agentid, clientid);
            CustomerEntity model = new CustomerEntity();
            if (ds.Tables["Customer"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Customer"].Rows[0]);
                model.Owner = OrganizationBusiness.GetUserByUserID(model.OwnerID, model.AgentID);
                model.Source = SystemBusiness.BaseBusiness.GetCustomSourcesByID(model.SourceID, model.AgentID, model.ClientID);
                model.Stage = SystemBusiness.BaseBusiness.GetCustomStageByID(model.StageID, model.AgentID, model.ClientID);
                if (model.Extent > 0)
                {
                    model.ExtentStr = GetExtents().Where(m => m.ExtentID == model.Extent.ToString()).FirstOrDefault().ExtentName;
                }

                model.City = CommonBusiness.Citys.Where(m => m.CityCode == model.CityCode).FirstOrDefault();

                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, model.AgentID);

                if (!string.IsNullOrEmpty(model.IndustryID))
                {
                    model.Industry = Manage.IndustryBusiness.GetIndustryDetail(model.IndustryID);
                }
                if (ds.Tables["Activity"].Rows.Count > 0)
                {
                    model.Activity = new ActivityEntity();
                    model.Activity.FillData(ds.Tables["Activity"].Rows[0]);
                }

                if (ds.Tables["Contact"].Rows.Count > 0)
                {
                    model.Contacts = new List<ContactEntity>();
                    foreach (DataRow dr in ds.Tables["Contact"].Rows)
                    {
                        ContactEntity con = new ContactEntity();
                        con.FillData(dr);
                        model.Contacts.Add(con);
                    }
                    
                }
            }
            return model;
        }

        #endregion

        #region 添加

        public string CreateCustomer(string name, int type, string sourceid, string activityid, string industryid, int extent, string citycode, string address, 
                                     string contactname, string mobile, string officephone, string email, string jobs, string desc, string ownerid, string operateid, string agentid, string clientid)
        {
            string id = Guid.NewGuid().ToString();
            bool bl = CustomDAL.BaseProvider.CreateCustomer(id, name, type, sourceid, activityid, industryid, extent, citycode, address, contactname, mobile, officephone, email, jobs, desc, ownerid, operateid, agentid, clientid);
            if (!bl)
            {
                id = "";
            }
            return id;
        }

        #endregion

        #region 编辑/删除

        public bool UpdateCustomer(string customerid, string name, int type, string industryid, int extent, string citycode, string address, string mobile, string officephone, string email, string jobs, string desc, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomer(customerid, name, type, industryid, extent, citycode, address, mobile, officephone, email, jobs, desc, operateid, agentid, clientid);
            if (!bl)
            {
                string msg = "编辑客户信息";
                LogBusiness.AddCustomerLog(customerid, msg, operateid, ip, "", agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerStage(string customerid, string stageid, string operateid, string ip,string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomerStage(customerid, stageid, operateid, agentid, clientid);
            if (bl)
            {
                var model = SystemBusiness.BaseBusiness.GetCustomStageByID(stageid, agentid, clientid);
                string msg = "客户阶段更换为：" + model.StageName;
                LogBusiness.AddCustomerLog(customerid, msg, operateid, ip, stageid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerOwner(string customerid, string userid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomerOwner(customerid, userid, operateid, agentid, clientid);
            if (bl)
            {
                var model = OrganizationBusiness.GetUserByUserID(userid, agentid);
                string msg = "客户拥有者更换为：" + model.Name;
                LogBusiness.AddCustomerLog(customerid, msg, operateid, ip, userid, agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerAgent(string customerid, string newagentid, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CustomDAL.BaseProvider.UpdateCustomerAgent(customerid, newagentid, operateid, agentid, clientid);
            return bl;
        }

        public bool UpdateCustomerStatus(string customerid, EnumCustomStatus status, string operateid, string ip, string agentid, string clientid)
        {

            bool bl = CommonBusiness.Update("Customer", "Status", (int)status, "CustomerID='" + customerid + "'");
            if (bl)
            {
                var model = CommonBusiness.GetEnumDesc(status);
                string msg = "客户状态更换为：" + model;
                LogBusiness.AddCustomerLog(customerid, msg, operateid, ip, status.ToString(), agentid, clientid);
            }
            return bl;
        }

        public bool UpdateCustomerMark(string customerid, int mark, string operateid, string ip, string agentid, string clientid)
        {
            bool bl = CommonBusiness.Update("Customer", "Mark", mark, "CustomerID='" + customerid + "'");
            if (bl)
            {
                string msg = "标记客户颜色";
                LogBusiness.AddCustomerLog(customerid, msg, operateid, ip, mark.ToString(), agentid, clientid);
            }
            return bl;
        }

        #endregion
    }
}

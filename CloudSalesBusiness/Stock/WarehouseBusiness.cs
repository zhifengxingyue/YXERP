﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;


using CloudSalesDAL;
using CloudSalesEntity;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class WarehouseBusiness
    {

        public static object SingleLock = new object();

        #region 库别-弃用

        /// <summary>
        /// 获取库别列表
        /// </summary>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public List<WareHouseType> GetWarehouseTypes(string clientid)
        {
            var dal = new WarehouseDAL();
            DataTable dt = dal.GetWarehouseTypes(clientid);
            List<WareHouseType> list = new List<WareHouseType>();
            foreach (DataRow item in dt.Rows)
            {
                WareHouseType model = new WareHouseType();
                model.FillData(item);
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 添加库别
        /// </summary>
        /// <param name="name">库别名称</param>
        /// <param name="description">描述</param>
        /// <param name="operateid">操作人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public string AddWarehouseType(string name, string description, string operateid, string clientid)
        {
            var id = Guid.NewGuid().ToString();
            var dal = new WarehouseDAL();
            if (dal.AddWarehouseType(id, name, description, operateid, clientid))
            {
                return id.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 编辑库别
        /// </summary>
        /// <param name="id">库别ID</param>
        /// <param name="name">库别名称</param>
        /// <param name="description">描述</param>
        /// <param name="operateid">操作人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public bool UpdateWarehouseType(string id, string name, string description, string operateid, string clientid)
        {
            var dal = new WarehouseDAL();
            return dal.UpdateWarehouseType(id, name, description);

        }
        /// <summary>
        /// 删除库别
        /// </summary>
        /// <param name="id">库别ID</param>
        /// <param name="operateid">操作人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public bool DeleteWarehouseType(string id, string operateid, string clientid)
        {
            return CommonBusiness.Update("WareHouseType", "Status", (int)CloudSalesEnum.EnumStatus.Delete, " TypeID='" + id + "'");
        }
        #endregion

        #region 查询

        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <param name="keyWords">关键词</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总记录数</param>
        /// <param name="pageCount">总页数</param>
        /// <param name="clientID">客户端ID</param>
        /// <returns></returns>
        public List<WareHouse> GetWareHouses(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new WarehouseDAL();
            DataSet ds = dal.GetWareHouses(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<WareHouse> list = new List<WareHouse>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                WareHouse model = new WareHouse();
                model.FillData(dr);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 获取所有仓库（ID和Name）
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public List<WareHouse> GetWareHouses(string clientID)
        {
            var dal = new WarehouseDAL();
            DataTable dt = dal.GetWareHouses(clientID);

            List<WareHouse> list = new List<WareHouse>();
            foreach (DataRow dr in dt.Rows)
            {
                WareHouse model = new WareHouse();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 根据ID获取仓库详情
        /// </summary>
        /// <param name="wareid"></param>
        /// <returns></returns>
        public WareHouse GetWareByID(string wareid)
        {
            var dal = new WarehouseDAL();
            DataTable dt = dal.GetWareByID(wareid);

            WareHouse model = new WareHouse();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
            }
            return model;
        }

        /// <summary>
        /// 获取货位列表
        /// </summary>
        /// <param name="keyWords">关键词</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="totalCount">总记录数</param>
        /// <param name="pageCount">总页数</param>
        /// <param name="clientID">客户端ID</param>
        /// <returns></returns>
        public List<DepotSeat> GetDepotSeats(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID, string wareid = "")
        {
            var dal = new WarehouseDAL();
            DataSet ds = dal.GetDepotSeats(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID, wareid);

            List<DepotSeat> list = new List<DepotSeat>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DepotSeat model = new DepotSeat();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }
        /// <summary>
        /// 根据仓库ID获取货位
        /// </summary>
        /// <param name="wareid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public List<DepotSeat> GetDepotSeatsByWareID(string wareid, string clientid)
        {
            var dal = new WarehouseDAL();
            DataTable dt = dal.GetDepotSeatsByWareID(wareid);

            List<DepotSeat> list = new List<DepotSeat>();
            foreach (DataRow dr in dt.Rows)
            {
                DepotSeat model = new DepotSeat();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }


        /// <summary>
        /// 获取货位详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DepotSeat GetDepotByID(string depotid)
        {
            var dal = new WarehouseDAL();
            DataTable dt = dal.GetDepotByID(depotid);

            DepotSeat model = new DepotSeat();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            return model;
        }

        #endregion

        #region 添加

        /// <summary>
        /// 添加仓库
        /// </summary>
        /// <param name="warecode">仓库编码</param>
        /// <param name="name">名称</param>
        /// <param name="shortname">简称</param>
        /// <param name="citycode">所在地区编码</param>
        /// <param name="status">状态</param>
        /// <param name="description">描述</param>
        /// <param name="operateid">操作人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public string AddWareHouse(string warecode, string name, string shortname, string citycode, int status, string description, string operateid, string clientid)
        {
            var id = Guid.NewGuid().ToString();
            var dal = new WarehouseDAL();
            if (dal.AddWareHouse(id, warecode, name, shortname, citycode, status, description, operateid, clientid))
            {
                return id.ToString();
            }
            return string.Empty;
        }
        /// <summary>
        /// 添加货位
        /// </summary>
        /// <param name="depotcode">货位编码</param>
        /// <param name="wareid">仓库ID</param>
        /// <param name="name">名称</param>
        /// <param name="status">状态</param>
        /// <param name="description">描述</param>
        /// <param name="operateid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public string AddDepotSeat(string depotcode, string wareid, string name, int status, string description, string operateid, string clientid)
        {
            var id = Guid.NewGuid().ToString();
            var dal = new WarehouseDAL();
            if (dal.AddDepotSeat(id, depotcode, wareid, name, status, description, operateid, clientid))
            {
                return id.ToString();
            }
            return string.Empty;
        }
        #endregion

        #region 编辑、删除
        /// <summary>
        /// 编辑仓库
        /// </summary>
        /// <param name="id">仓库ID</param>
        /// <param name="name">名称</param>
        /// <param name="shortname">简称</param>
        /// <param name="citycode">地区编码</param>
        /// <param name="status">状态</param>
        /// <param name="description">描述</param>
        /// <param name="operateid">操作人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public bool UpdateWareHouse(string id, string name, string shortname, string citycode, int status, string description, string operateid, string clientid)
        {
            var dal = new WarehouseDAL();
            return dal.UpdateWareHouse(id, name, shortname, citycode, status, description);
        }

        /// <summary>
        /// 编辑仓库状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="operateid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool UpdateWareHouseStatus(string id, EnumStatus status, string operateid, string clientid)
        {
            return CommonBusiness.Update("WareHouse", "Status", (int)status, " WareID='" + id + "'");
        }

        /// <summary>
        /// 编辑货位
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">名称</param>
        /// <param name="status">状态</param>
        /// <param name="description">描述</param>
        /// <param name="operateid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool UpdateDepotSeat(string id, string name, int status, string description, string operateid, string clientid)
        {
            var dal = new WarehouseDAL();
            return dal.UpdateDepotSeat(id, name, status, description);
        }

        /// <summary>
        /// 编辑货位状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="operateid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool UpdateDepotSeatStatus(string id, EnumStatus status, string operateid, string clientid)
        {
            return CommonBusiness.Update("DepotSeat", "Status", (int)status, " DepotID='" + id + "'");
        }

        #endregion
    }
}

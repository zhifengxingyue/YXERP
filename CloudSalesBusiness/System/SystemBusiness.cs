using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CloudSalesEntity;
using System.Data;
using CloudSalesDAL;
using CloudSalesEnum;

namespace CloudSalesBusiness
{
    public class SystemBusiness
    {
        #region Cache

        private static Dictionary<string, List<CustomSourceEntity>> _source;
        private static Dictionary<string, List<CustomStageEntity>> _stages;

        /// <summary>
        /// 客户来源
        /// </summary>
        private static Dictionary<string, List<CustomSourceEntity>> CustomSources
        {
            get
            {
                if (_source == null)
                {
                    _source = new Dictionary<string, List<CustomSourceEntity>>();
                }
                return _source;
            }
            set 
            {
                _source = value;
            }
        }

        /// <summary>
        /// 客户阶段
        /// </summary>
        private static Dictionary<string, List<CustomStageEntity>> CustomStages
        {
            get
            {
                if (_stages == null)
                {
                    _stages = new Dictionary<string, List<CustomStageEntity>>();
                }
                return _stages;
            }
            set
            {
                _stages = value;
            }
        }

        #endregion

        #region 查询

        /// <summary>
        /// 获取客户来源列表
        /// </summary>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public List<CustomSourceEntity> GetCustomSources(string agentid,string clientid)
        {
            if (CustomSources.ContainsKey(clientid)) 
            {
                return CustomSources[clientid];
            }

            List<CustomSourceEntity> list = new List<CustomSourceEntity>();
            DataTable dt = SystemDAL.BaseProvider.GetCustomSources(clientid);
            foreach (DataRow dr in dt.Rows)
            {
                CustomSourceEntity model = new CustomSourceEntity();
                model.FillData(dr);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
                list.Add(model);
            }
            CustomSources.Add(clientid, list);

            return list;

        }
        /// <summary>
        /// 根据ID获取客户来源
        /// </summary>
        /// <param name="sourceid"></param>
        /// <returns></returns>
        public CustomSourceEntity GetCustomSourcesByID(string sourceid, string agentid, string clientid)
        {
            var list = GetCustomSources(agentid, clientid);
            if (list.Where(m => m.SourceID == sourceid).Count() > 0)
            {
                return list.Where(m => m.SourceID == sourceid).FirstOrDefault();
            }

            CustomSourceEntity model = new CustomSourceEntity();
            DataTable dt = SystemDAL.BaseProvider.GetCustomSourceByID(sourceid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);
            }
            CustomSources[clientid].Add(model);
            return model;
        }

        /// <summary>
        /// 获取客户阶段列表
        /// </summary>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public List<CustomStageEntity> GetCustomStages(string agentid, string clientid)
        {
            if (CustomStages.ContainsKey(clientid))
            {
                return CustomStages[clientid].Where(m => m.Status == 1).OrderBy(m => m.Sort).ToList();
            }

            List<CustomStageEntity> list = new List<CustomStageEntity>();
            DataTable dt = SystemDAL.BaseProvider.GetCustomStages(clientid);
            foreach (DataRow dr in dt.Rows)
            {
                CustomStageEntity model = new CustomStageEntity();
                model.FillData(dr);
                list.Add(model);
            }
            CustomStages.Add(clientid, list);

            return list;
        }

        /// <summary>
        /// 根据ID获取客户阶段
        /// </summary>
        /// <param name="stageid"></param>
        /// <returns></returns>
        public CustomStageEntity GetCustomStageByID(string stageid, string agentid, string clientid)
        {
            var list = GetCustomStages(agentid, clientid);
            if (list.Where(m => m.StageID == stageid).Count() > 0)
            {
                return list.Where(m => m.StageID == stageid).FirstOrDefault();
            }

            CustomStageEntity model = new CustomStageEntity();
            DataTable dt = SystemDAL.BaseProvider.GetCustomStageByID(stageid);
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }
            CustomStages[clientid].Add(model);
            return model;
        }

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
            DataSet ds = SystemDAL.BaseProvider.GetWareHouses(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

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
            DataTable dt = SystemDAL.BaseProvider.GetWareHouses(clientID);

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
        public WareHouse GetWareByID(string wareid, string clientid)
        {
            DataTable dt = SystemDAL.BaseProvider.GetWareByID(wareid);

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
            DataSet ds = SystemDAL.BaseProvider.GetDepotSeats(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID, wareid);

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
            DataTable dt = SystemDAL.BaseProvider.GetDepotSeatsByWareID(wareid);

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
            DataTable dt = SystemDAL.BaseProvider.GetDepotByID(depotid);

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
        /// 创建客户来源
        /// </summary>
        /// <param name="sourcecode">来源编码</param>
        /// <param name="sourcename">名称</param>
        /// <param name="ischoose">是否允许选择</param>
        /// <param name="userid">创建人</param>
        /// <param name="clientid">客户端ID</param>
        /// <returns></returns>
        public string CreateCustomSource(string sourcecode, string sourcename, int ischoose, string userid, string agentid, string clientid,out int result)
        {
            string sourceid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateCustomSource(sourceid, sourcecode, sourcename, ischoose , userid, clientid, out result);
            if (bl)
            {
                if (!CustomSources.ContainsKey(clientid)) 
                {
                    GetCustomSources(agentid, clientid);
                }

                CustomSources[clientid].Add(new CustomSourceEntity()
                {
                    SourceID = sourceid.ToLower(),
                    SourceName = sourcename,
                    SourceCode = sourcecode,
                    IsChoose = ischoose,
                    IsSystem = 0,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    CreateUser = OrganizationBusiness.GetUserByUserID(userid, agentid),
                    ClientID = clientid
                });

                return sourceid;
            }
            return "";
        }

        /// <summary>
        /// 添加客户阶段
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sort"></param>
        /// <param name="pid"></param>
        /// <param name="userid"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public string CreateCustomStage(string name, int sort, string pid, string userid, string agentid, string clientid, out int result)
        {
            string stageid = Guid.NewGuid().ToString();

            bool bl = SystemDAL.BaseProvider.CreateCustomStage(stageid, name, sort, pid, userid, clientid, out result);
            if (bl)
            {
                if (!CustomStages.ContainsKey(clientid))
                {
                    GetCustomStages(agentid, clientid);
                }

                var list = CustomStages[clientid].Where(m => m.Sort >= sort && m.Status == 1).ToList();
                foreach (var model in list)
                {
                    model.Sort += 1;
                }

                CustomStages[clientid].Add(new CustomStageEntity()
                {
                    StageID = stageid.ToLower(),
                    StageName = name,
                    Sort = sort,
                    PID = pid,
                    Mark = 0,
                    Status = 1,
                    CreateTime = DateTime.Now,
                    CreateUserID = userid,
                    ClientID = clientid
                });

                return stageid;
            }
            return "";
        }


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
            if (SystemDAL.BaseProvider.AddWareHouse(id, warecode, name, shortname, citycode, status, description, operateid, clientid))
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
            if (SystemDAL.BaseProvider.AddDepotSeat(id, depotcode, wareid, name, status, description, operateid, clientid))
            {
                return id.ToString();
            }
            return string.Empty;
        }

        #endregion

        #region 编辑/删除

        /// <summary>
        /// 编辑客户来源
        /// </summary>
        /// <param name="sourceid"></param>
        /// <param name="sourcename"></param>
        /// <param name="ischoose"></param>
        /// <param name="userid"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool UpdateCustomSource(string sourceid, string sourcename, int ischoose, string userid,string ip, string agentid, string clientid)
        {
            var model = GetCustomSourcesByID(sourceid, agentid, clientid);
            if (string.IsNullOrEmpty(sourcename))
            {
                sourcename = model.SourceName;
            }
            bool bl = SystemDAL.BaseProvider.UpdateCustomSource(sourceid, sourcename, ischoose, clientid);
            if (bl)
            {
                model.SourceName = sourcename;
                model.IsChoose = ischoose;
            }
            return bl;
        }

        /// <summary>
        /// 编辑客户阶段名称
        /// </summary>
        /// <param name="stageid"></param>
        /// <param name="name"></param>
        /// <param name="userid"></param>
        /// <param name="ip"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool UpdateCustomStage(string stageid, string name, string userid, string ip, string agentid, string clientid)
        {
            var model = GetCustomStageByID(stageid, agentid, clientid);

            bool bl = SystemDAL.BaseProvider.UpdateCustomStage(stageid, name, clientid);
            if (bl)
            {
                model.StageName = name;
            }
            return bl;
        }

        /// <summary>
        /// 删除客户来源
        /// </summary>
        /// <param name="sourceid"></param>
        /// <param name="userid"></param>
        /// <param name="ip"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool DeleteCustomSource(string sourceid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetCustomSourcesByID(sourceid, agentid, clientid);
            //系统默认来源不能删除
            if (model.IsSystem == 1)
            {
                return false;
            }
            bool bl = SystemDAL.BaseProvider.DeleteCustomSource(sourceid, clientid);
            if (bl)
            {
                model.Status = 9;
            }
            return bl;
        }

        /// <summary>
        /// 删除客户来源
        /// </summary>
        /// <param name="sourceid"></param>
        /// <param name="userid"></param>
        /// <param name="ip"></param>
        /// <param name="agentid"></param>
        /// <param name="clientid"></param>
        /// <returns></returns>
        public bool DeleteCustomStage(string stageid, string userid, string ip, string agentid, string clientid)
        {
            var model = GetCustomStageByID(stageid, agentid, clientid);
            //新客户和成交客户不能删除
            if (model.Mark != 0)
            {
                return false;
            }
            bool bl = SystemDAL.BaseProvider.DeleteCustomStage(stageid, clientid);
            if (bl)
            {
                model.Status = 9;
                var list = CustomStages[clientid].Where(m => m.Sort > model.Sort && m.Status == 1).ToList();
                foreach (var stage in list)
                {
                    stage.Sort -= 1;
                }
            }
            return bl;
        }

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
        public bool UpdateWareHouse(string id,string code ,string name, string shortname, string citycode, int status, string description, string operateid, string clientid)
        {
            return SystemDAL.BaseProvider.UpdateWareHouse(id, code, name, shortname, citycode, status, description);
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
            return SystemDAL.BaseProvider.UpdateDepotSeat(id, name, status, description);
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

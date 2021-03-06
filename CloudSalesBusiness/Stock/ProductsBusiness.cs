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
    public class ProductsBusiness
    {
        /// <summary>
        /// 文件默认存储路径
        /// </summary>
        public string FILEPATH = CloudSalesTool.AppSettings.Settings["UploadFilePath"];
        public string TempPath = CloudSalesTool.AppSettings.Settings["UploadTempPath"];

        public static object SingleLock = new object();

        #region Cache

        private static Dictionary<string, List<ProductAttr>> _attrs;

        private static Dictionary<string, List<ProductAttr>> ClientAttrs
        {
            get 
            {
                if (_attrs == null)
                {
                    _attrs = new Dictionary<string, List<ProductAttr>>();
                }
                return _attrs;
            }
            set { _attrs = value; }
        }

        #endregion


        #region 查询

        public List<Brand> GetBrandList(string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetBrandList(keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<Brand> list = new List<Brand>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Brand model = new Brand();
                model.FillData(dr);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
                list.Add(model);
            }
            return list;
        }

        public List<Brand> GetBrandList(string clientID)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetBrandList(clientID);

            List<Brand> list = new List<Brand>();
            foreach (DataRow dr in dt.Rows)
            {
                Brand model = new Brand();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public Brand GetBrandByBrandID(string brandID)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetBrandByBrandID(brandID);

            Brand model = new Brand();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
                model.City = CommonBusiness.Citys.Where(c => c.CityCode == model.CityCode).FirstOrDefault();
            }
            return model;
        }

        public List<ProductUnit> GetClientUnits(string clientid)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetClientUnits(clientid);

            List<ProductUnit> list = new List<ProductUnit>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductUnit model = new ProductUnit();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public List<ProductAttr> GetAttrs(string clientid)
        {
            if (ClientAttrs.ContainsKey(clientid))
            {
                return ClientAttrs[clientid];
            }

            List<ProductAttr> list = new List<ProductAttr>();
            DataSet ds = new ProductsDAL().GetAttrs(clientid);
            foreach (DataRow dr in ds.Tables["Attrs"].Rows)
            {
                ProductAttr model = new ProductAttr();
                model.FillData(dr);
                model.AttrValues = new List<AttrValue>();
                foreach (DataRow item in ds.Tables["Values"].Rows)
                {
                    AttrValue attrValue = new AttrValue();
                    attrValue.FillData(item);
                    model.AttrValues.Add(attrValue);
                }
            }
            ClientAttrs.Add(clientid, list);

            return list;
        }

        /// <summary>
        /// 获取属性列表（包括属性值列表）
        /// </summary>
        public List<ProductAttr> GetAttrList(string categoryid, string keyWords, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string agentid, string clientid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetAttrList(categoryid, keyWords, pageSize, pageIndex, ref totalCount, ref pageCount, clientid);

            List<ProductAttr> list = new List<ProductAttr>();
            if (ds.Tables.Contains("Attrs"))
            {
                foreach (DataRow dr in ds.Tables["Attrs"].Rows)
                {
                    ProductAttr model = new ProductAttr();
                    model.FillData(dr);
                    model.CreateUser = OrganizationBusiness.GetUserByUserID(model.CreateUserID, agentid);

                    List<AttrValue> valueList = new List<AttrValue>();
                    foreach (DataRow drValue in ds.Tables["Values"].Select("AttrID='" + model.AttrID + "'"))
                    {
                        AttrValue valueModel = new AttrValue();
                        valueModel.FillData(drValue);
                        valueList.Add(valueModel);
                    }
                    model.AttrValues = valueList;

                    list.Add(model);
                }
            }
            return list;
        }

        public List<ProductAttr> GetAttrList(string categoryid, string clientid)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetAttrList(categoryid, clientid);

            List<ProductAttr> list = new List<ProductAttr>();
            foreach (DataRow dr in dt.Rows)
            {
                ProductAttr model = new ProductAttr();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public ProductAttr GetProductAttrByID(string attrid, string clientid)
        {
            var list = GetAttrs(clientid);
            if (list.Where(m => m.AttrID == attrid).Count() > 0)
            {
                return list.Where(m => m.AttrID == attrid).FirstOrDefault();
            }
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductAttrByID(attrid);

            ProductAttr model = new ProductAttr();
            if (ds.Tables.Contains("Attrs") && ds.Tables["Attrs"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Attrs"].Rows[0]);
                model.AttrValues = new List<AttrValue>();
                foreach (DataRow item in ds.Tables["Values"].Rows)
                {
                    AttrValue attrValue = new AttrValue();
                    attrValue.FillData(item);
                    model.AttrValues.Add(attrValue);
                }
            }

            ClientAttrs[clientid].Add(model);
            
            return model;
        }

        public List<Category> GetChildCategorysByID(string categoryid, string clientid)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetChildCategorysByID(categoryid, clientid);

            List<Category> list = new List<Category>();

            foreach (DataRow dr in dt.Rows)
            {
                Category model = new Category();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 获取产品分类
        /// </summary>
        public Category GetCategoryByID(string categoryid)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetCategoryByID(categoryid);

            Category model = new Category();
            if (dt.Rows.Count > 0)
            {
                model.FillData(dt.Rows[0]);
            }

            return model;
        }

        /// <summary>
        /// 获取产品分类详情（包括属性和值）
        /// </summary>
        public Category GetCategoryDetailByID(string categoryid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetCategoryDetailByID(categoryid);

            Category model = new Category();
            if (ds.Tables.Contains("Category") && ds.Tables["Category"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Category"].Rows[0]);
                List<ProductAttr> salelist = new List<ProductAttr>();
                List<ProductAttr> attrlist = new List<ProductAttr>();

                foreach (DataRow attr in ds.Tables["Attrs"].Rows)
                {

                    ProductAttr modelattr = new ProductAttr();
                    modelattr.FillData(attr);
                    if (modelattr.Type==1)
                    {
                        attrlist.Add(modelattr);
                    }
                    else if (modelattr.Type == 2)
                    {
                        salelist.Add(modelattr);
                    }
                    modelattr.AttrValues = new List<AttrValue>();
                    foreach (DataRow value in ds.Tables["Values"].Select("AttrID='" + modelattr.AttrID + "'"))
                    {
                        AttrValue valuemodel = new AttrValue();
                        valuemodel.FillData(value);
                        modelattr.AttrValues.Add(valuemodel);
                    }
                }

                model.SaleAttrs = salelist;
                model.AttrLists = attrlist;
            }

            return model;
        }

        public List<Products> GetProductList(string categoryid, string beginprice, string endprice, string keyWords, string orderby, bool isasc, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductList(categoryid, beginprice, endprice, keyWords, orderby, isasc ? 1 : 0, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<Products> list = new List<Products>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Products model = new Products();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        public Products GetProductByID(string productid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductByID(productid);

            Products model = new Products();
            if (ds.Tables.Contains("Product") && ds.Tables["Product"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Product"].Rows[0]);
                model.Category = GetCategoryDetailByID(model.CategoryID);
                var bigunit = new ProductUnit();
                bigunit.FillData(ds.Tables["Unit"].Select("UnitID='" + model.BigUnitID + "'").FirstOrDefault());
                model.BigUnit = bigunit;

                var smallunit = new ProductUnit();
                smallunit.FillData(ds.Tables["Unit"].Select("UnitID='" + model.SmallUnitID + "'").FirstOrDefault());
                model.SmallUnit = smallunit;

                model.ProductDetails = new List<ProductDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {
                    //子产品
                    ProductDetail detail = new ProductDetail();
                    detail.FillData(item);

                    Dictionary<string, string> attrs = new Dictionary<string, string>();
                    foreach (string attr in detail.SaleAttrValue.Split(','))
                    {
                        if (!string.IsNullOrEmpty(attr))
                        {
                            attrs.Add(attr.Split(':')[0], attr.Split(':')[1]);
                        }
                    }
                    detail.SaleAttrValueString = "";
                    foreach (var attr in model.Category.SaleAttrs)
                    {
                        if (attrs.ContainsKey(attr.AttrID))
                        {
                            detail.SaleAttrValueString += attr.AttrName + ":" + attr.AttrValues.Where(a => a.ValueID.ToLower() == attrs[attr.AttrID].ToLower()).FirstOrDefault().ValueName + ",";
                        }
                    }

                    if (detail.SaleAttrValueString.Length > 0)
                    {
                        detail.SaleAttrValueString = detail.SaleAttrValueString.Substring(0, detail.SaleAttrValueString.Length - 1);
                    }

                    model.ProductDetails.Add(detail);
                }
            }

            return model;
        }

        public bool IsExistProductCode(string code, string clientid)
        {
            object obj = CommonBusiness.Select("Products", " Count(0) ", "ClientID='" + clientid + "' and ProductCode='" + code + "'");
            return Convert.ToInt32(obj) > 0;
        }

        public List<Products> GetFilterProducts(string categoryid, List<FilterAttr> Attrs, int doctype, string beginprice, string endprice, string keyWords, string orderby, bool isasc, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string clientID)
        {
            var dal = new ProductsDAL();
            StringBuilder attrbuild = new StringBuilder();
            StringBuilder salebuild = new StringBuilder();
            foreach (var attr in Attrs)
            {
                if (attr.Type == EnumAttrType.Parameter)
                {
                    attrbuild.Append(" and p.ValueList like '%" + attr.ValueID + "%'");
                }
                else if (attr.Type == EnumAttrType.Specification)
                {
                    salebuild.Append(" and AttrValue like '%" + attr.ValueID + "%'");
                }
            }

            DataSet ds = dal.GetFilterProducts(categoryid, attrbuild.ToString(), salebuild.ToString(), doctype, beginprice, endprice, keyWords, orderby, isasc ? 1 : 0, pageSize, pageIndex, ref totalCount, ref pageCount, clientID);

            List<Products> list = new List<Products>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Products model = new Products();
                model.FillData(dr);
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 获取产品信息（加入购物车页面）
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public Products GetProductByIDForDetails(string productid)
        {
            var dal = new ProductsDAL();
            DataSet ds = dal.GetProductByIDForDetails(productid);

            Products model = new Products();
            if (ds.Tables.Contains("Product") && ds.Tables["Product"].Rows.Count > 0)
            {
                model.FillData(ds.Tables["Product"].Rows[0]);

                //单位
                model.BigUnit = new ProductUnit();
                model.BigUnit.FillData(ds.Tables["Unit"].Select("UnitID='" + model.BigUnitID + "'").FirstOrDefault());

                model.SmallUnit = new ProductUnit();
                model.SmallUnit.FillData(ds.Tables["Unit"].Select("UnitID='" + model.SmallUnitID + "'").FirstOrDefault());

                model.AttrLists = new List<ProductAttr>();
                model.SaleAttrs = new List<ProductAttr>();

                foreach (DataRow attrtr in ds.Tables["Attrs"].Rows)
                {
                    ProductAttr attrModel = new ProductAttr();
                    attrModel.FillData(attrtr);
                    attrModel.AttrValues = new List<AttrValue>();

                    //参数
                    if (attrModel.Type == (int)EnumAttrType.Parameter)
                    {
                        foreach (DataRow valuetr in ds.Tables["Values"].Select("AttrID='" + attrModel.AttrID + "'"))
                        {
                            AttrValue valueModel = new AttrValue();
                            valueModel.FillData(valuetr);
                            if (model.AttrValueList.IndexOf(valueModel.ValueID) >= 0)
                            {
                                attrModel.AttrValues.Add(valueModel);
                                model.AttrLists.Add(attrModel);
                                break;
                            }
                        }
                       
                    }
                    else
                    {
                        model.SaleAttrs.Add(attrModel);
                    }
                }

                model.ProductDetails = new List<ProductDetail>();
                foreach (DataRow item in ds.Tables["Details"].Rows)
                {

                    ProductDetail detail = new ProductDetail();
                    detail.FillData(item);

                    //填充存在的规格
                    foreach (var attrModel in model.SaleAttrs)
                    {
                        foreach (DataRow valuetr in ds.Tables["Values"].Select("AttrID='" + attrModel.AttrID + "'"))
                        {
                            AttrValue valueModel = new AttrValue();
                            valueModel.FillData(valuetr);
                            if (detail.AttrValue.IndexOf(valueModel.ValueID) >= 0)
                            {
                                if (attrModel.AttrValues.Where(v => v.ValueID == valueModel.ValueID).Count() == 0)
                                {
                                    attrModel.AttrValues.Add(valueModel);
                                }
                                break;
                            }
                        }
                    }
                    model.ProductDetails.Add(detail);
                }
            }

            return model;
        }
        #endregion

        #region 添加

        public string AddBrand(string name, string anotherName, string icoPath, string countryCode, string cityCode, int status, string remark, string brandStyle, string operateIP, string operateID, string clientID)
        {
            lock (SingleLock)
            {
                if (!string.IsNullOrEmpty(icoPath))
                {
                    if (icoPath.IndexOf("?") > 0)
                    {
                        icoPath = icoPath.Substring(0, icoPath.IndexOf("?"));
                    }
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(icoPath));
                    icoPath = FILEPATH + file.Name;
                    if (file.Exists)
                    {
                        file.MoveTo(HttpContext.Current.Server.MapPath(icoPath));
                    }
                }
                
                return new ProductsDAL().AddBrand(name, anotherName, icoPath, countryCode, cityCode, status, remark, brandStyle, operateIP, operateID, clientID);
            }
        }

        public string AddUnit(string unitName, string description,string operateid,string clientid)
        {
            var dal = new ProductsDAL();
            return dal.AddUnit(unitName, description, operateid, clientid);
        }

        public string AddProductAttr(string attrName, string description, string categoryID, int type, string operateid, string clientid)
        {
            var attrID = Guid.NewGuid().ToString().ToLower();
            var dal = new ProductsDAL();
            if (dal.AddProductAttr(attrID, attrName, description, categoryID, type, operateid, clientid))
            {
                if (ClientAttrs.ContainsKey(clientid))
                {
                    ClientAttrs[clientid].Add(new ProductAttr()
                    {
                        AttrID = attrID,
                        AttrName = attrName,
                        Description = description,
                        CategoryID = categoryID,
                        ClientID = clientid,
                        CreateTime = DateTime.Now,
                        CreateUserID = operateid,
                        Status = 1,
                        AttrValues = new List<AttrValue>()
                    });
                }
                return attrID;
            }
            return string.Empty;
        }

        public string AddAttrValue(string valueName, string attrID, string operateid, string clientid)
        {
            var valueID = Guid.NewGuid().ToString().ToLower();
            var dal = new ProductsDAL();
            if (dal.AddAttrValue(valueID, valueName, attrID, operateid, clientid))
            {
                var model = GetProductAttrByID(attrID, clientid);
                model.AttrValues.Add(new AttrValue()
                {
                    ValueID = valueID,
                    ValueName = valueName,
                    Status = 1,
                    AttrID = attrID,
                    ClientID = clientid,
                    CreateTime = DateTime.Now
                });

                return valueID;
            }
            return string.Empty;
        }

        public string AddCategory(string categoryCode, string categoryName, string pid, int status, List<string> attrlist, List<string> saleattr, string description, string operateid, string clientid)
        {
            var dal = new ProductsDAL();
            return dal.AddCategory(categoryCode, categoryName, pid, status, string.Join(",", attrlist), string.Join(",", saleattr), description, operateid, clientid);
        }

        public string AddProduct(string productCode, string productName, string generalName, bool iscombineproduct, string brandid, string bigunitid, string smallunitid, int bigSmallMultiple,
                                 string categoryid, int status, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price, decimal weight, bool isnew,
                                 bool isRecommend, int isallow, int isautosend, int effectiveDays, decimal discountValue, string productImg, string shapeCode, string description, List<ProductDetail> details, string operateid, string clientid)
        {
            lock (SingleLock)
            {
                if (!string.IsNullOrEmpty(productImg))
                {
                    if (productImg.IndexOf("?") > 0)
                    {
                        productImg = productImg.Substring(0, productImg.IndexOf("?"));
                    }
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(productImg));
                    productImg = FILEPATH + file.Name;
                    if (file.Exists)
                    {
                        file.MoveTo(HttpContext.Current.Server.MapPath(productImg));
                    }
                }

                var dal = new ProductsDAL();
                string pid = dal.AddProduct(productCode, productName, generalName, iscombineproduct, brandid, bigunitid, smallunitid, bigSmallMultiple, categoryid, status, attrlist,
                                        valuelist, attrvaluelist, commonprice, price, weight, isnew, isRecommend, isallow, isautosend, effectiveDays, discountValue, productImg, shapeCode, description, operateid, clientid);
                //产品添加成功添加子产品
                if (!string.IsNullOrEmpty(pid))
                {
                    foreach (var model in details)
                    {
                        model.ImgS = "";
                        dal.AddProductDetails(pid, model.DetailsCode, model.ShapeCode, model.SaleAttr, model.AttrValue, model.SaleAttrValue, model.Price, model.Weight, model.BigPrice, model.ImgS, model.Description, operateid, clientid);
                    }
                }
                return pid;
            }
        }
        
        public bool AddCategoryAttr(string categoryid, string attrid, int type, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            return dal.AddCategoryAttr(categoryid, attrid, type, operateID);
        }

        public string AddProductDetails(string productid, string productCode, string shapeCode, string attrlist, string valuelist, string attrvaluelist, decimal price, decimal weight,decimal bigprice, string productImg, string description, string operateid, string clientid)
        {
            lock (SingleLock)
            {
                if (!string.IsNullOrEmpty(productImg))
                {
                    if (productImg.IndexOf("?") > 0)
                    {
                        productImg = productImg.Substring(0, productImg.IndexOf("?"));
                    }
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(productImg));
                    productImg = FILEPATH + file.Name;
                    if (file.Exists)
                    {
                        file.MoveTo(HttpContext.Current.Server.MapPath(productImg));
                    }
                }
                else
                {
                    productImg = FILEPATH + DateTime.Now.ToString("yyyyMMddHHmmssms") + new Random().Next(1000, 9999).ToString() + ".png";
                }

                var dal = new ProductsDAL();
                return dal.AddProductDetails(productid, productCode, shapeCode, attrlist, valuelist, attrvaluelist, price, weight, bigprice, productImg, description, operateid, clientid);
            }
        }

        #endregion

        #region 编辑、删除

        public bool UpdateBrandStatus(string brandID, EnumStatus status, string operateIP, string operateID)
        {
            bool bl = CommonBusiness.Update("Brand", "Status", ((int)status).ToString(), " BrandID='" + brandID + "'");

            if (bl)
            {
                string message = "编辑品牌状态为：" + CommonBusiness.GetEnumDesc(status);
                LogBusiness.AddOperateLog(operateID, "ProductsBusiness.UpdateBrandStatus", EnumLogType.Update, EnumLogModules.Stock, EnumLogEntity.Brand, brandID, message, operateIP);
            }

            return bl;
        }

        public bool UpdateBrand(string brandID, string name, string anotherName, string countryCode, string cityCode, string icopath, int status, string remark, string brandStyle, string operateIP, string operateID)
        {
            if (!string.IsNullOrEmpty(icopath) && icopath.IndexOf(TempPath) >= 0)
            {
                if (icopath.IndexOf("?") > 0)
                {
                    icopath = icopath.Substring(0, icopath.IndexOf("?"));
                }
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(icopath));
                icopath = FILEPATH + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(icopath));
                }
            }
            var dal = new ProductsDAL();
            return dal.UpdateBrand(brandID, name, anotherName, countryCode, cityCode, status, icopath, remark, brandStyle, operateIP, operateID);
        }

        public bool UpdateUnit(string unitID, string unitName, string desciption, string operateID)
        {
            var dal = new ProductsDAL();
            DataTable dt = dal.GetUnitByUnitID(unitID);
            string message = "单位名称“" + dt.Rows[0]["unitName"].ToString() + "”变更为“" + unitName + "”；描述“" + dt.Rows[0]["Description"].ToString() + "”变更为“" + desciption + "”";
            LogBusiness.AddOperateLog(operateID, "ProductsBusiness.UpdateUnit", EnumLogType.Update, EnumLogModules.Stock, EnumLogEntity.ProductUnit, unitID, message,"");
            return dal.UpdateUnit(unitID, unitName, desciption);
        }

        public bool UpdateUnitStatus(string unitID, EnumStatus status, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            return dal.UpdateUnitStatus(unitID, (int)status);
        }

        public bool UpdateProductAttr(string attrID, string attrName, string description, string operateIP, string operateID,string clientid)
        {
            var dal = new ProductsDAL();
            var bl = dal.UpdateProductAttr(attrID, attrName, description);
            if (bl)
            {
                var model = GetProductAttrByID(attrID, clientid);
                model.AttrName = attrName;
                model.Description = description;
            }
            return bl;
        }

        public bool UpdateAttrValue(string valueID, string attrid, string valueName, string operateIP, string operateID, string clientid)
        {
            var dal = new ProductsDAL();
            var bl = dal.UpdateAttrValue(valueID, valueName);
            if (bl)
            {
                var model = GetProductAttrByID(attrid, clientid);
                var value = model.AttrValues.Where(m => m.ValueID == valueID).FirstOrDefault();
                value.ValueName = valueName;
            }
            return bl;
        }

        public bool UpdateProductAttrStatus(string attrid, EnumStatus status, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            return dal.UpdateProductAttrStatus(attrid, (int)status);
        }

        public bool UpdateCategoryAttrStatus(string categoryid, string attrid, EnumStatus status, int type, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            return dal.UpdateCategoryAttrStatus(categoryid, attrid, (int)status, type);
        }

        public bool UpdateAttrValueStatus(string valueid, EnumStatus status, string operateIP, string operateID)
        {
            var dal = new ProductsDAL();
            return dal.UpdateAttrValueStatus(valueid, (int)status);
        }

        public bool UpdateCategory(string categoryid, string categoryName, int status, List<string> attrlist, List<string> saleattr, string description, string operateid)
        {
            var dal = new ProductsDAL();
            return dal.UpdateCategory(categoryid, categoryName, status, string.Join(",", attrlist), string.Join(",", saleattr), description, operateid);
        }

        public bool DeleteCategory(string categoryid, string operateid, string ip, string agentid, string clientid, out int result)
        {
            var dal = new ProductsDAL();
            return dal.DeleteCategory(categoryid, operateid, out result);
        }

        public bool UpdateProductStatus(string productid, EnumStatus status, string operateIP, string operateID)
        {
            return CommonBusiness.Update("Products", "Status", ((int)status).ToString(), " ProductID='" + productid + "'");
        }

        public bool UpdateProductIsNew(string productid, bool isNew, string operateIP, string operateID)
        {
            return CommonBusiness.Update("Products", "IsNew", isNew ? "1" : "0", " ProductID='" + productid + "'");
        }

        public bool UpdateProductIsRecommend(string productid, bool isRecommend, string operateIP, string operateID)
        {
            return CommonBusiness.Update("Products", "IsRecommend", isRecommend ? "1" : "0", " ProductID='" + productid + "'");
        }

        public bool UpdateProduct(string productid,string productCode, string productName, string generalName, bool iscombineproduct, string brandid, string bigunitid, string smallunitid, int bigSmallMultiple,
                         int status, string categoryid, string attrlist, string valuelist, string attrvaluelist, decimal commonprice, decimal price, decimal weight, bool isnew,
                         bool isRecommend, int isallow, int isautosend, int effectiveDays, decimal discountValue, string productImg, string shapeCode, string description, string operateid, string clientid)
        {

            if (!string.IsNullOrEmpty(productImg) && productImg.IndexOf(TempPath) >= 0)
            {
                if (productImg.IndexOf("?") > 0)
                {
                    productImg = productImg.Substring(0, productImg.IndexOf("?"));
                }
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(productImg));
                productImg = FILEPATH + file.Name;
                if (file.Exists)
                {
                    file.MoveTo(HttpContext.Current.Server.MapPath(productImg));
                }
            }

            var dal = new ProductsDAL();
            return dal.UpdateProduct(productid, productCode, productName, generalName, iscombineproduct, brandid, bigunitid, smallunitid, bigSmallMultiple, status, categoryid,attrlist,
                                    valuelist, attrvaluelist, commonprice, price, weight, isnew, isRecommend, isallow, isautosend, effectiveDays, discountValue, productImg, shapeCode, description, operateid, clientid);
        }

        public bool UpdateProductDetailsStatus(string productdetailid, EnumStatus status, string operateIP, string operateID)
        {
            return CommonBusiness.Update("ProductDetail", "Status", (int)status, " ProductDetailID='" + productdetailid + "'");
        }

        public bool UpdateProductDetails(string detailid, string productid, string productCode, string shapeCode, decimal bigPrice, string attrlist, string valuelist, string attrvaluelist, decimal price, decimal weight, string description, string productImg, string operateid, string clientid)
        {
            lock (SingleLock)
            {
                if (!string.IsNullOrEmpty(productImg) && productImg.IndexOf(TempPath) >= 0)
                {
                    if (productImg.IndexOf("?") > 0)
                    {
                        productImg = productImg.Substring(0, productImg.IndexOf("?"));
                    }
                    FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(productImg));
                    productImg = FILEPATH + file.Name;
                    if (file.Exists)
                    {
                        file.MoveTo(HttpContext.Current.Server.MapPath(productImg));
                    }
                }
                var dal = new ProductsDAL();
                return dal.UpdateProductDetails(detailid, productid, productCode, shapeCode, bigPrice, attrlist, valuelist, attrvaluelist, price, weight, description, productImg);
            }
        }

        #endregion
    }
}

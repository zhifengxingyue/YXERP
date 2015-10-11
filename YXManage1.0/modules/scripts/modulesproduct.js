

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot");
    var VerifyObject;
    var ModulesProduct = {};
   
    //模块产品详情初始化
    ModulesProduct.detailInit = function (id) {
        ModulesProduct.detailEvent();

        if (id != 0) {
            $("#pageTitle").html("设置模块产品");
            $("#saveModulesProduct").val("保存");
            ModulesProduct.getModulesProductDetail(id);
        }
    }
    //绑定事件
    ModulesProduct.detailEvent = function () {
        ModulesProduct.Params = {
            pageIndex: 1,
            autoID: $("#id").val()
        };


        //验证插件
        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        //保存模块产品
        $("#saveModulesProduct").click(function () {

            if (!VerifyObject.isPass()) {
                return false;
            };

            var modulesProduct = {
                AutoID: $("#id").val(),
                ModulesID: $("#ModulesID").val(),
                Period: $("#Period").val(),
                PeriodQuantity: $("#PeriodQuantity").val(),
                UserQuantity: $("#UserQuantity").val(),
                Price: $("#Price").val(),
                Description: $("#Description").val()
            };

            Global.post("/ModulesProduct/SaveModulesProduct", { modulesProduct: JSON.stringify(modulesProduct) }, function (data) {
                if (data.Result == "1") {
                    location.href = "/ModulesProduct/Index";
                } else if (data.Result == "2") {
                    //alert("登陆账号已存在!");
                    //$("#loginName").val("");
                }
            });
        });
    };

    //模块产品详情
    ModulesProduct.getModulesProductDetail = function (id) {
        Global.post("/ModulesProduct/GetModulesProductDetail", { id: id }, function (data) {
            if (data.Result == "1") {
                var item = data.Item;
                $("#ModulesID").val(item.ModulesID);
                $("#Period").val(item.Period);
                $("#PeriodQuantity").val(item.PeriodQuantity);
                $("#UserQuantity").val(item.UserQuantity);
                $("#Price").val(item.Price);
                $("#Description").val(item.Description);

            } else if (data.Result == "2") {
                alert("登陆账号已存在!");
                $("#loginName").val("");
            }
        });
    };

    //模块产品列表初始化
    ModulesProduct.init = function () {
        ModulesProduct.Params = {
            pageIndex: 1
        };
        ModulesProduct.bindEvent();
        ModulesProduct.bindData();
    };
    //绑定事件
    ModulesProduct.bindEvent = function () {

    };
    //绑定数据
    ModulesProduct.bindData = function () {
        var _self = this;
        $("#client-header").nextAll().remove();
        Global.post("/ModulesProduct/GetModulesProducts", ModulesProduct.Params, function (data) {
            doT.exec("template/modulesproduct_list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $("#client-header").after(innerText);

                $(".table-list a.ico-del").bind("click", function () {
                    if (confirm("确定删除?"))
                    {
                        Global.post("/ModulesProduct/DeleteModulesProduct", { id: $(this).attr("data-id") }, function (data) {
                            if (data.Result == 1) {
                                location.href = "/ModulesProduct/Index";
                            }
                            else {
                                alert("删除失败");
                            }
                        });
                    }
                });
            });
            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: ModulesProduct.Params.pageIndex,
                display: 5,
                border: true,
                border_color: '#fff',
                text_color: '#333',
                background_color: '#fff',
                border_hover_color: '#ccc',
                text_hover_color: '#000',
                background_hover_color: '#efefef',
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    ModulesProduct.Params.pageIndex = page;
                    ModulesProduct.bindData();
                }
            });
        });
    }

    module.exports = ModulesProduct;
});
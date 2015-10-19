

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Verify = require("verify"),
        Global = require("global"),
        doT = require("dot");
    var VerifyObject;
    var Industry = {};
   
    //模块产品详情初始化
    Industry.detailInit = function (id) {
        Industry.detailEvent();

        if (id != 0) {
            $("#pageTitle").html("设置公司行业");
            $("#saveIndustry").val("保存");
            Industry.getIndustryDetail(id);
        }
    }
    //绑定事件
    Industry.detailEvent = function () {
        Industry.Params = {
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
        $("#saveIndustry").click(function () {

            if (!VerifyObject.isPass()) {
                return false;
            };

            var industry = {
                IndustryID: $("#IndustryID").val(),
                Name: $("#Name").val(),
                Description: $("#Description").val()
            };

            Global.post("/Industry/SaveIndustry", { industry: JSON.stringify(industry) }, function (data) {
                if (data.Result == "1") {
                    location.href = "/Industry/Index";
                } else if (data.Result == "2") {
                    //alert("登陆账号已存在!");
                }
            });
        });
    };

    //模块产品详情
    Industry.getIndustryDetail = function (id) {
        Global.post("/Industry/GetIndustryDetail", { id: id }, function (data) {
            if (data.Result == "1") {
                var item = data.Item;
                $("#Name").val(item.Name);
                $("#Description").val(item.Description);

            } else if (data.Result == "2") {
                alert("登陆账号已存在!");
                $("#loginName").val("");
            }
        });
    };

    //模块产品列表初始化
    Industry.init = function () {
        Industry.Params = {
            pageIndex: 1
        };
        Industry.bindEvent();
        Industry.bindData();
    };
    //绑定事件
    Industry.bindEvent = function () {

    };
    //绑定数据
    Industry.bindData = function () {
        var _self = this;
        $("#client-header").nextAll().remove();
        Global.post("/Industry/GetIndustrys", null, function (data) {
            doT.exec("template/Industry_list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $("#client-header").after(innerText);
            });

        });
    }

    module.exports = Industry;
});
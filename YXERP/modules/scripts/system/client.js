define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Upload = require("upload"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject;

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();

        ObjectJS.getDetail();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {

        //选择海报图片
        PosterIco = Upload.createUpload({
            element: "#Logo",
            buttonText: "选择LOGO",
            className: "",
            data: { folder: '/Content/tempfile/', action: 'add', oldPath: "" },
            success: function (data, status) {
                if (data.Items.length > 0) {
                    $("#PosterDisImg").show();
                    $("#PosterDisImg").attr("src", data.Items[0]);
                    $("#CompanyLogo").val(data.Items[0]);
                }
            }
        });

        //城市插件
        CityObject = City.createCity({
            elementID: "citySpan"
        });

        //切换
        $(".search-stages li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                $(".content-body div[name='clientInfo']").hide().eq(parseInt(_this.data("id"))).show();
            }
        });

        //保存公司基本信息
        $("#btnSaveClient").click(function () {
            ObjectJS.saveModel();
        });
    }

    //获取详情
    ObjectJS.getDetail = function () {
        var _self = this;
        Global.post("/System/GetClientDetail", null, function (data) {
            if (data.Item) {
                var item = data.Item;
                //基本信息
                $("#CompanyName").val(item.CompanyName);
                $("#ContactName").val(item.ContactName);
                $("#MobilePhone").val(item.MobilePhone);
                $("#OfficePhone").val(item.OfficePhone);
                $("#Industry").val(item.Industry);
                if (item.City)
                    CityObject.setValue(item.City.CityCode);
                if (item.Logo)
                {
                    $("#PosterDisImg").show().attr("src", item.Logo);
                    $("#CompanyLogo").val(item.Logo);
                }
                $("#Address").val(item.Address);
                $("#Description").val(item.Description);

                //授权信息
                $("#UserQuantity").html(item.UserQuantity);
                $("#EndTime").html(item.EndTime.toDate("yyyy-MM-dd"));
            }
        })
    }

    //保存实体
    ObjectJS.saveModel = function () {
        var _self = this;
        var model = {
            CompanyName: $("#CompanyName").val(),
            Logo:$("#CompanyLogo").val(),
            ContactName: $("#ContactName").val(),
            MobilePhone: $("#MobilePhone").val(),
            OfficePhone: $("#OfficePhone").val(),
            CityCode: CityObject.getCityCode(),
            Industry: $("#Industry").val(),
            Address: $("#Address").val(),
            Description:$("#Description").val()
        };

        Global.post("/System/SaveClient", { entity: JSON.stringify(model) }, function (data) {
            if (data.Result == 1) {
                alert("保存成功");
            }
        })
    }


    module.exports = ObjectJS;
});
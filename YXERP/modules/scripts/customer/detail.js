define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject;


    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (customerid) {
        var _self = this;
        Global.post("/Customer/GetCustomerByID", { customerid: customerid }, function (data) {
            if (data.model.CustomerID) {
                _self.bindCustomerInfo(data.model);
                _self.bindEvent(data.model);
            }
        });
        
    }
    ObjectJS.bindCustomerInfo = function (model) {
        $("#spCustomerName").html(model.Name);
        $("#lblMobile").text(model.MobilePhone || "--");
        $("#lblEmail").text(model.Email || "--");
        $("#lblIndustry").text(model.Industry ? model.Industry.Name : "--");
        $("#lblExtent").text(model.ExtentStr || "--");
        $("#lblCity").text(model.City ? model.City.Province + " " + model.City.City + " " + model.City.Counties : "--");
        $("#lblAddress").text(model.Address || "--");
        $("#lblTime").text(model.CreateTime.toDate("yyyy-MM-dd hh:mm:ss"));
        $("#lblUser").text(model.CreateUser ? model.CreateUser.Name : "--");

        if (model.Type == 0) {
            $(".companyinfo").hide();
        } else {
            $(".companyinfo").show();
        }
    }
    //绑定事件
    ObjectJS.bindEvent = function (model) {
        var _self = this;

        if (model.Status == 1) {
            $("#lblStatus").text("正常").addClass("normal");

            $("#recoveryCustomer").hide();

            $("#loseCustomer").click(function () {
                confirm("确认更换客户状态为丢失吗?", function () {
                    Global.post("/Customer/LoseCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                    });
                });
            });

            $("#closeCustomer").click(function () {
                confirm("确认关闭此客户吗?", function () {
                    Global.post("/Customer/CloseCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                    });
                });
            });

        } else if (model.Status == 2 || model.Status == 3) {
            $("#lblStatus").text(model.Status ? "已关闭" : "已丢失").addClass("red");

            $("#loseCustomer").hide();
            $("#closeCustomer").hide();

            $("#recoveryCustomer").click(function () {
                confirm("确认恢复此客户吗?", function () {
                    Global.post("/Customer/RecoveryCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                    });
                });
            });

        } else if (model.Status == 9) {
            $("#lblStatus").text("已删除");

            $("#loseCustomer").hide();
            $("#closeCustomer").hide();
            $("#recoveryCustomer").hide();
        }

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });
        CityObject = City.createCity({
            elementID: "city"
        });
        //切换类型
        $(".customtype").click(function () {
            var _this = $(this);
            if (!_this.hasClass("ico-checked")) {
                $(".customtype").removeClass("ico-checked").addClass("ico-check");
                _this.addClass("ico-checked").removeClass("ico-check");
                if (_this.data("type") == 1) {
                    $(".company").show();
                } else {
                    $(".company").hide();
                }
            }
        });

        $("#name").focus();
    }

    //保存实体
    ObjectJS.saveModel = function (activityid) {
        var _self = this;

        var model = {
            Name: $("#name").val().trim(),
            Type: $("#companyCustom").hasClass("ico-checked") ? 1 : 0,
            IndustryID: $("#industry").val().trim(),
            ActivityID: activityid,
            SourceID: $("#source").val().trim(),
            Extent: $("#extent").val().trim(),
            CityCode: CityObject.getCityCode(),
            Address: $("#address").val().trim(),
            ContactName: $("#contactName").val().trim(),
            MobilePhone: $("#contactMobile").val().trim(),
            Email: $("#email").val().trim(),
            Jobs: $("#jobs").val().trim(),
            Description: $("#remark").val().trim()
        };
        Global.post("/Customer/SaveCustomer", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.CustomerID) {
                confirm("客户保存成功,是否继续添加客户?", function () {
                    location.href = location.href;
                }, function () {
                    location.href = "/Customer/MyCustomer";
                })
                
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }

    module.exports = ObjectJS;
});
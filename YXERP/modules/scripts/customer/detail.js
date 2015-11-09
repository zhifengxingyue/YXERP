define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject,
        Verify = require("verify"), VerifyObject,
        doT = require("dot"),
        ChooseUser = require("chooseuser"),
        Easydialog = require("easydialog");


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

        $("#lblSource").text(model.Source ? model.Source.SourceName : "--");

        $("#lblOwner").text(model.Owner ? model.Owner.Name : "--");
        $("#changeOwner").data("userid", model.OwnerID);

        $("#lblReamrk").text(model.Description);

        if (model.Type == 0) {
            $("#lblType").html("个")
            $(".companyinfo").hide();
        } else {
            $("#lblType").html("企")
            $(".companyinfo").show();
        }
    }
    //绑定事件
    ObjectJS.bindEvent = function (model) {
        var _self = this;

        $("#updateCustomer").click(function () {
            _self.editCustomer(model);
        });

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

        $("#changeOwner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "更换拥有者",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("userid") != items[0].id) {
                            Global.post("/Customer/UpdateCustomOwner", {
                                userid: items[0].id,
                                ids: model.CustomerID
                            }, function (data) {
                                if (data.status) {
                                    _this.data("userid", items[0].id);
                                    $("#lblOwner").text(items[0].name);
                                }
                            });
                        } else {
                            alert("请选择不同人员进行转移!");
                        }
                    }
                }
            });
        });

    }

    ObjectJS.editCustomer = function (model) {
        var _self = this;
        doT.exec("template/customer/customer-detail.html", function (template) {
            var innerText = template(model);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: "编辑客户信息",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        var entity = {
                            CustomerID: model.CustomerID,
                            Name: $("#name").val().trim(),
                            Type: $("#companyCustom").hasClass("ico-checked") ? 1 : 0,
                            IndustryID: $("#industry").val().trim(),
                            Extent: $("#extent").val().trim(),
                            CityCode: CityObject.getCityCode(),
                            Address: $("#address").val().trim(),
                            MobilePhone: $("#contactMobile").val().trim(),
                            Email: $("#email").val().trim(),
                            Description: $("#remark").val().trim()
                        };
                        _self.saveModel(entity);
                    },
                    callback: function () {

                    }
                }
            });

            CityObject = City.createCity({
                cityCode: model.CityCode,
                elementID: "city"
            });
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });

            $("#extent").val(model.Extent);

            $("#industry").val(model.IndustryID);

            if (model.Type == 0) {
                $(".edit-company").hide();
            }
            //切换类型
            $(".customtype").click(function () {
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    $(".customtype").removeClass("ico-checked").addClass("ico-check");
                    _this.addClass("ico-checked").removeClass("ico-check");
                    if (_this.data("type") == 1) {
                        $(".edit-company").show();
                    } else {
                        $(".edit-company").hide();
                    }
                }
            });
        });
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;

        Global.post("/Customer/SaveCustomer", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.CustomerID) {
                location.href = location.href;
                
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }

    module.exports = ObjectJS;
});
define(function (require, exports, module) {
    var Global = require("global"),
        City = require("city"), CityObject, CityContact,
        Verify = require("verify"), VerifyPay, VerifyContact,
        doT = require("dot"),
        Easydialog = require("easydialog");
    require("pager");

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (id) {
        var _self = this;
        _self.billingid = id;
        Global.post("/Finance/GetPayableBillByID", { id: id }, function (data) {
            if (data.model.BillingID) {
                _self.bindInfo(data.model);
                _self.bindEvent(data.model);
            }
        });

        $("#addInvoice").hide();
    }

    //基本信息
    ObjectJS.bindInfo = function (model) {

        var _self = this;

        $("#infoBillingCode").html("账单编号：" + model.BillingCode);
        $("#infoSourceCode").attr("href", $("#infoSourceCode").attr("href") + "/" + model.DocID).html(model.DocCode);
        $("#infoTotalMoney").html(model.TotalMoney.toFixed(2));
        $("#infoPayMoney").html(model.PayMoney.toFixed(2));
        $("#infoInvoiceMoney").html(model.InvoiceMoney.toFixed(2));
        $("#infoCreateTime").html(model.CreateTime.toDate("yyyy-MM-dd hh:mm:ss"));
        $("#infoCreateUser").html(model.CreateUser ? model.CreateUser.Name : "--");

        _self.getPays(model.StorageBillingPays, true)
    }

    //绑定事件
    ObjectJS.bindEvent = function (model) {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        })
        //付款登记
        $("#addPay").click(function () {
            _self.addPay();
        });

        if (model.Status == 1) {
            $("#lblStatus").text("正常").addClass("normal");

            $("#recoveryCustomer").hide();

            //丢失客户
            $("#loseCustomer").click(function () {
                confirm("确认更换客户状态为丢失吗?", function () {
                    Global.post("/Customer/LoseCustomer", { ids: model.CustomerID }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        }
                    });
                });
            });
            //关闭客户
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
            //恢复客户
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

        //切换模块
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".nav-partdiv").hide();
            $("#" + _this.data("id")).show();

            $("#addContact").hide();

            if (_this.data("id") == "navLog" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getLogs(model.CustomerID, 1);
            } else if (_this.data("id") == "navContact") {
                $("#addContact").show();
                if ((!_this.data("first") || _this.data("first") == 0)) {
                    _this.data("first", "1");
                    _self.getContacts(model.CustomerID);
                }
            } else if (_this.data("id") == "navOrder" && (!_this.data("first") || _this.data("first") == 0)) {
                _this.data("first", "1");
                _self.getOrders(model.CustomerID, 1);
            }
        });


        $("#editContact").click(function () {
            var _this = $(this);
            Global.post("/Customer/GetContactByID", { id: _this.data("id") }, function (data) {
                _self.addContact(data.model);
            });
        });

        //删除联系人
        $("#deleteContact").click(function () {
            var _this = $(this);
            confirm("确认删除此联系人吗？", function () {
                Global.post("/Customer/DeleteContact", { id: _this.data("id") }, function (data) {
                    if (data.status) {
                        _self.getContacts(_self.customerid);
                    } else {
                        alert("网络异常,请稍后重试!");
                    }
                });
            });
        });

    }
    //绑定支付列表
    ObjectJS.getPays = function (items, empty) {
        var _self = this;
        if (empty) {
            $("#navPays .tr-header").nextAll().remove();
        }
        doT.exec("template/finance/storagebillingpays.html", function (template) {
            var innerhtml = template(items);
            innerhtml = $(innerhtml);

            $("#navPays .tr-header").after(innerhtml);
        });
    }

    //登记付款
    ObjectJS.addPay = function () {
        var _self = this;
        doT.exec("template/finance/payable-detail.html", function (template) {
            var innerText = template();
            Easydialog.open({
                container: {
                    id: "show-pays-detail",
                    header: "付款登记",
                    content: innerText,
                    yesFn: function () {
                        if (!VerifyPay.isPass()) {
                            return false;
                        }
                        var entity = {
                            BillingID: _self.billingid,
                            Type: 1,
                            PayType: 1,
                            PayMoney: $("#paymoney").val().trim(),
                            PayTime: $("#paytime").val().trim(),
                            Remark: $("#remark").val().trim()
                        };
                        _self.savePayablePay(entity);
                    },
                    callback: function () {

                    }
                }
            });

            $("#paymoney").focus();

            laydate({
                elem: '#paytime',
                format: 'YYYY-MM-DD',
                min: '1900-01-01',
                max: laydate.now(),
                istime: false,
                istoday: true
            });
            $("#paytime").val(Date.now().toString().toDate("yyyy-MM-dd"));

            VerifyPay = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }
    //保存付款
    ObjectJS.savePayablePay = function (model) {
        var _self = this;
        Global.post("/Finance/SaveStorageBillingPay", { entity: JSON.stringify(model) }, function (data) {
            if (data.item) {
                $("#infoPayMoney").html(($("#infoPayMoney").html() * 1 + data.item.PayMoney).toFixed(2));
                _self.getPays([data.item], false)
            } else {
                alert("网络异常,请稍后重试!");
            }
        });
    }

    module.exports = ObjectJS;
});
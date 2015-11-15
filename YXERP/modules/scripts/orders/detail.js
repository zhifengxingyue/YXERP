
define(function (require, exports, module) {
    var City = require("city"), CityObj,
        Global = require("global"),
        doT = require("dot"),
        ChooseUser = require("chooseuser");
    require("pager");
    var ObjectJS = {};

    ObjectJS.init = function (orderid, status) {
        var _self = this;
        _self.orderid = orderid;
        _self.status = status;
        _self.bindEvent();
        _self.getAmount();
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        //转移拥有者
        $("#changeOwner").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "更换拥有者",
                type: 1,
                single: true,
                callback: function (items) {
                    if (items.length > 0) {
                        if (_this.data("userid") != items[0].id) {
                            Global.post("/Orders/UpdateOrderOwner", {
                                userid: items[0].id,
                                ids: _this.data("id")
                            }, function (data) {
                                if (data.status) {
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

        if (_self.status == 1) {
            $("#btnreturn").hide();
            //编辑单价
            $(".price").change(function () {
                var _this = $(this);
                if (_this.val().isDouble() && _this.val() > 0) {
                    _self.editPrice(_this);
                } else {
                    _this.val(_this.data("value"));
                }
            });

            $("#btndelete").click(function () {
                confirm("订单删除后不可恢复，确认删除吗？", function () {
                    _self.deleteOrder();
                });
            });

            $("#btnconfirm").click(function () {
                confirm("确认审核订单吗？", function () {
                    Global.post("/Orders/EffectiveOrder", { orderid: _self.orderid }, function (data) {
                        if (data.status) {
                            location.href = location.href;
                        } else {
                            if (data.result = 0) {
                                alert("订单审核失败，可能因为订单状态已改变，请刷新页面后重试！");
                            } else {
                                alert("订单审核失败！");
                            }
                        }
                    });
                });
            });

        } else if (_self.status == 2) {
            $("#lblStatus").addClass("normal");
            $("#btnconfirm,#btndelete").hide();
            $(".cart-item").find(".tr-price input").prop("disabled", true);
        } else {
            $("#lblStatus").addClass("red");
            $("#btnconfirm,#btndelete,#btnreturn").hide();
            $(".cart-item").find(".tr-price input").prop("disabled", true);
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
                _self.getLogs(1);
            }
        });
        
    }
    //计算总金额
    ObjectJS.getAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            _this.html((_this.prevAll(".tr-quantity").find("label").text() * _this.prevAll(".tr-price").find("input").val()).toFixed(2));
            amount += _this.html() * 1;
        });
        $("#amount").text(amount.toFixed(2));
    }
    //更改数量
    ObjectJS.editPrice = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateOrderPrice", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            price: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("价格修改失败，可能因为订单状态已改变，请刷新页面后重试！");
            } else {
                ele.parent().nextAll(".amount").html((ele.parent().prevAll(".tr-quantity").find("label").text() * ele.val()).toFixed(2));
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }
    //删除订单
    ObjectJS.deleteOrder = function () {
        var _self = this;
        Global.post("/Orders/DeleteOrder", { orderid: _self.orderid }, function (data) {
            if (data.status) {
                location.href = location.href;
            } else {
                alert("订单删除失败，可能因为订单状态已改变，请刷新页面后重试！");
            }
        });
    }
    //获取日志
    ObjectJS.getLogs = function (page) {
        var _self = this;
        $("#customerLog").empty();
        Global.post("/Orders/GetOrderLogs", {
            orderid: _self.orderid,
            pageindex: page
        }, function (data) {

            doT.exec("template/common/logs.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);
                $("#orderLog").append(innerhtml);
            });
            $("#pagerLogs").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: page,
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
                float: "left",
                onChange: function (page) {
                    _self.getLogs(customerid, page);
                }
            });
        });
    }
    module.exports = ObjectJS;
})
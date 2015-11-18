
define(function (require, exports, module) {
    var City = require("city"), CityObj,
        Global = require("global"),
        ChooseUser = require("chooseuser");

    var ObjectJS = {};
    //添加页初始化
    ObjectJS.init = function (orderid) {
        var _self=this;
        _self.orderid = orderid;
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

        //编辑数量
        $(".quantity").change(function () {
            if ($(this).val().isInt() && $(this).val() > 0) {
                _self.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });
        //编辑单价
        $(".price").change(function () {
            var _this = $(this);
            if (_this.val().isInt() && _this.val() > 0) {
                _this.parent().nextAll(".amount").html((_this.parent().nextAll(".tr-quantity").find("input").val() * _this.val()).toFixed(2));
                _this.data("value", _this.val());
                _self.getAmount();
            } else {
                _this.val(_this.data("value"));
            }
        });
        //删除产品
        $(".ico-del").click(function () {
            var _this = $(this);
            confirm("确认从购物车移除此产品吗？", function () {
                Global.post("/ShoppingCart/DeleteCart", {
                    autoid: _this.data("id")
                }, function (data) {
                    if (!data.Status) {
                        alert("系统异常，请重新操作！");
                    } else {
                        _this.parents("tr.item").remove();
                        _self.getAmount();
                    }
                });
            });
        });

        //提交订单
        $("#btnconfirm").click(function () {
            confirm("请确认订单信息是否填写正确，提交后只能编辑价格，确认提交吗？", function () {
                _self.submitOrder();
            });
            
        });

        $("#btndelete").click(function () {
            confirm("订单删除后不可恢复，确认删除吗？", function () {
                _self.deleteOrder();
            });
        });

        CityObj = City.createCity({
            elementID: "city"
        });
        
    }
    //计算总金额
    ObjectJS.getAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            _this.html((_this.prevAll(".tr-quantity").find("input").val() * _this.prevAll(".tr-price").find("label").text()).toFixed(2));
            amount += _this.html() * 1;
        });
        $("#amount").text(amount.toFixed(2));
    }
    //更改数量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        Global.post("/ShoppingCart/UpdateCartQuantity", {
            autoid: ele.data("id"),
            quantity: ele.val()
        }, function (data) {
            if (!data.Status) {
                ele.val(ele.data("value"));
                alert("系统异常，请重新操作！");
            } else {
                ele.parent().nextAll(".amount").html((ele.parent().prevAll(".tr-price").find("input").val() * ele.val()).toFixed(2));
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //保存
    ObjectJS.submitOrder = function () {
        var _self = this;
        var totalamount = 0, bl = false;
        //单据明细
        $(".cart-item").each(function () {
            bl = true;
        });
        if (!bl) {
            return;
        }
        var entity = {
            OrderID: _self.orderid,
            PersonName: $("#personName").val().trim(),
            MobileTele: $("#mobileTele").val().trim(),
            CityCode: CityObj.getCityCode(),
            Address: $("#address").val().trim(),
            TypeID: $("#orderType").val().trim(),
            ExpressType: $("#expressType").val().trim(),
            Remark: $("#remark").val().trim()
        };
        Global.post("/Orders/SubmitOrder", { entity: JSON.stringify(entity) }, function (data) {
            if (data.status) {
                location.href = location.href;
            } else {
                location.href = location.href;
            }
        })
    }

    //删除订单
    ObjectJS.deleteOrder = function () {
        var _self = this;
        Global.post("/Orders/DeleteOrder", { orderid: _self.orderid }, function (data) {
            if (data.status) {
                location.href = "/Orders/MyOrder";
            } else {
                alert("订单删除失败，可能因为订单状态已改变，请刷新页面后重试！");
            }
        });
    }

    module.exports = ObjectJS;
})
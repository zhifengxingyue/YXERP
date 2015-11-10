define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject;

    var Model = {};

    var ObjectJS = {};

    ObjectJS.Params = {
        PageSize: 10,
        PageIndex: 1,
        KeyWords: "",
        IsAll: 0,
        Stage: -1,
        BeginTime: "",
        EndTime: "",
        FilterType: 0,
        DisplayType: 1
    };

    //初始化列表
    ObjectJS.init = function (isAll) {
        var _self = this;

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        _self.bindEvent();

        _self.getList();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        
        //选择人数
        $("#UserCount").blur(function () {
            var quantity = $(this).val();
            if (quantity == '') return false;

            Global.post("/Auction/GetBestWay",
                {
                    quantity: quantity,
                    periodQuantity:$("#UserYear").val()
                },
                function (data) {
                    var yearCount = $("#UserYear").val();
                    $(".productListTB td.tdBG2").removeClass("tdBG2").find("span").remove();

                    for (var i = 0; len = data.Items.length, i < len; i++) {
                        var item = data.Items[i];
                        var productCountHtml = '';
                        if (item.count > 1)
                            productCountHtml = '<span class="mLeft15" style="font-size:16px;color:#fff;">×' + item.count + '</span>';
                        $(".productListTB td[data-productID='" + item.id + "'][data-year='" + yearCount + "']").addClass("tdBG2").append(productCountHtml).data("productCount", item.count);
                    }

                    //统计产品的人数、金额
                    ObjectJS.getTotalPrice();

            });
        });

        //选择年份
        $("#UserYear").change(function () {
            $(".productListTB td.tdBG2").each(function () {
                var id = $(this).data("id");
                var year = $(this).data("year");
                var productCount = $(this).data("productCount");

                var yearCount = $("#UserYear").val();
                if (year != yearCount)
                {
                    $(this).removeClass("tdBG2");
                    var $span = null;
                    if ($(this).find("span").length > 0) {
                        $span = $(this).find("span");
                        $(this).find("span").remove();
                    }
                    $(".productListTB td[data-id='" + id + "'][data-year='" + yearCount + "']").addClass("tdBG2").append($span).data("productCount", productCount);
                }

            });

            //统计产品的人数、金额
            ObjectJS.getTotalPrice();

        });

        //进入确认订单
        $("#btn_sureOrder").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            };

            $(".productList").hide();
            $(".productOrder").show();

            $(".selectProduct").css("background", "url('/modules/images/auction/bg-select-product.png') center center");
            $(".selectProduct").children().eq(0).addClass("stepIcoFinish");
            $(".selectProduct").children().eq(1).addClass("stepDesFinish");

            $(".sureOrder").css("background", "url('/modules/images/auction/bg-sure-order-active.png') center center");
            $(".sureOrder").children().eq(0).addClass("stepIcoActive");
            $(".sureOrder").children().eq(1).addClass("stepDesActive");

            $("#orderUserCount").html($("#UserCount").val());
            $("#orderYear").html($("#UserYear").val());
            $("#orderPrice").html($("#Price").html());
        });

        //返回选择产品
        $("#btn_backSelectPrpduct").click(function () {
            $(".productList").show();
            $(".productOrder").hide();

            $(".selectProduct").css("background", "url('/modules/images/auction/bg-select-product-active.png') center center");
            $(".selectProduct").children().eq(0).removeClass("stepIcoFinish");
            $(".selectProduct").children().eq(1).removeClass("stepDesFinish");

            $(".sureOrder").css("background", "url('/modules/images/auction/bg-sure-order.png') center center");
            $(".sureOrder").children().eq(0).removeClass("stepIcoActive");
            $(".sureOrder").children().eq(1).removeClass("stepDesActive");

        });

        //进入支付订单
        $("#btn_payOrder").click(function () {
            $(".productOrder").hide();
            $(".payProductOrder").show();

            $(".sureOrder").css("background", "url('/modules/images/auction/bg-sure-order.png') center center");
            $(".sureOrder").children().eq(0).addClass("stepIcoFinish");
            $(".sureOrder").children().eq(1).addClass("stepDesFinish");

            $(".payOrder").css("background", "url('/modules/images/auction/bg-pay-order-active.png') center center");
            $(".payOrder").children().eq(0).addClass("stepIcoActive");
            $(".payOrder").children().eq(1).addClass("stepDesActive");

            ObjectJS.addClientOrder();
        });
    }

    //获取总金额、人数
    ObjectJS.getTotalPrice = function () {
        //统计产品的人数、金额
        var $arr = $(".productListTB td.tdBG2");
        var len = $arr.length;
        var userCount = 0;
        var yearCount = 0;
        var totalPrice = 0;
        for (var i = 0; i < len; i++) {
            userCount += (parseInt($arr.eq(i).data("usercount")) * parseInt($arr.eq(i).data("productCount")));
            totalPrice += (parseInt($arr.eq(i).data("price")) * parseInt($arr.eq(i).data("productCount")));
            yearCount = parseInt($arr.eq(i).data("year"));
        }
        $("#UserCount").val(userCount);
        $("#Price").html(totalPrice);
    }

    //获取产品列表
    ObjectJS.getList = function () {
        var _self = this;
        Global.post("/Auction/GetProductList",
            null,
            function (data) {
                var len = data.Items.length;
                var html5 = '<tr><td class="tdBG">5人</td>';
                var html = '<tr><td class="tdBG">10人</td>';
                var html2 = '<tr><td class="tdBG">20人</td>';
                var html3 = '<tr><td class="tdBG">50人</td>';
                var html4 = '<tr class="trLast"><td class="tdBG">100人</td>';

                for (var i = 0; i < len; i++) {
                    var item = data.Items[i];
                    if (item.UserQuantity == 5)
                    {
                        html5 += '<td  name="productItem" data-id="1" data-productID="' + item.ProductID + '" data-usercount="5" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + item.Price;

                        html += '</td>';
                    }
                    else if (item.UserQuantity == 10)
                    {
                        html += '<td  name="productItem" data-id="2" data-productID="' + item.ProductID + '" data-usercount="10" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + item.Price;
                    
                        html += '</td>';
                    }
                    else if (item.UserQuantity == 20)
                    {
                        html2 += '<td  name="productItem" data-id="3" data-productID="' + item.ProductID + '" data-usercount="20" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + item.Price;

                        html += '</td>';
                    }
                    else if (item.UserQuantity == 50)
                    {
                        html3 += '<td  name="productItem" data-id="4" data-productID="' + item.ProductID + '" data-usercount="50" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + item.Price;

                        html += '</td>';
                    }
                    else if (item.UserQuantity == 100)
                    {
                        html4 += '<td  name="productItem" data-id="5" data-productID="' + item.ProductID + '" data-usercount="100" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + item.Price;

                        html += '</td>';
                    }
                }

                html += '</tr>';
                html2 += '</tr>';
                html3 += '</tr>';
                html4 += '</tr>';
                html5 += '</tr>';
                $(".productListTB").append(html5).append(html).append(html2).append(html3).append(html4);

                //$(".productListTB td[name='productItem']").click(function () {

                //    if ($(this).hasClass("tdBG2"))
                //        $(this).removeClass("tdBG2");
                //    else
                //    {
                //        var productID = $(this).data("id");
                //        var yearCount = $(this).data("year");
                //        $("#productID").val(productID);
                //        $("#yearCount").val(yearCount);

                //        $(".productListTB td.tdBG2").each(function () {
                //            var id = $(this).data("id");
                //            var year = $(this).data("year");

                //            var productID = $("#productID").val();
                //            var yearCount = $("#yearCount").val();

                //            if (year != yearCount) {
                //                $(this).removeClass("tdBG2");
                //                $(".productListTB td[data-id='" + id + "'][data-year='" + yearCount + "']").addClass("tdBG2");
                //            }

                //        });

                //        $(this).addClass("tdBG2");
                //    }

                //    //统计产品的人数、年数、金额
                //    var $arr = $(".productListTB td.tdBG2");
                //    var len = $arr.length;
                //    var userCount = 0;
                //    var yearCount = 0;
                //    var totalPrice = 0;
                //    for (var i = 0; i < len; i++) {
                //        userCount += parseInt($arr.eq(i).data("usercount"));
                //        totalPrice += parseInt($arr.eq(i).data("price"));
                //        yearCount = parseInt($arr.eq(i).data("year"));
                //    }
                //    $("#UserCount").val(userCount);
                //    $("#Price").html(totalPrice);
                //    $("#UserYear").val(yearCount);


                //});

            }
        );
    }

    //根据人数、年数生成客户订单
    ObjectJS.addClientOrder = function () {
        var _self = this;
        Global.post("/Auction/AddClientOrder",
            {
                quantity: $("#UserCount").val(),
                periodQuantity: $("#UserYear").val()
            },
            function (data) {
                if (data.ID)
                {
                    
                }


            }
        );
    }
    module.exports = ObjectJS;
});
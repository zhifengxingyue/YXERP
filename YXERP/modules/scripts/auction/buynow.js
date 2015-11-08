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
        

        //进入确认订单
        $("#btn_sureOrder").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            };

            $(".productList").hide();
            $(".productOrder").show();

            $(".selectProduct").css("background", "url('/modules/images/auction/bg-select-product.png') center center");
            $(".selectProduct").children().eq(0).removeClass("stepIcoActive");
            $(".selectProduct").children().eq(1).removeClass("stepDesActive");

            $(".sureOrder").css("background", "url('/modules/images/auction/bg-sure-order-active.png') center center");
            $(".sureOrder").children().eq(0).addClass("stepIcoActive");
            $(".sureOrder").children().eq(1).addClass("stepDesActive");
        });

        //进入支付订单
        $("#btn_payOrder").click(function () {
            $(".productOrder").hide();
            $(".payProductOrder").show();

            $(".sureOrder").css("background", "url('/modules/images/auction/bg-sure-order.png') center center");
            $(".sureOrder").children().eq(0).removeClass("stepIcoActive");
            $(".sureOrder").children().eq(1).removeClass("stepDesActive");

            $(".payOrder").css("background", "url('/modules/images/auction/bg-pay-order-active.png') center center");
            $(".payOrder").children().eq(0).addClass("stepIcoActive");
            $(".payOrder").children().eq(1).addClass("stepDesActive");
        });

        //选择人数
        $("#UserCount").blur(function(){

        });

        //选择年份
        $("#UserYear").change(function () {
                var yearCount = $(this).val();
                $("#yearCount").val(yearCount);

                $(".productListTB td.tdBG2").each(function () {
                    var id = $(this).data("id");
                    var year = $(this).data("year");

                    var productID = $("#productID").val();
                    var yearCount = $("#yearCount").val();

                    if (year != yearCount)
                    {
                        $(this).removeClass("tdBG2");
                        $(".productListTB td[data-id='" + id + "'][data-year='" + yearCount + "']").addClass("tdBG2");
                    }

                });

                //统计产品的人数、年数、金额
                var $arr = $(".productListTB td.tdBG2");
                var len = $arr.length;
                var userCount = 0;
                var yearCount = 0;
                var totalPrice = 0;
                for (var i = 0; i < len; i++) {
                    userCount += parseInt($arr.eq(i).data("usercount"));
                    totalPrice += parseInt($arr.eq(i).data("price"));
                    yearCount = parseInt($arr.eq(i).data("year"));
                }
                $("#UserCount").val(userCount);
                $("#Price").html(totalPrice);


            });


    }

    //获取产品列表
    ObjectJS.getList = function () {
        var _self = this;
        Global.post("/Auction/GetProductList",
            null,
            function (data) {
                var len = data.Items.length;
                var html = '<tr><td class="tdBG">10人</td>';
                var html2 = '<tr><td class="tdBG">20人</td>';
                var html3 = '<tr><td class="tdBG">50人</td>';
                var html4 = '<tr class="trLast"><td class="tdBG">100人</td>';

                for (var i = 0; i < len; i++) {
                    var item = data.Items[i];
                    if (item.UserQuantity == 10)
                    {
                        html += '<td  name="productItem" data-id="1" data-usercount="10" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + item.Price + '</td>';
                    }
                    else if (item.UserQuantity == 20)
                    {
                        html2 += '<td  name="productItem" data-id="2" data-usercount="20" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + item.Price + '</td>';
                    }
                    else if (item.UserQuantity == 50)
                    {
                        html3 += '<td  name="productItem" data-id="3" data-usercount="50" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + item.Price + '</td>';
                    }
                    else if (item.UserQuantity == 100)
                    {
                        html4 += '<td  name="productItem" data-id="4" data-usercount="100" data-price="' + item.Price + '" data-year="' + item.PeriodQuantity + '">' + item.Price + '</td>';
                    }
                }
                html += '</tr>';
                html2 += '</tr>';
                html3 += '</tr>';
                html4 += '</tr>';
                $(".productListTB").append(html).append(html2).append(html3).append(html4);

                $(".productListTB td[name='productItem']").click(function () {

                    if ($(this).hasClass("tdBG2"))
                        $(this).removeClass("tdBG2");
                    else
                    {
                        var productID = $(this).data("id");
                        var yearCount = $(this).data("year");
                        $("#productID").val(productID);
                        $("#yearCount").val(yearCount);

                        $(".productListTB td.tdBG2").each(function () {
                            var id = $(this).data("id");
                            var year = $(this).data("year");

                            var productID = $("#productID").val();
                            var yearCount = $("#yearCount").val();

                            if (year != yearCount) {
                                $(this).removeClass("tdBG2");
                                $(".productListTB td[data-id='" + id + "'][data-year='" + yearCount + "']").addClass("tdBG2");
                            }

                        });

                        $(this).addClass("tdBG2");
                    }

                    //统计产品的人数、年数、金额
                    var $arr = $(".productListTB td.tdBG2");
                    var len = $arr.length;
                    var userCount = 0;
                    var yearCount = 0;
                    var totalPrice = 0;
                    for (var i = 0; i < len; i++) {
                        userCount += parseInt($arr.eq(i).data("usercount"));
                        totalPrice += parseInt($arr.eq(i).data("price"));
                        yearCount = parseInt($arr.eq(i).data("year"));
                    }
                    $("#UserCount").val(userCount);
                    $("#Price").html(totalPrice);
                    $("#UserYear").val(yearCount);


                });

            }
        );
    }

    //加载列表
    ObjectJS.bindList = function (items) {
        if (items.length > 0) {
            var _self = this;
            if (ObjectJS.Params.DisplayType == 1) {
                doT.exec("template/activity/activity_list.html", function (template) {
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);

                    //操作
                    innerhtml.find(".dropdown").click(function () {
                        var _this = $(this);
                        var position = _this.find(".ico-dropdown-white").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));

                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 39 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    });

                    $(".activityList").html(innerhtml);

                    require.async("businesscard", function () {
                        $(".activitymember").businessCard();
                    });

                });
            }
            else {
                doT.exec("template/activity/activity_card_list.html", function (template) {
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);

                    //操作
                    innerhtml.find(".dropdown").click(function () {
                        var _this = $(this);
                        var position = _this.find(".ico-dropdown-white").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));

                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 30 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    });

                    $(".activityCardList").html(innerhtml);

                    require.async("businesscard", function () {
                        $(".activitymember").businessCard();
                    });
                });
            }


        }
        else {
            $(".tr-header").after("<tr><td colspan='7' style='padding:15px 0px;'><div style='margin:0px auto; width:300px;'><div class='left' style='padding-top:4px;'>暂无数据！</div><div class='left'><a href='/Activity/Detail' class='ico-add  mTop4'>添加活动</a></div><div class='clear'></div></div></td></tr>");
        }
    }

    module.exports = ObjectJS;
});
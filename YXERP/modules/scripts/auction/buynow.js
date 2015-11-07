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

        

        //_self.getList();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(".productListTB td[name='productItem']").click(function () {
            if ($(this).hasClass("tdBG2"))
                $(this).removeClass("tdBG2");
            else
                $(this).addClass("tdBG2");
        });

        //进入确认订单
        $("#btn_step1").click(function () {
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
        $("#btn_step2").click(function () {
            $(".productOrder").hide();
            $(".payProductOrder").show();

            $(".sureOrder").css("background", "url('/modules/images/auction/bg-sure-order.png') center center");
            $(".sureOrder").children().eq(0).removeClass("stepIcoActive");
            $(".sureOrder").children().eq(1).removeClass("stepDesActive");

            $(".payOrder").css("background", "url('/modules/images/auction/bg-pay-order-active.png') center center");
            $(".payOrder").children().eq(0).addClass("stepIcoActive");
            $(".payOrder").children().eq(1).addClass("stepDesActive");
        });

    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        Global.post("/Activity/GetActivityList",
            {
                pageSize: ObjectJS.Params.PageSize,
                pageIndex: ObjectJS.Params.PageIndex,
                keyWords: ObjectJS.Params.KeyWords,
                isAll: ObjectJS.Params.IsAll,
                beginTime: ObjectJS.Params.BeginTime,
                endTime: ObjectJS.Params.EndTime,
                stage: ObjectJS.Params.Stage,
                filterType: ObjectJS.Params.FilterType
            },
            function (data) {
                _self.bindList(data.Items);

                $("#pager").paginate({
                    total_count: data.TotalCount,
                    count: data.PageCount,
                    start: _self.Params.PageIndex,
                    display: 5,
                    images: false,
                    mouse: 'slide',
                    onChange: function (page) {
                        _self.Params.PageIndex = page;
                        _self.getList();
                    }
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
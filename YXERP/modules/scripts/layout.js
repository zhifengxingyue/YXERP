﻿/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        doT = require("dot"),
        Global = require("global");

    var LayoutObject = {};
    //初始化数据
    LayoutObject.init = function () {
        LayoutObject.bindStyle();
        LayoutObject.bindEvent();
    }

    //绑定事件
    LayoutObject.bindEvent = function () {
        //调整浏览器窗体
        $(window).resize(function () {
            //Height = document.documentElement.clientHeight, Width = document.documentElement.clientWidth;
            LayoutObject.bindStyle();
        });

        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("modules") && !$(e.target).hasClass("modules")) {
                $("#choosemodules").removeClass("hover");
                $(".choose-modules").fadeOut("1000");
            }
            if (!$(e.target).parents().hasClass("currentuser") && !$(e.target).hasClass("currentuser")) {
                $(".dropdown-userinfo").fadeOut("1000");
            }
        });

        //选择一级菜单
        $("#choosemodules").click(function () {
            var _this = $(this);

            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                if ($(".choose-modules").length == 0) {
                    Global.post("/Base/GetTopMenus", {}, function (data) {
                        doT.exec("template/common/choosemodules.html", function (templateFun) {
                            var innerHTML = templateFun(data.Items);
                            innerHTML = $(innerHTML);
                            //鼠标进入
                            innerHTML.find(".modules-item").mouseenter(function () {
                                var _this = $(this).find("img");
                                _this.attr("src", _this.data("hover"));
                            });
                            //鼠标离开
                            innerHTML.find(".modules-item").mouseleave(function () {
                                var _this = $(this).find("img");
                                _this.attr("src", _this.data("ico"));
                            });

                            innerHTML.fadeIn("1000");
                            $("body").append(innerHTML);
                        });
                    });
                } else {
                    $(".choose-modules").fadeIn("1000");
                }
            } else {
                _this.removeClass("hover");
                $(".choose-modules").fadeOut("1000");
            }
        });
   

        //登录信息展开
        $("#currentUser").click(function () {
            $(".dropdown-userinfo").fadeIn("1000");
        });

        //二级菜单图标事件处理
        $("#controllerMenu a").mouseenter(function () {
            var _this = $(this).find("img");
            _this.attr("src", _this.data("hover"));
        });

        $("#controllerMenu a").mouseleave(function () {
            if (!$(this).hasClass("select")) {
                var _this = $(this).find("img");
                _this.attr("src", _this.data("ico"));
            }
        });

        $("#controllerMenu .select img").attr("src", $("#controllerMenu .select img").data("hover"));

        //二级菜单选中名称
        $(".controller-name").html($("#controllerMenu .select span").html());

       
    }
    //绑定元素定位和样式
    LayoutObject.bindStyle = function () {

    }

    //首页JS
    LayoutObject.initHome = function () {
        LayoutObject.bindStyle();
        LayoutObject.bindEvent();
        LayoutObject.homeEvent();
    }

    LayoutObject.homeEvent = function () {

        //图标居中
        $("#menuItems img").each(function () {
            var _this = $(this);
            _this.css({ top: _this.parent().height() / 2 - _this.height() / 2, left: _this.parent().width() / 2 - _this.width() / 2 })
        });

        var width = document.documentElement.clientWidth;
        if (width < 1200) {
            width = 1200;
        }
        $("#leftBody").css("margin-left", (width - 1200) / 2);

        $("#rightBody").css("width", width / 2 - 50);

    }
    module.exports = LayoutObject;
})
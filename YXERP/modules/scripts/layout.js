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
    //绑定元素定位和样式
    LayoutObject.bindStyle = function () {

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
                $(".choose-modules").fadeOut("1000");
            }
            if (!$(e.target).parents().hasClass("currentuser") && !$(e.target).hasClass("currentuser")) {
                $(".dropdown-userinfo").fadeOut("1000");
            }
        });

        //选择一级菜单
        $("#choosemodules").click(function () {
            var offset = $(this).offset();
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
        });

        //登录信息展开
        $("#currentUser").click(function () {
            var offset = $(this).offset();
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
    module.exports = LayoutObject;
})
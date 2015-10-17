/*
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
                $(".choose-modules").hide();
            }
        });

        //选择一级菜单
        $("#choosemodules").click(function () {
            var offset = $(this).offset();
            if ($(".choose-modules").length == 0) {
                Global.post("/Base/GetTopMenus", {}, function (data) {
                    doT.exec("template/common/choosemodules.html", function (templateFun) {
                        var innerHHML = templateFun(data.Items);
                        innerHHML = $(innerHHML);
                        innerHHML.css({ "top": offset.top + 42, "left": 25 });
                        $("body").append(innerHHML);
                    });
                });
            } else {
                $(".choose-modules").show();
            }
        });

        //二级菜单选中名称
        $(".controller-name").html($("#controllerMenu .select").html());
    }
    module.exports = LayoutObject;
})
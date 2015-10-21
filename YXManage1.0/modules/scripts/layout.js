/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        Global = require("global");

    var Height = document.documentElement.clientHeight - 84,
        Width = document.documentElement.clientWidth;

    var LayoutObject = {};
    //初始化数据
    LayoutObject.init = function () {
        LayoutObject.bindStyle();
        LayoutObject.bindEvent();
    }
    //绑定元素定位和样式
    LayoutObject.bindStyle = function () {
        var _height = Height, _width = Width - 180;
        $(".main-content").css({ "height": _height, "width": _width });
    }
    //绑定事件
    LayoutObject.bindEvent = function () {
        //调整浏览器窗体
        $(window).resize(function () {
            Height = document.documentElement.clientHeight - 84, Width = document.documentElement.clientWidth;
            LayoutObject.bindStyle();
            $(".controller.select").click();
        });
        //二级菜单选中名称
        $(".controller-name").html($("#controllerMenu .select").html());
    }
    module.exports = LayoutObject;
})
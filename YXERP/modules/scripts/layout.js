/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
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
            //Height = document.documentElement.clientHeight - 84, Width = document.documentElement.clientWidth;
            LayoutObject.bindStyle();
        });
        //二级菜单选中名称
        $(".controller-name").html($("#controllerMenu .select").html());
    }
    module.exports = LayoutObject;
})
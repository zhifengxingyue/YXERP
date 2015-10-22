define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser");

    var Model = {};

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.getList();
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        //选择最高领导人
        $(".create-first").click(function () {
            ChooseUser.create({
                title: "选择最高领导人",
                type: 1,
                single: true,
                callback: function (items) {
                    
                }
            });
        });
    }
    module.exports = ObjectJS;
});
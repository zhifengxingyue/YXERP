define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser");

    var Model = {};

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (parentid) {
        var _self = this;
        _self.bindEvent(parentid);
    }
    //绑定事件
    ObjectJS.bindEvent = function (parentid) {
        var _self = this;

        //选择最高领导人
        if (!parentid) {
            $("#structure").hide();
            $(".create-first").show().click(function () {
                ChooseUser.create({
                    title: "选择最高领导人",
                    type: 3,
                    single: true,
                    callback: function (items) {
                        if (items.length == 1) {
                            Global.post("/Organization/UpdateUserParentID", {
                                userid: items[0].id,
                                parentid: "6666666666"
                            }, function (data) {
                                if (data.status) {
                                    alert("成功！");
                                }
                            });
                        }
                    }
                });
            });
        } 
    }
    module.exports = ObjectJS;
});
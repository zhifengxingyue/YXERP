
/*
    --选择用户插件--
    --引用
    Verify = require("chooseuser")
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");

    require("plug/chooseuser/style.css");

    var PlugJS = function (options) {
        var _this = this;
        _this.setting = $.extend([], _this.default, options);
        _this.init();
    }

    //默认参数
    PlugJS.prototype.default = {
        title:"选择员工", //标题
        type: 1,  //类型 1：云销 2：明道
        url: "",
        callback: null   //回调
    };

    PlugJS.prototype.init = function () {
        var _self = this, url = "";
        if (!_self.setting.url && _self.setting.type == 1) {
            url = "/Organization/GetUsers";
        } else if (!_self.setting.url && _self.setting.type == 2) {
            url = "/Organization/GetMDUsers";
        }
        Global.post(url, {}, function (data) {
            doT.exec("/plug/chooseuser/chooseuser.html", function (template) {
                var innerHtml = template(data.items.users);

                Easydialog.open({
                    container: {
                        id: "choose-user-add",
                        header: _self.setting.title,
                        content: innerHtml,
                        yesFn: function () {

                            _self.setting.callback && _self.setting.callback();
                        },
                        callback: function () {

                        }
                    }
                });

                require.async("search", function () {
                    $("#chooseuserSearch").searchKeys(function (keyWords) {

                    });
                });
            });
        });
        
    };

    exports.create = function (options) {
        return new PlugJS(options);
    }
});
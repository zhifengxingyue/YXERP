define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser");

    var Model = {}, CacheChild = [];

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (model) {
        var _self = this;

        model = JSON.parse(model.replace(/&quot;/g, '"'));
        _self.bindEvent(model.UserID);
        _self.bindElement($(".user-item"));
        if (model.UserID) {
            _self.getChild(model.ChildUsers);
        }
    }
    //绑定事件
    ObjectJS.bindEvent = function (parentid) {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).hasClass("ico-dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

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
                                ids: items[0].id,
                                parentid: "6666666666"
                            }, function (data) {
                                if (data.status) {
                                    location.href = location.href;
                                }
                            });
                        }
                    }
                });
            });
        }
        //添加下属
        $("#addChild").click(function () {
            var _this = $(this);
            ChooseUser.create({
                title: "添加下属",
                type: 3,
                single: false,
                callback: function (items) {
                    if (items.length > 0) {
                        var ids = "";
                        for (var i = 0; i < items.length; i++) {
                            ids += items[i].id + ",";
                        }
                        Global.post("/Organization/UpdateUserParentID", {
                            ids: ids,
                            parentid: _this.data("id")
                        }, function (data) {
                            if (data.status) {
                                location.href = location.href;
                            }
                        });
                    }
                }
            });
        });
    }
    ObjectJS.bindElement = function (element) {
        var _self = this;
        element.find(".ico-dropdown").click(function () {
            var _this = $(this);
            var offset = _this.offset();
            $(".dropdown-ul li").data("id", _this.data("id"));
            $(".dropdown-ul").css({ "top": offset.top + 20, "left": offset.left }).show().mouseleave(function () {
                $(this).hide();
            });
        });
    }
    //绑定下级
    ObjectJS.getChild = function (items) {
        var _self = this;
        for (var i = 0, j = items.length; i < j; i++) {
            var menu = items[i];
            CacheChild[menu.UserID] = menu.ChildUsers;
        }
        doT.exec("template/organization/structure.html", function (template) {
            var innerHtml = template(items);
            innerHtml = $(innerHtml);

            $("#structure").append(innerHtml);

            _self.bindElement(innerHtml);

            innerHtml.find(".openchild").click(function () {
                var _this = $(this);
                var _obj = _self.getChild(_this.attr("data-id"), _this.prevUntil("div").html(), _this.attr("data-eq"), menus);
                _this.parent().after(_obj);
                _this.on("click", function () {
                    if (_this.attr("data-state") == "close") {
                        _this.attr("data-state", "open");
                        _this.removeClass("icoopen").addClass("icoclose");

                        $("#" + _this.attr("data-id")).show();

                    } else { //隐藏子下属
                        _this.attr("data-state", "close");
                        _this.removeClass("icoclose").addClass("icoopen");

                        $("#" + _this.attr("data-id")).hide();
                    }
                });
            });
        });
    }
    module.exports = ObjectJS;
});
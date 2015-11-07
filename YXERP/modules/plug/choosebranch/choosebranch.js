
/* 
作者：Allen
日期：2015-10-25
示例:
    $(...).choosebranch(options);
*/

define(function (require, exports, module) {
    require("plug/choosebranch/style.css");
    var Global = require("global"),
        doT = require("dot");
    (function ($) {
        $.fn.chooseBranch = function (options) {
            var opts = $.extend({}, $.fn.chooseBranch.defaults, options);
            return this.each(function () {
                var _this = $(this);
                $.fn.drawChooseBranch(_this, opts);
            })
        }
        $.fn.chooseBranch.defaults = {
            prevText: "",//文本前缀
            defaultText: "",
            defaultValue: "",
            userid: "",
            agentid: "",
            width: "180",
            onChange: function () { }
        };
        $.fn.drawChooseBranch = function (obj, opts) {

            obj.data("itemid", Global.guid());

            if (!obj.hasClass("choosebranch-module")) {
                obj.addClass("choosebranch-module").css("width", opts.width);
            }
            var _input = $('<div class="choosebranch-text">' + opts.prevText + opts.defaultText + '</div>');
            _input.css("width", opts.width - 30);
            var _ico = $('<div class="choosebranch-ico"><span></span></div>');
            obj.append(_input).append(_ico);

            //处理事件
            obj.click(function () {
                var _this = $(this);
                if (_this.hasClass("hover")) {
                    $("#" + obj.data("itemid")).hide();
                    _this.removeClass("hover");
                } else {
                    $.fn.drawChooseBranchItems(obj, opts);
                    _this.addClass("hover");
                }
            });

            $(document).click(function (e) {
                //隐藏下拉
                var bl = false;
                $(e.target).parents().each(function () {
                    var _this = $(this);
                    if (_this.data("itemid") == obj.data("itemid") || _this.attr("id") == obj.data("itemid")) {
                        bl = true;
                    }
                });
                if (!bl) {
                    obj.removeClass("hover");
                    $("#" + obj.data("itemid")).hide();
                }
            });
        }
        $.fn.drawChooseBranchItems = function (obj, opts) {
            var cacheChild = [];
            var offset = obj.offset();
            if ($("#" + obj.data("itemid")).length == 1) {
                $("#" + obj.data("itemid")).css({ "top": offset.top + 27, "left": offset.left }).show();
            } else {
                var _items = $("<div class='choosebranch-items-modules' id='" + obj.data("itemid") + "'></div>").css("width", opts.width);

                if (opts.defaultText) {
                    _items.append("<div class='default-item change-user' data-id='" + opts.defaultValue + "'>" + opts.defaultText + "</div>");
                }

                //搜索下属
                //$(".btnSearchBranch").click(function () {
                //    var _ele = innerText.find("a[data-search*='" + $(".searchInputBranch").val() + "']").first();
                //    innerText.find("a").css("color", "#333");
                //    _ele.parents().prev().each(function () {
                //        if ($(this).attr("class") == "branchItem") {
                //            $(this).find(".openBranch[data-state='close']").first().click();
                //        }
                //    })
                //    _ele.css("color", "#06c");
                //    $(".searchInputBranch").focus();
                //})

                //$(".searchInputBranch").keypress(function (event) {
                //    if (event.keyCode == 13) {
                //        $(".btnSearchBranch").click();
                //    }
                //})

                Global.post("/Plug/GetUserBranchs", {
                    userid: opts.userid,
                    agentid: opts.agentid
                }, function (data) {
                    for (var i = 0, j = data.items.length; i < j; i++) {
                        var user = data.items[i];
                        cacheChild[user.UserID] = user.ChildUsers;
                    }
                    doT.exec("plug/choosebranch/users.html", function (template) {
                        var innerHtml = template(data.items);
                        innerHtml = $(innerHtml);

                        _items.append(innerHtml);

                        //展开
                        innerHtml.find(".openchild").each(function () {
                            var _this = $(this);
                            var _obj = $.fn.drawChooseBranchChild(_this.attr("data-id"), _this.prevUntil("div").html(), _this.attr("data-eq"), cacheChild);
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

                        _items.find(".change-user").click(function () {
                            obj.find(".choosebranch-text").html(opts.prevText + $(this).html());
                            obj.data("id", $(this).data("id"));
                            obj.removeClass("hover");
                            $("#" + obj.data("itemid")).hide();
                            opts.onChange({
                                userid: $(this).data("id"),
                                name: $(this).html()
                            });
                        });
                    });
                    _items.css({ "top": offset.top + 27, "left": offset.left });

                    obj.after(_items);
                });
            }
        }

        $.fn.drawChooseBranchChild = function (pid, provHtml, isLast, cacheChild) {
            var _self = this;
            var _div = $(document.createElement("div")).attr("id", pid).addClass("hide").addClass("childbox");
            for (var i = 0; i < cacheChild[pid].length; i++) {
                var _item = $(document.createElement("div")).addClass("branchitem");

                //添加左侧背景图
                var _leftBg = $(document.createElement("div")).css("display", "inline-block").addClass("left");
                _leftBg.append(provHtml);
                if (isLast == "last") {
                    _leftBg.append("<span class='null left'></span>");
                } else {
                    _leftBg.append("<span class='line left'></span>");
                }
                _item.append(_leftBg);

                //是否最后一位
                if (i == cacheChild[pid].length - 1) {
                    _item.append("<span class='lastline left'></span>");

                    //加载显示下属图标和缓存数据
                    if (cacheChild[pid][i].ChildUsers.length > 0) {
                        _item.append("<span data-id='" + cacheChild[pid][i].UserID + "' data-eq='last' data-state='close' class='icoopen openchild left'></span>");
                        if (!cacheChild[cacheChild[pid][i].UserID]) {
                            cacheChild[cacheChild[pid][i].UserID] = cacheChild[pid][i].ChildUsers;
                        }
                    }
                } else {
                    _item.append("<span class='leftline left'></span>");

                    //加载显示下属图标和缓存数据
                    if (cacheChild[pid][i].ChildUsers.length > 0) {
                        _item.append("<span data-id='" + cacheChild[pid][i].UserID + "' data-eq='' data-state='close' class='icoopen openchild left'></span>");
                        if (!cacheChild[cacheChild[pid][i].UserID]) {
                            cacheChild[cacheChild[pid][i].UserID] = cacheChild[pid][i].ChildUsers;
                        }
                    }
                }

                _item.append("<span data-id='" + cacheChild[pid][i].UserID + "' class='left name change-user'>" + cacheChild[pid][i].Name + "</span>")

                _div.append(_item);

                //默认加载下级
                _item.find(".openchild").each(function () {
                    var _this = $(this);
                    var _obj = $.fn.drawChooseBranchChild(_this.attr("data-id"), _leftBg.html(), _this.attr("data-eq"), cacheChild);
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
            }
            return _div;
        }
    })(jQuery)
    module.exports = jQuery;
});
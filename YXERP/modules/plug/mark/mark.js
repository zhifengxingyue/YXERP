
/* 
作者：Allen
日期：2015-11-6
示例:
    $(...).markColor(callback(value));
*/

define(function (require, exports, module) {
    require("plug/mark/style.css");
    var Global = require("global");
    (function ($) {
        $.fn.markColor = function (callback) {
            return this.each(function () {
                $this = $(this);
                $.fn.drawmarkColor($this, callback);
            });
        }
        $.fn.drawmarkColor = function (obj, callback) {
            var colors = ["#FFF", "#FF7C7C", "#3BB3FF", "#9F74FF", "#FFC85D", "#FFF65F"];
            obj.addClass("mark-color").css("background-color", colors[obj.data("value")]);
            if (obj.data("value") == 0) {
                obj.css("border", "solid 1px #ccc");
            } else {
                obj.css("border", "solid 1px " + colors[obj.data("value")]);
            }
            obj.data("itemid", Global.guid());
            obj.click(function () {
                $(".mark-color-list").hide();
                var _this = $(this);
                var position = _this.position();
                if ($("#" + _this.data("itemid")).length == 0) {
                    var _colorBody = $("<ul id='" + _this.data("itemid") + "' class='mark-color-list'></ul>");
                    for (var i = 0; i < colors.length; i++) {
                        var _color = $("<li data-value='" + i + "' class='mark-color-item'><span></span></li>");
                        _color.find("span").css("background-color", colors[i]);
                        _color.click(function () {
                            var _i = $(this);
                            !!callback && callback(i, function (status) {
                                if (status) {
                                    if (_i.data("value") == 0) {
                                        _this.css("border", "solid 1px #ccc");
                                    } else {
                                        _this.css("border", "solid 1px " + colors[_i.data("value")]);
                                    }
                                    _this.css("background-color", colors[_i.data("value")]);
                                }
                            });
                            _colorBody.hide();
                        });
                        _colorBody.append(_color);
                    }
                    _colorBody.css({ "top": position.top + 25, "left": position.left - colors.length * 13 + 10 }).show();
                    $("body").append(_colorBody);
                } else {
                    $("#" + _this.data("itemid")).css({ "top": position.top + 25, "left": position.left - colors.length * 13 + 10 }).show();
                }
                return false;
            });

            $(document).click(function (e) {
                if ($(e.target).data("itemid") != obj.data("itemid") && $(e.target).attr("id") != obj.data("itemid")) {
                    $("#" + obj.data("itemid")).hide();
                }
            });
            
        }
    })(jQuery)
    module.exports = jQuery;
});
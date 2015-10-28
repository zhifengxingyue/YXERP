define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Easydialog = require("easydialog");
    var Model = {};

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.bindElement($(".stages-item"));
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).hasClass("ico-dropdown")) {
                $(".dropdown-ul").hide();
            }
        });
        //添加新阶段
        $("#addObject").click(function () {
            $(".dropdown-ul").hide();

            var _this = $(this), input = $("#" + _this.data("id")), parent = input.parents(".stages-item").first();
            //复制并处理新对象
            var element = parent.clone();

            element.find(".name span").html("").hide();
            var _input = element.find(".name input");
            _input.attr("id", "").data("sort", _input.data("sort") + 1).show();
            
            element.find(".ico-dropdown").data("type", "0").data("id", "").hide();
            element.find(".child-items").empty();
            _self.bindElement(element);
            parent.after(element);
            _input.focus();
        });
        //删除阶段
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("客户阶段删除后不可恢复,此阶段客户自动更换为上个阶段,确认删除吗？", function () {
                _self.deleteModel(_this.data("id"), function (status) {
                    if (status) {
                        location.href = location.href;
                    } 
                });
            });
        });
        //编辑阶段名称
        $("#editObject").click(function () {

            $(".dropdown-ul").hide();

            var _this = $(this), input = $("#" + _this.data("id")), span = input.siblings("span");
            var input = $("#" + _this.data("id"));
            
            input.siblings().hide();
            input.parent().siblings(".ico-dropdown").hide();
            input.show();
            input.focus();

            input.val(span.html());
        });

    }
    //元素绑定事件
    ObjectJS.bindElement = function (items) {
        var _self = this;
        //下拉事件
        items.find(".ico-dropdown").click(function () {
            var _this = $(this);
            if (_this.data("type") != 0) {
                $("#deleteObject").hide();
            } else {
                $("#deleteObject").show();
            }
            var offset = _this.offset();
            $(".dropdown-ul li").data("id", _this.data("id")).data("sort", _this.data("sort"));
            var left = offset.left;
            if (left > document.documentElement.clientWidth - 150) {
                left = left - 150;
            }
            $(".dropdown-ul").css({ "top": offset.top + 20, "left": left }).show().mouseleave(function () {
                $(this).hide();
            });
        });
        //文本改变事件
        items.find(".name input").blur(function () {
            var _this = $(this), span = _this.siblings("span");
            if (_this.val() != span.html()) {
                var model = {
                    StageID: _this.attr("id"),
                    StageName: _this.val(),
                    Sort: _this.data("sort")
                };
                _self.saveModel(model);
            } else {
                if (!_this.attr("id")) {
                    _this.parents(".stages-item").first().remove();
                } else {
                    span.html(_this.val()).show();
                    _this.val("").hide();
                    _this.parent().siblings(".ico-dropdown").show();
                }
            }
        });
    }
    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/System/SaveCustomStage", { entity: JSON.stringify(model) }, function (data) {
            if (data.status == 1) {
                if (model.StageID) {
                    var _this = $("#" + model.StageID), span = _this.siblings("span");
                    span.html(_this.val()).show();
                    _this.val("").hide();
                    _this.parent().siblings(".ico-dropdown").show();
                } else {
                    location.href = location.href;
                }
            } else {
                alert("系统异常!");
            }
        })
    }
    //删除
    ObjectJS.deleteModel = function (id, callback) {
        Global.post("/System/DeleteCustomStage", { id: id }, function (data) {
            !!callback && callback(data.status);
        })
    }

    module.exports = ObjectJS;
});
﻿define(function (require, exports, module) {
    var Global = require("global");
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.bindElementEvent($(".entity-item"));
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $("#addEntity").click(function () {
            var _this = $(this);
            var _ele = $('<li class="entity-item"><input type="text" maxlength="20" data-id="" data-value="" value="" /><span data-id="" class="ico-delete"></span></li>');
            _self.bindElementEvent(_ele);
            _this.before(_ele);
            _ele.find("input").focus();
        });
    }

    //附加元素事件
    ObjectJS.bindElementEvent = function (elments) {
        var _self = this;
        elments.find("input").focus(function () {
            var _this = $(this);
            _this.select();
        });
        elments.find("input").blur(function () {
            var _this = $(this);
            //为空
            if (_this.val() == "") {
                if (_this.data("id") == "") {
                    _this.parent().remove();
                } else {
                    _this.val(_this.data("value"));
                }
            } else if (_this.val() != _this.data("value")) {
                var model = {
                    DepartID: _this.data("id"),
                    Name: _this.val(),
                    ParentID: "",
                    Description: ""
                };
                Global.post("/Organization/SaveDepartment", { entity: JSON.stringify(model) }, function (data) {

                    if (data.ID.length > 0) {
                        _this.data("id", data.ID);
                        _this.data("value", model.Name);
                        _this.next().data("id", data.ID);
                    }
                })
            }
        });
        elments.find(".ico-delete").click(function () {
            var _this = $(this);
            if (_this.data("id") != "") {
                if (confirm("部门删除后不可恢复,确认删除吗？")) {
                    _self.deleteModel(_this.data("id"), function (status) {
                        if (status == 1) {
                            _this.parent().remove();
                        } else if (status == 10002) {
                            alert("此部门存在员工，请移除员工后重新操作！");
                        }
                    });
                }
            } else {
                _this.parent().remove();
            }
        })
    }
    //删除
    ObjectJS.deleteModel = function (id, callback) {
        Global.post("/Organization/DeleteDepartment", { departid: id }, function (data) {
            !!callback && callback(data.Status);
        })
    }

    module.exports = ObjectJS;
});
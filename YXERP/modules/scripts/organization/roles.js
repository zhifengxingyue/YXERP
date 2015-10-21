﻿define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog");

    var Model = {},
        cacheMenu = [];

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
        $("#createModel").click(function () {
            var _this = $(this);
            Model.RoleID = "";
            Model.Name = "";
            Model.Description = "";
            _self.createModel();
        });
    }
    //添加/编辑弹出层
    ObjectJS.createModel = function () {
        var _self = this;

        doT.exec("template/organization/role-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !Model.RoleID ? "新建角色" : "编辑角色",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        Model.Name = $("#modelName").val();
                        Model.Description = $("#modelDescription").val();
                        Model.ParentID = "";
                        _self.saveModel(Model);
                    },
                    callback: function () {

                    }
                }
            });
            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
            $("#modelName").focus();
            $("#modelName").val(Model.Name);
            $("#modelDescription").val(Model.Description);

        }); 
    }
    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        Global.post("/Organization/GetRoles", {}, function (data) {
            _self.bindList(data.items);
        });
    }
    //加载列表
    ObjectJS.bindList = function (items) {
        var _self = this;
        doT.exec("template/organization/roles.html", function (template) {
            var innerhtml = template(items);
            innerhtml = $(innerhtml);

            //初始管理员角色不能编辑
            innerhtml.find(".ico-del,.setpermission,.ico-edit").each(function () {
                if ($(this).data("type") == 1) {
                    $(this).remove();
                }
            });

            //删除
            innerhtml.find(".ico-del").click(function () {
                var _this = $(this);
                if (confirm("角色删除后不可恢复,确认删除吗？")) {
                    _self.deleteModel(_this.data("id"), function (status) {
                        if (status == 1) {
                            _this.parent().parent().remove();
                        } else if (status == 10002) {
                            alert("此角色存在员工，请移除员工后重新操作！");
                        }
                    });
                }
            });

            //编辑
            innerhtml.find(".ico-edit").click(function () {
                var _this = $(this);
                Model.RoleID = _this.data("id");
                Model.Name = _this.parent().siblings(".name").html();
                Model.Description = _this.parent().siblings(".desc").html();
                _self.createModel();
            });

            $(".tr-header").after(innerhtml);
        });
    }
    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/Organization/SaveRole", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.RoleID.length > 0) {
                _self.getList();
                //_self.bindList([data.model]);
            }
        })
    }
    //删除
    ObjectJS.deleteModel = function (id, callback) {
        Global.post("/Organization/DeleteRole", { roleid: id }, function (data) {
            !!callback && callback(data.status);
        })
    }

    //绑定权限页样式
    ObjectJS.initPermission = function (roleid, permissions, menus) {
        var _self = this;
        permissions = JSON.parse(permissions.replace(/&quot;/g, '"'));
        menus = JSON.parse(menus.replace(/&quot;/g, '"'));

        _self.bindMenu(permissions, menus);

        _self.bindPermissionEvent(roleid);

        
    }

    ObjectJS.bindPermissionEvent = function (roleid) {
        $("#savePermission").click(function () {
            var menus = "";
            $("#rolePermission input").each(function () {
                if ($(this).prop("checked")) {
                    menus += $(this).data("id") + ",";
                }
            });
            Global.post("/Organization/SaveRolePermission", {
                roleid: roleid,
                permissions: menus
            }, function (data) {
                if (data.status) {
                    alert("角色权限设置成功！");
                } else {
                    alert("角色权限设置失败！");
                }
            });
        });
    }

    ObjectJS.bindMenu = function (permissions, menus) {
        var _self = this;
        for (var i = 0, j = permissions.length; i < j; i++) {
            var menu = permissions[i];
            cacheMenu[menu.MenuCode] = menu.ChildMenus;
        }

        doT.exec("template/organization/permissions.html", function (template) {
            var innerHtml = template(permissions);
            innerHtml = $(innerHtml);

            $("#rolePermission").append(innerHtml);

            innerHtml.find("input").change(function () {
                var _this = $(this);
                $("#" + _this.data("id")).find("input").prop("checked", _this.prop("checked"));
            });

            //默认选中拥有权限
            innerHtml.find("input").each(function () {
                var _this = $(this);
                for (var i = 0, j = menus.length; i < j; i++) {
                    if (_this.data("id") == menus[i].MenuCode) {
                        _this.prop("checked", true);
                    }
                }
            });

            innerHtml.find(".openchild").each(function () {
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

    //展开下级
    ObjectJS.getChild = function (menuCode, provHtml, isLast, menus) {
        var _self = this;
        var _div = $(document.createElement("div")).attr("id", menuCode).addClass("hide").addClass("childbox");
        for (var i = 0; i < cacheMenu[menuCode].length; i++) {
            var _item = $(document.createElement("div")).addClass("menuitem");

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
            if (i == cacheMenu[menuCode].length - 1) {
                _item.append("<span class='lastline left'></span>");

                //加载显示下属图标和缓存数据
                if (cacheMenu[menuCode][i].ChildMenus > 0) {
                    _item.append("<span data-id='" + cacheMenu[menuCode][i].MenuCode + "' data-eq='last' data-state='close' class='icoopen openchild'></span>");
                    if (!cacheMenu[cacheMenu[menuCode][i].MenuCode]) {
                        cacheMenu[cacheMenu[menuCode][i].MenuCode] = cacheMenu[menuCode][i].ChildMenus;
                    }
                }
            } else {
                _item.append("<span class='leftline left'></span>");

                //加载显示下属图标和缓存数据
                if (cacheMenu[menuCode][i].ChildMenus > 0) {
                    _item.append("<span data-id='" + cacheMenu[menuCode][i].MenuCode + "' data-eq='' data-state='close' class='icoOpen openchild'></span>");
                    if (!cacheMenu[cacheMenu[menuCode][i].MenuCode]) {
                        cacheMenu[cacheMenu[menuCode][i].MenuCode] = cacheMenu[menuCode][i].ChildMenus;
                    }
                }
            }

            _item.append("<label class='pLeft5 left'><input type='checkbox' class='left'  value='" + cacheMenu[menuCode][i].MenuCode + "' data-id='" + cacheMenu[menuCode][i].MenuCode + "' /><span>" + cacheMenu[menuCode][i].Name + "</span></label>");

            _div.append(_item);

            _item.find("input").change(function () {
                var _this = $(this);
                $("#" + _this.data("id")).find("input").prop("checked", _this.prop("checked"));
                //下架选中上级默认选中
                if (_this.prop("checked")) {
                    _this.parents().each(function () {
                        var _parent = $(this);
                        if (_parent.hasClass("childbox")) {
                            _parent.prev().find("input").prop("checked", true);
                        }
                    });
                }
            });
            //默认加载下级
            _item.find(".openchild").each(function () {
                var _this = $(this);
                var _obj = _self.getChild(_this.attr("data-id"), _leftBg.html(), _this.attr("data-eq"), menus);
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

        //默认选中拥有权限
        _div.find("input").each(function () {
            var _this = $(this);
            for (var i = 0, j = menus.length; i < j; i++) {
                if (_this.data("id") == menus[i].MenuCode) {
                    _this.prop("checked", true);
                }
            }
        });
        return _div;
    }

    module.exports = ObjectJS;
});
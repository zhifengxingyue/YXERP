define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser");

    var Model = {};

    var ObjectJS = {};

    ObjectJS.Params = {
        PageIndex: 1,
        DepartID:"",
        RoleID:"",
        KeyWords: ""
    };

    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
        _self.getList();
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.keyWords = keyWords;
                ObjectJS.Params.DepartID = $("#Departments").val();
                ObjectJS.Params.RoleID = $("#Roles").val();
                ObjectJS.getList();
            });
        });

        $("#Departments,#Roles").change(function () {
            ObjectJS.Params.PageIndex = 1;
            ObjectJS.Params.keyWords = $(".search-ipt").val();
            ObjectJS.Params.DepartID = $("#Departments").val();
            ObjectJS.Params.RoleID = $("#Roles").val();
            ObjectJS.getList();
        });

        //添加明道用户
        $("#addMDUser").click(function () {
            ChooseUser.create({
                title: "明道用户导入",
                type: 2,
                single: false,
                callback: function (items) {
                    var ids = "";
                    for (var i = 0; i < items.length; i++) {
                        ids += items[i].id + ",";
                    }
                    if (ids.length > 0) {
                        Global.post("/Organization/SaveMDUser", {
                            parentid: "",
                            mduserids: ids
                        }, function (data) {
                            if (data.status) {
                                _self.getList();
                            }
                        });
                    }
                }
            });
        });
        //注销员工
        $("#deleteObject").click(function () {
            var _this = $(this);
            if (confirm("员工注销后不能再使用系统，确认注销吗?")) {
                Global.post("/Organization/DeleteUserByID", {
                    userid: _this.data("id")
                }, function (data) {
                    if (data.status) {
                        _self.getList();
                    }
                });
            }
        });
        //设置角色
        $("#setObjectRole").click(function () {
            var _this = $(this);
            Global.post("/Organization/GetRoles", {}, function (data) {
                doT.exec("template/organization/setuserrole.html", function (template) {
                    var innerHtml = template(data.items);
                    Easydialog.open({
                        container: {
                            id: "show-model-setRole",
                            header: "设置员工角色",
                            content: innerHtml,
                            yesFn: function () {
                                $("#setUserRoleBox .role-item").each(function () {
                                    var _role = $(this);
                                    //保存角色
                                    if (_role.hasClass("hover")) {
                                        if (_role.data("id") == _this.data("roleid")) {
                                            return;
                                        }
                                        Global.post("/Organization/UpdateUserRole", {
                                            userid: _this.data("id"),
                                            roleid: _role.data("id")
                                        }, function (data) {
                                            if (data.status) {
                                                _self.getList();
                                            }
                                        });
                                        return;
                                    }
                                });
                            },
                            callback: function () {

                            }
                        }
                    });
                    //默认选中当前角色
                    $("#setUserRoleBox .role-item[data-id=" + _this.data("roleid") + "]").addClass("hover");

                    $("#setUserRoleBox .role-item").click(function () {
                        $(this).siblings().removeClass("hover");
                        $(this).addClass("hover");
                    });
                });
            });
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        Global.post("/Organization/GetUsers", { filter: JSON.stringify(ObjectJS.Params) }, function (data) {
            _self.bindList(data.items);
        });
    }
    //加载列表
    ObjectJS.bindList = function (items) {
        var _self = this;
        doT.exec("template/organization/users.html", function (template) {
            var innerhtml = template(items);
            innerhtml = $(innerhtml);

            //操作
            innerhtml.find(".dropdown").click(function () {
                var _this = $(this);
                var position = _this.find(".ico-dropdown").position();
                $(".dropdown-ul li").data("id", _this.data("id")).data("roleid", _this.data("roleid"));

                $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left-80 }).show().mouseleave(function () {
                    $(this).hide();
                });
            });

            $(".tr-header").after(innerhtml);
        });
    }

    //添加/编辑弹出层
    ObjectJS.createModel = function () {
        var _self = this;

        doT.exec("template/organization/department-detail.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !Model.DepartID ? "新建部门" : "编辑部门",
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
    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/Organization/SaveDepartment", { entity: JSON.stringify(model) }, function (data) {
            if (data.model.DepartID.length > 0) {
                _self.getList();
            }
        })
    }

    module.exports = ObjectJS;
});
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog");

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
        $("#createModel").click(function () {
            var _this = $(this);
            Model.DepartID = "";
            _self.createModel();
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
                    header: !Model.DepartID ? "创建部门" : "编辑部门",
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
        Global.post("/Organization/GetDepartments", {}, function (data) {
            doT.exec("template/organization/departments.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                innerhtml.find(".ico-del").click(function () {
                    var _this = $(this);
                    if (confirm("部门删除后不可恢复,确认删除吗？")) {
                        _self.deleteModel(_this.data("id"), function (status) {
                            if (status == 1) {
                                _this.parent().parent().remove();
                            } else if (status == 10002) {
                                alert("此部门存在员工，请移除员工后重新操作！");
                            }
                        });
                    }
                });

                $(".tr-header").after(innerhtml);
            });
        });
    }

    //附加元素事件
    ObjectJS.saveModel = function (model) {
        console.log(model);
        Global.post("/Organization/SaveDepartment", { entity: JSON.stringify(model) }, function (data) {
            if (data.ID.length > 0) {
                _this.data("id", data.ID);
                _this.data("value", model.Name);
                _this.next().data("id", data.ID);
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
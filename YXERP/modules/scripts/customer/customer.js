define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog");
    require("pager");
    require("mark");

    var Params = {
        SearchType: 1,
        SourceID: "",
        StageID: "",
        Status: -1,
        UserID: "",
        AgentID: "",
        TeamID: "",
        Keywords: "",
        BeginTime: "",
        EndTime: "",
        PageIndex: 1,
        PageSize: 20
    };

    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (type) {
        var _self = this;
        Params.SearchType = type;
        _self.getList();
        _self.bindEvent();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });
        //切换阶段
        $(".search-stages li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.StageID = _this.data("id");
                _self.getList();
            }
        });
        //切换状态
        $(".search-status li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                Params.PageIndex = 1;
                Params.Status = _this.data("id");
                _self.getList();
            }
        });
        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.PageIndex = 1;
                Params.Keywords = keyWords;
                _self.getList();
            });
        });
        //客户来源
        Global.post("/Customer/GetCustomerSources", { }, function (data) {
            require.async("dropdown", function () {
                $("#customerSource").dropdown({
                    prevText: "来源-",
                    defaultText: "全部",
                    defaultValue: "",
                    data: data.items,
                    dataValue: "SourceID",
                    dataText: "SourceName",
                    width: "180",
                    onChange: function (data) {
                        Params.PageIndex = 1;
                        Params.SourceID = data.value;
                        _self.getList();
                    }
                });
            });
        });
        //全部选中
        $("#check-all").click(function () {
            var _this = $(this);
            if (!_this.hasClass("ico-checked")) {
                _this.addClass("ico-checked").removeClass("ico-check");
                $(".table-list .check").addClass("ico-checked").removeClass("ico-check");
            } else {
                _this.addClass("ico-check").removeClass("ico-checked");
                $(".table-list .check").addClass("ico-check").removeClass("ico-checked");
            }
        });
        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);
            Global.post("/Organization/GetDepartmentByID", { id: _this.data("id") }, function (data) {
                var model = data.model;
                Model.DepartID = model.DepartID;
                Model.Name = model.Name;
                Model.Description = model.Description;
                _self.createModel();
            });
        });

    }
    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        Global.post("/Customer/GetCustomers", { filter: JSON.stringify(Params) }, function (data) {
            _self.bindList(data);
        });
    }
    //加载列表
    ObjectJS.bindList = function (data) {
        var _self = this;

        doT.exec("template/customer/customers.html", function (template) {
            var innerhtml = template(data.items);
            innerhtml = $(innerhtml);

            //下拉事件
            innerhtml.find(".dropdown").click(function () {
                var _this = $(this);
                var position = _this.find(".ico-dropdown").position();
                $(".dropdown-ul li").data("id",_this.data("id"));
                $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 80 }).show().mouseleave(function () {
                    $(this).hide();
                });
                return false;
            });
            innerhtml.find(".check").click(function () {
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                } else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });
            innerhtml.click(function () {
                var _this = $(this).find(".check");
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                } else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });

            innerhtml.find(".mark").markColor(function (value, callback) {
                callback(true);
            });


            $(".tr-header").after(innerhtml);

        });
        $("#pager").paginate({
            total_count: data.totalCount,
            count: data.pageCount,
            start: Params.PageIndex,
            display: 5,
            border: true,
            border_color: '#fff',
            text_color: '#333',
            background_color: '#fff',
            border_hover_color: '#ccc',
            text_hover_color: '#000',
            background_hover_color: '#efefef',
            rotate: true,
            images: false,
            mouse: 'slide',
            onChange: function (page) {
                Params.PageIndex = page;
                _self.getList();
            }
        });
    }

    module.exports = ObjectJS;
});
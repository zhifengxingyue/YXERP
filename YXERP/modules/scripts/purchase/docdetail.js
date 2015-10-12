
define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot");
    require("pager");

    var Params = {
        pageIndex: 1,
        totalCount: 0,
        docID: ""
    };
    var ObjectJS = {};
    //初始化
    ObjectJS.init = function (docID) {
        var _self = this;
        Params.docID = docID;
        _self.bindEvent();
        _self.getList();
    }
    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

    }
    //获取单据列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".log-body").empty();
        var url = "/Purchase/GetStorageDocLog",
            template = "template/purchase/storagedocaction.html";
        

        Global.post(url, Params, function (data) {
            doT.exec(template, function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".log-body").append(innerText);

            });
            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Params.pageIndex,
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
                float: "normal",
                onChange: function (page) {
                    Params.pageIndex = page;
                    _self.getList();
                }
            });
        });
    }

    module.exports = ObjectJS;
})
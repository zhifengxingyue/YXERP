define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Upload = require("upload"), PosterIco, editor,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser");
    require("pager");
    var Model = {};

    var ObjectJS = {};

    ObjectJS.Params = {
        PageSize: 1,
        PageIndex:1,
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

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.keyWords = keyWords;
                ObjectJS.Params.DepartID = $("#Departments").val();
                ObjectJS.Params.RoleID = $("#Roles").val();
                ObjectJS.getList();
            });
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        Global.post("/Activity/GetActivityList",
            { pageSize: ObjectJS.Params.PageSize, pageIndex: ObjectJS.Params.PageIndex, KeyWords: ObjectJS.Params.KeyWords },
            function (data) {
                _self.bindList(data.Items);

                $("#pager").paginate({
                    total_count: data.TotalCount,
                    count: data.PageCount,
                    start: _self.Params.PageIndex,
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
                        _self.Params.PageIndex = page;
                        _self.getList();
                    }
                });
            });
    }

    //加载列表
    ObjectJS.bindList = function (items) {
        if (items.length > 0) {
            var _self = this;
            doT.exec("template/activity/activity_list.html", function (template) {
                var innerhtml = template(items);
                innerhtml = $(innerhtml);

                $(".tr-header").after(innerhtml);
            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='5' style='padding:15px 0px;'><div style='margin:0px auto; width:300px;'><div class='left' style='padding-top:4px;'>暂无数据！</div><div class='left'><a href='BrandAdd' class='ico-add  mTop4'>添加活动</a></div><div class='clear'></div></div></td></tr>");
        }
    }

    //初始化
    ObjectJS.initDetail= function (Editor, id) {
        var _self = this;
        editor = Editor;
        _self.bindDetailEvent();
        if(id)
            _self.getDetail();
    }

    //绑定事件
    ObjectJS.bindDetailEvent = function () {
        var _self = this;

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.keyWords = keyWords;
                ObjectJS.Params.DepartID = $("#Departments").val();
                ObjectJS.Params.RoleID = $("#Roles").val();
                ObjectJS.getList();
            });
        });

        //选择海报图片
        PosterIco = Upload.createUpload({
            element: "#Poster",
            buttonText: "选择海报图片",
            className: "edit-brand",
            data: { folder: '/Content/tempfile/', action: 'add', oldPath: "" },
            success: function (data, status) {
                if (data.Items.length > 0) {
                    _self.IcoPath = data.Items[0];
                    $("#PosterDisImg").attr("src", data.Items[0]);
                    $("#PosterImg").val(data.Items[0]);
                }
            }
        });

        //添加负责人
        $("#addOwner").click(function () {
            ChooseUser.create({
                title: "添加负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    for (var i = 0; i < items.length; i++) {
                        _self.createMember(items[i], "OwnerIDs",true);
                    }
                }
            });
        });

        //添加成员
        $("#addMember").click(function () {
            ChooseUser.create({
                title: "添加成员",
                type: 1,
                single: false,
                callback: function (items) {
                    for (var i = 0; i < items.length; i++) {
                        _self.createMember(items[i], "MemberIDs",false);
                    }

                }
            });
        });

        $("#btnSaveActivity").click(function () {
            var OwnerID='', MemberID='';
            $("#OwnerIDs .member").each(function () {
                OwnerID += $(this).attr("bindID")+"|";
            });
            $("#MemberIDs .member").each(function () {
                MemberID += $(this).attr("bindID") + "|";
            });

            var model = {
                ActivityID: $("#ActivityID").val(),
                Name: $("#Name").val(),
                Poster: $("#PosterImg").val(),
                OwnerID: OwnerID,
                MemberID: MemberID,
                BeginTime: $("#BeginTime").val(),
                EndTime: $("#EndTime").val(),
                Address: $("#Address").val(),
                Remark: encodeURI(editor.getContent())
            };

            _self.saveModel(model);
        });
        
    }

    //获取详情
    ObjectJS.getDetail = function () {
        var _self = this;
        Global.post("/Activity/GetActivityDetail",
            { activityID: $("#ActivityID").val() },
            function (data) {
                if (data.Item) {
                    var item = data.Item;
                    $("#Name").val(item.Name);
                    ObjectJS.createMember2(item.Owner, "OwnerIDs", true);
                    for (var i = 0; i < item.Members.length; i++) {
                        ObjectJS.createMember2(item.Members[i], "MemberIDs", false);
                    }
                    $("#PosterDisImg").attr("src", item.Poster);
                    $("#PosterImg").val(item.Poster);
                    $("#EndTime").val(item.EndTime.toDate("yyyy-MM-dd"));
                    $("#BeginTime").val(item.BeginTime.toDate("yyyy-MM-dd"));
                    $("#Address").val(item.Address);
                    editor.ready(function () {
                        editor.setContent(decodeURI(item.Remark));
                    });
                }
            });
    }

    //拼接一个用户成员
    ObjectJS.createMember = function (item, id, isSingle) {
        if ($("#" + id + " div[bindID='" + item.id + "']").html())
            return false;

        var html = '<div class="member left" bindID="' + item.id + '">';
        html += '    <div class="left pRight5">';
        html += '          <img src="' + item.avatar + '" />';
        html += '     </div>';
        html += '      <div class="left mRight10 pLeft5"><a href="javascript:void(0);" onclick="$(this).parents(\'.member\').remove();">×</a></div>';
        html += '      <div class="clear"></div>';
        html += '   </div>';

        if (isSingle)
            $("#" + id).html(html);
        else
            $("#" + id).append(html);
    }

    //拼接一个用户成员
    ObjectJS.createMember2 = function (item, id, isSingle) {
        if (item.Avatar == '')
            item.Avatar = "/modules/images/defaultavatar.png";
        var html = '<div class="member left" bindID="' + item.UserID + '">';
        html += '    <div class="left pRight5">';
        html += '          <img src="' + item.Avatar + '" />';
        html += '     </div>';
        html += '      <div class="left mRight10 pLeft5"><a href="javascript:void(0);" onclick="$(this).parents(\'.member\').remove();">×</a></div>';
        html += '      <div class="clear"></div>';
        html += '   </div>';

        if (isSingle)
            $("#" + id).html(html);
        else
            $("#" + id).append(html);
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/Activity/SavaActivity", { entity: JSON.stringify(model) }, function (data) {
            if (data.ID.length > 0) {
                _self.getList();
            }
        })
    }

    
    //删除
    ObjectJS.deleteModel = function (id, callback) {
        Global.post("/Activity/DeleteDepartment", { departid: id }, function (data) {
            !!callback && callback(data.status);
        })
    }

    module.exports = ObjectJS;
});
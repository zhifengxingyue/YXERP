define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Upload = require("upload"), PosterIco, editor,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser");

    var Model = {};

    var ObjectJS = {};

    ObjectJS.Params = {
        pageSize: 1,
        pageIndex:1,
        KeyWords: ""
    };

    //初始化
    ObjectJS.init = function (Editor) {
        var _self = this;
        editor = Editor;
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

    ObjectJS.createMember = function (item, id, isSingle) {
        if ( $( "#" + id + " div[bindID='" + item.id + "']" ).html() )
            return false;

        var html='<div class="member left" bindID="'+item.id+'">';
        html+='    <div class="left pRight5">';
        html += '          <img src="' + item.avatar + '" />';
        html+='     </div>';
        html+='      <div class="left mRight10 pLeft5"><a href="javascript:void(0);" onclick="$(this).parents(\'.member\').remove();">×</a></div>';
        html+='      <div class="clear"></div>';
        html += '   </div>';

        if (isSingle)
            $("#" + id).html(html);
        else
            $("#" + id).append(html);
    }


    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        Global.post("/Activity/GetActivityList",
            { pageSize: ObjectJS.Params.pageSize, pageIndex: ObjectJS.Params.pageIndex, KeyWords:ObjectJS.Params.KeyWords},
            function (data) {
                _self.bindList(data.Items);
        });
    }

    //加载列表
    ObjectJS.bindList = function (items) {
        if (items.length > 0) {
            var _self = this;
            doT.exec("template/activity/activity_list.html", function (template) {
                var innerhtml = template(items);
                innerhtml = $(innerhtml);

                //操作
                innerhtml.find(".dropdown").click(function () {
                    var _this = $(this);
                    var position = _this.find(".ico-dropdown").position();
                    $(".dropdown-ul li").data("id", _this.data("id"));
                    $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 80 }).show().mouseleave(function () {
                        $(this).hide();
                    });
                });

                $(".tr-header").after(innerhtml);
            });
        }
        else {
            $(".tr-header").after("<tr><td colspan='5' style='padding:15px 0px;'><div style='margin:0px auto; width:300px;'><div class='left' style='padding-top:4px;'>暂无数据！</div><div class='left'><a href='BrandAdd' class='ico-add  mTop4'>添加活动</a></div><div class='clear'></div></div></td></tr>");
        }
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
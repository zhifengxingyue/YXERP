define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Upload = require("upload"), PosterIco, editor,
        Easydialog = require("easydialog"),
        Verify = require("verify"), VerifyObject,
        ChooseUser = require("chooseuser");
    
    require("pager");
    var Model = {};

    var ObjectJS = {};

    ObjectJS.Params = {
        PageSize: 20,
        PageIndex:1,
        KeyWords: "",
        IsAll: 0,
        Stage:-1,
        BeginTime: "",
        EndTime: "",
        FilterType: 0,
        DisplayType:1
    };

    //初始化列表
    ObjectJS.init = function (isAll) {
        var _self = this;
        _self.bindEvent();
        _self.Params.IsAll= isAll;
        _self.getList();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        $("#SearchActivity").click(function () {
            ObjectJS.Params.PageIndex = 1;
            ObjectJS.Params.BeginTime = $("#BeginTime").val();
            ObjectJS.Params.EndTime = $("#EndTime").val();
            ObjectJS.getList();
        });

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.KeyWords = keyWords;
                ObjectJS.getList();
            });
        });

        //搜索
        require.async("dropdown", function () {
            var Stages = [
                {
                    ID: "1",
                    Name: "已结束"
                },
                {
                    ID: "2",
                    Name: "进行中"
                },
                {
                    ID: "3",
                    Name: "未开始"
                }
            ];
            $("#ActivityStage").dropdown({
                prevText: "活动阶段-",
                defaultText: "所有",
                defaultValue: "-1",
                data: Stages,
                dataValue: "ID",
                dataText: "Name",
                width: "150",
                onChange: function (data) {
                    ObjectJS.Params.PageIndex = 1;
                    ObjectJS.Params.Stage = data.value;
                    ObjectJS.getList();
                }
            });
            var Types = [
                {
                    ID: "1",
                    Name: "我负责的"
                },
                {
                    ID: "2",
                    Name: "我参与的"
                }
            ];
            $("#ActivityType").dropdown({
                prevText: "活动过滤-",
                defaultText: "所有",
                defaultValue: "-1",
                data: Types,
                dataValue: "ID",
                dataText: "Name",
                width: "150",
                onChange: function (data) {
                    ObjectJS.Params.PageIndex = 1;
                    ObjectJS.Params.FilterType = data.value;
                    ObjectJS.getList();
                }
            });

        });

        //删除活动
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("确认删除活动吗?", function () {
                Global.post("/Activity/DeleteActivity", { activityID: _this.data("id") }, function (data) {
                    if (data.Result == 1) {
                        if (ObjectJS.Params.IsAll == 0)
                            location.href = "/Activity/MyActivity";
                        else
                            location.href = "/Activity/Activitys";
                    }
                    else {
                        alert("删除失败");
                    }
                });
            });
        });

        //编辑活动
        $("#setObjectRole").click(function () {
            var _this = $(this);
            location.href = "/Activity/Operate/" + _this.data("id");
        });

        //显示模式切换
        $(".displayTab").click(function () {
            var type = parseInt($(this).data("type"));

            ObjectJS.Params.DisplayType = type;
            if (type == 1) {
                $("tr.list-item").remove();
                $(".table-list").show();
                $(".activityCardList").hide();
            }
            else {
                $(".activityCardList").html('');
                $(".table-list").hide();
                $(".activityCardList").show();
            }
            ObjectJS.getList();
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        Global.post("/Activity/GetActivityList",
            {
                pageSize: ObjectJS.Params.PageSize,
                pageIndex: ObjectJS.Params.PageIndex,
                keyWords: ObjectJS.Params.KeyWords,
                isAll: ObjectJS.Params.IsAll,
                beginTime: ObjectJS.Params.BeginTime,
                endTime: ObjectJS.Params.EndTime,
                stage: ObjectJS.Params.Stage,
                filterType: ObjectJS.Params.FilterType
            },
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
            }
        );
    }

    //加载列表
    ObjectJS.bindList = function (items) {
        if (items.length > 0) {
            var _self = this;
            if (ObjectJS.Params.DisplayType == 1) {
                doT.exec("template/activity/activity_list.html", function (template) {
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);
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
            else
            {
                doT.exec("template/activity/activity_card_list.html", function (template) {
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);
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

                    $(".activityCardList").append(innerhtml);
                    require.async("businesscard", function () {
                        $("div.member").businessCard();
                    });
                });
            }

           
        }
        else {
            $(".tr-header").after("<tr><td colspan='7' style='padding:15px 0px;'><div style='margin:0px auto; width:300px;'><div class='left' style='padding-top:4px;'>暂无数据！</div><div class='left'><a href='/Activity/Detail' class='ico-add  mTop4'>添加活动</a></div><div class='clear'></div></div></td></tr>");
        }
    }

    //初始化操作
    ObjectJS.initOperate = function (Editor, id) {
        var _self = this;
        editor = Editor;
        _self.bindOperateEvent();

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        if(id)
            _self.getDetail(1);
    }

    //绑定事件
    ObjectJS.bindOperateEvent = function () {
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

        //添加客户
        $("#addCustom").click(function () {
            ChooseUser.create({
                title: "添加成员",
                type: 1,
                single: false,
                callback: function (items) {
                    for (var i = 0; i < items.length; i++) {
                        _self.createMember(items[i], "Customs", false);
                    }

                }
            });
        });

        $("#btnSaveActivity").click(function () {
            if (!VerifyObject.isPass()) {
                return false;
            };

            var OwnerID='', MemberID='';
            $("#OwnerIDs .member").each(function () {
                OwnerID += $(this).data("id")+"|";
            });
            $("#MemberIDs .member").each(function () {
                MemberID += $(this).data("id") + "|";
            });

            if (OwnerID == '' || MemberID=='')
            {
                alert("请选择负责人和成员");
                return false;
            }

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
    ObjectJS.getDetail = function (option) {
        var _self = this;
        Global.post("/Activity/GetActivityDetail",
            { activityID: $("#ActivityID").val() },
            function (data) {
                if (data.Item) {
                    var item = data.Item;
                    if (option == 1) {
                        $("#Name").val(item.Name);
                        $("#PosterImg").val(item.Poster);
                        $("#EndTime").val(item.EndTime.toDate("yyyy-MM-dd"));
                        $("#BeginTime").val(item.BeginTime.toDate("yyyy-MM-dd"));
                        $("#Address").val(item.Address);

                        editor.ready(function () {
                            editor.setContent(decodeURI(item.Remark));
                        });
                    }
                    else
                    {
                        $("#Name").html(item.Name);
                        $("#EndTime").html(item.EndTime.toDate("yyyy-MM-dd"));
                        $("#BeginTime").html(item.BeginTime.toDate("yyyy-MM-dd"));
                        $("#Address").html(item.Address);
                    }

                    $("#PosterDisImg").attr("src", item.Poster);
                    ObjectJS.createMemberDetail(item.Owner, "OwnerIDs");
                    for (var i = 0; i < item.Members.length; i++) {
                        ObjectJS.createMemberDetail(item.Members[i], "MemberIDs");
                    }
                    require.async("businesscard", function () {
                        $("div.member").businessCard();
                    });

                }
            });
    }

    //拼接一个用户成员
    ObjectJS.createMember = function (item, id, isSingle) {
        if ($("#" + id + " div[data-id='" + item.id + "']").html())
            return false;

        var html = '<div class="member left" data-id="' + item.id + '">';
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

        require.async("businesscard", function () {
            $("div.member").businessCard();
        });
    }

    //拼接一个用户成员
    ObjectJS.createMemberDetail = function (item, id) {
        if (item.Avatar == '')
            item.Avatar = "/modules/images/defaultavatar.png";
        var html = '<div class="member left" data-id="' + item.UserID + '">';
        html += '    <div class="left pRight5">';
        html += '          <img src="' + item.Avatar + '" />';
        html += '     </div>';
        html += '      <div class="left mRight10 pLeft5"><a href="javascript:void(0);" onclick="$(this).parents(\'.member\').remove();">×</a></div>';
        html += '      <div class="clear"></div>';
        html += '   </div>';

        $("#" + id).append(html);

       
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/Activity/SavaActivity", { entity: JSON.stringify(model) }, function (data) {
            if (data.ID.length > 0) {
                location.href = "/Activity/MyActivity"
            }
        })
    }


    //初始化详情
    ObjectJS.initDetail = function () {
        var _self = this;
        _self.bindDetailEvent();

        _self.getDetail(2);
    }

    //绑定事件
    ObjectJS.bindDetailEvent = function () {
        var _self = this;
       
        $("#btn_shareTask").click(function () {
            var OwnerID = '', MemberID = '';
            $("#OwnerIDs .member").each(function () {
                OwnerID += $(this).data("id") + "|";
            });
            $("#MemberIDs .member").each(function () {
                MemberID += $(this).data("id") + "|";
            });


            if (OwnerID == '' || MemberID == '') {
                alert("请选择负责人和成员");
                return false;
            }
            var model = {
                ActivityID: $("#ActivityID").val(),
                Name: $("#Name").html(),
                OwnerID: OwnerID,
                MemberID: MemberID,
                BeginTime: $("#BeginTime").html(),
                EndTime: $("#EndTime").html(),
                Address: $("#Address").html(),
            };

            Global.post("/Activity/ShareTask", { entity: JSON.stringify(model) }, function (data) {
                if (data.Result > 0) {
                    alert("分享成功");
                }
            })
        });

        $("#btn_shareCalendar").click(function () {
            var OwnerID = '', MemberID = '';
            $("#OwnerIDs .member").each(function () {
                OwnerID += $(this).data("id") + "|";
            });
            $("#MemberIDs .member").each(function () {
                MemberID += $(this).data("id") + "|";
            });


            if (OwnerID == '' || MemberID == '') {
                alert("请选择负责人和成员");
                return false;
            }
            var model = {
                ActivityID: $("#ActivityID").val(),
                Name: $("#Name").html(),
                OwnerID: OwnerID,
                MemberID: MemberID,
                BeginTime: $("#BeginTime").html(),
                EndTime: $("#EndTime").html(),
                Address: $("#Address").html(),
            };

            Global.post("/Activity/ShareCalendar", { entity: JSON.stringify(model) }, function (data) {
                if (data.Result > 0) {
                    alert("分享成功");
                }
            })

        });
    }



    module.exports = ObjectJS;
});
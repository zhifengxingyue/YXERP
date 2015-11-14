/*
*布局页JS
*/
define(function (require, exports, module) {
    var $ = require("jquery"),
        doT = require("dot"),
        Global = require("global"),
        Easydialog = require("easydialog");

    var LayoutObject = {};
    //初始化数据
    LayoutObject.init = function () {
        LayoutObject.bindStyle();
        LayoutObject.bindEvent();
    }

    //绑定事件
    LayoutObject.bindEvent = function () {
        //调整浏览器窗体
        $(window).resize(function () {
            //Height = document.documentElement.clientHeight, Width = document.documentElement.clientWidth;
            LayoutObject.bindStyle();
        });

        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("modules") && !$(e.target).hasClass("modules")) {
                $("#choosemodules").removeClass("hover");
                $(".choose-modules").fadeOut("1000");
            }
            if (!$(e.target).parents().hasClass("currentuser") && !$(e.target).hasClass("currentuser")) {
                $(".dropdown-userinfo").fadeOut("1000");
            }

            if (!$(e.target).parents().hasClass("companyName") && !$(e.target).hasClass("companyName")) {
                $(".dropdown-companyinfo").fadeOut("1000");
            }
        });

        //选择一级菜单
        $("#choosemodules").click(function () {
            var _this = $(this);

            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                if ($(".choose-modules").length == 0) {
                    Global.post("/Base/GetTopMenus", {}, function (data) {
                        doT.exec("template/common/choosemodules.html", function (templateFun) {
                            var innerHTML = templateFun(data.Items);
                            innerHTML = $(innerHTML);
                            //鼠标进入
                            innerHTML.find(".modules-item").mouseenter(function () {
                                var _this = $(this).find("img");
                                _this.attr("src", _this.data("hover"));
                            });
                            //鼠标离开
                            innerHTML.find(".modules-item").mouseleave(function () {
                                var _this = $(this).find("img");
                                _this.attr("src", _this.data("ico"));
                            });

                            innerHTML.fadeIn("1000");
                            $("body").append(innerHTML);
                        });
                    });
                } else {
                    $(".choose-modules").fadeIn("1000");
                }
            } else {
                _this.removeClass("hover");
                $(".choose-modules").fadeOut("1000");
            }
        });
   

        //登录信息展开
        $("#currentUser").click(function () {
            $(".dropdown-userinfo").fadeIn("1000");
        });

        //公司名称信息展开
        $(".companyName").click(function () {
            $(".dropdown-companyinfo").fadeIn("1000");
        });

        //二级菜单图标事件处理
        $("#controllerMenu a").mouseenter(function () {
            var _this = $(this).find("img");
            _this.attr("src", _this.data("hover"));
        });

        $("#controllerMenu a").mouseleave(function () {
            if (!$(this).hasClass("select")) {
                var _this = $(this).find("img");
                _this.attr("src", _this.data("ico"));
            }
        });

        $("#controllerMenu .select img").attr("src", $("#controllerMenu .select img").data("hover"));

        //二级菜单选中名称
        $(".controller-name").html($("#controllerMenu .select span").html());

        //意见反馈
        $(".feedback").click(function () {
            doT.exec("template/feedback/feedback_add.html", function (template) {
                var html = template([]);

                Easydialog.open({
                    container: {
                        id: "show-model-feedback",
                        header: "意见反馈",
                        content: html,
                        yesFn: function () {
                            var entity = {
                                Title: $("#feedback-title").val(),
                                ContactName: $("#feedback-contactname").val(),
                                MobilePhone: $("#feedback-mobilephone").val(),
                                Type: $("#feedback-type").val(),
                                FilePath: $("#feedback-filepath").val(),
                                Remark: $("#feedback-remark").val()
                            };
                            Global.post("/FeedBack/InsertFeedBack", { entity: JSON.stringify(entity) }, function (data) {
                                if (data.Result == 1) {
                                    alert("反馈成功");
                                }
                            });
                        },
                        callback: function () {

                        }
                    }
                });

                $("#feedback-contactname").val($("#txt_username").val());
                $("#feedback-mobilephone").val($("#txt_usermobilephone").val());

                var Upload = require("upload");
                //选择意见反馈附件
                Upload.createUpload({
                    element: "#feedback-file",
                    buttonText: "选择附件",
                    className: "",
                    data: { folder: '/Content/tempfile/', action: 'add', oldPath: "" },
                    success: function (data, status) {
                        if (data.Items.length > 0) {
                            $("#feedback-filepath").val(data.Items[0]);
                            var arr=data.Items[0].split("/");
                            $("#feedback-filename").html(arr[arr.length-1]);
                        }
                    }
                });

            });

        });

       
    }
    //绑定元素定位和样式
    LayoutObject.bindStyle = function () {

    }


    module.exports = LayoutObject;
})
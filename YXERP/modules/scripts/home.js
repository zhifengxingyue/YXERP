

define(function (require, exports, module) {

    require("jquery");
    var Global = require("global"),
        doT = require("dot");

    var Home = {};
    //登陆初始化
    Home.initLogin = function () {
        Home.bindLoginEvent();
    }
    //绑定事件
    Home.bindLoginEvent = function () {

        $(document).on("keypress", function (e) {
            if (e.keyCode == 13) {
                $("#btnLogin").click();
            }
        });

        //登录
        $("#btnLogin").click(function () {
            if (!$("#iptUserName").val()) {
                alert("请输入账号！");
                return;
            }
            if (!$("#iptPwd").val()) {
                alert("请输入密码！");
                return;
            }
            Global.post("/Home/UserLogin", {
                userName: $("#iptUserName").val(),
                pwd: $("#iptPwd").val(),
                remember: $(".cb-remember-password").hasClass("ico-checked") ? 1 : 0
            }, function (data) {
                if (data) {
                    location.href = "/Home/Index";
                } else {
                    alert("账号或密码不正确！")
                }
            });
        });
        if (!$("#iptUserName").val()) {
            $("#iptUserName").focus();
        } else {
            $("#iptPwd").focus();
        }

        //记录密码
        $(".cb-remember-password").click(function () {
            var _this = $(this);
            if (_this.hasClass("ico-check")) {
                _this.removeClass("ico-check").addClass("ico-checked");
            } else {
                _this.removeClass("ico-checked").addClass("ico-check");
            }
        });

    }


    //首页JS
    Home.initHome = function () {
        Home.bindStyle();
        Home.bindEvent();
    }

    Home.bindStyle = function () {

        //图标居中
        //$("#menuItems img").each(function () {
        //    var _this = $(this);
        //    _this.css({ top: _this.parent().height() / 2 - _this.height() / 2, left: _this.parent().width() / 2 - _this.width() / 2 })
        //});

        var width = document.documentElement.clientWidth, height = document.documentElement.clientHeight - 200;
        console.log(document.documentElement);

        var unit = 40;
        if (width >= height * 3) {
            unit = height / 2;
        }

        $("#menuItems a").each(function () {
            var _this = $(this), unit_w = _this.data("width"), unit_h = _this.data("height");
            _this.css({
                width: unit_w * unit,
                height: unit_h * unit
            })
        });

    }

    Home.bindEvent = function () {
        //调整浏览器窗体
        $(window).resize(function () {
            Home.bindStyle();
        });

        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("modules") && !$(e.target).hasClass("modules")) {
                $("#choosemodules").removeClass("hover");
                $(".choose-modules").fadeOut("1000");
            }
            if (!$(e.target).parents().hasClass("currentuser") && !$(e.target).hasClass("currentuser")) {
                $(".dropdown-userinfo").fadeOut("1000");
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
    }

    module.exports = Home;
});


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

        var width = document.documentElement.clientWidth - 360, height = document.documentElement.clientHeight - 200;

        var unit = 40;
        if (width >= height * 3) {
            unit = height / 2;
        } else {
            unit = width / 6;
        }

        $("#menuItems").css({
            marginLeft: (document.documentElement.clientWidth - unit * 6 - 25) / 2,
            marginTop: (document.documentElement.clientHeight - unit * 2) / 2
        })

        $(".middle-items").each(function () {
            var _this = $(this), unit_w = _this.data("width"), unit_h = _this.data("height");
            _this.css({
                width: unit_w * unit + 25,
                height: unit_h * unit
            });
        });

        $("#menuItems a").each(function () {
            var _this = $(this), unit_w = _this.data("width"), unit_h = _this.data("height");
            _this.css({
                width: unit_w * unit - unit_h % 2 * 5,
                height: unit_h * unit - unit_h % 2 * 5
            })
        });

        $("#menuItems img").each(function () {
            var _this = $(this);
            _this.css({ top: _this.parent().height() / 2 - _this.height() / 2, left: _this.parent().width() / 2 - _this.width() / 2 })
        });

    }

    Home.bindEvent = function () {
        //调整浏览器窗体
        $(window).resize(function () {
            Home.bindStyle();
        });

        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("currentuser") && !$(e.target).hasClass("currentuser")) {
                $(".dropdown-userinfo").fadeOut("1000");
            }
        });

        //折叠收藏
        $("#choosemodules").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.addClass("hover");
                $(".bottom-body").css("height", "0");
            } else {
                $(".bottom-body").css("height", "40px");
                _this.removeClass("hover");
            }
        });

        //登录信息展开
        $("#currentUser").click(function () {
            $(".dropdown-userinfo").fadeIn("1000");
        });
    }

    module.exports = Home;
});
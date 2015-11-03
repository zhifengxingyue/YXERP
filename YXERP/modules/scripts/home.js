

define(function (require, exports, module) {

    require("jquery");
    var Global = require("global");

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

    module.exports = Home;
});
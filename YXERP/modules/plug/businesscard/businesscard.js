
/* 
作者：Allen
日期：2014-11-15
示例:
    $(...).searchKeys(callback);
*/

define(function (require, exports, module) {
    require("plug/businesscard/style.css");
    (function ($) {
        var Defaults = {
            element: "#"
        };

        $.fn.businessCard = function (options) {
            //Defaults = $.extend({}, Defaults, options);
            //drawBusinessCard($(this));
            
            $(this).each(function () {
                drawBusinessCard($(this));
            });
            
        }

        var fadeInTimer = null;
        var fadeOutTimer = null;
        var drawBusinessCard = function (obj) {

            obj.mouseenter(function () {

                if ($("div.businessCard").length == 0) {
                    var _businessCardHtml = '<div class="businessCard">';
                    _businessCardHtml += '<ul>';
                    _businessCardHtml += '<li>';
                    _businessCardHtml += '<div class="left"><img class="userAvatar" src="https://dn-mdpic.qbox.me/UserAvatar/6f00e353-446b-4d2f-a0e1-6dc527e5ca27.jpg?imageView2/1/w/100/h/100/q/90" /></div>';
                    _businessCardHtml += '<div class="left mLeft10">name</div>';
                    _businessCardHtml += '<div class="clear"></div>';
                    _businessCardHtml += '</li>';
                    _businessCardHtml += '<li>';
                    _businessCardHtml += '上海万企明道软件有限公司';
                    _businessCardHtml += '</li>';
                    _businessCardHtml += '<li>';
                    _businessCardHtml += '研发总经理';
                    _businessCardHtml += '</li>';
                    _businessCardHtml += '</ul>';
                    _businessCardHtml += '</div>';
                    _businessCardHtml = $(_businessCardHtml);

                    _businessCardHtml.mouseenter(function () {
                        clearTimeout(fadeOutTimer);
                    });
                    _businessCardHtml.mouseleave(function () {
                        _businessCardHtml.fadeOut();
                    });

                    $("body").append(_businessCardHtml);

                } else
                {
                    $("div.businessCard").mouseenter(function () {
                        clearTimeout(fadeOutTimer);
                    }).mouseleave(function () {
                        $("div.businessCard").fadeOut();
                    });
                }

                var left = $(this).offset().left;
                var top = $(this).offset().top - 104;

                $("div.businessCard").css({ "left": left + "px", "top": top + "px" });
                fadeInTimer = setTimeout(function () { $("div.businessCard").fadeIn(); }, 50); 
            });

            obj.mouseleave(function () {
                clearTimeout(fadeInTimer);
                fadeOutTimer = setTimeout(function () { $("div.businessCard").fadeOut(); }, 1000);
            });
        }

    })(jQuery)
    module.exports = jQuery;
});
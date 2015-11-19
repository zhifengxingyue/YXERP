/*

--引用
var express = require("express");
--实例化
var express = express.create({
    elementID: "" //父块状元素ID
});
express.getID(); 
express.getName(); 
express.setValue(id); 
*/
define(function (require, exports, module) {

    var $ = require("jquery"), Global = require("global"), Option = "<option value=''>请选择</option>";
    var Default = {
        elementID: "express",
        value: "",
        dataUrl: "/Plug/GetExpress"
    };
    var Express = function (options) {
        this.setting = [];
        this.init(options);
    };
    //初始化
    Express.prototype.init = function (options) {
        var _self = this, _element;
        _self.setting = $.extend([], Default, options);
        _element = $("#" + _self.setting.elementID)
        _self.express = $("<select></select>", { id: _self.setting.elementID + "_express", "class": "" }).append(Option);
       
        _element.append(_self.express);
       
        Global.post(_self.setting.dataUrl, { }, function (data) {
            var _length = data.items.length;
            for (var i = 0; i < _length; i++) {
                _self.express.append("<option value=" + data.items[i].ExpressID + ">" + data.items[i].Name + "</option>");
            }
            _self.express.val(_self.setting.value);
        });
    }
    //获取快递公司ID
    Express.prototype.getID = function () {
        var _self = this;
        return _self.express.val();
    }
    //获取快递公司名称
    Express.prototype.getName = function () {
        var _self = this;
        return _self.express.find("option:selected").text();
    }
    //获取地区名称
    Express.prototype.setValue = function (id) {
        var _self = this;
        var express = _self.express.find("option[value='" + id + "']");
        express.prop("selected", "selected");
    }
    exports.create = function (options) {
        return new Express(options);
    }
});

﻿
/*基础配置*/
seajs.config({
    base: "/modules/",
    alias: {
        "jquery": "/Scripts/jquery-1.11.1.js",
        "global": "scripts/global.js",
        //HTML模板引擎
        "dot": "plug/doT.js",
        //分页控件
        "pager": "plug/datapager/paginate.js"
    }
});


seajs.config({
    alias: {
        //数据验证
        "verify": "plug/verify.js",
        //城市地区
        "city": "plug/city.js",
        //快递公司
        "express": "plug/express.js",
        //上传
        "upload": "plug/upload/upload.js",
        //开关插件
        "switch": "plug/switch/switch.js",
        //标签插件
        "mark": "plug/mark/mark.js",
        //弹出层插件
        "easydialog": "plug/easydialog/easydialog.js",
        //搜索插件
        "search": "plug/seach_keys/seach_keys.js",
        //购物车
        "cart": "plug/shoppingcart/shoppingcart.js",
        //选择员工
        "chooseuser": "plug/chooseuser/chooseuser.js",
        //选择客户
        "choosecustomer": "plug/choosecustomer/choosecustomer.js",
        //选择下属
        "choosebranch": "plug/choosebranch/choosebranch.js",
        //选择员工
        "dropdown": "plug/dropdown/dropdown.js",
        //显示用户名片层
        "businesscard": "plug/businesscard/businesscard.js"
    }
});


/**
 * JavaScript utility
 * */
var JSUtils = function () {
};
/**
 * Http RequestHeader ContentType
 * */
var ContentType = {
    /**
     * application/x-www-form-urlencoded
     */
    Form: 'application/x-www-form-urlencoded',
    /**
     * application/json
     */
    Json: 'application/json',
    /**
     * false
     */
    FormData: false
}
JSUtils.prototype = {
    /** ��ȡ��ǰʱ��ʱ���
     * @returns {Number} ʱ���
     */
    GetUnix_tmsp: function () {
        var timestamp = Math.round(Date.now() / 1000);
        return timestamp;
    },
    /**
     *  ��ȡʱ�����ʱ��
     * @param {Number} unix_timestamp ʱ���
     * @returns {Date} ʱ��
     */
    GetUnix_tmspToTime: function (unix_timestamp) {
        var result = new Date(unix_timestamp * 1000).toLocaleString();
        return result;
    },
    /**
     *  ��ȡ��ַ���Ĳ���
     * @param {String} name ʱ���
     * @returns {any} ����ֵ
     */
    GetQueryString: function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", i);
        var result = window.location.search.substr(1).match(reg);
        if (result != null) {
            return decodeURIComponent(result[2]);
        } else {
            return null;
        }
    },
    /**
     * �ж�Ԫ�����������Ƿ����
     * @param {Array} arr ����
     * @param {any} element Ԫ��
     * @returns {Boolean} �Ƿ����
     */
    IsInArray: function (arr, element) {
        for (var i = 0; i < arr.length; i++) {
            if (element === arr[i]) {
                return true;
            }
        }
        return false;
    },
    Extend: function (target, source) {
        var result = target;
        for (var p in source) {
            if (source.hasOwnProperty(p)) {
                result[p] = source[p];
            }
        }
        return result;
    }
    ,
    /**
     * JavaScript Http Request
     * @param {Object} options { type | async | contentType | data | success(response, status,xhr) | error(response,status,xhr) }
     */
    Ajax: function (options) {
        var xhr = new XMLHttpRequest();
        var defaults = {
            type: 'GET',
            async: true,
            contentType: ContentType.Form,
            data: null,
            success: function (response, status, xhr) { },
            error: function (response, status, xhr) { }
        };
        var model = this.Extend(defaults, options);
        if (model.type == "GET") {
            xhr.open(model.type, model.url + "?" + this.ConvertData(model.data), model.async);
        } else {
            xhr.open(model.type, model.url, model.async);
        }
        if (model.contentType != ContentType.FormData) {
            xhr.setRequestHeader('Content-type', model.contentType);
        }
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                model.success(xhr.response, xhr.status, xhr);
            } else {
                model.error(xhr.response, xhr.status, xhr);
            }
        };
        switch (model.contentType) {
            case ContentType.Form:
                xhr.send(model.type == "GET" ? null : this.ConvertData(model.data));
                break;
            case ContentType.Json:
                if (model.type == "GET") {
                    return;
                }
                xhr.send(JSON.stringify(model.data));
                break;
            case ContentType.FormData:
                if (model.type == "GET") {
                    return;
                }
                xhr.send(model.data);
                break;
        }
    },
    ConvertData: function (data) {
        if (typeof data === 'object') {
            var convertResult = "";
            for (var c in data) {
                convertResult += c + "=" + data[c] + "&";
            }
            convertResult = convertResult.substring(0, convertResult.length - 1)
            return convertResult;
        } else {
            return data;
        }
    },
    /**
     * Cookie MaxSize 4kb,MaxLength 20
     */
    SetCookie: function (key, value, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toGMTString();
        document.cookie = key + "=" + value + "; " + expires;
    },
    GetCookie: function (key) {
        var arr, reg = new RegExp("(^| )" + key + "=([^;]*)(;|$)");
        if (arr = document.cookie.match(reg))
            return unescape(arr[2]);
        else
            return null;
    },
    DelCookie: function (key) {
        var exp = new Date();
        exp.setTime(exp.getTime() - 1);
        var cval = getCookie(key);
        if (cval != null)
            document.cookie = key + "=" + cval + ";expires=" + exp.toGMTString();
    },
    /**
     * localStorage MaxSize 5MB
     */
    SetLocalStorage: function (key, value) {
        localStorage.setItem(key, value);
    },
    GetLocalStorage: function (key) {
        localStorage.getItem(key);
    },
    RemoveLoaclStorage: function (key) {
        localStorage.removeItem(key);
    },
    /**
     * SetSessionStorage MaxSize 5MB
     */
    SetSessionStorage: function (key, value) {
        sessionStorage.setItem(key, value);
    },
    GetSessionStorage: function (key) {
        sessionStorage.getItem(key);
    },
    RemoveSessionStorage: function (key) {
        sessionStorage.removeItem(key);
    }
};
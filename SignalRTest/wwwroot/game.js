//消息处理
var MSG = {
    isStart: false,
    connection: null,

    /**
     * 初始化信息处理 返回promis
     * @param {string} _url 消息地址
     * @param {Function} _func 相关绑定函数
     */
    init: function (url) {
        if (url == null)
            url = "/game";
        MSG.connection = new signalR.HubConnectionBuilder().withUrl(url).build(); 
    },
    /**
     * 注册监听函数
     * @param {any} key 监听键名
     * @param {any} func 监听执行函数
     */
    reg: function (key,func) {
        MSG.connection.on(key, function (result) {
            func(result);
        });
    },
    /**启动消息 返回promis*/
    start: function () {
        if (MSG.isStart) {
            return;
        }
        MSG.isStart = true;
        var _result = MSG.connection.start();
        _result.then(function (_return) {

        }).catch(function (err) {
            MSG.isStart = false;
            return console.error(err.toString());
        });
        return _result;
    },

   /**
    * 停止消息 返回promis
    * */
    stop: function () {
        if (!MSG.isStart) {
            return;
        }
        var _result = MSG.connection.stop();
        _result.then(function (_return) {

        }).catche(function (err) {

        });
        MSG.isStart = false;
        return _result;
    }, 
    /**
     * 
     * @param {any} api
     * @param {any} msg
     */
    send: function (api, msg) {
        if (!MSG.isStart) {
            return;
        }
        var _result = MSG.connection.invoke(api, msg);
        //回调处理
        _result.then(function (_return) {

        });
        //错误处理
        _result.catch(function (_error) {

        });
        return _result;
    },
    /**重新连接 未实现
     *@param  {number} num 尝试连接次数
     */
    reconnection: function (num) {
        if (MSG.isStart) {

        }
    }
}; 
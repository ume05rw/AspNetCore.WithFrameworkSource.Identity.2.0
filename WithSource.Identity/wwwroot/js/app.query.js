var app = app || {};
(function () {

    var appStatics = {
        MethodType: {
            Get: "GET",
            Post: "POST",
            Put: "PUT",
            Patch: "PATCH",
            Delete: "DELETE",
            //Head: "HEAD",
            //Connect: "CONNECT",
            //Options: "OPTIONS",
            //Trace: "TRACE",
        }
    };

    var appImplement = {
        _maxId: 0,
        _querys: {},

        _defaultConfig: {
            method: "POST",
            retryCount: 3,
            retryDelay: 2000,
            url: null,
            XCsrfToken: null,
            caller: null,
            callback: app.nullFunction,
            failureCallback: app.nullFunction,
        },

        _getId: function () {
            this._maxId += 1;
            return this._maxId;
        },

        id: null,
        count: 1,
        config: {},

        __constructor: function (config) {
            //コンストラクタ
            this.id = this._getId();

            this.config = $.extend({}, this._defaultConfig, config);
            this.count = 0;
            if (this.config.caller == null) {
                this.config.caller = this;
            }

            if ($.isFunction(this.config.callback)) {
                this.config.callback
                    = this.config.callback.bind(this.config.caller);
            }

            if ($.isFunction(this.config.failureCallback)) {
                this.config.failureCallback
                    = this.config.failureCallback.bind(this.config.caller);
            }

            this._querys[this.queryId] = this.config;
        },

        exec: function (value) {

            if ($.isArray(value)
                || $.isEmptyObject(value)
                || $.isPlainObject(value)) {
                value = JSON.stringify(value);
            }

            this.count++;

            $.ajax({
                url: this.config.url,
                method: this.config.method,
                cache: false,
                data: value,
                type: "json",
                beforeSend: function (xhr) {
                    if (this.config.XCsrfToken) {
                        xhr.setRequestHeader("X-CSRF-Token", this.config.XCsrfToken);
                    }
                    //xhr.setRequestHeader("X-CSRF-Token", csrf);
                    //xhr.setRequestHeader("_Token", csrf);
                }.bind(this),
            }).done(function (data) {

                if ($.isFunction(this.config.callback)) {
                    try {
                        this.config.callback(data);
                    } catch (ex) {
                        app.log(ex);
                    }
                }

            }.bind(this)).fail(function (data) {

                if (this.count <= this.config.retryCount) {
                    //リトライ
                    setTimeout(function () {
                        app.log("retrying...");
                        this.exec(value);
                    }.bind(this), this.config.retryDelay);

                } else {
                    if ($.isFunction(this.config.failureCallback)) {
                        try {
                            this.config.failureCallback(data);
                        } catch (ex) {
                            app.log(ex);
                        }
                    }
                }

            }.bind(this));
        }
    };

    app.query = appImplement.__constructor;
    app.query.prototype = $.extend({}, appImplement);
    $.extend(app.query, appStatics);
})();

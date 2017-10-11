var app = app || {};
(function () {
    var appImplement = {

        _defaultConfig: {
            dulation: 300,
            elementId: "infoBox",
            type: "info",
        },

        $warningBox: null,
        config: {},
        __constructor: function (config) {

            this.config = $.extend({}, this._defaultConfig, config);

            if ($.inArray(this.config.type, [
                "success",
                "info",
                "warning",
                "danger",
            ]) < 0) {
                alert("app.infoBox: invalid type: " + this.config.type);
                return;
            }

            this.$warningBox = $("#" + this.config.elementId);
            if (this.$warningBox.length <= 0) {
                $("body").append('<div id="' + this.config.elementId + '"></div>')
                this.$warningBox = $("#" + this.config.elementId);
            }

            this.$warningBox
                .removeClass("alert alert-success alert-info alert-warning alert-danger")
                .removeAttr("role")
                .addClass("alert alert-" + this.config.type)
                .attr("role", "alert")
                .css("display", "none");
        },

        _setMessage: function (message) {

            var box = this.$warningBox;
            box.empty();

            if ($.isArray(message)
                || $.isPlainObject(message)) {
                $.each(message, function () {
                    box.append('<strong>' + this + '</string><br />');
                });
            } else {
                box.append('<strong>' + message + '</string>');
            }

            return box;
        },

        show: function (message) {
            this.$warningBox.hide(
                this.config.dulation,
                function () {
                    this._setMessage(message)
                        .show(this.config.dulation);
                }.bind(this)
            );
        }
    };

    var appStatics = {
        Type: {
            Success: "success",
            Info: "info",
            Warning: "warning",
            Danger: "danger",
        }
    };

    app.infoBox = appImplement.__constructor;
    app.infoBox.prototype = $.extend({}, appImplement);
    $.extend(app.infoBox, appStatics);
})();
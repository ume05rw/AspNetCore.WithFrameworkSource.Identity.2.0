var app = app || {};
app.log = function (message) {
    var date = new Date();
    try {
        console.log(
            ('0000' + date.getFullYear()).slice(-4)
            + "-" + ('00' + (date.getMonth() + 1)).slice(-2)
            + "-" + ('00' + date.getDate()).slice(-2)
            + " " + ('00' + date.getHours()).slice(-2)
            + ":" + ('00' + date.getMinutes()).slice(-2)
            + ":" + ('00' + date.getSeconds()).slice(-2)
            + "." + ('000' + date.getMilliseconds()).slice(-3)
            + " : " + message);
    } catch (e) {
        alert(e);
    }
};
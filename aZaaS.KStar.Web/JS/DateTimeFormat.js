define(function (require, exports, module) {
    var getDateTimeFormat = function () {
        return '{0:' + window.DateTimeFormat + '}';
    }
    module.exports = getDateTimeFormat;
})
var babel = require("babel-core");

module.exports = function (callback, code) {
    var rs = babel.transform(code,
        {
            presets: [
                "es2015",
                "react"
            ]
        });

    callback(null, rs.code);
}
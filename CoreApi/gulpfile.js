var gulp = require('gulp');
var replace = require('gulp-replace');
var _    = require('lodash');

gulp.task('default', ['copy-assets','copy-semantic-ui']);

gulp.task('copy-assets', function () {

    var assets = {
        js: [
            './node_modules/jquery/dist/jquery.min.js',
            './node_modules/bootstrap/dist/js/bootstrap.min.js'
        ],
        css: [
            './node_modules/bootstrap/dist/css/bootstrap.min.css',
            './node_modules/font-awesome/css/*.min.css'
        ],
        fonts: [
            './node_modules/bootstrap/dist/fonts/**',
            './node_modules/font-awesome/fonts/**'
        ]
    };
    
    _(assets).forEach(function(asset, type) {
        _(asset).forEach(function(fileName, index){
            if (fileName.indexOf('font-awesome/css') !== -1)
            {
                gulp.src(fileName)
                .pipe(replace('../fonts/fontawesome-webfont', '../fonts/fontawesome-webfont'))
                .pipe(gulp.dest('./wwwroot/dist/' + type));
            }
            else
            {
                gulp.src(fileName).pipe(gulp.dest('./wwwroot/dist/' + type));
            }
        });
    });

});

gulp.task('copy-semantic-ui', function() {

    var assets = {
        js: [
            './semantic-ui/dist/semantic.min.js',
        ],
        css: [
            './semantic-ui/dist/semantic.min.css',
        ],
        images: [
            './semantic-ui/dist/themes/default/assets/images/**',
        ]
    };
    
    _(assets).forEach(function(assets, type) {

        if (type === "css")
        {
            gulp.src(assets)
            .pipe(replace('themes/default/assets/', '../'))
            .pipe(replace('./fonts/icons', './fonts/fontawesome-webfont'))
            .pipe(gulp.dest('./wwwroot/dist/' + type));
        }
        else
        {
            gulp.src(assets)
            .pipe(gulp.dest('./wwwroot/dist/' + type));
        }

    });

});
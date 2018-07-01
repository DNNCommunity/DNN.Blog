var gulp = require('gulp'),
  rename = require('gulp-rename'),
  path = require('path'),
  less = require('gulp-less'),
  lessPluginCleanCSS = require('less-plugin-clean-css'),
  cleancss = new lessPluginCleanCSS({
    advanced: true
  });

gulp.task('less', function() {
  return gulp.src('css/template.less')
    .pipe(less({
      paths: [path.join(__dirname, 'less', 'includes')],
      plugins: [cleancss]
    }))
    .pipe(rename('template.css'))
    .pipe(gulp.dest('../..'));
});

gulp.task('watch', function() {
  gulp.watch('css/**/*.less', ['less']);
});


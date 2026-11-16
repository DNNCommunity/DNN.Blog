const chokidar = require("chokidar"),
  fs = require("fs-extra"),
  path = require("path"),
  deepmerge = require("deepmerge");

var pkg = require("./package.json"),
  pkgBlog = require("./Server/Blog/dnn.json");
if (fs.existsSync("./local.json")) {
  var local = require("./local.json");
  pkg = deepmerge(pkg, local);
}

var log = console.log.bind(console);

function copy(start, srcRelativePath, destDir) {
  if (!fs.existsSync(srcRelativePath)) {
    return null;
  }
  var relPath = path.relative(start, srcRelativePath);
  const fullDestPath = path.join(destDir, relPath);
  log("Copying: " + srcRelativePath);
  return fs
    .ensureDir(path.dirname(fullDestPath))
    .then(() => fs.copy(srcRelativePath, fullDestPath))
    .catch((err) => {
      console.error(err);
    });
}

// Chokidar 4+ no longer treats ignored strings as globs (exact paths only).
// Use RegExp matchers instead; paths are normalized to forward slashes on Windows.
var ignore = [
  /\/obj\//,
  /\/bin\//,
  /\.(vb|cs|suo|user)$/i,
  /\/dnn\.json$/,
  /\.[^./]{2}proj$/,
  /\/web[^/]*\.config$/i,
  /\/packages\.config$/,
  /(^|\/)\./,
  /\/launchSettings\.json$/,
];

const allDlls = pkgBlog.pathsAndFiles.assemblies
  // .concat(pkgBlog.pathsAndFiles.assemblies)
  .map(dll => "bin/" + dll);

const watcher = (src, dest) =>
  chokidar
    .watch(src, {
      ignored: ignore,
      persistent: true
    })
    .on("add", path => copy(src, path, dest))
    .on("change", path => copy(src, path, dest));
// Todo: delete events?

// Initialize watchers.
const BlogWatcher = watcher(
  "Server/Blog",
  pkg.dnn.pathsAndFiles.devSitePath +
    "\\DesktopModules\\MVC\\Blog"
);

const DllWatcher = chokidar
  .watch(allDlls, {
    persistent: true
  })
  .on("add", path => {
    copy("bin", path, pkg.dnn.pathsAndFiles.devSitePath + "\\bin");
    copy(
      "bin",
      path.replace(".dll", ".pdb"),
      pkg.dnn.pathsAndFiles.devSitePath + "\\bin"
    );
  })
  .on("change", path => {
    copy("bin", path, pkg.dnn.pathsAndFiles.devSitePath + "\\bin");
    copy(
      "bin",
      path.replace(".dll", ".pdb"),
      pkg.dnn.pathsAndFiles.devSitePath + "\\bin"
    );
  });

import commonConfig from "../../rsbuild.common.config";

const clientAppConfig = Object.assign({}, commonConfig, {
  source: {
    entry: {
      modulecss: "./Client/Css/Module/scss/styles.scss",
    },
  },
  output: {
    target: "web",
    cleanDistPath: false,
    distPath: {
      root: "./Server/Blog",
      js: "../../.out",
      html: "../../.out",
      css: "",
    },
    filename: {
      css: "module.css",
    },
  },
  // Keep Font Awesome as a sibling runtime @import (Server/Blog/css/...),
  // matching the historic module.css behavior instead of bundling it.
  tools: {
    cssLoader: {
      import: {
        filter: (url: string) => !url.includes("font-awesome"),
      },
    },
  },
});

export default clientAppConfig;

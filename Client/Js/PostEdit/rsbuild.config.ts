import path from "path";
import commonConfig from "../../rsbuild.common.config";

const htmlEditorRoot = path.resolve(__dirname, "../HtmlEditor");

const clientAppConfig = Object.assign({}, commonConfig, {
  source: {
    entry: {
      postedit: "./Client/Js/PostEdit/App.tsx",
    },
    alias: {
      "@": htmlEditorRoot,
    },
  },
  output: {
    target: "web",
    filenameHash: false,
    cleanDistPath: false,
    injectStyles: true,
    distPath: {
      root: "./Server/Blog/js",
      js: "",
      css: "../../../.out",
      html: "../../../.out",
    },
    filename: {
      js: "postedit.js",
    },
    legalComments: "none",
  },
  performance: {
    chunkSplit: {
      strategy: "all-in-one",
    },
  },
  tools: {
    postcss: (opts: { postcssOptions: { plugins?: any[] } }) => {
      const tailwind = require("tailwindcss");
      const autoprefixer = require("autoprefixer");
      opts.postcssOptions = opts.postcssOptions || {};
      opts.postcssOptions.plugins = [
        tailwind({
          config: path.resolve(htmlEditorRoot, "tailwind.config.js"),
        }),
        autoprefixer(),
      ];
      return opts;
    },
    rspack: {
      output: {
        asyncChunks: false,
      },
    },
  },
});

export default clientAppConfig;

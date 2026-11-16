import path from "path";
import commonConfig from "../../rsbuild.common.config";

const clientAppConfig = Object.assign({}, commonConfig, {
  source: {
    entry: {
      htmleditor: "./Client/Js/HtmlEditor/App.tsx",
    },
    alias: {
      "@": path.resolve(__dirname),
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
      js: "htmleditor.js",
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
          config: path.resolve(__dirname, "tailwind.config.js"),
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

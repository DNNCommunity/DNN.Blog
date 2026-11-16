import commonConfig from "../../rsbuild.common.config";

const clientAppConfig = Object.assign({}, commonConfig, {
  source: {
    entry: {
      comments: "./Client/Js/Comments/App.tsx",
    },
  },
  output: {
    target: "web",
    filenameHash: false,
    cleanDistPath: false,
    injectStyles: false,
    distPath: {
      root: "./Server/Blog/js",
      js: "",
      css: "../../../.out",
      html: "../../../.out",
    },
    filename: {
      js: "comments.js",
    },
    legalComments: "none",
  },
  performance: {
    chunkSplit: {
      strategy: "all-in-one",
    },
  },
  tools: {
    rspack: {
      output: {
        asyncChunks: false,
      },
    },
  },
});

export default clientAppConfig;

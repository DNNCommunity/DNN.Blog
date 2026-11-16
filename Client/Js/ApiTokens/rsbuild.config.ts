import commonConfig from "../../rsbuild.common.config";

const clientAppConfig = Object.assign({}, commonConfig, {
  source: {
    entry: {
      apitokens: "./Client/Js/ApiTokens/App.tsx",
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
      js: "apitokens.js",
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

import path from "path";
import commonConfig from "../../rsbuild.common.config";

const richEditorRoot = path.resolve(__dirname, "../RichEditor");

const clientAppConfig = Object.assign({}, commonConfig, {
  source: {
    entry: {
      postedit: "./Client/Js/PostEdit/App.tsx",
    },
    alias: {
      "@": richEditorRoot,
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
    rspack: {
      output: {
        asyncChunks: false,
      },
    },
  },
});

export default clientAppConfig;

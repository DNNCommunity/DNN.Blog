import commonConfig from "../../rsbuild.common.config";

const clientAppConfig = Object.assign({}, commonConfig, {
  source: {
    entry: {
      blog: "./Client/Js/Blog/App.tsx",
    },
  },
  output: {
    target: "web",
    filenameHash: false,
    cleanDistPath: false,
    distPath: {
      root: "./Server/Blog/js",
      js: "",
      css: "../../../.out",
      html: "../../../.out",
    },
    filename: {
      js: "blog.js",
    },
    legalComments: "none",
  },
});

export default clientAppConfig;

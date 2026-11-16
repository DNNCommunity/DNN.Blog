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
});

export default clientAppConfig;

import { defineConfig } from "@rsbuild/core";
import { pluginReact } from "@rsbuild/plugin-react";
import { pluginSass } from "@rsbuild/plugin-sass";

export default defineConfig({
  plugins: [pluginReact(), pluginSass()],
  dev: {
    writeToDisk: (file) => !file.includes(".hot-update."),
  },
  performance: {
    chunkSplit: {
      strategy: "all-in-one",
    },
  },
});

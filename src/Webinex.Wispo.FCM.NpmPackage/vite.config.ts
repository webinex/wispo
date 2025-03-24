import { defineConfig } from "vite";
import { resolve } from "node:path";
import dts from "vite-plugin-dts";
// @ts-expect-error no definition file
import peerDepsExternal from "rollup-plugin-peer-deps-external";

export default defineConfig({
  plugins: [
    peerDepsExternal({ includeDependencies: true }),
    dts({
      include: ["lib"],
      pathsToAliases: true,
      tsconfigPath: resolve(__dirname, "./tsconfig.lib.json"),
      outDir: resolve(__dirname, "./dist/types"),
    }),
  ],
  build: {
    copyPublicDir: false,
    minify: true,
    sourcemap: true,
    lib: {
      entry: resolve(__dirname, "lib/index.ts"),
      formats: ["es"],
      fileName: "index",
    },
  },
});

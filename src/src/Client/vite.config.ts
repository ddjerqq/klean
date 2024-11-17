import { svelte } from '@sveltejs/vite-plugin-svelte';
import routify from '@roxi/routify/vite-plugin';
import { defineConfig } from 'vite';
import path from 'path';

const dev = process.env.NODE_ENV === 'development';

export default defineConfig({
  clearScreen: false,
  plugins: [
    routify(),
    svelte({
      compilerOptions: {
        dev: dev,
      },
    }),
  ],
  build: {
    minify: "terser",
    rollupOptions: {
      output: {
        manualChunks: id => id.includes('node_modules') ? 'vendor' : 'bundle',
      },
    },
  },
  server: {
    host: '127.0.0.1',
    port: 2080,
    hmr: dev,
    // proxy: {
    //   '^/api': {
    //     target: 'http://localhost:1080',
    //     secure: false,
    //   },
    // },
  },
  resolve: {
    alias: {
      $lib: path.resolve(__dirname, 'src/lib'),
    },
  },
});

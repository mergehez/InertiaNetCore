import vue from '@vitejs/plugin-vue';
import { defineConfig } from 'vite';
import path from 'path';
import laravel from 'laravel-vite-plugin';
import { mkdirSync } from 'node:fs';

const outDir = '../../wwwroot/build';
mkdirSync(outDir, { recursive: true });

export default defineConfig({
    plugins: [
        laravel({
            input: ['src/app.ts', 'src/app.scss'],
            publicDirectory: outDir,
            refresh: true,
        }),
        vue({
            template: {
                transformAssetUrls: {
                    base: null,
                    includeAbsolute: false,
                },
            },
        }),
    ],
    resolve: {
        alias: {
            '@': path.resolve(__dirname, 'src'),
        },
    },
    build: {
        outDir,
        emptyOutDir: true,
    },
});

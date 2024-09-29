import { createApp, h } from 'vue';
import '@/app.scss';
import { createInertiaApp } from '@inertiajs/vue3';
import { resolvePageComponent } from 'laravel-vite-plugin/inertia-helpers';
import AppLayout from '@/layout/AppLayout.vue';

const appName = window.document.getElementsByTagName('title')[0]?.innerText || 'Inertia';

createInertiaApp({
    // @ts-ignore
    resolve: async (name) => {
        const page = await resolvePageComponent(`./${name}.vue`, import.meta.glob('./**/*.vue'));
        (page as any).default.layout ??= AppLayout;
        // if (name.startsWith('Auth/')){
        // }else{
        //     page.default.layout ??= DefaultLayoutFile;
        // }
        return page;
    },
    setup({ el, App, props, plugin }) {
        createApp({ render: () => h(App, props) })
            .use(plugin)
            .mount(el);
    },
    title: (title) => `${title} - ${appName}`,
    progress: {
        color: '#4B5563',
    },
});
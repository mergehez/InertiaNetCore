import { router } from '@inertiajs/vue3';
import { reactive } from 'vue';

export const globalState = reactive({
    routerWaiting: false,
})

export const initGlobalListeners = () => {
    router.on('before', () => {
        console.log('router before');
        globalState.routerWaiting = true;
    });

    router.on('finish', () => {
        console.log('router finish');
        globalState.routerWaiting = false;
    });
}
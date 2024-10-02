<script setup lang="ts">
import { router, usePage } from '@inertiajs/vue3';
import { ref, unref } from 'vue';
import { globalState } from '@/utils/globalState';
import Spinner from '@/components/Spinner.vue';

const page = usePage<{
    nowDirect: string;
    nowFunc: string;
    nowAlways: string;
    nowLazy?: string;
}>();

const lastProps = ref(unref(page.props));
const loadingNowLazyAsync = ref(false);
function reloadRouter(opts?: any) {
    lastProps.value = unref(page.props);
    router.reload(opts);
}
</script>

<template>
    <div>
        <h1 class="font-bold text-2xl mb-4">Index</h1>
        <p class="mb-2 italic">C# code from HomeController:</p>

        <pre class="text-xs bg-gray-100 dark:bg-gray-900 p-3 mb-10 rounded-md">
public IActionResult Index()
{
    var now = GetNow();
    return Inertia.Render("pages/PageIndex", new InertiaProps
    {
        ["NowDirect"] = now,
        ["NowFunc"] = () => now,
        ["NowAlways"] = Inertia.Always(() => now),
        ["NowLazy"] = Inertia.Lazy(() => now),
        ["NowLazyAsync"] = Inertia.Lazy(async () =>
        {
            await Task.Delay(2000);
            return now;
        }),
    });
}</pre
        >

        <p class="my-2 italic">The result received from it:</p>
        <table>
            <tbody>
                <tr>
                    <td class="font-bold pr-4">NowDirect</td>
                    <td class="font-mono" style="width: 200px">{{ page.props.nowDirect }}</td>
                    <td class="font-bold pl-4" :class="lastProps.nowDirect == page.props.nowDirect ? '' : 'text-green-600'">({{ lastProps.nowDirect == page.props.nowDirect ? 'same' : 'changed' }})</td>
                </tr>
                <tr>
                    <td class="font-bold pr-4">NowFunc</td>
                    <td class="font-mono" style="width: 200px">{{ page.props.nowFunc }}</td>
                    <td class="font-bold pl-4" :class="lastProps.nowFunc == page.props.nowFunc ? '' : 'text-green-600'">({{ lastProps.nowFunc == page.props.nowFunc ? 'same' : 'changed' }})</td>
                </tr>
                <tr>
                    <td class="font-bold pr-4">NowAlways</td>
                    <td class="font-mono" style="width: 200px">{{ page.props.nowAlways }}</td>
                    <td class="font-bold pl-4" :class="lastProps.nowAlways == page.props.nowAlways ? '' : 'text-green-600'">({{ lastProps.nowAlways == page.props.nowAlways ? 'same' : 'changed' }})</td>
                </tr>
                <tr>
                  <td class="font-bold pr-4">NowLazy</td>
                  <td class="font-mono" style="width: 200px">{{ page.props.nowLazy }}</td>
                  <td class="font-bold pl-4" :class="lastProps.nowLazy == page.props.nowLazy ? '' : 'text-green-600'">({{ lastProps.nowLazy == page.props.nowLazy ? 'same' : 'changed' }})</td>
                </tr>
                <tr>
                  <td class="font-bold pr-4">NowLazyAsync</td>
                  <td class="font-mono" style="width: 200px">{{ page.props.nowLazyAsync }}</td>
                  <td class="font-bold pl-4" :class="lastProps.nowLazyAsync == page.props.nowLazyAsync ? '' : 'text-green-600'">({{ lastProps.nowLazyAsync == page.props.nowLazyAsync ? 'same' : 'changed' }})</td>
                </tr>
            </tbody>
        </table>

        <div class="mt-12 flex flex-col gap-1 items-start">
            <button @click="reloadRouter()" class="bg-gray-100 dark:bg-gray-800 border-gray-300 dark:border-gray-700 hover:opacity-80 border px-4 rounded py-1">Reload router</button>
            <button @click="reloadRouter({ only: ['nowDirect'] })" class="bg-gray-100 dark:bg-gray-800 border-gray-300 dark:border-gray-700 hover:opacity-90 border px-4 rounded py-1">Reload router (only NowDirect)</button>
            <button @click="reloadRouter({ only: ['nowFunc'] })" class="bg-gray-100 dark:bg-gray-800 border-gray-300 dark:border-gray-700 hover:opacity-90 border px-4 rounded py-1">Reload router (only NowFunc)</button>
            <button @click="reloadRouter({ only: ['nowAlways'] })" class="bg-gray-100 dark:bg-gray-800 border-gray-300 dark:border-gray-700 hover:opacity-90 border px-4 rounded py-1">Reload router (only NowAlways)</button>
            <button @click="reloadRouter({ only: ['nowLazy'] })" class="bg-gray-100 dark:bg-gray-800 border-gray-300 dark:border-gray-700 hover:opacity-90 border px-4 rounded py-1">Reload router (only NowLazy)</button>
            <button
                @click="reloadRouter({ only: ['nowLazyAsync'], onBefore: () => loadingNowLazyAsync = true, onFinish: () => loadingNowLazyAsync = false })"
                class="bg-gray-100 dark:bg-gray-800 border-gray-300 dark:border-gray-700 hover:opacity-90 border px-4 rounded py-1 flex items-center gap-2">
                <Spinner :visible="loadingNowLazyAsync" class="size-4" />
                Reload router (only NowLazyAsync)
            </button>
        </div>
    </div>
</template>
<script setup lang="ts">
import { Link, usePage } from '@inertiajs/vue3';
import { ref } from 'vue';
import ThemeSwitcher from '@/components/ThemeSwitcher.vue';

const showMenu = ref(true);
const showInertiaProps = ref(true);

const page = usePage<{
    auth: {
        token: string;
        username: string;
    };
}>();
</script>

<template>
    <div class="w-screen h-screen flex flex-col bg-gray-200 dark:bg-gray-800 text-gray-900 dark:text-gray-100">
        <nav class="flex px-5 py-2 bg-white dark:bg-gray-950 items-center gap-2">
            <button class="hover:bg-gray-200 dark:hover:bg-gray-800 p-2 rounded transition-colors duration-300 text-xl" @click="showMenu = !showMenu">
                <svg xmlns="http://www.w3.org/2000/svg" width="1em" height="1em" viewBox="0 0 48 48">
                    <path fill="none" stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="4" d="M7.95 11.95h32m-32 12h32m-32 12h32" />
                </svg>
            </button>
            <Link class="rounded transition-colors duration-300" href="/">InertiaNetCore</Link>

            <i class="flex-1"></i>
            <ThemeSwitcher />
            <button class="hover:bg-gray-200 dark:hover:bg-gray-800 p-2 rounded transition-colors duration-300 text-xl" @click="showInertiaProps = !showInertiaProps">
                <svg xmlns="http://www.w3.org/2000/svg" width="1em" height="1em" viewBox="0 0 32 32" :class="showInertiaProps ? 'text-green-600 dark:text-green-600' : ''">
                    <path fill="currentColor" d="m29.83 20l.34-2l-5.17-.85v-4.38l5.06-1.36l-.51-1.93l-4.83 1.29A9 9 0 0 0 20 5V2h-2v2.23a8.8 8.8 0 0 0-4 0V2h-2v3a9 9 0 0 0-4.71 5.82L2.46 9.48L2 11.41l5 1.36v4.38L1.84 18l.32 2L7 19.18a8.9 8.9 0 0 0 .82 3.57l-4.53 4.54l1.42 1.42l4.19-4.2a9 9 0 0 0 14.2 0l4.19 4.2l1.42-1.42l-4.54-4.54a8.9 8.9 0 0 0 .83-3.57ZM15 25.92A7 7 0 0 1 9 19v-6h6ZM9.29 11a7 7 0 0 1 13.42 0ZM23 19a7 7 0 0 1-6 6.92V13h6Z" />
                </svg>
            </button>
            <span v-if="page.props.auth?.username">Hello, {{ page.props.auth?.username }}</span>
        </nav>

        <div class="flex-1 flex p-3 gap-3 relative">
            <div
                class="flex flex-col px-2 py-3 bg-white dark:bg-gray-950 rounded-lg duration-300"
                style="width: 200px"
                :style="{
                    marginLeft: showMenu ? '0' : '-212px',
                    transition: 'margin-left 0.3s ease-in-out',
                }"
            >
                <Link class="px-4 hover:bg-gray-200 dark:hover:bg-gray-800 py-2 rounded transition-colors duration-300" href="/">Home</Link>
                <Link class="px-4 hover:bg-gray-200 dark:hover:bg-gray-800 py-2 rounded transition-colors duration-300" href="/users">Users</Link>
                <Link class="px-4 hover:bg-gray-200 dark:hover:bg-gray-800 py-2 rounded transition-colors duration-300" href="/about">About</Link>
            </div>
            <div class="flex-1 p-5 bg-white dark:bg-gray-950 rounded-lg">
                <slot />
            </div>

            <div v-if="showInertiaProps" class="absolute top-0 right-7 -mt-2">
                <div class="rounded-md border border-gray-300 dark:border-gray-600 p-3 bg-gray-200 dark:bg-gray-800">
                    <div class="font-bold text-xl mb-1">usePage()</div>
                    <pre class="bg-gray-100 dark:bg-gray-900 p-3 rounded leading-tight text-xs"
                         style="max-height: 60vh; overflow: auto;"
                    >{{ JSON.stringify(page, null, 2) }}</pre>
                </div>
            </div>
        </div>
    </div>
</template>

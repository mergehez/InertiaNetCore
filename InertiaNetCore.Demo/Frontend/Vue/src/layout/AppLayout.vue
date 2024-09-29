<script setup lang="ts">
import { Link, usePage } from '@inertiajs/vue3';
import { ref } from 'vue';
import ThemeSwitcher from '@/components/ThemeSwitcher.vue';

const showMenu = ref(true);

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
        </div>
    </div>
</template>

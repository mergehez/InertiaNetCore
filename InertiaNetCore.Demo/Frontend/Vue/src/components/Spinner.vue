<script setup lang="ts">

import { twMerge } from 'tailwind-merge';

withDefaults(defineProps<{
    loading?: boolean,
    visible?: boolean,
}>(), {
    loading: true,
    visible: true,
})
</script>

<template>
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"
         class="text-gray-400 dark:text-gray-600 fill-blue-600 dark:fill-blue-300"
         :class="[{
            'hidden': !visible,
            'animate-spin': loading,
         }, twMerge('size-6', $attrs.class as any)]">
        <path fill="currentColor" fill-rule="evenodd" d="M12 19a7 7 0 1 0 0-14a7 7 0 0 0 0 14m0 3c5.523 0 10-4.477 10-10S17.523 2 12 2S2 6.477 2 12s4.477 10 10 10" clip-rule="evenodd" opacity="0.2" />
        <path fill="currentFill" d="M2 12C2 6.477 6.477 2 12 2v3a7 7 0 0 0-7 7z" />
    </svg>
</template>
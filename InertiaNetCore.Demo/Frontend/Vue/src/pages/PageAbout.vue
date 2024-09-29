<script setup lang="ts">

import { usePage } from '@inertiajs/vue3';

const page = usePage<{
    name: string,
    version: string,
}>()
</script>

<template>
    <h1 class="font-bold text-2xl mb-4">About</h1>
    <p><b>{{ page.props.name }}</b> v{{ page.props.version }}</p>
</template>
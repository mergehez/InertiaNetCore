<script setup lang="ts">

import { usePage } from '@inertiajs/vue3';

const page = usePage<{
    users: string[],
}>()
</script>

<template>
    <h1 class="font-bold text-2xl mb-4">Users</h1>
    <p><b>Users:</b> {{ page.props.users.join(', ') }}</p>
</template>
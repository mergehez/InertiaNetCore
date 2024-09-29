<script setup lang="ts">

import { usePage } from '@inertiajs/vue3';
import { Link } from '@inertiajs/vue3';

const page = usePage<{
    names: string[],
    ages: number[]
}>()
</script>

<template>
    <h1 class="font-bold text-2xl mb-4">Index</h1>
    <p class="mb-2">This is the index page. Following values are passed from asp.net core controller via inertia:</p>
    <p><b>Names:</b> {{ page.props.names.join(', ') }}</p>
    <p><b>Ages:</b> {{ page.props.ages.join(', ') }}</p>

    <div class="mt-12">
        <Link href="/users" class="text-blue-700 hover:underline rounded py-1">-> Let's go to users page</Link>
    </div>
</template>
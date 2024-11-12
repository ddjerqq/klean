<svelte:head>
    <title>Home</title>
    <meta name="description" content="Svelte demo app"/>
</svelte:head>

<div class="flex flex-col gap-3 w-full max-w-sm">
    {#each Array(9) as _, i}
        <a href="/{i}" class="p-3 rounded-md">
            {i}
        </a>
    {/each}
</div>
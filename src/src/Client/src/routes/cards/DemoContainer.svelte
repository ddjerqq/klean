<script lang="ts">
  import type { Snippet } from 'svelte';

  let { children }: { children: Snippet } = $props();
</script>

<div class="flex items-center justify-center [&>div]:w-full">
  {@render children?.()}
</div>
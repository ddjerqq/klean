<script lang="ts">
  import {Sun, Moon} from 'lucide-svelte';
  import {toggleMode} from "mode-watcher";
  import Button from "$lib/components/ui/button/button.svelte";
</script>

<Button onclick={toggleMode} variant="outline" size="icon">
    <Sun class="size-[1.2rem] rotate-0 scale-100 transition-all dark:rotate-90 dark:scale-0"/>
    <Moon class="absolute size-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100"/>
    <span class="sr-only">Toggle theme</span>
</Button>
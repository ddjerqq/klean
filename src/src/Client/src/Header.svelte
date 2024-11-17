<script lang="ts">
  import { Input } from '$lib/components/ui/input/index';
  import DarkModeSwitch from '$lib/components/DarkModeSwitch.svelte';

  const links = [
    {name: 'Dashboard', path: 'dashboard'},
    {name: 'Cards', path: 'cards'},
    {name: 'Authentication', path: 'authentication'},
  ];

  const currentPath = $derived(window.location.pathname);
</script>

<header class="border-b px-8 mb-4">
  <div class="h-16 flex justify-between gap-4 items-center ">
    <nav class="flex items-center gap-4 lg:gap-6">
      {#each links as link (link.path)}
        <a href={link.path}
           class:text-muted-foreground={!currentPath.includes(link.path)}
           class:text-primary={currentPath.includes(link.path)}
           class="hover:text-primary text-sm font-medium transition-colors">
          {link.name}
        </a>
      {/each}
    </nav>

    <div class="flex items-center gap-4">
      <div>
        <Input type="search" placeholder="Search..." class="h-9 md:w-[100px] lg:w-[300px]" />
      </div>

      <DarkModeSwitch />
    </div>
  </div>
</header>
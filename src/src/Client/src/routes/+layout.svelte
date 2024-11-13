<script lang="ts">
  import * as Sidebar from "$lib/components/ui/sidebar/index.js";
  import {Toaster, toast} from "svelte-sonner";
  import {Button} from "$lib/components/ui/button/index.js";
  import AppSidebar from "$lib/components/AppSidebar.svelte";

  let {children} = $props();

  const showPromise = () => {
    const promise = new Promise((resolve, reject) => setTimeout(() => {
      if (Math.random() > 0.5) {
        resolve({ name: 'Svelte Sonner' });
      } else {
        reject();
      }
    }, 1500));

    toast.promise(promise, {
      loading: 'Loading...',
      success: (data) => {
        return data.name +  " toast has been added";
      },
      error: 'Error... :( Try again!',
    });
  }
</script>

<Toaster richColors />

<Button variant="outline" onclick={showPromise}>
    Show Toast
</Button>

<Sidebar.Provider>
    <AppSidebar/>
    <main>
        <Sidebar.Trigger/>
        {@render children?.()}
    </main>
</Sidebar.Provider>
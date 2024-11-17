<script context="module">
  import { Router, createRouter } from '@roxi/routify'
  import routes from '../.routify/routes.default.js'
  import { Toaster } from 'svelte-sonner'
  import { ModeWatcher } from 'mode-watcher'
  import Header from './Header.svelte'

  export const router = createRouter({ routes })
</script>

<ModeWatcher themeStorageKey="theme" />
<Toaster richColors />

<main class="h-full flex flex-col">
  <Header />
  <Router {router}/>

  <!--    <Footer/>-->
</main>
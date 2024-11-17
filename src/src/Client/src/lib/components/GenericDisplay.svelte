<script lang="ts" generics="T">
  import type {Snippet} from "svelte";

  type PropTypes = {
    data: T[];
    children: Snippet<[T]>;
  }

  let {data, children}: PropTypes = $props();
</script>

<ul>
    {#each data as data}
        <li>
            {@render children(data)}
        </li>
    {/each}
</ul>

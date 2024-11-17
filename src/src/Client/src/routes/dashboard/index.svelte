<script lang="ts">
  import {Download} from "lucide-svelte";
  import * as Card from "$lib/components/ui/card/index";
  import * as Tabs from "$lib/components/ui/tabs/index";
  import Button from "$lib/components/ui/button/button.svelte";
  import Overview from "./tabs/Overview.svelte";
</script>

<div class="h-full flex-1 space-y-4 p-8 pt-6">
    <div class="flex items-center justify-between space-y-2">
        <h2 class="text-3xl font-bold tracking-tight">Dashboard</h2>
        <div class="flex items-center space-x-2">
            <Button size="sm">
                <Download class="mr-2 h-4 w-4"/>
                Download
            </Button>
        </div>
    </div>

    <Tabs.Root value="overview" class="space-y-4">
        <Tabs.List>
            <Tabs.Trigger value="overview">Overview</Tabs.Trigger>
            <Tabs.Trigger value="analytics">Analytics</Tabs.Trigger>
            <Tabs.Trigger value="reports">Reports</Tabs.Trigger>
            <Tabs.Trigger value="notifications">Notifications</Tabs.Trigger>
        </Tabs.List>
        <Tabs.Content value="overview" class="space-y-4">
            <Overview/>
        </Tabs.Content>
        <Tabs.Content value="analytics" class="space-y-4">
            <Overview/>
        </Tabs.Content>
        <Tabs.Content value="reports" class="space-y-4">
            <Overview/>
        </Tabs.Content>
        <Tabs.Content value="notifications" class="space-y-4">
            <Overview/>
        </Tabs.Content>
    </Tabs.Root>
</div>

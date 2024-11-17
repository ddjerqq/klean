<script>
  import RecentSales from "./RecentSales.svelte";
  import BarChart from "$lib/components/charts/BarChart.svelte";
  import {DollarSign, CreditCard, Users, Activity} from "lucide-svelte";
  import Button from "$lib/components/ui/button/button.svelte";
  import * as Card from "$lib/components/ui/card/index";
  import * as Tabs from "$lib/components/ui/tabs/index";
</script>

<div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
    <Card.Root>
        <Card.Header class="flex flex-row items-center justify-between space-y-0 pb-2">
            <Card.Title class="text-sm font-medium">Total Revenue</Card.Title>
            <DollarSign class="text-muted-foreground h-4 w-4"/>
        </Card.Header>
        <Card.Content>
            <div class="text-2xl font-bold">$45,231.89</div>
            <p class="text-muted-foreground text-xs">+20.1% from last month</p>
        </Card.Content>
    </Card.Root>
    <Card.Root>
        <Card.Header class="flex flex-row items-center justify-between space-y-0 pb-2">
            <Card.Title class="text-sm font-medium">Subscriptions</Card.Title>
            <Users class="text-muted-foreground h-4 w-4"/>
        </Card.Header>
        <Card.Content>
            <div class="text-2xl font-bold">+2350</div>
            <p class="text-muted-foreground text-xs">+180.1% from last month</p>
        </Card.Content>
    </Card.Root>
    <Card.Root>
        <Card.Header class="flex flex-row items-center justify-between space-y-0 pb-2">
            <Card.Title class="text-sm font-medium">Sales</Card.Title>
            <CreditCard class="text-muted-foreground h-4 w-4"/>
        </Card.Header>
        <Card.Content>
            <div class="text-2xl font-bold">+12,234</div>
            <p class="text-muted-foreground text-xs">+19% from last month</p>
        </Card.Content>
    </Card.Root>
    <Card.Root>
        <Card.Header class="flex flex-row items-center justify-between space-y-0 pb-2">
            <Card.Title class="text-sm font-medium">Active Now</Card.Title>
            <Activity class="text-muted-foreground h-4 w-4"/>
        </Card.Header>
        <Card.Content>
            <div class="text-2xl font-bold">+573</div>
            <p class="text-muted-foreground text-xs">+201 since last hour</p>
        </Card.Content>
    </Card.Root>
</div>
<div class="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
    <Card.Root class="col-span-4">
        <Card.Header>
            <Card.Title>Overview</Card.Title>
        </Card.Header>
        <Card.Content>
            <BarChart/>
        </Card.Content>
    </Card.Root>
    <Card.Root class="col-span-4 lg:col-span-3">
        <Card.Header>
            <Card.Title>Recent Sales</Card.Title>
            <Card.Description>You made 265 sales this month.</Card.Description>
        </Card.Header>
        <Card.Content>
            <RecentSales/>
        </Card.Content>
    </Card.Root>
</div>
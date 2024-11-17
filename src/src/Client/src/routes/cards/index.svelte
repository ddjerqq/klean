<script lang="ts">
  import CookieSettings from "./CookieSettings.svelte";
  import CreateAccount from "./CreateAccount.svelte";
  import Github from "./Github.svelte";
  import Notifications from "./Notifications.svelte";
  import PaymentMethod from "./PaymentMethod.svelte";
  import ReportIssue from "./ReportIssue.svelte";
  import Share from "./Share.svelte";
  import TeamMembers from "./TeamMembers.svelte";
  import DemoContainer from "./DemoContainer.svelte";
</script>

<div class="hidden items-start justify-center gap-6 rounded-lg p-8 md:grid lg:grid-cols-2 xl:grid-cols-3">
    <div class="col-span-2 grid items-start gap-6 lg:col-span-1">
        <DemoContainer>
            <CreateAccount/>
        </DemoContainer>
        <DemoContainer>
            <PaymentMethod/>
        </DemoContainer>
    </div>
    <div class="col-span-2 grid items-start gap-6 lg:col-span-1">
        <DemoContainer>
            <TeamMembers/>
        </DemoContainer>
        <DemoContainer>
            <Share/>
        </DemoContainer>
        <DemoContainer>
            <Notifications/>
        </DemoContainer>
    </div>
    <div class="col-span-2 grid items-start gap-6 lg:col-span-2 lg:grid-cols-2 xl:col-span-1 xl:grid-cols-1">
        <DemoContainer>
            <ReportIssue/>
        </DemoContainer>
        <DemoContainer>
            <Github/>
        </DemoContainer>
        <DemoContainer>
            <CookieSettings/>
        </DemoContainer>
    </div>
</div>
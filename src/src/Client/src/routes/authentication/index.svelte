<script lang="ts">
  import { authStore } from '$lib/stores/authStore';
  import type { AuthState } from '$lib/types/AuthState';

  import Button from '$lib/components/ui/button/button.svelte';
  import { toast } from 'svelte-sonner';

  const payload = {
    email: 'g@nachkebia.dev',
    password: 'password',
  };

  const onclick = () => {
    toast.promise(authStore.login(payload), {
      loading: 'Logging in...',
      error: 'Bad credentials, please try again!',
      success: (data: AuthState | null) => `Welcome back, ${data!.user!.full_name}!`,
    });
  };
</script>

<div class="container h-full flex-col items-center justify-center">
  <Button variant="outline" {onclick}>
    Login
  </Button>
</div>

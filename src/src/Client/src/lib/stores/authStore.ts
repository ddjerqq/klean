import { writable, get } from 'svelte/store';
import type { AuthState } from '$lib/types/AuthState';
import { Api } from '$lib/apiClient/Api';
import type { LoginCommand, RegisterCommand, User } from '$lib/apiClient/data-contracts';

function createAuthStore() {
  const { subscribe, set } = writable<AuthState>({
    isAuthenticated: false,
    user: null,
    token: null,
  });

  const api = new Api();

  return {
    subscribe,
    login: async (loginCommand: LoginCommand): Promise<AuthState> => {
      const response = await api.v1AuthLoginCreate(loginCommand);

      await new Promise((resolve) => setTimeout(resolve, 1000));

      if (response.ok) {
        const authState = {
          isAuthenticated: true,
          user: response.data,
          token: response.headers.get('authorization')!,
        } satisfies AuthState;

        set(authState);
      } else {
        console.error(response.error);
      }

      return get(authStore);
    },
  };
}

export const authStore = createAuthStore();

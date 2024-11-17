import type { User } from '$lib/apiClient/data-contracts';

export type AuthState =
  | {
      isAuthenticated: true;
      user: User;
      token: string;
    }
  | {
      isAuthenticated: false;
      user: null;
      token: null;
    };

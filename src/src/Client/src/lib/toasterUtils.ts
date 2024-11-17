import {toast} from "svelte-sonner";

export const showPromise = () => {
  const promise = new Promise<string>((resolve, reject) => setTimeout(() => {
    if (Math.random() > 0.5) {
      resolve('Svelte Sonner');
    } else {
      reject();
    }
  }, 1500));

  toast.promise(promise, {
    loading: 'Loading...',
    success: (data: string) => {
      return data + " toast has been added";
    },
    error: 'Error... :( Try again!',
  });
}
import { writable } from "svelte/store";

const count = writable(0, () => {
  console.log("got us a subscriber");
  return () => console.log("unsubscribe called");
});

count.set(1);

const unsubscribe = count.subscribe((value) => {
  console.log("count is", value);
});

unsubscribe();

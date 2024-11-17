import { readable } from "svelte/store";

const time = readable(new Date(), set => {
  set(new Date());

  const interval = setInterval(() => {
    set(new Date());
  }, 1000);

  return () => clearInterval(interval);
});

const ticktock = readable("tick", (set, update) => {
  const interval = setInterval(() => {
    update(sound => sound === "tick" ? "tock" : "tick");
  })

  return () => clearInterval(interval);
})
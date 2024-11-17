import { mount } from "svelte";
import App from "./App.svelte";

export const app = mount(App, { target: document.getElementById("app") as HTMLElement });

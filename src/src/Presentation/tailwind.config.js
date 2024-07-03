const defaultTheme = require('tailwindcss/defaultTheme');
const colors = require('tailwindcss/colors');

/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './**/*.{razor,js,css}',
    './wwwroot/index.html'
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: colors.emerald,
        gray: colors.neutral,
      },
      fontFamily: {
        sans: ['Inter var', ...defaultTheme.fontFamily.sans],
      },
      transitionTimingFunction: {
        "sweet": "cubic-bezier(0.34, 1.56, 0.64, 1)",
        "jumpy": "cubic-bezier(0.68, -0.6, 0.32, 1.6)",
      },
    },
  }
};
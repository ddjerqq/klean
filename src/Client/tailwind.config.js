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
        primary: colors.emerald
      },
      fontFamily: {
        sans: ['Inter var', ...defaultTheme.fontFamily.sans],
      },
    },
  },
  plugins: [],
};
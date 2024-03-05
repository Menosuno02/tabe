/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ['./**/*.razor', './**/*.cshtml'],
    theme: {
        extend: {},
    },
    darkMode: 'media',
    plugins: [require("daisyui")],
}

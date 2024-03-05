/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ['./**/*.razor', './**/*.cshtml'],
    theme: {
        extend: {},
    },
    darkMode: 'selector',
    plugins: [require("daisyui")],
}

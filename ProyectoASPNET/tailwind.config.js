/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './**/*.razor',
        './**/*.cshtml',
    ],
    theme: {
        extend: {},
        colors: {
            'tabepurple': "#4C38FF"
        }
    },
    darkMode: 'media',
    plugins: [
        require("daisyui"),
    ],
}

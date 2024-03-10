/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
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
        require('flowbite/plugin')
    ],
}

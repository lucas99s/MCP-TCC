import type { Config } from "tailwindcss";

const config: Config = {
  darkMode: ["class"],
  content: [
    "./app/**/*.{ts,tsx}",
    "./components/**/*.{ts,tsx}",
    "./lib/**/*.{ts,tsx}",
    "./types/**/*.{ts,tsx}"
  ],
  theme: {
    extend: {
      colors: {
        border: "hsl(214 32% 91%)",
        background: "hsl(210 20% 98%)",
        foreground: "hsl(220 32% 15%)",
        card: "hsl(0 0% 100%)"
      },
      boxShadow: {
        panel: "0 18px 48px -24px rgba(15, 23, 42, 0.28)"
      }
    }
  },
  plugins: []
};

export default config;

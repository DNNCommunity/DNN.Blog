/** @type {import('tailwindcss').Config} */
module.exports = {
  /*
   * Use declaration !important so utilities apply on portal roots that carry
   * both `blog-he-portal` and utility classes (selector-scoping cannot match self).
   * Class names are HtmlEditor-only; they won't collide with DNN skin rules.
   */
  important: true,
  corePlugins: {
    preflight: false,
  },
  content: ["./Client/Js/HtmlEditor/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        border: "hsl(var(--he-border))",
        input: "hsl(var(--he-input))",
        ring: "hsl(var(--he-ring))",
        background: "hsl(var(--he-background))",
        foreground: "hsl(var(--he-foreground))",
        primary: {
          DEFAULT: "hsl(var(--he-primary))",
          foreground: "hsl(var(--he-primary-foreground))",
        },
        secondary: {
          DEFAULT: "hsl(var(--he-secondary))",
          foreground: "hsl(var(--he-secondary-foreground))",
        },
        muted: {
          DEFAULT: "hsl(var(--he-muted))",
          foreground: "hsl(var(--he-muted-foreground))",
        },
        accent: {
          DEFAULT: "hsl(var(--he-accent))",
          foreground: "hsl(var(--he-accent-foreground))",
        },
        destructive: {
          DEFAULT: "hsl(var(--he-destructive))",
          foreground: "hsl(var(--he-destructive-foreground))",
        },
        popover: {
          DEFAULT: "hsl(var(--he-popover))",
          foreground: "hsl(var(--he-popover-foreground))",
        },
        brand: "hsl(var(--he-brand))",
      },
      borderRadius: {
        lg: "var(--he-radius)",
        md: "calc(var(--he-radius) - 2px)",
        sm: "calc(var(--he-radius) - 4px)",
      },
      fontFamily: {
        heading: [
          "ui-sans-serif",
          "-apple-system",
          "BlinkMacSystemFont",
          "Segoe UI",
          "Helvetica",
          "Arial",
          "sans-serif",
        ],
      },
    },
  },
  plugins: [
    require("tailwindcss-animate"),
    require("tailwind-scrollbar-hide"),
  ],
};

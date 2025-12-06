import { dirname } from "path";
import { fileURLToPath } from "url";
import { FlatCompat } from "@eslint/eslintrc";
import { fixupConfigRules } from "@eslint/compat";
import simpleImportSort from "eslint-plugin-simple-import-sort";
import unusedImports from "eslint-plugin-unused-imports";
import tseslint from "typescript-eslint";
import nextPlugin from "@next/eslint-plugin-next";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const compat = new FlatCompat({
  baseDirectory: __dirname,
});

/** @type {import('eslint').Linter.Config[]} */
const eslintConfig = [
  {
    ignores: [
      ".next/**",
      "node_modules/**",
      "dist/**",
      "storybook-static/**",
      "coverage/**",
      "**/*.json",
    ],
  },

  {
    settings: {
      react: {
        version: "detect",
      },
    },
  },

  ...tseslint.configs.recommended,

  ...fixupConfigRules(compat.extends("plugin:react/recommended")),
  ...fixupConfigRules(compat.extends("plugin:react-hooks/recommended")),

  // Storybook
  ...compat.extends("plugin:storybook/recommended"),

  // カスタム
  {
    files: ["**/*.{js,jsx,ts,tsx}"],
    plugins: {
      "simple-import-sort": simpleImportSort,
      "unused-imports": unusedImports,
      "@next/next": nextPlugin,
    },
    languageOptions: {
    },
    rules: {
      ...nextPlugin.configs.recommended.rules,
      ...nextPlugin.configs["core-web-vitals"].rules,

      "no-console": ["warn", { allow: ["warn", "error"] }],
      
      "react/react-in-jsx-scope": "off",

      "@typescript-eslint/no-unused-vars": [
        "error",
        {
          argsIgnorePattern: "^_",
          varsIgnorePattern: "^_",
          caughtErrorsIgnorePattern: "^_",
        },
      ],

      "@typescript-eslint/consistent-type-imports": [
        "warn",
        {
          prefer: "type-imports",
          fixStyle: "inline-type-imports",
        },
      ],

      "simple-import-sort/imports": "error",
      "simple-import-sort/exports": "error",

      "unused-imports/no-unused-imports": "error",
    },
  },

  ...compat.extends("prettier"),
];

export default eslintConfig;
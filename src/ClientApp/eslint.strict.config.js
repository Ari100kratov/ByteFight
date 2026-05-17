import eslintConfigPrettier from "eslint-config-prettier"
import reactHooks from "eslint-plugin-react-hooks"
import tseslint from "typescript-eslint"

import baseConfig from "./eslint.config.js"

function asWarnings(configs) {
  return configs.map((config) => ({
    ...config,
    rules: Object.fromEntries(
      Object.entries(config.rules ?? {}).map(([ruleName, ruleValue]) => [
        ruleName,
        toWarning(ruleValue),
      ]),
    ),
  }))
}

function toWarning(ruleValue) {
  if (ruleValue === "off" || ruleValue === 0) {
    return ruleValue
  }

  if (Array.isArray(ruleValue)) {
    return ["warn", ...ruleValue.slice(1)]
  }

  return "warn"
}

export default [
  ...baseConfig,
  ...asWarnings(tseslint.configs.strictTypeChecked),
  ...asWarnings(tseslint.configs.stylisticTypeChecked),
  {
    files: ["**/*.{ts,tsx}"],
    rules: {
      ...Object.fromEntries(
        Object.entries(reactHooks.configs.recommended.rules).map(([ruleName, ruleValue]) => [
          ruleName,
          toWarning(ruleValue),
        ]),
      ),
    },
  },
  eslintConfigPrettier,
]

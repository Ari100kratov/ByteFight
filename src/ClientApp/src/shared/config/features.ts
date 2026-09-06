/**
 * Флаги возможностей клиента.
 *
 * codeMode — скрытый режим программирования поведения героя.
 * По умолчанию выключен: игра управляется вручную.
 * Включается переменной окружения VITE_FEATURE_CODE_MODE=true.
 */
export const FEATURE_FLAGS = {
  codeMode: import.meta.env.VITE_FEATURE_CODE_MODE === 'true',
} as const

/**
 * 共通スタイル定義
 * Tailwind CSSクラスを再利用可能な形で定義
 */

export const buttonStyles = {
    primary: 'px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed',
    secondary: 'px-4 py-2 bg-gray-600 text-white rounded hover:bg-gray-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed',
    danger: 'px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed',
    ghost: 'px-4 py-2 text-gray-700 hover:bg-gray-100 rounded transition-colors',
};

export const cardStyles = {
    base: 'bg-white rounded-lg shadow-md p-4',
    hoverable: 'bg-white rounded-lg shadow-md p-4 hover:shadow-lg transition-shadow cursor-pointer',
    bordered: 'bg-white rounded-lg border-2 border-gray-200 p-4',
};

export const inputStyles = {
    base: 'px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent',
    error: 'px-3 py-2 border border-red-300 rounded focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent',
};

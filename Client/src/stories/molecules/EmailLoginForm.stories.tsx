import type { Meta, StoryObj } from '@storybook/nextjs-vite';

import EmailLoginForm from '@/components/molecules/EmailLoginForm';

const meta = {
    title: 'Molecules/EmailLoginForm',
    component: EmailLoginForm,
    parameters: {
        layout: 'centered',
    },
    tags: ['autodocs'],
} satisfies Meta<typeof EmailLoginForm>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
    args: {
        onLogin: async (email, password) => {
            console.log('Login:', email, password);
            await new Promise(resolve => setTimeout(resolve, 1000));
        },
        onSignUp: async (email, password) => {
            console.log('Sign up:', email, password);
            await new Promise(resolve => setTimeout(resolve, 1000));
        },
        onCancel: () => console.log('Cancel clicked'),
    },
};

export const WithError: Story = {
    args: {
        onLogin: async (email, password) => {
            console.log('Login:', email, password);
            await new Promise(resolve => setTimeout(resolve, 1000));
        },
        onSignUp: async (email, password) => {
            console.log('Sign up:', email, password);
            await new Promise(resolve => setTimeout(resolve, 1000));
        },
        onCancel: () => console.log('Cancel clicked'),
        error: 'ログインに失敗しました',
    },
};

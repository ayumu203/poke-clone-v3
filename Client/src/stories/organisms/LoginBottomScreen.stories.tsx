import type { Meta, StoryObj } from '@storybook/react';
import LoginBottomScreen from '@/components/organisms/LoginBottomScreen';

const meta = {
    title: 'Organisms/LoginBottomScreen',
    component: LoginBottomScreen,
    parameters: {
        layout: 'centered',
    },
    tags: ['autodocs'],
} satisfies Meta<typeof LoginBottomScreen>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
    args: {
        onGoogleLogin: async () => {
            console.log('Google login');
            await new Promise(resolve => setTimeout(resolve, 1000));
        },
        onEmailLogin: async (email, password) => {
            console.log('Email login:', email, password);
            await new Promise(resolve => setTimeout(resolve, 1000));
        },
    },
};

export const WithError: Story = {
    args: {
        onGoogleLogin: async () => {
            console.log('Google login');
            await new Promise(resolve => setTimeout(resolve, 1000));
        },
        onEmailLogin: async (email, password) => {
            console.log('Email login:', email, password);
            await new Promise(resolve => setTimeout(resolve, 1000));
        },
        error: 'Googleログインに失敗しました',
    },
};

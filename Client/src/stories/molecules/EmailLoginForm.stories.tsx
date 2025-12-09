import type { Meta, StoryObj } from '@storybook/react';
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
        onSubmit: async (email, password) => {
            console.log('Login:', email, password);
            await new Promise(resolve => setTimeout(resolve, 1000));
        },
        onCancel: () => console.log('Cancel clicked'),
    },
};

export const WithError: Story = {
    args: {
        onSubmit: async (email, password) => {
            console.log('Login:', email, password);
            await new Promise(resolve => setTimeout(resolve, 1000));
        },
        onCancel: () => console.log('Cancel clicked'),
        error: 'ログインに失敗しました',
    },
};

import type { Meta, StoryObj } from '@storybook/react';
import Button from '@/components/atoms/Button';

const meta = {
    title: 'Atoms/Button',
    component: Button,
    parameters: {
        layout: 'centered',
    },
    tags: ['autodocs'],
    argTypes: {
        variant: {
            control: 'select',
            options: ['primary', 'secondary', 'google'],
        },
        fullWidth: { control: 'boolean' },
        disabled: { control: 'boolean' },
    },
} satisfies Meta<typeof Button>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Primary: Story = {
    args: {
        children: 'ログイン',
        variant: 'primary',
    },
};

export const Secondary: Story = {
    args: {
        children: 'キャンセル',
        variant: 'secondary',
    },
};

export const Google: Story = {
    args: {
        children: 'Googleでログイン',
        variant: 'google',
    },
};

export const FullWidth: Story = {
    args: {
        children: 'メールでログイン',
        variant: 'primary',
        fullWidth: true,
    },
};

export const Disabled: Story = {
    args: {
        children: 'ログイン中...',
        variant: 'primary',
        disabled: true,
    },
};

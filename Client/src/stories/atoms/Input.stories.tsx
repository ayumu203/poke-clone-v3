import type { Meta, StoryObj } from '@storybook/nextjs-vite';
import { useState } from 'react';

import Input from '@/components/atoms/Input';

const meta = {
    title: 'Atoms/Input',
    component: Input,
    parameters: {
        layout: 'centered',
    },
    tags: ['autodocs'],
    argTypes: {
        type: {
            control: 'select',
            options: ['text', 'email', 'password'],
        },
    },
} satisfies Meta<typeof Input>;

export default meta;
type Story = StoryObj<typeof meta>;

const InputWithState = ({ type, placeholder }: { type?: 'text' | 'email' | 'password'; placeholder?: string }) => {
    const [value, setValue] = useState('');
    return <Input type={type} placeholder={placeholder} value={value} onChange={setValue} />;
};

export const Text: Story = {
    render: () => <InputWithState type="text" placeholder="テキストを入力" />,
    parameters: { docs: { source: { type: 'code' } } },
    args: {} as never,
};

export const Email: Story = {
    render: () => <InputWithState type="email" placeholder="メールアドレス" />,
    parameters: { docs: { source: { type: 'code' } } },
    args: {} as never,
};

export const Password: Story = {
    render: () => <InputWithState type="password" placeholder="パスワード" />,
    parameters: { docs: { source: { type: 'code' } } },
    args: {} as never,
};

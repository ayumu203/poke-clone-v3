import type { Meta, StoryObj } from '@storybook/nextjs-vite';
import { useState } from 'react';

import ToggleSwitch from '@/components/atoms/ToggleSwitch';

const meta = {
    title: 'Atoms/ToggleSwitch',
    component: ToggleSwitch,
    parameters: {
        layout: 'centered',
    },
    tags: ['autodocs'],
    argTypes: {
        enabled: { control: 'boolean' },
        label: { control: 'text' },
    },
} satisfies Meta<typeof ToggleSwitch>;

export default meta;
type Story = StoryObj<typeof meta>;

// 制御されたコンポーネントのラッパー
const ToggleSwitchWithState = ({ enabled: initialEnabled, label }: { enabled: boolean; label?: string }) => {
    const [enabled, setEnabled] = useState(initialEnabled);
    return <ToggleSwitch enabled={enabled} onToggle={() => setEnabled(!enabled)} label={label} />;
};

export const Default: Story = {
    render: () => <ToggleSwitchWithState enabled={false} />,
    parameters: { docs: { source: { type: 'code' } } },
    args: {} as never,
};

export const Enabled: Story = {
    render: () => <ToggleSwitchWithState enabled={true} />,
    parameters: { docs: { source: { type: 'code' } } },
    args: {} as never,
};

export const WithLabel: Story = {
    render: () => <ToggleSwitchWithState enabled={false} label="BGM" />,
    parameters: { docs: { source: { type: 'code' } } },
    args: {} as never,
};

export const EnabledWithLabel: Story = {
    render: () => <ToggleSwitchWithState enabled={true} label="サウンド" />,
    parameters: { docs: { source: { type: 'code' } } },
    args: {} as never,
};

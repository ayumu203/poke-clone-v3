import type { Meta, StoryObj } from '@storybook/react';
import TopScreen from '@/components/organisms/TopScreen';

const meta = {
    title: 'Organisms/TopScreen',
    component: TopScreen,
    parameters: {
        layout: 'centered',
    },
    tags: ['autodocs'],
    argTypes: {
        backgroundImage: { control: 'text' },
    },
} satisfies Meta<typeof TopScreen>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
    args: {
        backgroundImage: '/Images/art_003.png',
    },
};

export const Art001: Story = {
    args: {
        backgroundImage: '/Images/art_001.png',
    },
};

export const Art002: Story = {
    args: {
        backgroundImage: '/Images/art_002.png',
    },
};

export const WithChildren: Story = {
    args: {
        backgroundImage: '/Images/art_003.png',
        children: (
            <div className="flex items-center justify-center h-full">
                <div className="bg-white bg-opacity-80 p-4 rounded">
                    <p className="text-black font-bold">ポケモン情報がここに表示されます</p>
                </div>
            </div>
        ),
    },
};

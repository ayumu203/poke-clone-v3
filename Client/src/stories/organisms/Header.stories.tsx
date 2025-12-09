import type { Meta, StoryObj } from '@storybook/react';
import Header from '@/components/organisms/Header';

const meta = {
    title: 'Organisms/Header',
    component: Header,
    parameters: {
        layout: 'fullwidth',
    },
    tags: ['autodocs'],
} satisfies Meta<typeof Header>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {};

export const WithBGMEnabled: Story = {
    play: async ({ canvasElement }) => {
        // ストーリーブック上でBGMトグルの状態を確認できるようにする
        const button = canvasElement.querySelector('button');
        if (button) {
            button.click();
        }
    },
};

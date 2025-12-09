import type { Meta, StoryObj } from '@storybook/react';
import BottomScreen from '@/components/organisms/BottomScreen';

const meta = {
    title: 'Organisms/BottomScreen',
    component: BottomScreen,
    parameters: {
        layout: 'centered',
    },
    tags: ['autodocs'],
} satisfies Meta<typeof BottomScreen>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {};

export const WithMenu: Story = {
    args: {
        children: (
            <div className="p-4">
                <h2 className="text-xl font-bold mb-4">バトルメニュー</h2>
                <div className="grid grid-cols-2 gap-2">
                    <button className="bg-blue-500 text-white p-2 rounded">たたかう</button>
                    <button className="bg-green-500 text-white p-2 rounded">ポケモン</button>
                    <button className="bg-yellow-500 text-white p-2 rounded">どうぐ</button>
                    <button className="bg-red-500 text-white p-2 rounded">にげる</button>
                </div>
            </div>
        ),
    },
};

export const WithText: Story = {
    args: {
        children: (
            <div className="p-4 flex items-center justify-center h-full">
                <p className="text-center">
                    メニューやバトルコマンドがここに表示されます
                </p>
            </div>
        ),
    },
};

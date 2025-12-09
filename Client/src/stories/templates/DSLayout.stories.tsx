import type { Meta, StoryObj } from '@storybook/nextjs-vite';

import BottomScreen from '@/components/organisms/BottomScreen';
import Header from '@/components/organisms/Header';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';

const meta = {
    title: 'Templates/DSLayout',
    component: DSLayout,
    parameters: {
        layout: 'fullwidth',
    },
    tags: ['autodocs'],
} satisfies Meta<typeof DSLayout>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
    args: {
        children: (
            <>
                <Header />
                <TopScreen backgroundImage="/Images/art_003.png" />
                <BottomScreen />
            </>
        ),
    },
};

export const WithDifferentBackground: Story = {
    args: {
        children: (
            <>
                <Header />
                <TopScreen backgroundImage="/Images/art_001.png" />
                <BottomScreen />
            </>
        ),
    },
};

export const WithBattleMenu: Story = {
    args: {
        children: (
            <>
                <Header />
                <TopScreen backgroundImage="/Images/art_003.png">
                    <div className="flex items-center justify-center h-full">
                        <div className="bg-white bg-opacity-80 p-4 rounded">
                            <p className="text-black font-bold">ピカチュウ</p>
                            <p className="text-black">HP: 95/95</p>
                        </div>
                    </div>
                </TopScreen>
                <BottomScreen>
                    <div className="p-4">
                        <h2 className="text-xl font-bold mb-4">バトルメニュー</h2>
                        <div className="grid grid-cols-2 gap-2">
                            <button className="bg-blue-500 text-white p-2 rounded">たたかう</button>
                            <button className="bg-green-500 text-white p-2 rounded">ポケモン</button>
                            <button className="bg-yellow-500 text-white p-2 rounded">どうぐ</button>
                            <button className="bg-red-500 text-white p-2 rounded">にげる</button>
                        </div>
                    </div>
                </BottomScreen>
            </>
        ),
    },
};

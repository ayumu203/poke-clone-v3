import type { Meta, StoryObj } from '@storybook/react';
import ExternalLink from '@/components/atoms/ExternalLink';

const meta = {
    title: 'Atoms/ExternalLink',
    component: ExternalLink,
    parameters: {
        layout: 'centered',
    },
    tags: ['autodocs'],
    argTypes: {
        href: { control: 'text' },
        children: { control: 'text' },
    },
} satisfies Meta<typeof ExternalLink>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
    args: {
        href: 'https://pokeapi.co/',
        children: 'PokeAPI',
    },
};

export const GitHub: Story = {
    args: {
        href: 'https://github.com/ayumu203/poke-clone-v3/',
        children: 'Github',
    },
};

export const Japanese: Story = {
    args: {
        href: 'https://booth.pm/ja/items/5677827',
        children: '背景画像',
    },
};

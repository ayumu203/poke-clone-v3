import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';

export async function POST(request: Request) {
    try {
        const { token } = await request.json();

        if (!token) {
            return NextResponse.json({ error: 'Token is required' }, { status: 400 });
        }

        // Set HTTPOnly cookie with security attributes
        const cookieStore = await cookies();
        cookieStore.set('session', token, {
            httpOnly: true,
            secure: process.env.NODE_ENV === 'production',
            sameSite: 'strict',
            maxAge: 60 * 60, // 1 hour
            path: '/',
        });

        return NextResponse.json({ success: true });
    } catch (error) {
        console.error('Failed to create session:', error);
        return NextResponse.json({ error: 'Failed to create session' }, { status: 500 });
    }
}

import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';

export async function GET() {
    try {
        const cookieStore = await cookies();
        const session = cookieStore.get('session');

        if (!session) {
            return NextResponse.json({ authenticated: false });
        }

        // TODO: Optionally verify token with Firebase Admin SDK
        // For now, just check if session exists
        return NextResponse.json({
            authenticated: true,
            // Don't send the actual token in response for security
        });
    } catch (error) {
        console.error('Failed to verify session:', error);
        return NextResponse.json({ error: 'Failed to verify session' }, { status: 500 });
    }
}

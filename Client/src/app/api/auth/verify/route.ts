import { cookies } from 'next/headers';
import { NextResponse } from 'next/server';

import { verifyIdToken } from '@/lib/firebase/admin';

export async function GET() {
    try {
        // Cookieからセッショントークンを取得
        const cookieStore = await cookies();
        const sessionToken = cookieStore.get('session')?.value;

        if (!sessionToken) {
            return NextResponse.json({ valid: false, error: 'No session token' }, { status: 401 });
        }

        // Firebase Admin SDKでトークンを検証
        const decodedToken = await verifyIdToken(sessionToken);

        return NextResponse.json({
            valid: true,
            uid: decodedToken.uid,
            email: decodedToken.email,
        });
    } catch (error) {
        console.error('Failed to verify session:', error);
        return NextResponse.json({ error: 'Invalid token' }, { status: 401 });
    }
}

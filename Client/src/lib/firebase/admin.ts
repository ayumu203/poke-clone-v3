import admin from 'firebase-admin';

// Firebase Admin SDKの初期化
if (!admin.apps.length) {
    admin.initializeApp({
        credential: admin.credential.cert({
            projectId: process.env.FIREBASE_PROJECT_ID,
            clientEmail: process.env.FIREBASE_CLIENT_EMAIL,
            privateKey: process.env.FIREBASE_PRIVATE_KEY?.replace(/\\n/g, '\n'),
        }),
    });
}

/**
 * Firebase IDトークンを検証する
 * @param token - 検証するIDトークン
 * @returns デコードされたトークン情報
 * @throws トークンが無効の場合、エラーをスロー
 */
export const verifyIdToken = async (token: string) => {
    try {
        const decodedToken = await admin.auth().verifyIdToken(token);
        return decodedToken;
    } catch (error) {
        console.error('Token verification failed:', error);
        throw new Error('Invalid token');
    }
};

/**
 * Firebase Admin Auth インスタンスを取得
 */
export const getAuth = () => admin.auth();

import { initializeApp } from 'firebase/app';
import { getAuth } from 'firebase/auth';

const firebaseConfig = {
    apiKey: process.env.NEXT_PUBLIC_FIREBASEAPI_KEY!,
    authDomain: process.env.NEXT_PUBLIC_FIREBASEAUTH_DOMAIN!,
    projectId: process.env.NEXT_PUBLIC_FIREBASEPROJECT_ID!,
    storageBucket: process.env.NEXT_PUBLIC_FIREBASESTORAGE_BUCKET!,
    messagingSenderId: process.env.NEXT_PUBLIC_FIREBASEMESSAGING_SENDER_ID!,
    appId: process.env.NEXT_PUBLIC_FIREBASEAPP_ID!,
};

// Initialize Firebase
export const app = initializeApp(firebaseConfig);
export const auth = getAuth(app);
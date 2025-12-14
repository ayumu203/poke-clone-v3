"use client";

import { FirebaseError } from "firebase/app";
import { useRouter } from "next/navigation";
import { useState } from "react";

import BottomScreen from "@/components/organisms/BottomScreen";
import LoginBottomScreen from "@/components/organisms/LoginBottomScreen";
import TopScreen from "@/components/organisms/TopScreen";
import DSLayout from "@/components/templates/DSLayout";
import { useAuth } from "@/hooks/useAuth";
import { useCommonTexts,usePageConfig } from "@/hooks/usePageConfig";

export default function LoginPage() {
  const { loginWithEmail, signUpWithEmail, loginWithGoogle, isAuthenticated, loading } = useAuth();
  const router = useRouter();
  const pageConfig = usePageConfig("login");
  const commonTexts = useCommonTexts();
  const [error, setError] = useState("");

  const { t } = pageConfig;

  const getErrorMessage = (error: unknown): string => {
    if (error instanceof FirebaseError) {
      switch (error.code) {
        case "auth/invalid-credential":
          return t.errors.invalidCredential;
        case "auth/email-already-in-use":
          return t.errors.emailInUse;
        case "auth/weak-password":
          return t.errors.weakPassword;
        case "auth/invalid-email":
          return t.errors.invalidEmail;
        default:
          return `エラーが発生しました: ${error.code}`;
      }
    }
    return t.errors.unexpected;
  };

  const handleGoogleLogin = async () => {
    setError("");
    try {
      await loginWithGoogle();
      router.push("/");
    } catch (err) {
      setError(getErrorMessage(err));
      console.error(err);
    }
  };

  const handleEmailLogin = async (email: string, password: string) => {
    setError("");
    try {
      await loginWithEmail(email, password);
      router.push("/");
    } catch (err) {
      setError(getErrorMessage(err));
      console.error(err);
    }
  };

  const handleEmailSignUp = async (email: string, password: string) => {
    setError("");
    try {
      await signUpWithEmail(email, password);
      router.push("/");
    } catch (err) {
      setError(getErrorMessage(err));
      console.error(err);
    }
  };

  // ローディング中
  if (loading) {
    return (
      <DSLayout bgmSrc={pageConfig.bgm}>
        <TopScreen backgroundImage={pageConfig.image} />
        <BottomScreen>
          <div className="flex h-full items-center justify-center bg-white">
            <p className="text-xl font-bold">{commonTexts.loading}</p>
          </div>
        </BottomScreen>
      </DSLayout>
    );
  }

  if (isAuthenticated) {
    return (
      <DSLayout bgmSrc={pageConfig.bgm}>
        <TopScreen backgroundImage={pageConfig.image} />
        <BottomScreen>
          <div className="flex h-full items-center justify-center">
            <p className="text-xl font-bold text-gray-800">{t.alreadyLoggedIn}</p>
          </div>
        </BottomScreen>
      </DSLayout>
    );
  }

  return (
    <DSLayout bgmSrc={pageConfig.bgm}>
      <TopScreen backgroundImage={pageConfig.image} />
      <LoginBottomScreen
        onGoogleLogin={handleGoogleLogin}
        onEmailLogin={handleEmailLogin}
        onEmailSignUp={handleEmailSignUp}
        error={error}
      />
    </DSLayout>
  );
}

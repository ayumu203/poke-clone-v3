'use client';

import { useCallback, useEffect, useRef, useState } from 'react';

interface UseAudioOptions {
    src: string;
    loop?: boolean;
    volume?: number;
    autoPlay?: boolean;
}

interface UseAudioReturn {
    isPlaying: boolean;
    play: () => Promise<void>;
    pause: () => Promise<void>;
    stop: () => void;
    setVolume: (volume: number) => void;
    currentTime: number;
    duration: number;
}

/**
 * BGM管理用のカスタムフック
 * - BGMの再生・一時停止・停止
 * - ボリューム調整
 * - ループ再生
 */
export function useAudio({
    src,
    loop = true,
    volume = 0.5,
    autoPlay = false,
}: UseAudioOptions): UseAudioReturn {
    const audioRef = useRef<HTMLAudioElement | null>(null);
    const playPromiseRef = useRef<Promise<void> | null>(null);
    const [isPlaying, setIsPlaying] = useState(false);
    const [currentTime, setCurrentTime] = useState(0);
    const [duration, setDuration] = useState(0);

    // Audio要素の初期化
    useEffect(() => {
        const audio = new Audio(src);
        audio.loop = loop;
        audio.volume = volume;
        audioRef.current = audio;

        // イベントリスナーの設定
        const handleTimeUpdate = () => setCurrentTime(audio.currentTime);
        const handleDurationChange = () => setDuration(audio.duration);
        const handleEnded = () => setIsPlaying(false);
        const handlePlay = () => setIsPlaying(true);
        const handlePause = () => setIsPlaying(false);

        audio.addEventListener('timeupdate', handleTimeUpdate);
        audio.addEventListener('durationchange', handleDurationChange);
        audio.addEventListener('ended', handleEnded);
        audio.addEventListener('play', handlePlay);
        audio.addEventListener('pause', handlePause);

        // autoPlayの処理
        if (autoPlay) {
            const promise = audio.play();
            if (promise !== undefined) {
                playPromiseRef.current = promise;
                promise.catch((error) => {
                    console.warn('Autoplay prevented:', error);
                });
            }
        }

        // クリーンアップ
        return () => {
            audio.removeEventListener('timeupdate', handleTimeUpdate);
            audio.removeEventListener('durationchange', handleDurationChange);
            audio.removeEventListener('ended', handleEnded);
            audio.removeEventListener('play', handlePlay);
            audio.removeEventListener('pause', handlePause);

            // 再生中の音声を停止
            if (playPromiseRef.current) {
                playPromiseRef.current.then(() => {
                    audio.pause();
                    audio.currentTime = 0;
                }).catch(() => {
                    // エラーは無視
                });
            } else {
                audio.pause();
                audio.currentTime = 0;
            }
        };
    }, [src, loop, volume, autoPlay]);

    // 再生
    const play = useCallback(async () => {
        if (audioRef.current) {
            try {
                // 既存の再生プロミスを待つ
                if (playPromiseRef.current) {
                    await playPromiseRef.current;
                }
                playPromiseRef.current = audioRef.current.play();
                await playPromiseRef.current;
                playPromiseRef.current = null;
            } catch (_error) {
                playPromiseRef.current = null;
                throw _error;
            }
        }
    }, []);

    // 一時停止
    const pause = useCallback(async () => {
        if (audioRef.current) {
            try {
                // 再生プロミスを待ってから一時停止
                if (playPromiseRef.current) {
                    await playPromiseRef.current;
                }
                audioRef.current.pause();
            } catch (_error) {
                // エラーは無視（既に停止している可能性がある）
            } finally {
                playPromiseRef.current = null;
            }
        }
    }, []);

    // 停止（一時停止 + 先頭に戻る）
    const stop = useCallback(() => {
        if (audioRef.current) {
            audioRef.current.pause();
            audioRef.current.currentTime = 0;
        }
    }, []);

    // ボリューム設定
    const setBgmVolume = useCallback((newVolume: number) => {
        if (audioRef.current) {
            audioRef.current.volume = Math.max(0, Math.min(1, newVolume));
        }
    }, []);

    return {
        isPlaying,
        play,
        pause,
        stop,
        setVolume: setBgmVolume,
        currentTime,
        duration,
    };
}

# Poke-Clone-V3

## 概要

このリポジトリはWebプラットフォームにおけるオンラインゲーム開発の練習の一環として作成しました. \
今回の最終目標はCPUとの対戦だけでなく, オンライン対戦を行うことができるようにしていく予定です. 

<img width="2159" height="1126" alt="サンプル画面" src="https://github.com/user-attachments/assets/e850496e-d3cc-4226-a4fa-fdd49f910046" />
※ 画像はpoke-clone で開発したものです.

## 使用技術

- **使用言語**

[![開発言語](https://skillicons.dev/icons?i=ts,cs)](https://skillicons.dev)

- **フレームワーク**

[![フレームワーク](https://skillicons.dev/icons?i=react,nextjs,dotnet&theme=light)](https://skillicons.dev)

- **本番環境**

[![My Skills](https://skillicons.dev/icons?i=azure,vercel&theme=light)](https://skillicons.dev)

## 導入方法

```bash
# リポジトリのクローン
git clone https://github.com/ayumu203/poke-clone-v3.git
cd poke-clone-v3
# 以降は以下のドキュメントを参考に実行
```

- [起動方法](https://github.com/ayumu203/poke-clone-v3/blob/main/docs/%E8%B5%B7%E5%8B%95%E6%96%B9%E6%B3%95%E7%AD%89.md)
- [API使用方法](https://github.com/ayumu203/poke-clone-v3/blob/feature/api-endpoints/docs/API%E6%A4%9C%E8%A8%BC.md)

## クラス図(バックエンド)

<img width="1656" height="1123" alt="バックエンドクラス図" src="https://github.com/user-attachments/assets/be29eda6-6a46-42ef-90a7-b0426e4e828c" />

## 進捗

### バックエンド

- [x] .NETを利用したAPIサーバの作成.
- [x] EF Coreによるモデルの作成.
- [x] ポケモン・技データ保存バッチ処理の実装.
- [x] APIエンドポイントの作成.
- [x] 簡易JWT認証の作成.
- [ ] 対戦ロジックの実装
- [ ] SignalRによるリアルタイム対戦の実装.

### フロントエンド

- [ ] 未定（まだ考えてない...）

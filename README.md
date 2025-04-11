# PunchShooting

## プロジェクトの説明

個人で開発中のゲームです。

## ゲームのジャンル

縦スクロール2Dシューティング

## プラットフォーム

Unity2022.3.32

## 使用ライブラリ

UniTask

R3

VContainer

Addressable

InputSystem

ImtStateMachine

## 操作方法

・移動　左スティック or WASD
・攻撃方向　右スティック or IJKL

## 実装済み項目

自機の操作、敵の生成、ダメージ処理、バトル仮ステート

## バトルシーンソース

Assets/PunchShooting/Scripts/Battle/Scenes/BattleScene.cs

## こだわりポイント

UnityのプロジェクトではMonoBehaviourに何でも書きがちですが、本プロジェクトではなるべくMonoBehaviourで書かなくて良いロジックとコントローラ部をピュアC#で実装するように心がけました。


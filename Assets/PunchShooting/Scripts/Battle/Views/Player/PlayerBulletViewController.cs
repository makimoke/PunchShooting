using System;
using PunchShooting.Battle.Definitions;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PunchShooting.Battle.Views.Player
{
    //プレイヤ弾管理
    public class PlayerBulletViewController : IDisposable
    {
        private readonly PlayerBulletView _playerBulletView;
        public readonly Subject<SpriteCollisionResult> OnCollidedSubject = new(); //敵と衝突した

        private DisposableBag _disposableBag;
        private SpriteBlinkViewController _spriteBlinkViewController;

        public PlayerBulletViewController(PlayerBulletView playerBulletView)
        {
            _playerBulletView = playerBulletView;
        }

        public bool IsOffScreen { private set; get; } //画面外か？
        public long InstanceId => _playerBulletView.InstanceId;

        public void Dispose()
        {
            _disposableBag.Dispose();
        }

        public void Initialize()
        {
            _playerBulletView.OnTriggerEnterSubject
                .Subscribe(collider =>
                {
                    //敵と衝突時
                    if (collider.CompareTag("Enemy"))
                    {
                        var spriteView = collider.gameObject.GetComponent<BaseSpriteView>();
                        //Debug.Log($"Bullet:{instanceId} {collisionResult.InstanceId} {spriteView.InstanceId}");
                        OnCollidedSubject.OnNext(new SpriteCollisionResult(_playerBulletView.InstanceId, spriteView.InstanceId));
                    }
                })
                .AddTo(ref _disposableBag);

            _spriteBlinkViewController = new SpriteBlinkViewController(_playerBulletView.SpriteRenderer);
        }

        //Viewを破棄
        public void Destroy()
        {
            Object.Destroy(_playerBulletView.gameObject);
        }

        //毎フレーム呼ばれる
        public void Update(float deltaTime)
        {
            //移動
            _playerBulletView.Move(deltaTime);

            //画面外判定（TODO:関数化）
            if (_playerBulletView.Position.y >= SpriteViewDefinition.OffscreenTop)
            {
                IsOffScreen = true;
            }
        }

        public void Blink(Color blinkColor, float blinkSecond)
        {
            _spriteBlinkViewController.Blink(blinkColor, blinkSecond);
        }
    }
}
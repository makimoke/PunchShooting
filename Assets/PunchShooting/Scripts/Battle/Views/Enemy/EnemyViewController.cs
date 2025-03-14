using System;
using R3;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace PunchShooting.Battle.Views.Enemy
{
    //敵管理
    public class EnemyViewController : IDisposable
    {
        private readonly EnemyView _enemyView;
        public readonly Subject<SpriteCollisionResult> OnCollidedSubject = new(); //プレイヤと衝突

        private DisposableBag _disposableBag;
        private SpriteBlinkViewController _spriteBlinkViewController;


        [Inject]
        public EnemyViewController(EnemyView enemyView)
        {
            _enemyView = enemyView;
        }

        public bool IsOffScreen { private set; get; } //画面外か？
        public long InstanceId => _enemyView.InstanceId;

        public void Dispose()
        {
            _disposableBag.Dispose();
        }

        public void Initialize()
        {
            _enemyView.OnTriggerEnterSubject
                .Subscribe(collider =>
                {
                    //プレイヤと衝突時
                    if (collider.CompareTag("Player"))
                    {
                        var spriteView = collider.gameObject.GetComponent<BaseSpriteView>();
                        //Debug.Log($"CreateEnemy:{instanceId} {collisionResult.InstanceId} {spriteView.InstanceId}");
                        OnCollidedSubject.OnNext(new SpriteCollisionResult(_enemyView.InstanceId, spriteView.InstanceId));
                    }
                })
                .AddTo(ref _disposableBag);

            _spriteBlinkViewController = new SpriteBlinkViewController(_enemyView.SpriteRenderer);
        }

        //敵を破棄
        public void Destroy()
        {
            Object.Destroy(_enemyView.gameObject);
        }

        //毎フレーム呼ばれる
        public void Update(float deltaTime)
        {
            //移動（仮）
            _enemyView.AddPosition(new Vector3(0.0f, -2.0f * deltaTime, 0.0f));

            //画面外判定
            if (_enemyView.Position.y <= -5.40f)
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
using System;
using System.Collections.Generic;
using PunchShooting.Battle.Definitions.Player;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using Object = UnityEngine.Object;

namespace PunchShooting.Battle.Views.Player
{
    //自機
    public class PlayerShipViewController : IDisposable
    {
        //定数
        private const float InputMoveScale = 0.02f;

        //private const float ShotTimeInterval = 1.0f; //弾発射間隔
        private readonly List<PlayerBulletView> _bulletViews = new();
        private readonly PlayerBulletViewCreator _playerBulletViewCreator;
        private readonly PlayerInput _playerInput;
        private readonly PlayerResourceProvider _playerResourceProvider;
        public readonly Subject<SpriteCollisionResult> OnCollidedBulletSubject = new(); //弾と敵が衝突した

        public readonly Subject<long> OnDestroyedBulletSubject = new();

        //private float _bulletCounter = ShotTimeInterval;
        private DisposableBag _disposableBag;
        private PlayerShipView _playerShipView;


        [Inject]
        public PlayerShipViewController(PlayerResourceProvider playerResourceProvider,
            PlayerBulletViewCreator playerBulletViewCreator,
            PlayerInput playerInput)
        {
            _playerResourceProvider = playerResourceProvider;
            _playerBulletViewCreator = playerBulletViewCreator;
            _playerInput = playerInput;
        }

        public void Dispose()
        {
            _disposableBag.Dispose();
        }

        public void Initialize()
        {
            _playerShipView = _playerResourceProvider.InstantiatePlayerShip();
        }

        //毎フレーム呼ばれる
        public void Update(float deltaTime)
        {
            //入力で移動
            var inputMoveAxis = _playerInput.actions["Move"].ReadValue<Vector2>();
            if (inputMoveAxis != Vector2.zero)
            {
                _playerShipView.AddPosition(inputMoveAxis * InputMoveScale);
            }

/*            //時間で弾発射
            _bulletCounter -= deltaTime;
            if (_bulletCounter <= 0.0f)
            {
                _bulletCounter = ShotTimeInterval;
                var bulletView = _playerBulletViewCreator.CreateBullet(PlayerResourceDefinition.PrefabId.Bul, PlayerResourceDefinition.SpriteId.Bul001,
                    new Vector3(-0.6f, 0.38f, 0.0f) + _playerShipView.Position);
                _bulletViews.Add(bulletView);
                bulletView.OnTriggerEnterSubject
                    .Subscribe(collisionResult => { Debug.Log("Enemy On:" + collisionResult.Collider.tag); })
                    .AddTo(ref _disposableBag);
            }*/

            UpdateBullets(deltaTime);
        }

        public void UpdateBullets(float deltaTime)
        {
            //弾移動
            foreach (var bulletView in _bulletViews)
            {
                bulletView.AddPosition(new Vector3(0.0f, 4.0f * deltaTime, 0.0f));
            }

            //画面外削除
            for (var i = _bulletViews.Count - 1; i >= 0; i--)
            {
                var bulletView = _bulletViews[i];
                if (bulletView.Position.y >= 5.40f)
                {
                    DestroyBullet(bulletView);
                }
            }
        }

        public void CreateBullet(long instanceId)
        {
            var bulletView = _playerBulletViewCreator.CreateBullet(instanceId, PlayerResourceDefinition.PrefabId.Bul, PlayerResourceDefinition.SpriteId.Bul001,
                new Vector3(-0.6f, 0.38f, 0.0f) + _playerShipView.Position);
            _bulletViews.Add(bulletView);
            bulletView.OnTriggerEnterSubject
                .Subscribe(collider =>
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        var spriteView = collider.gameObject.GetComponent<BaseSpriteView>();
                        //Debug.Log($"CreateBullet:{instanceId} {collisionResult.InstanceId} {spriteView.InstanceId}");
                        OnCollidedBulletSubject.OnNext(new SpriteCollisionResult(instanceId, spriteView.InstanceId));
                    }
                })
                .AddTo(ref _disposableBag);
        }

        public void DestroyBullet(PlayerBulletView bulletView)
        {
            OnDestroyedBulletSubject.OnNext(bulletView.InstanceId);
            _bulletViews.Remove(bulletView);
            Object.Destroy(bulletView.gameObject);
        }

        public void DestroyAllBullets()
        {
            foreach (var bullet in _bulletViews)
            {
                Object.Destroy(bullet.gameObject);
            }

            _bulletViews.Clear();
        }
    }
}
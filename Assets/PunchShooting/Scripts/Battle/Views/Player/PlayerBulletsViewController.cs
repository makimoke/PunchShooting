using System;
using System.Collections.Generic;
using PunchShooting.Battle.Definitions;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace PunchShooting.Battle.Views.Player
{
    //プレイヤ弾管理
    public class PlayerBulletsViewController : IDisposable
    {
        private readonly List<PlayerBulletViewController> _bulletViewControllers = new();
        private readonly PlayerBulletViewCreator _playerBulletViewCreator;
        private readonly PlayerInput _playerInput;
        public readonly Subject<SpriteCollisionResult> OnCollidedBulletSubject = new(); //弾と敵が衝突した

        public readonly Subject<long> OnDestroyedBulletSubject = new();

        private DisposableBag _disposableBag;


        [Inject]
        public PlayerBulletsViewController(PlayerBulletViewCreator playerBulletViewCreator,
            PlayerInput playerInput)
        {
            _playerBulletViewCreator = playerBulletViewCreator;
            _playerInput = playerInput;
        }

        public void Dispose()
        {
            _disposableBag.Dispose();
        }

        public void Initialize()
        {
        }

        //毎フレーム呼ばれる
        public void Update(float deltaTime)
        {
            //弾移動
            foreach (var bulletViewController in _bulletViewControllers)
            {
                bulletViewController.Update(deltaTime);
            }

            //画面外削除（逆回し）
            for (var i = _bulletViewControllers.Count - 1; i >= 0; i--)
            {
                var bulletViewController = _bulletViewControllers[i];
                if (bulletViewController.IsOffScreen)
                {
                    OnDestroyedBulletSubject.OnNext(bulletViewController.InstanceId);
                    bulletViewController.Destroy();
                    _bulletViewControllers.Remove(bulletViewController);
                }
            }
        }

        public void CreateBullet(long instanceId, SpriteResourceDefinition.PrefabId prefabId, SpriteResourceDefinition.SpriteId spriteId, Vector3 position)
        {
            var bulletViewController = _playerBulletViewCreator.CreateBullet(instanceId, prefabId, spriteId, position);
            _bulletViewControllers.Add(bulletViewController);

            //敵との衝突
            bulletViewController.OnCollidedSubject
                .Subscribe(collisionResult => OnCollidedBulletSubject.OnNext(collisionResult))
                .AddTo(ref _disposableBag);
        }

        public void DestroyBullet(long instanceId)
        {
            var bulletViewController = _bulletViewControllers.Find(item => item.InstanceId == instanceId);
            if (bulletViewController != null)
            {
                bulletViewController.Destroy();
                _bulletViewControllers.Remove(bulletViewController);
            }
        }

        public void DestroyAllBullets()
        {
            foreach (var bullet in _bulletViewControllers)
            {
                bullet.Destroy();
            }

            _bulletViewControllers.Clear();
        }

        //ダメージを受けた
        public void ReceivedDamage(long instanceId, int value)
        {
            var bulletViewController = _bulletViewControllers.Find(bullet => bullet.InstanceId == instanceId);
            if (bulletViewController != null)
            {
                bulletViewController.Blink(Color.red, 0.1f);
            }
        }
    }
}
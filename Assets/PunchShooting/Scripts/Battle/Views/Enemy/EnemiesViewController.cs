using System;
using System.Collections.Generic;
using PunchShooting.Battle.Definitions;
using R3;
using UnityEngine;
using VContainer;

namespace PunchShooting.Battle.Views.Enemy
{
    //敵全体管理
    public class EnemiesViewController : IDisposable
    {
        //定数

        private readonly List<EnemyViewController> _enemyViewControllers = new();

        private readonly EnemyViewCreator _enemyViewCreator;
        public readonly Subject<SpriteCollisionResult> OnCollidedEnemySubject = new(); //弾と敵が衝突した

        public readonly Subject<long> OnDestroyedEnemySubject = new();

        private DisposableBag _disposableBag;


        [Inject]
        public EnemiesViewController(EnemyViewCreator enemyViewCreator)
        {
            _enemyViewCreator = enemyViewCreator;
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
            UpdateEnemies(deltaTime);
        }

        private void UpdateEnemies(float deltaTime)
        {
            //移動
            foreach (var enemyViewController in _enemyViewControllers)
            {
                enemyViewController.Update(deltaTime);
            }

            //画面外削除（逆回し）
            for (var i = _enemyViewControllers.Count - 1; i >= 0; i--)
            {
                var enemyViewController = _enemyViewControllers[i];
                if (enemyViewController.IsOffScreen)
                {
                    OnDestroyedEnemySubject.OnNext(enemyViewController.InstanceId);
                    enemyViewController.DestroyEnemy();
                    _enemyViewControllers.Remove(enemyViewController);
                }
            }
        }

        //ダメージを受けた
        public void ReceivedDamage(long instanceId, int value)
        {
            var enemyViewController = _enemyViewControllers.Find(enemy => enemy.InstanceId == instanceId);
            if (enemyViewController != null)
            {
                enemyViewController.Blink(Color.red, 0.1f);
            }
        }

        public void CreateEnemy(long instanceId, SpriteResourceDefinition.PrefabId prefabId, SpriteResourceDefinition.SpriteId spriteId, Vector3 position)
        {
            var enemyViewController = _enemyViewCreator.CreateEnemy(instanceId, prefabId, spriteId, position);

            _enemyViewControllers.Add(enemyViewController);
            //プレイヤとの衝突
            enemyViewController.OnCollidedEnemySubject
                .Subscribe(collisionResult => OnCollidedEnemySubject.OnNext(collisionResult))
                .AddTo(ref _disposableBag);
        }

        public void DestroyEnemy(long instanceId)
        {
            var enemyViewController = _enemyViewControllers.Find(item => item.InstanceId == instanceId);
            if (enemyViewController != null)
            {
                enemyViewController.DestroyEnemy();
                _enemyViewControllers.Remove(enemyViewController);
            }
        }

        public void DestroyAllEnemis()
        {
            foreach (var enemyViewController in _enemyViewControllers)
            {
                enemyViewController.DestroyEnemy();
            }

            _enemyViewControllers.Clear();
        }
    }
}
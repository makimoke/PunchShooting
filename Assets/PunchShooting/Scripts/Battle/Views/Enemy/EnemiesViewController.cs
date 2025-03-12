using System;
using System.Collections.Generic;
using PunchShooting.Battle.Definitions;
using R3;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace PunchShooting.Battle.Views.Enemy
{
    //敵全体管理
    public class EnemiesViewController : IDisposable
    {
        private readonly EnemyResourceProvider _enemyResourceProvider;

        private readonly EnemyViewCreator _enemyViewCreator;
        //定数

        private readonly List<EnemyView> _enemyViews = new();
        public readonly Subject<SpriteCollisionResult> OnCollidedEnemySubject = new(); //弾と敵が衝突した

        public readonly Subject<long> OnDestroyedEnemySubject = new();

        private DisposableBag _disposableBag;


        [Inject]
        public EnemiesViewController(EnemyResourceProvider enemyResourceProvider,
            EnemyViewCreator enemyViewCreator)
        {
            _enemyResourceProvider = enemyResourceProvider;
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
            foreach (var enemyView in _enemyViews)
            {
                enemyView.AddPosition(new Vector3(0.0f, -2.0f * deltaTime, 0.0f));
            }

            //画面外削除
            for (var i = _enemyViews.Count - 1; i >= 0; i--)
            {
                var enemyView = _enemyViews[i];
                if (enemyView.Position.y <= -5.40f)
                {
                    DestroyEnemy(enemyView);
                }
            }
        }

        public void CreateEnemy(long instanceId, SpriteResourceDefinition.PrefabId prefabId, SpriteResourceDefinition.SpriteId spriteId, Vector3 position)
        {
            var enemyView = _enemyViewCreator.CreateEnemy(instanceId, prefabId, spriteId, position);
            _enemyViews.Add(enemyView);
            enemyView.OnTriggerEnterSubject
                .Subscribe(collider =>
                {
                    //プレイヤと衝突時
                    if (collider.CompareTag("Player"))
                    {
                        var spriteView = collider.gameObject.GetComponent<BaseSpriteView>();
                        //Debug.Log($"CreateEnemy:{instanceId} {collisionResult.InstanceId} {spriteView.InstanceId}");
                        OnCollidedEnemySubject.OnNext(new SpriteCollisionResult(instanceId, spriteView.InstanceId));
                    }
                })
                .AddTo(ref _disposableBag);
        }

        public void DestroyEnemy(EnemyView enemyView)
        {
            OnDestroyedEnemySubject.OnNext(enemyView.InstanceId);
            _enemyViews.Remove(enemyView);
            Object.Destroy(enemyView.gameObject);
        }

        public void DestroyAllEnemis()
        {
            foreach (var enemy in _enemyViews)
            {
                Object.Destroy(enemy.gameObject);
            }

            _enemyViews.Clear();
        }
    }
}
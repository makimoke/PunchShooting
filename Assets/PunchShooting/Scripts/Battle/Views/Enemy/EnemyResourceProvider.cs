using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PunchShooting.Battle.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using Object = UnityEngine.Object;

namespace PunchShooting.Battle.Views.Enemy
{
    //敵リソース管理
    public class EnemyResourceProvider : IDisposable
    {
        private readonly BattleFieldView _battleFieldView;
        private readonly Dictionary<SpriteResourceDefinition.PrefabId, GameObject> _prefabDictionary = new();
        private readonly Dictionary<SpriteResourceDefinition.SpriteId, Sprite> _spriteDictionary = new();

        [Inject]
        public EnemyResourceProvider(BattleFieldView battleFieldView)
        {
            _battleFieldView = battleFieldView;
        }

        public void Dispose()
        {
            foreach (var prefab in _prefabDictionary.Values)
            {
                Addressables.Release(prefab);
            }

            foreach (var sprites in _spriteDictionary.Values)
            {
                Addressables.Release(sprites);
            }
        }

        public async UniTask LoadAsync()
        {
            //プレハブ
            _prefabDictionary[SpriteResourceDefinition.PrefabId.Enemy001] = await Addressables.LoadAssetAsync<GameObject>("Assets/PunchShooting/Prefabs/Battle/Enemy/Enemy001.prefab").Task;

            //スプライト
            _spriteDictionary[SpriteResourceDefinition.SpriteId.Enemy001] = await Addressables.LoadAssetAsync<Sprite>("Assets/PunchShooting/Sprites/Battle/Enemy/spr_enemy_001.png").Task;
        }

        public EnemyView InstantiateEnemy(SpriteResourceDefinition.PrefabId prefabId)
        {
            var enemyShipObj = Object.Instantiate(_prefabDictionary[prefabId], _battleFieldView.Transform);
            return enemyShipObj.GetComponent<EnemyView>();
        }

        public void DestroyEnemyShip(EnemyView enemyShipView)
        {
            Object.Destroy(enemyShipView.gameObject);
        }

        public GameObject FindPrefab(SpriteResourceDefinition.PrefabId prefabId)
        {
            return _prefabDictionary[prefabId];
        }

        public Sprite FindSprite(SpriteResourceDefinition.SpriteId spriteId)
        {
            return _spriteDictionary[spriteId];
        }
    }
}
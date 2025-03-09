using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PunchShooting.Battle.Definitions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using Object = UnityEngine.Object;

namespace PunchShooting.Battle.Views.Player
{
    //プレイヤリソース管理
    public class PlayerResourceProvider : IDisposable
    {
        private readonly BattleFieldView _battleFieldView;
        private readonly Dictionary<SpriteResourceDefinition.PrefabId, GameObject> _prefabDictionary = new();
        private readonly Dictionary<SpriteResourceDefinition.SpriteId, Sprite> _spriteDictionary = new();

        [Inject]
        public PlayerResourceProvider(BattleFieldView battleFieldView)
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
            _prefabDictionary[SpriteResourceDefinition.PrefabId.PShip] = await Addressables.LoadAssetAsync<GameObject>("Assets/PunchShooting/Prefabs/Battle/Player/PlayerShip.prefab").Task;
            _prefabDictionary[SpriteResourceDefinition.PrefabId.PBul] = await Addressables.LoadAssetAsync<GameObject>("Assets/PunchShooting/Prefabs/Battle/Player/PlayerBullet.prefab").Task;

            //スプライト
            _spriteDictionary[SpriteResourceDefinition.SpriteId.PBul001] = await Addressables.LoadAssetAsync<Sprite>("Assets/PunchShooting/Sprites/Battle/Player/spr_pbul_001.png").Task;
            _spriteDictionary[SpriteResourceDefinition.SpriteId.PBul002] = await Addressables.LoadAssetAsync<Sprite>("Assets/PunchShooting/Sprites/Battle/Player/spr_pbul_002.png").Task;
        }

        public PlayerShipView InstantiatePlayerShip(SpriteResourceDefinition.PrefabId prefabId)
        {
            var playerShipObj = Object.Instantiate(_prefabDictionary[prefabId], _battleFieldView.Transform);
            var playerShipView = playerShipObj.GetComponent<PlayerShipView>();
            playerShipView.AdjustColliderSize();
            return playerShipView;
        }

        public void DestroyPlayerShip(PlayerShipView playerShipView)
        {
            Object.Destroy(playerShipView.gameObject);
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
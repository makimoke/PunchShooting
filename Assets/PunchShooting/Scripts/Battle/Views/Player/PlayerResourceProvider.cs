using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PunchShooting.Battle.Definitions.Player;
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
        private readonly Dictionary<PlayerResourceDefinition.PrefabId, GameObject> _prefabDictionary = new();
        private readonly Dictionary<PlayerResourceDefinition.SpriteId, Sprite> _spriteDictionary = new();

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
            _prefabDictionary[PlayerResourceDefinition.PrefabId.PShip] = await Addressables.LoadAssetAsync<GameObject>("Assets/PunchShooting/Prefabs/PlayerShip.prefab").Task;
            _prefabDictionary[PlayerResourceDefinition.PrefabId.PBul] = await Addressables.LoadAssetAsync<GameObject>("Assets/PunchShooting/Prefabs/PlayerBullet.prefab").Task;

            //スプライト
            _spriteDictionary[PlayerResourceDefinition.SpriteId.PBul001] = await Addressables.LoadAssetAsync<Sprite>("Assets/PunchShooting/Sprites/spr_pbul_001.png").Task;
            _spriteDictionary[PlayerResourceDefinition.SpriteId.PBul002] = await Addressables.LoadAssetAsync<Sprite>("Assets/PunchShooting/Sprites/spr_pbul_002.png").Task;
        }

        public PlayerShipView InstantiatePlayerShip()
        {
            var playerShipObj = Object.Instantiate(_prefabDictionary[PlayerResourceDefinition.PrefabId.PShip], _battleFieldView.Transform);
            return playerShipObj.GetComponent<PlayerShipView>();
        }

        public void DestroyPlayerShip(PlayerShipView playerShipView)
        {
            Object.Destroy(playerShipView.gameObject);
        }

        public GameObject FindPrefab(PlayerResourceDefinition.PrefabId prefabId)
        {
            return _prefabDictionary[prefabId];
        }

        public Sprite FindSprite(PlayerResourceDefinition.SpriteId spriteId)
        {
            return _spriteDictionary[spriteId];
        }
    }
}
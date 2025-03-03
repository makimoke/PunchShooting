using System;
using Cysharp.Threading.Tasks;
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
        private GameObject _playerShipPrefab;

        [Inject]
        public PlayerResourceProvider(BattleFieldView battleFieldView)
        {
            _battleFieldView = battleFieldView;
        }

        public void Dispose()
        {
            Addressables.Release(_playerShipPrefab);
        }

        public async UniTask LoadAsync()
        {
            //プレハブ
            _playerShipPrefab = await Addressables.LoadAssetAsync<GameObject>("Assets/PunchShooting/Prefabs/PlayerShip.prefab").Task;
        }

        public PlayerShipView InstantiatePlayerShip()
        {
            var playerShipObj = Object.Instantiate(_playerShipPrefab, _battleFieldView.Transform);
            return playerShipObj.GetComponent<PlayerShipView>();
        }

        public void DestroyPlayerShip(PlayerShipView playerShipView)
        {
            Object.Destroy(playerShipView.gameObject);
        }
    }
}
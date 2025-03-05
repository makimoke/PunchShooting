using System;
using System.Collections.Generic;
using PunchShooting.Battle.Definitions.Player;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace PunchShooting.Battle.Views.Player
{
    public class PlayerBulletCreator : IDisposable
    {
        private readonly BattleFieldView _battleFieldView;
        private readonly List<GameObject> _bulletObjects = new();
        private readonly PlayerResourceProvider _playerResourceProvider;
        private GameObject _bulletPrefab;

        [Inject]
        public PlayerBulletCreator(BattleFieldView battleFieldView,
            PlayerResourceProvider playerResourceProvider)
        {
            _battleFieldView = battleFieldView;
            _playerResourceProvider = playerResourceProvider;
        }

        public void Dispose()
        {
            DestroyAllBullets();
        }

        public void Reset()
        {
            DestroyAllBullets();
        }

        public GameObject CreateBullet(PlayerResourceDefinition.PrefabId prefabId, PlayerResourceDefinition.SpriteId spriteId, Vector3 position)
        {
            if (!_bulletPrefab)
            {
                //１回目は取得する
                _bulletPrefab = _playerResourceProvider.FindPrefab(prefabId);
            }

            var bulletObject = Object.Instantiate(_bulletPrefab, _battleFieldView.Transform);
            bulletObject.transform.localPosition = position;
            _bulletObjects.Add(bulletObject);
            var playerBulletView = bulletObject.GetComponent<PlayerBulletView>();
            playerBulletView.SetSprite(_playerResourceProvider.FindSprite(spriteId));

            return bulletObject;
        }

        public void DestroyBullet(GameObject bulletObject)
        {
            _bulletObjects.Remove(bulletObject);
            Object.Destroy(bulletObject);
        }

        public void DestroyAllBullets()
        {
            foreach (var bullet in _bulletObjects)
            {
                Object.Destroy(bullet);
            }

            _bulletObjects.Clear();
        }
    }
}
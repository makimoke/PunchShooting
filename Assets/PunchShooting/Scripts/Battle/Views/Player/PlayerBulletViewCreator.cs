using PunchShooting.Battle.Definitions.Player;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace PunchShooting.Battle.Views.Player
{
    public class PlayerBulletViewCreator
    {
        private readonly BattleFieldView _battleFieldView;
        private readonly PlayerResourceProvider _playerResourceProvider;
        private GameObject _bulletPrefab;

        [Inject]
        public PlayerBulletViewCreator(BattleFieldView battleFieldView,
            PlayerResourceProvider playerResourceProvider)
        {
            _battleFieldView = battleFieldView;
            _playerResourceProvider = playerResourceProvider;
        }

        public PlayerBulletView CreateBullet(PlayerResourceDefinition.PrefabId prefabId, PlayerResourceDefinition.SpriteId spriteId, Vector3 position)
        {
            if (!_bulletPrefab)
            {
                //１回目は取得する
                _bulletPrefab = _playerResourceProvider.FindPrefab(prefabId);
            }

            var bulletObject = Object.Instantiate(_bulletPrefab, _battleFieldView.Transform);
            bulletObject.transform.localPosition = position;
            var playerBulletView = bulletObject.GetComponent<PlayerBulletView>();
            playerBulletView.SetSprite(_playerResourceProvider.FindSprite(spriteId));

            return playerBulletView;
        }
    }
}
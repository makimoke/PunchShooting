using PunchShooting.Battle.Definitions;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace PunchShooting.Battle.Views.Player
{
    public class PlayerBulletViewCreator
    {
        private readonly BattleFieldView _battleFieldView;
        private readonly PlayerResourceProvider _playerResourceProvider;

        [Inject]
        public PlayerBulletViewCreator(BattleFieldView battleFieldView,
            PlayerResourceProvider playerResourceProvider)
        {
            _battleFieldView = battleFieldView;
            _playerResourceProvider = playerResourceProvider;
        }

        public PlayerBulletViewController CreateBullet(long instanceId, SpriteResourceDefinition.PrefabId prefabId, SpriteResourceDefinition.SpriteId spriteId, Vector2 position, Vector2 velocity)
        {
            var bulletPrefab = _playerResourceProvider.FindPrefab(prefabId);
            var bulletObject = Object.Instantiate(bulletPrefab, _battleFieldView.Transform);
            bulletObject.transform.localPosition = position;
            var playerBulletView = bulletObject.GetComponent<PlayerBulletView>();
            playerBulletView.SetSprite(_playerResourceProvider.FindSprite(spriteId));
            playerBulletView.AdjustColliderSize();
            playerBulletView.InstanceId = instanceId;
            playerBulletView.Velocity = velocity;

            var angle = Vector2.Angle(velocity, Vector2.up);
            if (velocity.x > 0)
            {
                angle *= -1;
            }

            playerBulletView.SetAngleZ(angle);

            var playerBulletViewController = new PlayerBulletViewController(playerBulletView);
            playerBulletViewController.Initialize();

            return playerBulletViewController;
        }
    }
}
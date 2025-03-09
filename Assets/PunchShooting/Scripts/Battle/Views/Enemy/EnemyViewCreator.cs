using PunchShooting.Battle.Definitions;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace PunchShooting.Battle.Views.Enemy
{
    public class EnemyViewCreator
    {
        private readonly BattleFieldView _battleFieldView;
        private readonly EnemyResourceProvider _enemyResourceProvider;

        [Inject]
        public EnemyViewCreator(BattleFieldView battleFieldView,
            EnemyResourceProvider enemyResourceProvider)
        {
            _battleFieldView = battleFieldView;
            _enemyResourceProvider = enemyResourceProvider;
        }

        public EnemyView CreateEnemy(long instanceId, SpriteResourceDefinition.PrefabId prefabId, SpriteResourceDefinition.SpriteId spriteId, Vector3 position)
        {
            var bulletPrefab = _enemyResourceProvider.FindPrefab(prefabId);
            var bulletObject = Object.Instantiate(bulletPrefab, _battleFieldView.Transform);
            bulletObject.transform.localPosition = position;
            var enemyView = bulletObject.GetComponent<EnemyView>();
            enemyView.SetSprite(_enemyResourceProvider.FindSprite(spriteId));
            enemyView.InstanceId = instanceId;
            return enemyView;
        }
    }
}
using PunchShooting.Battle.Data;
using PunchShooting.Battle.Data.Enemy;
using VContainer;

namespace PunchShooting.Battle.Logic.Enemy
{
    public class EnemyStatusLogic
    {
        private readonly InstanceIdGenerator _instanceIdGenerator;
        private readonly EnemyStatusDataAccessor _enemyStatusDataAccessor;

        [Inject]
        public EnemyStatusLogic(EnemyStatusDataAccessor enemyStatusDataAccessor,
            InstanceIdGenerator instanceIdGenerator)
        {
            _enemyStatusDataAccessor = enemyStatusDataAccessor;
            _instanceIdGenerator = instanceIdGenerator;
        }

        public ObjectStatus CreateEnemy(ObjectBaseParam baseParam)
        {
            var id = _instanceIdGenerator.GenerateId();

            var status = new ObjectStatus(id, baseParam);
            _enemyStatusDataAccessor.AddStatus(status);

            return status;
        }

        public void RemoveEnemy(long id)
        {
            _enemyStatusDataAccessor.RemoveStatus(id);
        }

        public void RemoveAllEnemis()
        {
            _enemyStatusDataAccessor.RemoveAllStatuses();
        }
    }
}
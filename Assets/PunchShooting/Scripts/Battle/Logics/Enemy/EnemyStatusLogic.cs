using System.Linq;
using PunchShooting.Battle.Data;
using PunchShooting.Battle.Data.Enemy;
using R3;
using VContainer;

namespace PunchShooting.Battle.Logics.Enemy
{
    public class EnemyStatusLogic
    {
        private readonly EnemyStatusDataAccessor _enemyStatusDataAccessor;
        private readonly InstanceIdGenerator _instanceIdGenerator;
        public readonly Subject<ObjectStatus> OnDamageSubject = new(); //ダメージを受けた
        public readonly Subject<long> OnDeadSubject = new(); //死亡した

        [Inject]
        public EnemyStatusLogic(EnemyStatusDataAccessor enemyStatusDataAccessor,
            InstanceIdGenerator instanceIdGenerator)
        {
            _enemyStatusDataAccessor = enemyStatusDataAccessor;
            _instanceIdGenerator = instanceIdGenerator;
        }

        public ObjectStatus CreateEnemy(ObjectSettings baseParam)
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

        public void ProcessDamage()
        {
            //ダメージ処理
            foreach (var enemyStatus in _enemyStatusDataAccessor.StatusEnumerable)
            {
                if (enemyStatus.Damage > 0)
                {
                    OnDamageSubject.OnNext(enemyStatus);
                    enemyStatus.ReflectDamage();
                }
            }

            //死亡処理
            foreach (var enemyStatus in _enemyStatusDataAccessor.StatusEnumerable.Reverse())
            {
                if (enemyStatus.isDead)
                {
                    OnDeadSubject.OnNext(enemyStatus.InstanceId);
                    RemoveEnemy(enemyStatus.InstanceId);
                }
            }
        }
    }
}
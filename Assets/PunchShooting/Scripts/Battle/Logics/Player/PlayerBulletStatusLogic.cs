using System.Linq;
using PunchShooting.Battle.Data;
using PunchShooting.Battle.Data.Player;
using R3;
using VContainer;

namespace PunchShooting.Battle.Logics.Player
{
    public class PlayerBulletStatusLogic
    {
        private readonly InstanceIdGenerator _instanceIdGenerator;
        private readonly PlayerBulletStatusDataAccessor _playerBulletStatusDataAccessor;
        public readonly Subject<ObjectStatus> OnDamageSubject = new(); //ダメージを受けた
        public readonly Subject<long> OnDeadSubject = new(); //死亡した


        [Inject]
        public PlayerBulletStatusLogic(PlayerBulletStatusDataAccessor playerBulletStatusDataAccessor,
            InstanceIdGenerator instanceIdGenerator)
        {
            _playerBulletStatusDataAccessor = playerBulletStatusDataAccessor;
            _instanceIdGenerator = instanceIdGenerator;
        }

        public ObjectStatus CreateBullet(ObjectBaseParam baseParam)
        {
            var id = _instanceIdGenerator.GenerateId();

            var status = new ObjectStatus(id, baseParam);
            _playerBulletStatusDataAccessor.AddStatus(status);

            return status;
        }

        public void RemoveBullet(long id)
        {
            _playerBulletStatusDataAccessor.RemoveStatus(id);
        }

        public void RemoveAllBullets()
        {
            _playerBulletStatusDataAccessor.RemoveAllStatuses();
        }

        public void ProcessDamage()
        {
            //ダメージ処理
            foreach (var bulletStatus in _playerBulletStatusDataAccessor.StatusEnumerable)
            {
                if (bulletStatus.Damage > 0)
                {
                    OnDamageSubject.OnNext(bulletStatus);
                    bulletStatus.ReflectDamage();
                }
            }

            //死亡処理
            foreach (var bulletStatus in _playerBulletStatusDataAccessor.StatusEnumerable.Reverse())
            {
                if (bulletStatus.isDead)
                {
                    OnDeadSubject.OnNext(bulletStatus.InstanceId);
                    RemoveBullet(bulletStatus.InstanceId);
                }
            }
        }
    }
}
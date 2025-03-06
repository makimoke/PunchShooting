using PunchShooting.Battle.Data;
using VContainer;

namespace PunchShooting.Battle.Logic
{
    public class PlayerBulletStatusLogic
    {
        private readonly InstanceIdGenerator _instanceIdGenerator;
        private readonly PlayerBulletStatusDataAccessor _playerBulletStatusDataAccessor;

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
    }
}
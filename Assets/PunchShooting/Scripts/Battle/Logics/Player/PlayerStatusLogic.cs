using PunchShooting.Battle.Data;
using PunchShooting.Battle.Data.Player;
using PunchShooting.Battle.Definitions.Player;
using R3;
using VContainer;

namespace PunchShooting.Battle.Logics.Player
{
    public class PlayerStatusLogic
    {
        private readonly PlayerStatusDataAccessor _playerStatusDataAccessor;
        public readonly Subject<ObjectStatus> OnDamageSubject = new(); //ダメージを受けた
        public readonly Subject<Unit> OnDeadSubject = new(); //死亡した


        [Inject]
        public PlayerStatusLogic(PlayerStatusDataAccessor playerStatusDataAccessor)
        {
            _playerStatusDataAccessor = playerStatusDataAccessor;
        }

        public void ProcessDamage()
        {
            var playerStatus = _playerStatusDataAccessor.Status;
            if (playerStatus.Damage > 0)
            {
                OnDamageSubject.OnNext(playerStatus);
                playerStatus.ReflectDamage();
            }

            if (playerStatus.isDead)
            {
                OnDeadSubject.OnNext(Unit.Default);
            }
        }

        public bool Cooldown(PlayerWeaponDefinition.WeaponIndex weaponIndex, float deltaTime, float reloadTime)
        {
            var weaponStatus = _playerStatusDataAccessor.WeaponStatuses[(int)weaponIndex];
            weaponStatus.BulletCooldown -= deltaTime;
            if (weaponStatus.BulletCooldown <= 0.0f)
            {
                weaponStatus.BulletCooldown = reloadTime;
                return true;
            }

            return false;
        }
    }
}
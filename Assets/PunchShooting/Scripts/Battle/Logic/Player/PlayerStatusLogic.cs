using PunchShooting.Battle.Data;
using PunchShooting.Battle.Data.Player;
using R3;
using VContainer;

namespace PunchShooting.Battle.Logic.Player
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
    }
}
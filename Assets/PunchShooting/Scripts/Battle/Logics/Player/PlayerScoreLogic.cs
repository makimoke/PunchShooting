using PunchShooting.Battle.Data.Player;
using VContainer;

namespace PunchShooting.Battle.Logics.Player
{
    public class PlayerScoreLogic
    {
        private readonly PlayerScoreDataAccessor _playerScoreDataAccessor;

        [Inject]
        public PlayerScoreLogic(PlayerScoreDataAccessor playerScoreDataAccessor)
        {
            _playerScoreDataAccessor = playerScoreDataAccessor;
        }

        public void AddScore(int score)
        {
            _playerScoreDataAccessor.Score += score;
        }

        public void Reset()
        {
            _playerScoreDataAccessor.Score = 0;
        }
    }
}
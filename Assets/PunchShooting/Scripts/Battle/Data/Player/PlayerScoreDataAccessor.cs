// スコアデータホルダ

using R3;

namespace PunchShooting.Battle.Data.Player
{
    public class PlayerScoreDataAccessor
    {
        private readonly ReactiveProperty<int> _scoreProperty = new(0);

        public int Score
        {
            set => _scoreProperty.Value = value;
            get => _scoreProperty.Value;
        }

        public ReadOnlyReactiveProperty<int> ScoreProperty => _scoreProperty;
    }
}
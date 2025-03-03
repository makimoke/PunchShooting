// ステージ状態

using R3;

namespace PunchShooting.Battle.Data
{
    public class StageStatusDataAccessor
    {
        public enum StageStatus
        {
            Ready, // 準備中
            Playing, // ゲーム中
            GameOver, // ゲームオーバー
            StageClear // ステージクリア
        }

        private readonly ReactiveProperty<StageStatus> _statusProperty = new();

        public StageStatus Status
        {
            set => _statusProperty.Value = value;
            get => _statusProperty.Value;
        }

        public ReadOnlyReactiveProperty<StageStatus> StatusProperty => _statusProperty;

        public int LivingEnemyCount { get; set; } //生存敵数
    }
}

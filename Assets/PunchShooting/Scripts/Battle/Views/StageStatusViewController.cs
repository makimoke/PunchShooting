using System;
using PunchShooting.Battle.Data;
using PunchShooting.Battle.Data.Player;
using R3;
using VContainer;

namespace PunchShooting.Battle.Views
{
    // 画面上情報
    public class StageStatusViewController : IDisposable
    {
        private readonly PlayerScoreDataAccessor _playerScoreDataAccessor;
        private readonly PlayerStatusDataAccessor _playerStatusDataAccessor;
        private readonly StageStatusDataAccessor _stageStatusDataAccessor;
        private readonly StatusBarView _statusBarView;
        private DisposableBag _disposableBag;

        [Inject]
        public StageStatusViewController(StageStatusDataAccessor stageStatusDataAccessor,
            PlayerStatusDataAccessor playerStatusDataAccessor,
            PlayerScoreDataAccessor playerScoreDataAccessor,
            StatusBarView statusBarView)
        {
            _stageStatusDataAccessor = stageStatusDataAccessor;
            _playerStatusDataAccessor = playerStatusDataAccessor;
            _playerScoreDataAccessor = playerScoreDataAccessor;
            _statusBarView = statusBarView;
        }

        public void Dispose()
        {
            _disposableBag.Dispose();
        }

        public void Initialize()
        {
            _stageStatusDataAccessor.StatusProperty
                .Subscribe(status => _statusBarView.SetStageStatus(status))
                .AddTo(ref _disposableBag);
            _playerStatusDataAccessor.HpProperty
                .Subscribe(hp => _statusBarView.SetHp(hp))
                .AddTo(ref _disposableBag);
            _playerScoreDataAccessor.ScoreProperty
                .Subscribe(score => _statusBarView.SetScore(score))
                .AddTo(ref _disposableBag);
        }
    }
}
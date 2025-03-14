using System;
using PunchShooting.Battle.Data;
using R3;
using VContainer;

namespace PunchShooting.Battle.Views
{
    // 画面上情報
    public class StageStatusViewController : IDisposable
    {
        private DisposableBag _disposableBag;
        private readonly StageStatusDataAccessor _stageStatusDataAccessor;
        private readonly StatusBarView _statusBarView;

        [Inject]
        public StageStatusViewController(StageStatusDataAccessor stageStatusDataAccessor,
            StatusBarView statusBarView)
        {
            _stageStatusDataAccessor = stageStatusDataAccessor;
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
        }
    }
}
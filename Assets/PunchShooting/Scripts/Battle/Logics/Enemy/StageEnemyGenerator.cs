using System;
using Cysharp.Threading.Tasks;
using PunchShooting.Battle.Data.Enemy;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace PunchShooting.Battle.Logics.Enemy
{
    public class StageEnemyGenerator : IDisposable
    {
        public readonly Subject<StageEnemyCreateParam> OnCreateEnemySubject = new(); //敵生成時
        private readonly Vector2 SquareSize = new(1.3f, 1.3f); //1コマのサイズ
        private readonly Vector2 StartPosition = new(0.0f, 6.0f);
        private int _currentRow;
        private float _currentSeconds;
        private StageEnemyPlaceParam _stageEnemyPlaceParam;

        [Inject]
        private StageEnemyGenerator()
        {
        }

        // 生成終了
        public bool IsCompleted => _currentRow == _stageEnemyPlaceParam.EnemySquadPlaceParams.Length;

        public void Dispose()
        {
            Addressables.Release(_stageEnemyPlaceParam);
        }

        public async UniTask LoadAsync()
        {
            _stageEnemyPlaceParam = await Addressables
                .LoadAssetAsync<StageEnemyPlaceParam>("Assets/PunchShooting/SObjects/Battle/Enemy/so_stage_enemy_place_001.asset").Task;
        }

        public void Reset()
        {
            _currentRow = 0;
            _currentSeconds = 0;
        }

        public void Update(float deltaTime)
        {
            _currentSeconds += deltaTime;
            for (var i = _currentRow; i < _stageEnemyPlaceParam.EnemySquadPlaceParams.Length; i++)
            {
                var squadParam = _stageEnemyPlaceParam.EnemySquadPlaceParams[i];
                if (squadParam.WaitSeconds <= _currentSeconds)
                {
                    foreach (var enemyPlaceParam in squadParam.EnemyPlaceParams)
                    {
                        var position = StartPosition +
                                       (squadParam.SquarePosition + enemyPlaceParam.SquarePosition) * SquareSize;

                        OnCreateEnemySubject.OnNext(new StageEnemyCreateParam(enemyPlaceParam.Id, position));
                    }

                    _currentRow = i + 1;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
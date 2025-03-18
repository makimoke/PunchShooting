using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PunchShooting.Battle.Definitions.Enemy;
using UnityEngine.AddressableAssets;

namespace PunchShooting.Battle.Data.Enemy
{
    //敵基本パラメータ
    public class EnemySettingsDataAccessor : IDisposable
    {
        private readonly Dictionary<EnemySettingsDefinition.ParamId, ObjectSettings> _paramDictionary = new();

        public void Dispose()
        {
            foreach (var settings in _paramDictionary.Values)
            {
                Addressables.Release(settings);
            }
        }
        
        public async UniTask LoadAsync()
        {
            _paramDictionary[EnemySettingsDefinition.ParamId.Enemy001] =
                await Addressables.LoadAssetAsync<ObjectSettings>("Assets/PunchShooting/SObjects/Battle/Enemy/so_enemy_001.asset").Task;
        }

        public ObjectSettings FindSettings(EnemySettingsDefinition.ParamId paramId)
        {
            return _paramDictionary[paramId];
        }
    }
}
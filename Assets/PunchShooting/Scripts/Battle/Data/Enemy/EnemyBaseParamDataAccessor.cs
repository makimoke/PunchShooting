using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PunchShooting.Battle.Definitions.Enemy;
using UnityEngine.AddressableAssets;

namespace PunchShooting.Battle.Data.Enemy
{
    //敵基本パラメータ
    public class EnemyBaseParamDataAccessor : IDisposable
    {
        private readonly Dictionary<EnemyBaseParamDefinition.ParamId, ObjectBaseParam> _paramDictionary = new();

        public void Dispose()
        {
            foreach (var baseParam in _paramDictionary.Values)
            {
                Addressables.Release(baseParam);
            }
        }
        
        public async UniTask LoadAsync()
        {
            _paramDictionary[EnemyBaseParamDefinition.ParamId.Enemy001] =
                await Addressables.LoadAssetAsync<ObjectBaseParam>("Assets/PunchShooting/SObjects/Battle/Enemy/so_enemy_001.asset").Task;
        }

        public ObjectBaseParam FindBaseParam(EnemyBaseParamDefinition.ParamId paramId)
        {
            return _paramDictionary[paramId];
        }
    }
}
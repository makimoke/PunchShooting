using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PunchShooting.Battle.Definitions.Player;
using UnityEngine.AddressableAssets;

namespace PunchShooting.Battle.Data.Player
{
    //プレイヤ弾基本パラメータ
    public class PlayerBulletBaseParamDataAccessor : IDisposable
    {
        private readonly Dictionary<PlayerBulletBaseParamDefinition.ParamId, ObjectBaseParam> _paramDictionary = new();

        public void Dispose()
        {
            foreach (var baseParam in _paramDictionary.Values)
            {
                Addressables.Release(baseParam);
            }
        }
        
        public async UniTask LoadAsync()
        {
            _paramDictionary[PlayerBulletBaseParamDefinition.ParamId.PBul001] =
                await Addressables.LoadAssetAsync<ObjectBaseParam>("Assets/PunchShooting/SObjects/Battle/Player/so_pbul_001.asset").Task;
            _paramDictionary[PlayerBulletBaseParamDefinition.ParamId.PBul002] =
                await Addressables.LoadAssetAsync<ObjectBaseParam>("Assets/PunchShooting/SObjects/Battle/Player/so_pbul_002.asset").Task;
        }

        public ObjectBaseParam FindBaseParam(PlayerBulletBaseParamDefinition.ParamId paramId)
        {
            return _paramDictionary[paramId];
        }
    }
}
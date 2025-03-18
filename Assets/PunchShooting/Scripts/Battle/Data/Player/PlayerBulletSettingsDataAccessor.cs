using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PunchShooting.Battle.Definitions.Player;
using UnityEngine.AddressableAssets;

namespace PunchShooting.Battle.Data.Player
{
    //プレイヤ弾基本パラメータ
    public class PlayerBulletSettingsDataAccessor : IDisposable
    {
        private readonly Dictionary<PlayerBulletSettingsDefinition.ParamId, PlayerBulletSettings> _paramDictionary = new();

        public void Dispose()
        {
            foreach (var settings in _paramDictionary.Values)
            {
                Addressables.Release(settings);
            }
        }

        public async UniTask LoadAsync()
        {
            _paramDictionary[PlayerBulletSettingsDefinition.ParamId.PBul001] =
                await Addressables.LoadAssetAsync<PlayerBulletSettings>("Assets/PunchShooting/SObjects/Battle/Player/so_pbul_001.asset").Task;
            _paramDictionary[PlayerBulletSettingsDefinition.ParamId.PBul002] =
                await Addressables.LoadAssetAsync<PlayerBulletSettings>("Assets/PunchShooting/SObjects/Battle/Player/so_pbul_002.asset").Task;
        }

        public PlayerBulletSettings FindSettings(PlayerBulletSettingsDefinition.ParamId paramId)
        {
            return _paramDictionary[paramId];
        }
    }
}
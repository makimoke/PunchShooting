using System;
using PunchShooting.Battle.Definitions.Enemy;
using UnityEngine;

namespace PunchShooting.Battle.Data.Enemy
{
// 敵配置
    [Serializable]
    public class EnemyPlaceParam
    {
        public EnemyBaseParamDefinition.ParamId Id;
        public Vector2 SquarePosition = Vector2.zero; // マス目配置位置
    }

// 分隊配置
    [Serializable]
    public class EnemySquadPlaceParam
    {
        public float WaitSeconds; //待ち時間(s)
        public Vector2 SquarePosition = Vector2.zero; // マス目配置位置
        public EnemyPlaceParam[] EnemyPlaceParams; // 敵配置
    }


    [CreateAssetMenu(fileName = "so_stage_enemy_place_000", menuName = "PunchShooting/StageEnemyPlaceParam")]
// ステージ敵配置
    public class StageEnemyPlaceParam : ScriptableObject
    {
        public EnemySquadPlaceParam[] EnemySquadPlaceParams; // 分隊配置
    }
}
using PunchShooting.Battle.Definitions.Enemy;
using UnityEngine;

namespace PunchShooting.Battle.Data.Enemy
{
    public class StageEnemyCreateParam
    {
        public StageEnemyCreateParam(EnemyBaseParamDefinition.ParamId id, Vector3 position)
        {
            Id = id;
            Position = position;
        }

        public EnemyBaseParamDefinition.ParamId Id { get; private set; }
        public Vector3 Position { get; private set; }
    }
}
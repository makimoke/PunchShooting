using UnityEngine;

namespace PunchShooting.Battle.Views
{
    //スプライト衝突結果
    public class SpriteCollisionResult
    {
        public SpriteCollisionResult(long instanceId, Collider2D collider)
        {
            InstanceId = instanceId;
            Collider = collider;
        }

        public long InstanceId { get; private set; }
        public Collider2D Collider { get; private set; }
    }
}
using UnityEngine;

namespace PunchShooting.Battle.Data.Player
{
    [CreateAssetMenu(fileName = "NewParam", menuName = "PunchShooting/PlayerBulletSettings")]
    public class PlayerBulletSettings : ObjectSettings
    {
        [Header("Battle Properties")] public float CoolTime = 1.0f;

        public Vector2 Position;
    }
}
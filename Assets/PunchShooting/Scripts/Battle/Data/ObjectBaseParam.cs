using PunchShooting.Battle.Definitions;
using UnityEngine;

namespace PunchShooting.Battle.Data
{
    [CreateAssetMenu(fileName = "NewParam", menuName = "PunchShooting/Object")]
    public class ObjectBaseParam : ScriptableObject
    {
        public string Name; // 名前

        [Header("Resources Properties")] public SpriteResourceDefinition.PrefabId PrefabId;
        public SpriteResourceDefinition.SpriteId SpriteId;

        [Header("Battle Properties")] public int Hp = 10; // 体力

        public int Str = 10; // 攻撃力
        public int Score = 10; // スコア

        [Header("Movement Properties")] public float MoveSpeed = 10; // 移動速度

        public Vector2 MoveVector = Vector2.zero; // 移動方向
    }
}
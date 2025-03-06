using UnityEngine;

namespace PunchShooting.Battle.Data
{
    public class ObjectStatus
    {
        public ObjectStatus(int hp, int str)
        {
            Hp = hp;
            Str = str;
        }

        public ObjectStatus(ObjectBaseParam baseParam)
        {
            Hp = baseParam.Hp;
            Str = baseParam.Str;
            Score = baseParam.Score;
            MoveSpeed = baseParam.MoveSpeed;
            MoveVector = baseParam.MoveVector.normalized;
        }

        public long InstanceId { get; set; } // インスタンスID
        
        // パラメータ
        public int Hp { get; set; } // 体力
        public int Str { get; set; } // 攻撃力
        public int Score { get; set; } // スコア
        public float MoveSpeed { get; set; } // 移動速度
        public Vector3 MoveVector { get; set; } // 移動方向

        // ダメージ
        public int Damage { get; set; } // ダメージ値
        public int CurrentHp => Hp - Damage; // 現在のHP
        public bool isDead => CurrentHp <= 0;

        public void ReflectDamage()
        {
            Hp -= Damage;
            Damage = 0;
        }
    }
}

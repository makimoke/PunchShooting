using R3;

namespace PunchShooting.Battle.Data
{
    //プレイヤステータス
    public class PlayerStatusDataAccessor
    {
        private readonly ReactiveProperty<int> _hpMaxProperty = new(0);
        private readonly ReactiveProperty<int> _hpProperty = new(0);
        public ObjectStatus Status { get; set; }

        public int Hp
        {
            set => _hpProperty.Value = value;
            get => _hpProperty.Value;
        }

        public int HpMax
        {
            set => _hpMaxProperty.Value = value;
            get => _hpMaxProperty.Value;
        }

        public ReadOnlyReactiveProperty<int> HpProperty => _hpProperty;
        public ReadOnlyReactiveProperty<int> HpMaxProperty => _hpMaxProperty;

        public void Reset()
        {
            HpMax = 50;
            Hp = HpMax;
            Status = new ObjectStatus(Hp, 10000);
        }

        public void ReflectDamage()
        {
            Status.ReflectDamage();
            Hp = Status.Hp;
        }
    }
}

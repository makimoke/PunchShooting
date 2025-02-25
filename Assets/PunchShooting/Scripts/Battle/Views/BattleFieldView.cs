using UnityEngine;

namespace PunchShooting.Battle.Views
{
    public class BattleFieldView : MonoBehaviour
    {
        public Transform Transform { get; private set; }

        private void Start()
        {
            Transform = transform;
        }
    }
}
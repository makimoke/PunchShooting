using UnityEngine;

namespace PunchShooting.Battle.Views.Player
{
    //自機
    public class PlayerShipView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Start()
        {
        }

        public void AddPosition(Vector3 vector)
        {
            transform.localPosition += vector;
        }

        //スプライト関連の処理を書く
    }
}
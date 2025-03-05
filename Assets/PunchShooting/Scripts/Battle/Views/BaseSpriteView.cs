using UnityEngine;

namespace PunchShooting.Battle.Views
{
    //スプライトViewクラス
    public abstract class BaseSpriteView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void AddPosition(Vector3 vector)
        {
            transform.localPosition += vector;
        }

        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}
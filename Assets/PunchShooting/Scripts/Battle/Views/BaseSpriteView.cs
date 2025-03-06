using R3;
using UnityEngine;

namespace PunchShooting.Battle.Views
{
    //スプライトViewクラス
    public abstract class BaseSpriteView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        public readonly Subject<Collider2D> OnTriggerEnterSubject = new();

        public long InstanceId { get; set; }

        public Vector3 Position => transform.localPosition;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            OnTriggerEnterSubject.OnNext(collider);
        }

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
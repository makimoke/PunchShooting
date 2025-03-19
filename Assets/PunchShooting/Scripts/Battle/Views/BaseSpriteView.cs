using R3;
using UnityEngine;

namespace PunchShooting.Battle.Views
{
    //スプライトViewクラス
    public abstract class BaseSpriteView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D boxCollider2D;
        public readonly Subject<Collider2D> OnTriggerEnterSubject = new();
        public Vector3 Velocity { get; set; } = Vector3.zero;
        public Vector3 Acceleration { get; set; } = Vector3.zero;

        public long InstanceId { get; set; }

        public Vector2 Position => transform.localPosition;
        public SpriteRenderer SpriteRenderer => spriteRenderer;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            OnTriggerEnterSubject.OnNext(collider);
        }

        public void AddPosition(Vector3 vector)
        {
            transform.localPosition += vector;
        }

        public void Move(float deltaTime)
        {
            Velocity += Acceleration * deltaTime;
            transform.localPosition += Velocity * deltaTime;
        }

        public void SetAngleZ(float angleZ)
        {
            transform.localEulerAngles = new Vector3(0, 0, angleZ);
        }

        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }

        public void AdjustColliderSize()
        {
            boxCollider2D.size = spriteRenderer.bounds.size * 0.8f; //暫定対応
        }
    }
}
using TMPro;
using UnityEngine;

namespace PunchShooting.Battle.Views
{
    public class FpsDisplayView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fpsText;
        private float fps;

        // 変数
        private int frameCount;
        private float prevTime;

        // 初期化処理
        private void Start()
        {
            // 変数の初期化
            frameCount = 0;
            prevTime = 0.0f;

            Application.targetFrameRate = 60;
        }

        // 更新処理
        private void Update()
        {
            frameCount++;
            var time = Time.realtimeSinceStartup - prevTime;

            if (time >= 0.5f)
            {
                fps = frameCount / time;

                frameCount = 0;
                prevTime = Time.realtimeSinceStartup;
            }

            fpsText.text = "FPS:" + ((int)fps);
        }
    }
}

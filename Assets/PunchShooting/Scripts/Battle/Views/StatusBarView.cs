using PunchShooting.Battle.Data;
using TMPro;
using UnityEngine;

namespace PunchShooting.Battle.Views
{
    // 画面上情報
    public class StatusBarView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI stageStatusText;
        [SerializeField] private TextMeshProUGUI hpText;

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }

        public void SetStageStatus(StageStatusDataAccessor.StageStatus stageStatus)
        {
            stageStatusText.text = stageStatus.ToString();
        }

        public void SetHp(int hp)
        {
            hpText.text = hp.ToString();
        }
    }
}
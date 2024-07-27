using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class HighScorePanel:MonoBehaviour
    {
        [SerializeField] private TMP_Text highScoreText;
        // Bu field HigshScoretexti tutuyor

        public void UpdateHighScore(int highscore)
        // Burada method int tipte param alıyor.
        {
            if (highScoreText != null)
            {
                highScoreText.text = $"High Score: {highscore}";
            }
            // Eğer text boş değilse  yazdırğımız stryi atayacak.
        }

    }
}
using TMPro;
using UnityEngine;

namespace _Scripts.UI.Leaderboard
{
    public class LeaderboardBlock : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void Init(string text)
        {
            _text.text = text;
        }
    }
}
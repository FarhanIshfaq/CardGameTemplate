using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardManager : SingletonBase<LeaderboardManager>
{
    [SerializeField] private Text leaderboardText;

    private readonly List<int> scores = new List<int>();
    private const int MaxScores = 5;

    private void Awake()
    {
        LoadScores();
    }

    // Save a new score and update leaderboard
    public void SaveScore(int score)
    {
        scores.Add(score);
        scores.Sort();
        if (scores.Count > MaxScores)
            scores.RemoveAt(scores.Count - 1); // Keep only top 5

        PlayerPrefs.SetString("Leaderboard", string.Join(",", scores));
        PlayerPrefs.Save();

        DisplayLeaderboard();
    }

    // Load scores from PlayerPrefs
    private void LoadScores()
    {
        scores.Clear();
        string savedScores = PlayerPrefs.GetString("Leaderboard", "");
        if (!string.IsNullOrEmpty(savedScores))
        {
            foreach (string score in savedScores.Split(','))
            {
                if (int.TryParse(score, out int parsedScore))
                    scores.Add(parsedScore);
            }
            scores.Sort();
        }
        DisplayLeaderboard();
    }

    // Display scores on UI
    private void DisplayLeaderboard()
    {
        if (leaderboardText == null) return;

        leaderboardText.text = "Leaderboard\n";
        for (int i = 0; i < scores.Count; i++)
            leaderboardText.text += $"{i + 1}. {scores[i]} Turns\n";
    }

    // Reset all scores
    public void ResetScores()
    {
        scores.Clear();
        PlayerPrefs.DeleteKey("Leaderboard");
        PlayerPrefs.Save();
        DisplayLeaderboard();
    }
}

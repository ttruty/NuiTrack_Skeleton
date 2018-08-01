using UnityEngine;
using UnityEngine.UI;

public class GameProgress : MonoBehaviour
{
    public static GameProgress instance = null;

    [SerializeField]
    Text scoreText;

    int currentScore = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void UpdateScoreText()
    {
        scoreText.text = "Your score: " + currentScore;
    }

    public void AddScore(int val)
    {
        currentScore += val;
        UpdateScoreText();
    }

    public void RemoveScore(int val)
    {
        currentScore -= val;
        UpdateScoreText();
    }

}

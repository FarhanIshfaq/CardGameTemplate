using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum GameMode
{
    WithoutTimer,
    TimerMode,
    LimitedTurnsMode
}
public class GameManager : MonoBehaviour
{


    [Header("Scriptable Objects")]
    [SerializeField] private GameData data;

    [Header("Grid Settings")]
    [SerializeField] private CardGrid cardGrid;
    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 4;

    [Header("UI Elements")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text turnsText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text modeText;

    [Header("Game Settings")]
    [SerializeField] private GameMode currentGameMode = GameMode.WithoutTimer;
    [SerializeField] private int maxTurns = 20;
    [SerializeField] private float timeLimit = 60f;

    private List<Card> cards;
    private Card firstSelectedCard;
    private Card secondSelectedCard;

    private int matches;
    private int turns;
    private bool isProcessing;
    private bool isGameOver;

    private float timer;

    private static readonly WaitForSeconds matchDelay = new WaitForSeconds(0.5f);

    private void Start()
    {
        InitializeAudio();
        StartLevel(rows, columns, currentGameMode);
    }

    private void InitializeAudio()
    {
        if (data && AudioManager.Instance)
            AudioManager.Instance.PlayMusic(data.backgroundMusic);
    }

    public void StartLevel(int rows, int columns, GameMode gameMode)
    {
        ResetGameVariables(gameMode);

        if (data)
            cards = cardGrid.GenerateGrid(rows, columns, data.cardSprites, OnCardClicked);

        UpdateUI();
    }

    private void ResetGameVariables(GameMode gameMode)
    {
        matches = turns = 0;
        isProcessing = isGameOver = false;
        timer = timeLimit;
        currentGameMode = gameMode;
    }

    private void Update()
    {
        if (isGameOver) return;

        switch (currentGameMode)
        {
            case GameMode.TimerMode:
                timer -= Time.deltaTime;
                if (timer <= 0) GameOver("Time's up!");
                UpdateTimerUI();
                break;

            case GameMode.LimitedTurnsMode:
                if (turns >= maxTurns) GameOver("Out of turns!");
                break;
        }
    }

    private void OnCardClicked(Card clickedCard)
    {
        if (isProcessing || clickedCard == firstSelectedCard || isGameOver) return;

        AudioManager.Instance?.PlaySFX(data.flipSound);

        clickedCard.Flip();

        if (firstSelectedCard == null)
        {
            firstSelectedCard = clickedCard;
        }
        else
        {
            secondSelectedCard = clickedCard;
            turns++;
            isProcessing = true;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return matchDelay;

        if (firstSelectedCard.Id == secondSelectedCard.Id)
        {
            HandleMatch();
        }
        else
        {
            firstSelectedCard.FlipBack();
            secondSelectedCard.FlipBack();
        }

        firstSelectedCard = secondSelectedCard = null;
        isProcessing = false;

        UpdateUI();
    }

    private void HandleMatch()
    {
        firstSelectedCard.Match();
        secondSelectedCard.Match();
        matches++;

        AudioManager.Instance?.PlaySFX(data.matchSound);

        if (matches == cards.Count / 2)
            OnGameWon();
    }

    private void UpdateUI()
    {
        scoreText.text = $"Matches: {matches}";

        if (currentGameMode == GameMode.LimitedTurnsMode)
            turnsText.text = $"Turns: {turns}/{maxTurns}";
        else
            turnsText.text = $"Turns: {turns}";

        timerText.gameObject.SetActive(currentGameMode == GameMode.TimerMode);
        turnsText.gameObject.SetActive(currentGameMode != GameMode.TimerMode);

        modeText.text = $"Mode: {currentGameMode}";
    }

    private void UpdateTimerUI()
    {
        timerText.text = $"Time: {Mathf.Max(0, Mathf.Ceil(timer))}s";
    }

    private void OnGameWon()
    {
        isGameOver = true;
        AudioManager.Instance?.PlaySFX(data.winSound);
        LeaderboardManager.Instance?.SaveScore(turns);
        Debug.Log($"You Win! Turns: {turns}");
    }

    private void GameOver(string message)
    {
        isGameOver = true;
        Debug.Log(message);
    }
}

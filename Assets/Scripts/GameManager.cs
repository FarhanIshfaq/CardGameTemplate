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
    [SerializeField] private GameData Data;

    [Header("Grid Settings")]
    [SerializeField] private CardGrid cardGrid;
    [SerializeField] private int rows = 3;
    [SerializeField] private int columns = 4;

    [Header("UI Elements")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text turnsText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text modeText;
    [SerializeField] private Button resetButton;  // Add Reset Button


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
    private bool canClick = true;

    private static readonly WaitForSeconds matchDelay = new WaitForSeconds(0.5f);

    private void Start()
    {
        InitializeAudio();
        StartLevel(rows, columns, currentGameMode);
        resetButton.gameObject.SetActive(false);
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetGame);
    }
    public void ResetGame()
    {
        isGameOver = false;
        firstSelectedCard = null;
        secondSelectedCard = null;
        canClick = true;
        StartLevel(rows, columns, currentGameMode);
        Logger.Log("Game Reset");

        if (resetButton != null)
            resetButton.gameObject.SetActive(false); // Hide reset button until next game end
    }
    private void InitializeAudio()
    {
        if (Data && AudioManager.Instance)
            AudioManager.Instance.PlayMusic(Data.backgroundMusic);
    }

    public void StartLevel(int rows, int columns, GameMode gameMode)
    {
        ResetGameVariables(gameMode);

        if (Data)
            cards = cardGrid.GenerateGrid(rows, columns, Data.cardSprites, OnCardClicked);

        UpdateUI();
    }

    private void ResetGameVariables(GameMode gameMode)
    {
        matches = turns = 0;
        isProcessing = isGameOver = false;
        timer = timeLimit;
        currentGameMode = gameMode;
        canClick = true;
    }

    private void Update()
    {
        if (isGameOver) return;

        if (currentGameMode == GameMode.TimerMode)
            UpdateTimer();
        else if (currentGameMode == GameMode.LimitedTurnsMode && turns >= maxTurns)
            GameOver("Out of turns!");
    }

    private void UpdateTimer()
    {
        timer -= Time.deltaTime;
        if (timer <= 0) GameOver("Time's up!");
        UpdateTimerUI();
    }

    private void OnCardClicked(Card clickedCard)
    {
        if (!canClick || clickedCard.IsFlipping || clickedCard == firstSelectedCard || isGameOver) return;

        AudioManager.Instance?.PlaySFX(Data.flipSound);
        clickedCard.Flip(true);

        if (firstSelectedCard == null)
            firstSelectedCard = clickedCard;
        else
        {
            secondSelectedCard = clickedCard;
            turns++;
            canClick = false;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return matchDelay;

        if (firstSelectedCard.Id == secondSelectedCard.Id)
            HandleMatch();
        else
        {
            firstSelectedCard.Flip(false);
            secondSelectedCard.Flip(false);
        }

        firstSelectedCard = secondSelectedCard = null;
        canClick = true;
        UpdateUI();
    }

    private void HandleMatch()
    {
        firstSelectedCard.Match();
        secondSelectedCard.Match();
        matches++;

        AudioManager.Instance?.PlaySFX(Data.matchSound);

        if (matches == cards.Count / 2)
            OnGameWon();
    }

    private void UpdateUI()
    {
        scoreText.text = $"Matches: {matches}";
        turnsText.text = currentGameMode == GameMode.LimitedTurnsMode ? $"Turns: {turns}/{maxTurns}" : $"Turns: {turns}";
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
        AudioManager.Instance?.PlaySFX(Data.winSound);
        LeaderboardManager.Instance?.SaveScore(turns);
        Logger.Log($"You Win! Turns: {turns}");
        resetButton.gameObject.SetActive(true);
    }

    private void GameOver(string message)
    {
        isGameOver = true;
        resetButton.gameObject.SetActive(true);
        Logger.Log(message);
    }
}

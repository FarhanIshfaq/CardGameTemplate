using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private Button resetButton;

    [Header("Game Settings")]
    [SerializeField] private GameMode currentGameMode = GameMode.WithoutTimer;
    [SerializeField] private int maxTurns = 20;
    [SerializeField] private float timeLimit = 60f;

    private List<Card> _cards;
    private Card _firstSelectedCard;
    private Card _secondSelectedCard;

    private int _matches;
    private int _turns;
    private bool _isProcessing;
    private bool _isGameOver;

    private float _timer;
    private bool _canClick = true;

    public float  matchDelay = 0.4f;

    private void Start()
    {
        InitializeAudio();
        StartLevel(rows, columns, currentGameMode);
        resetButton.gameObject.SetActive(false);
        resetButton.onClick.AddListener(ResetGame);
        timerText.gameObject.SetActive(currentGameMode == GameMode.TimerMode);
        turnsText.gameObject.SetActive(currentGameMode != GameMode.TimerMode);
    }

    public void ResetGame()
    {
        _isGameOver = false;
        _firstSelectedCard = null;
        _secondSelectedCard = null;
        _canClick = true;

        foreach (var card in _cards)
        {
            card.gameObject.SetActive(true); // Ensure cards are visible
        }

        StartLevel(rows, columns, currentGameMode);
        Logger.Log("Game Reset");
        resetButton.gameObject.SetActive(false);
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
            _cards = cardGrid.GenerateGrid(rows, columns, Data.cardSprites, OnCardClicked);
        UpdateUI();
    }

    private void ResetGameVariables(GameMode gameMode)
    {
        _matches = _turns = 0;
        _isProcessing = _isGameOver = false;
        _timer = timeLimit;
        currentGameMode = gameMode;
        _canClick = true;
    }

    private void Update()
    {
        if (_isGameOver) return;

        if (currentGameMode == GameMode.TimerMode)
            UpdateTimer();
        else if (currentGameMode == GameMode.LimitedTurnsMode && _turns >= maxTurns)
            GameOver("Out of turns!");
    }

    private void UpdateTimer()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0) GameOver("Time's up!");
        UpdateTimerUI();
    }

    private void OnCardClicked(Card clickedCard)
    {
        if (!_canClick || clickedCard.IsFlipping || clickedCard == _firstSelectedCard || _isGameOver) return;

        AudioManager.Instance?.PlaySFX(Data.flipSound);
        clickedCard.Flip(true);

        if (_firstSelectedCard == null)
        {
            _firstSelectedCard = clickedCard;
        }
        else
        {
            Logger.Log("CheckMatch Start");
            _secondSelectedCard = clickedCard;
            _turns++;
            _canClick = false;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(matchDelay);
        Logger.Log("Checking");
        if (_firstSelectedCard.Id == _secondSelectedCard.Id)
        {
            HandleMatch();
        }
        else
        {
            Logger.Log("not matched");
            _firstSelectedCard.Flip(false);
            _secondSelectedCard.Flip(false);
        }

        _firstSelectedCard = _secondSelectedCard = null;
        _canClick = true;
        UpdateUI();
    }

    private void HandleMatch()
    {
        _firstSelectedCard.Match();
        _secondSelectedCard.Match();
        _matches++;

        AudioManager.Instance?.PlaySFX(Data.matchSound);

        if (_matches == _cards.Count / 2)
            OnGameWon();
    }

    private void UpdateUI()
    {
        scoreText.text = $"Matches: {_matches}";
        turnsText.text = currentGameMode == GameMode.LimitedTurnsMode ? $"Turns: {_turns}/{maxTurns}" : $"Turns: {_turns}";

        modeText.text = $"Mode: {currentGameMode}";
    }

    private void UpdateTimerUI()
    {
        timerText.text = $"Time: {Mathf.Max(0, Mathf.Ceil(_timer))}s";
    }

    private void OnGameWon()
    {
        _isGameOver = true;
        AudioManager.Instance?.PlaySFX(Data.winSound);
        LeaderboardManager.Instance?.SaveScore(_turns);
        Logger.Log($"You Win! Turns: {_turns}");
        resetButton.gameObject.SetActive(true);
    }

    private void GameOver(string message)
    {
        _isGameOver = true;
        resetButton.gameObject.SetActive(true);
        Logger.Log(message);
    }
}

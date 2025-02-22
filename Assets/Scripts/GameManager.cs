using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private List<Card> cards;
    private Card firstSelectedCard;
    private Card secondSelectedCard;
    private int matches;
    private int turns;
    private bool isProcessing;

    private void Start()
    {
        //InitializeAudio();
        StartLevel(rows, columns);
    }

    private void InitializeAudio()
    {
        if (data != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(data.backgroundMusic);
        }
    }

    public void StartLevel(int rows, int columns)
    {
        matches = 0;
        turns = 0;
        isProcessing = false;
        UpdateUI();

        if (data != null)
        {
            cards = cardGrid.GenerateGrid(rows, columns, data.cardSprites, OnCardClicked);
        }
    }

    private void OnCardClicked(Card clickedCard)
    {
        if (isProcessing || clickedCard == firstSelectedCard) return;

        //AudioManager.Instance.PlaySFX(data.flipSound);

        if (firstSelectedCard == null)
        {
            firstSelectedCard = clickedCard;
            firstSelectedCard.Flip();
        }
        else
        {
            secondSelectedCard = clickedCard;
            secondSelectedCard.Flip();
            turns++;
            isProcessing = true;

            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.7f);

        if (firstSelectedCard.Id == secondSelectedCard.Id)
        {
            firstSelectedCard.Match();
            secondSelectedCard.Match();
            matches++;

            //AudioManager.Instance.PlaySFX(data.matchSound);

            if (matches == cards.Count / 2)
                OnGameWon();
        }
        else
        {
            firstSelectedCard.FlipBack();
            secondSelectedCard.FlipBack();
        }

        firstSelectedCard = null;
        secondSelectedCard = null;
        isProcessing = false;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = $"Matches: {matches}";
        turnsText.text = $"Turns: {turns}";
    }

    private void OnGameWon()
    {
        //AudioManager.Instance.PlaySFX(data.winSound);
        LeaderboardManager.Instance.SaveScore(turns);
        Debug.Log($"You Win! Turns: {turns}");
    }

}

using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 2)]
public class GameData : ScriptableObject
{
    [Header("Card Sprites")]
    public Sprite[] cardSprites;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip winSound;
    public AudioClip comboSound;
}

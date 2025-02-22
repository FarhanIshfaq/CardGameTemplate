using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public GameMode gameMode = GameMode.WithoutTimer;
    public void LoadLevel(int rows, int columns)
    {
        Logger.Log("LevelManager Call");
        gameManager.StartLevel(rows, columns, gameMode);
    }
}

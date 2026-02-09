using UnityEngine;

public enum GameState           // ゲームの状態
{
    InGame,                     // ゲーム中
    GameClear,                  // ゲームクリア
    GameOver,                   // ゲームオーバー
    GameEnd,                    // ゲーム終了
}
public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // ゲームの状態
    public static GameState gameState;
    void Start()
    {
        gameState = GameState.InGame;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

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
    public string nextSceneName;            // 次のシーン名

    // スコア追加
    public static int totalScore;   // 合計スコア

    void Start()
    {
        gameState = GameState.InGame;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (gameState == GameState.GameClear)
        {
            gameState = GameState.GameEnd;  // ゲームの状態を更新
        }
        else if (gameState == GameState.GameOver)
        {
            gameState = GameState.GameEnd;  // ゲームの状態を更新
        }
    }

    //リスタート
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //次へ
    public void Next()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntranceController : MonoBehaviour
{
    public int doorNumber;      // ドア番号
    public string sceneName;    // 移行したいシーン名
    public bool opened;         // 開錠状況

    bool isPlayerTouch;         // プレイヤーとの接触状態

    bool announcement;          // アナウンス中かどうか

    GameObject worldUI;         // Canvasオブジェクト
    GameObject talkPanel;       // TalkPanelオブジェクト
    TextMeshProUGUI messageText;    // MessageTextオブジェクトのTextMeshProUGUIコンポーネント
    World_PlayerController worldPlayerCnt;  // World_Playerオブジェクトのコンポーネント

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        worldPlayerCnt = GameObject.FindGameObjectWithTag("Player").GetComponent<World_PlayerController>();
        worldUI = GameObject.FindGameObjectWithTag("WorldUI");
        talkPanel = worldUI.transform.Find("TalkPanel").gameObject;
        messageText = talkPanel.transform.Find("MessageText").gameObject.GetComponent<TextMeshProUGUI>();

        if (World_UIController.keyOpened != null)
        {
            opened = World_UIController.keyOpened[doorNumber];
        }
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーと接触 AND ActionButtonが押されている
        if (isPlayerTouch && worldPlayerCnt.IsActionButtonPressed)
        {
            // アナウンス中じゃなければ
            if (!announcement)
            {
                Time.timeScale = 0;     // ゲーム進行をストップ
                if (opened)     // 開錠済み
                {
                    Time.timeScale = 1;     // ゲーム進行を再開
                    // 該当ドア番号をGameManagerに管理しておいてもらう
                    GameManager.currentDoorNumber = doorNumber;
                    SceneManager.LoadScene(sceneName);
                    return;
                }
                // 未開錠の場合
                else if (GameManager.keys > 0)  // 鍵を持っている
                {
                    // 鍵を開ける音を鳴らす
                    SoundManager.currentSoundManager.PlaySE(SEType.DoorOpen);
                    messageText.text = "新たなステージへの扉を開けた！";
                    GameManager.keys--;     // 鍵を消耗
                    opened = true;          // 開錠フラグを立てる
                    // World_UIControllerが所持している開錠の帳簿(keyOpenedディクショナリー)に開錠したという情報を記録
                    World_UIController.keyOpened[doorNumber] = true;
                    announcement = true;    // アナウンス中フラグ

                    SaveDataManager.SaveGameData(); // 鍵を開けたらオートセーブ
                }
                // 未開錠で鍵も持っていない場合
                else
                {
                    // 鍵の音を鳴らす
                    SoundManager.currentSoundManager.PlaySE(SEType.DoorClosed);
                    messageText.text = "鍵が足りません！";
                    announcement = true;    // アナウンス中
                }
            }
            // すでにアナウンス中なら announcement == true
            else
            {
                Time.timeScale = 1;     // ゲーム進行を戻す
                string s = "";
                if (!opened)
                {
                    s = "(ロック)";
                }
                messageText.text = sceneName + s;
                announcement = false;   // アナウンス中フラグを解除
            }

            //連続入力にならないように一度リセット　※次にボタンが押されるまではfalse
            worldPlayerCnt.IsActionButtonPressed = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 接触判定をtrueにしてパネルを表示
            isPlayerTouch = true;
            talkPanel.SetActive(true);
            // パネルのメッセージに行き先となるシーン名を表示
            // 未開錠の場合は蛇足で(ロック)と書き加える
            string s = "";
            if (!opened)
            {
                s = "(ロック)";
            }
            messageText.text = sceneName + s;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 接触判定をfalseに戻してパネルを非表示にする
            isPlayerTouch = false;
            if (messageText != null) // NullReferenceExceptionを防ぐ
            {
                talkPanel.SetActive(false);
                Time.timeScale = 1f; // ゲーム進行を再開
            }
        }
    }
}

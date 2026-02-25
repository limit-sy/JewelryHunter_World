using UnityEngine;
using UnityEngine.SceneManagement;  // シーンの切り替えに必要
using UnityEngine.InputSystem;
using UnityEngine.UI;     // InputSystem を使うのに必要

public class TitleManager : MonoBehaviour
{
    public string sceneName;    // 読み込むシーン名

    public GameObject startButton;  // スタートボタンオブジェクト
    public GameObject continueButton;   // コンティニューボタンオブジェクト

    //public InputAction submitAction;    // 決定の Input Action
    //void OnEnable()
    //{
    //    submitAction.Enable();  // Input Action を有効化
    //}
    //void OnDisable()
    //{
    //    submitAction.Disable(); // Input Action を無効化
    //}
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // InputSystem_ActionsでUIマップのSubmitアクションが押されたとき
    void OnSubmit(InputValue value)
    {
        Load();
    }

    void Start()
    {
        // PlayerPrefsからJSON文字列をロード
        string jsonData = PlayerPrefs.GetString("SaveData");

        // JSONデータが存在しない場合、エラーを回避し処理を中断
        if (string.IsNullOrEmpty(jsonData))
        {
            continueButton.GetComponent<Button>().interactable = false; // ボタン機能を無効
        }

        SoundManager.currentSoundManager.StopBGM(); // BGMをストップ
        SoundManager.currentSoundManager.PlayBGM(BGMType.Title);    // タイトルのBGMを再生
    }

    // Update is called once per frame
    void Update()
    {
        // Inspector 上で登録したキーのいずれかが押されていたら成立
        //if (submitAction.WasPressedThisFrame())
        //{
        //    Load();
        //}
        // 列挙型のKeyboard型の値を変数kbに代入
        //Keyboard kb = Keyboard.current;
        // キーボードがつながっていれば
        //if (kb != null)
        //{
        //    // エンターキーが押された状態なら
        //    if (kb.enterKey.wasPressedThisFrame)
        //    {
        //        Load();
        //    }
        //}
    }

    // シーンを読み込む
    public void Load()
    {
        SaveDataManager.Initialize();
        //GameManager.totalScore = 0; // 新しくゲームを始めるにあたってスコアをリセット
        SceneManager.LoadScene(sceneName);
    }

    // セーブデータを読み込んでから始める
    public void ContinueLoad()
    {
        SaveDataManager.LoadGameData(); // セーブデータを読み込む
        SceneManager.LoadScene(sceneName);
    }
}

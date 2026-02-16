using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Rigidbody2D rbody;              // Rigidbody2D 型の変数
    float axisH = 0.0f;             // 入力
    public float speed = 3.0f;      // 移動速度
    public float jump = 9.0f;       // ジャンプ力
    public LayerMask groundLayer;   // 着地できるレイヤー
    bool goJump = false;            // ジャンプ開始フラグ
    bool onGround = false;          // 地面フラグ

    // アニメーション対応
    Animator animator; // アニメーター

    //値はあくまでアニメーションクリップ名
    public string stopAnime = "Idle";
    public string moveAnime = "Run";
    public string jumpAnime = "Jump";
    public string goalAnime = "Goal";
    public string deadAnime = "Dead";
    string nowAnime = "";
    string oldAnime = "";

    public int score = 0; // スコア

    InputAction moveAction; // Moveアクション
    InputAction jumpAction; // Jumpアクション
    PlayerInput input;      // PlayerInputコンポーネント

    GameManager gm; // GameManagerスクリプト

    void OnMove(InputValue value)
    {
        // 取得した情報をVector2形式で抽出
        Vector2 moveInput = value.Get<Vector2>();
        axisH = moveInput.x;    // そのX成分をaxisHに代入
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
            {
                goJump = true;
            }
    }

    void Start()
    {
        // Rigidbody2D を取ってくる
        rbody = this.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();        // Animator を取ってくる
        nowAnime = stopAnime;                       // 停止から開始する
        oldAnime = stopAnime;                       // 停止から開始する

        input = GetComponent<PlayerInput>();    // PlayerInputコンポーネントの追加
        moveAction = input.currentActionMap.FindAction("Move");     // Moveアクションを取得
        jumpAction = input.currentActionMap.FindAction("Jump");     // Jumpアクションを取得
        InputActionMap uiMap = input.actions.FindActionMap("UI");   // UIマップ取得
        uiMap.Disable();                                            // UIマップ無効化

        // GameObject型のアタッチされている特定のコンポーネントを探してくるメソッド
        gm = GameObject.FindFirstObjectByType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState != GameState.InGame)
        {
            return; // Updateを中断
        }

        // 地上判定
        onGround = Physics2D.CircleCast(transform.position, // 発射位置
                                        0.2f,               // 円の半径
                                        Vector2.down,       // 発射方向
                                        0.0f,               // 発射距離
                                        groundLayer);       // 検出するレイヤー
        //if (Input.GetButtonDown("Jump"))    // キャラクターをジャンプさせる
        //{
        //    goJump = true;  // ジャンプフラグを立てる
        //}

        // InputActionのPlayerマップの"Jump"アクションに登録されたボタンが押されたか
        //if (jumpAction.WasPressedThisFrame())
        //{
        //    goJump = true;
        //}

        // 水平方向の入力をチェックする
        //axisH = Input.GetAxisRaw("Horizontal");
        // InputActionのPlayerマップの"Move"アクションに登録されたボタンをVector2形式で読み取り、そのうちのX成分をaxisHに代入
        axisH = moveAction.ReadValue<Vector2>().x;

        // 向きの調整
        if (axisH > 0.0f)
        {
            // 右移動
            // Debug.Log("右移動");
            transform.localScale = new Vector2(1, 1);
        }
        else if (axisH < 0.0f)
        {
            // 左移動
            // Debug.Log("左移動");
            transform.localScale = new Vector2(-1, 1);  // 左右反転させる
        }

        // アニメーション更新
        if (onGround)       // 地面の上
        {
            if (axisH == 0)
            {
                nowAnime = stopAnime; // 停止中
            }
            else
            {
                nowAnime = moveAnime; // 移動
            }
        }
        else                // 空中
        {
            nowAnime = jumpAnime;
        }
        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime); // アニメーション再生
        }
    }

    void FixedUpdate()
    {
        if (GameManager.gameState != GameState.InGame)
        {
            return;     // FixedUpdateを中断
        }
        //if (onGround || axisH != 0) // 地面の上 or 速度が 0 ではない
        //{
            // 速度を更新する
            rbody.linearVelocity = new Vector2(axisH * speed, rbody.linearVelocity.y);
        //}
        if (onGround && goJump)     // 地面の上でジャンプキーが押された
        {
            // ジャンプさせる
            Vector2 jumpPw = new Vector2(0, jump);          // ジャンプさせるベクトルを作る
            rbody.AddForce(jumpPw, ForceMode2D.Impulse);    // 瞬間的な力を加える
            goJump = false;                                 // ジャンプフラグを下ろす
        }
    }

    // 接触開始
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Goal();    // ゴール！！
        }
        else if (collision.gameObject.tag == "Dead")
        {
            GameOver();     // ゲームオーバー
        }
        else if (collision.gameObject.tag == "ScoreItem")
        {
            // スコアアイテム
            ScoreItem item = collision.gameObject.GetComponent<ScoreItem>();  // ScoreItemを得る			
            score = item.itemdata.value;                // スコアを得る
            UIController ui = Object.FindFirstObjectByType<UIController>();      // UIControllerを探す
            if (ui != null)
            {
                ui.UpdateScore(score);                  // スコア表示を更新する
            }
            score = 0; //次に備えてスコアをリセット
            Destroy(collision.gameObject);              // アイテム削除する
        }
    }

    // ゴール
    public void Goal()
    {
        animator.Play(goalAnime);
        GameManager.gameState = GameState.GameClear;    // ステータス切り替え
        GameStop();     // プレイヤーのVelocityをストップ
    }
    // ゲームオーバー
    public void GameOver()
    {
        animator.Play(deadAnime);
        GameManager.gameState = GameState.GameOver;
        GameStop();             // ゲーム停止

        // ゲームオーバー演出
        GetComponent<CapsuleCollider2D>().enabled = false;      // 当たりを消す
        rbody.AddForce(new Vector2(0, 5), ForceMode2D.Impulse); // 上に少し跳ね上げる

        Destroy(gameObject, 2.0f); // 2秒後にヒエラルキーからオブジェクトを抹消
    }

    // プレイヤー停止
    void GameStop()
    {
        rbody.linearVelocity = new Vector2(0, 0);           // 速度を0にして強制停止

        input.currentActionMap.Disable();   // いったんPlayerマップを無効化
        input.SwitchCurrentActionMap("UI"); // アクションマップをUIマップに切り替え
        input.currentActionMap.Enable();    // UIマップを有効化
    }

    // UI表示時にSubmitボタンが押されたら
    void OnSubmit(InputValue value)
    {
        // もしゲーム中でなければ
        if (GameManager.gameState != GameState.InGame)
        {
            // GameManagerスクリプトのGameEndメソッドの発動
            gm.GameEnd();
        }
    }

    //プレイヤーのaxisH()の値を取得
    public float GetAxisH()
    {
        return axisH;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
}

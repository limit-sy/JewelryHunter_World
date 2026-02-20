using UnityEngine;
using UnityEngine.SceneManagement;

// どのアイテムタイプか
public enum AdventItemType
{
    None,
    Arrow,
    Key,
    Life
}

public class Advent_Item : MonoBehaviour
{
    // オブジェクトのアイテムタイプ
    public AdventItemType type = AdventItemType.None;
    // タイプがArrowだった場合の増加量
    public int numberOfArrow = 10;
    // タイプがLifeだった場合の回復量
    public int recoveryValue = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Playerタグと接触したらタイプごとの処理
        if (collision.gameObject.tag == "Player")
        {
            // 自分のタイプが対象
            switch (type)
            {
                case AdventItemType.Key:
                    GameManager.keys++;
                    // keyGotに登録
                    GameManager.keyGot[SceneManager.GetActiveScene().name] = true;
                    break;
                case AdventItemType.Arrow:
                    // staticな矢を変数分だけ増加
                    GameManager.arrows += numberOfArrow;
                    break;
                case AdventItemType.Life:
                    PlayerController.PlayerRecovery(recoveryValue);
                    break;
                default:
                    break;
            }

            // アイテムゲット演出
            GetComponent<CircleCollider2D>().enabled = false;      // 当たりを消す
            Rigidbody2D rbody = GetComponent<Rigidbody2D>();
            rbody.gravityScale = 1.0f; //重力を戻す
            rbody.AddForce(new Vector2(0, 3), ForceMode2D.Impulse); // 上に少し跳ね上げる
            Destroy(gameObject, 0.5f); // 1秒後にヒエラルキーからオブジェクトを抹消
        }
    }
}

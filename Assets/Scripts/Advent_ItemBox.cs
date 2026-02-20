using UnityEngine;
using UnityEngine.SceneManagement;

public class Advent_ItemBox : MonoBehaviour
{
    public Sprite openImage;    // 開いたときの画像
    public GameObject itemPrefab;   // 宝箱の中に格納するオブジェクト
    public bool isClosed = true;    // 閉じているかのフラグ
    public AdventItemType type = AdventItemType.None;   // 宝箱のタイプ

    // Start is called once before the first execution of update after the MonoBehaviour is created
    void Start()
    {
        // もしも自分のタイプがkeyだったら
        if (type == AdventItemType .Key) {
            // もしもGameManagerのkeyGotディクショナリーの該当シーンの記録がtrueだったら※取得済み
            if (GameManager.keyGot[SceneManager.GetActiveScene().name]) {
                // close状態をfalse
                isClosed = false;
                // 見た目をオープンの絵にすること
                GetComponent<SpriteRenderer>().sprite = openImage;
            }
        }
    }

    // 宝箱がcloseの時、触れた相手がPlayerなら
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isClosed && collision.gameObject.tag == "Player")
        {
            // 宝箱の絵をOpenに変更
            GetComponent<SpriteRenderer>().sprite = openImage;
            // closeのフラグを解除
            isClosed = false;
            // その場に変数に指定したプレハブオブジェクトを生成
            //(もし変数にプレハブオブジェクトが指定されていれば)
            if (itemPrefab != null) {
                Instantiate(
                    itemPrefab,             // 生成する対象オブジェクト
                    transform.position,     // 生成位置
                    Quaternion.identity     // 生成時の回転 (※今回は無回転)
                    );
            }
        }
    }
}

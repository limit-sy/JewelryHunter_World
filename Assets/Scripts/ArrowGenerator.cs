using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    GameObject[] objects;   // ItemBoxタグが付いているオブジェクトを格納
    GameObject player;
    public Sprite itemBoxClose; // 指定したオブジェクトの絵を閉じた絵にする
    public Sprite itemBoxOpen;  // 指定したオブジェクトの絵を開けた絵にする

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ItemBoxタグのオブジェクトを複数取得
        objects = GameObject.FindGameObjectsWithTag("ItemBox");
        // 拾ってきたItemBoxを全部オープン済みにする
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<SpriteRenderer>().sprite = itemBoxOpen;
            objects[i].GetComponent<Advent_ItemBox>().isClosed = false;
        }
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Playerが存在している。かつ、矢の残数が0
        if(player != null && GameManager.arrows <= 0)
        {
            // 全ItemBoxのうち、ひとつでもClose状態(アイテムが未取得)なら何もしない
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].GetComponent<Advent_ItemBox>().isClosed)
                {
                    return;
                }
            }

            // どのItemBoxを復活させるのか数字をランダムに選ぶ
            int index = Random.Range(0, objects.Length);    // 0番以上 objects配列の数未満 のランダム
            objects[index].GetComponent<Advent_ItemBox>().isClosed = true;  // Close(アイテム未取得)の復活
            objects[index].GetComponent<SpriteRenderer>().sprite = itemBoxClose;    // 絵を閉じた絵に変更
        }
    }
}

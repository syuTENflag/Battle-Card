using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理用の名前空間を追加

public class Start : MonoBehaviour
{
    // 移動したいシーンの名前
    public string Scenename;

    // ボタンがクリックされたときに呼ばれる関数
    public void ChangeScene()
    {
        SceneManager.LoadScene(Scenename);
    }
}

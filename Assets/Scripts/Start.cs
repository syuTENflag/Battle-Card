using UnityEngine;
using UnityEngine.SceneManagement; // �V�[���Ǘ��p�̖��O��Ԃ�ǉ�

public class Start : MonoBehaviour
{
    // �ړ��������V�[���̖��O
    public string Scenename;

    // �{�^�����N���b�N���ꂽ�Ƃ��ɌĂ΂��֐�
    public void ChangeScene()
    {
        SceneManager.LoadScene(Scenename);
    }
}

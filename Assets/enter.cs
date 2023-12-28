using UnityEngine;
using UnityEngine.UI;

public class ButtonEnterTrigger : MonoBehaviour
{
    public Button Button; // �� Unity ��݋����ָ����İ��o

    void Update()
    {
        // ������� Enter �I
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // ���а��o�c������
            Button.onClick.Invoke();
        }
    }
}

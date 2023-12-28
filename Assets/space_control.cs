using UnityEngine;
using UnityEngine.UI;

public class ButtonSpaceTrigger : MonoBehaviour
{
    public Button Button; // �� Unity ��݋����ָ����İ��o

    void Update()
    {
        // ������¿հ��I
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ���а��o�c������
            Button.onClick.Invoke();
        }
    }
}

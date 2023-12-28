using UnityEngine;
using UnityEngine.UI;

public class ButtonSpaceTrigger : MonoBehaviour
{
    public Button Button; // 在 Unity 編輯器中指定你的按鈕

    void Update()
    {
        // 如果按下空白鍵
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 呼叫按鈕點擊函數
            Button.onClick.Invoke();
        }
    }
}

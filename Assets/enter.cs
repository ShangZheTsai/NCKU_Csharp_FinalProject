using UnityEngine;
using UnityEngine.UI;

public class ButtonEnterTrigger : MonoBehaviour
{
    public Button Button; // 在 Unity 編輯器中指定你的按鈕

    void Update()
    {
        // 如果按下 Enter 鍵
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // 呼叫按鈕點擊函數
            Button.onClick.Invoke();
        }
    }
}

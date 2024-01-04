using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using UnityEngine;

public class LLM : MonoBehaviour
{
    /// <summary>      
    /// api地址
    /// </summary>
    [SerializeField] protected string url;
    /// <summary>
    /// 提示詞，與使用者的輸入一起發送
    /// </summary>
    [Header("發送的提示詞設定")]
    [SerializeField] protected string m_Prompt = string.Empty;      //string.Empty等同於""，但string.Empty更能提高代碼的可讀性
    /// <summary>
    /// 語言
    /// </summary
    [Header("設置回覆的語言")]
    [SerializeField] public string lan = "繁體中文";
    /// <summary>
    /// 上下文保留條數
    /// </summary>
    [Header("上下文保留條數")]
    [SerializeField] protected int m_HistoryKeepCount = 15;
    /// <summary>
    /// 缓存對話
    /// 用來傳入給AI看上下文，在ChatSample的m_ChatHistory才是聊天紀錄
    /// SendData的訊息包含角色和輸入的訊息
    /// </summary>
    [SerializeField] public List<SendData> m_DataList = new List<SendData>();
    /// <summary>
    /// 計算方法調用的時間
    /// 之後繼承會調用
    /// </summary>
    [SerializeField] protected Stopwatch stopwatch = new Stopwatch();
    /// <summary>
    /// 發送訊息
    /// 使用virtual讓之後繼承LLM這個類別的模型可以override
    /// Action<T>是一種內建的委託類型。當你看到Action<string>，這意味著它是一個引用一個方法的委託，這個方法接受一個string類型的參數並且沒有回傳值。委託本質上是對方法的引用。它允許你將方法作為參數傳遞，就是等同於傳入一個帶string參數方法，然後在這個函式裡面為callback取代之，
    /// </summary>
    public virtual void PostMsg(string _msg, Action<string> _callback)
    {
        //上下文條數設置
        CheckHistory();
        //提示詞處理
        string message = //"當前為角色的人物設定：" + m_Prompt +
                                _msg + " 請用：" + lan + "並回覆的字數請限制在60個字左右";

        //缓存發送的訊息列表
        m_DataList.Add(new SendData("user", message));

        //開始一個協程，用於執行Request方法(下面)。這個方法似乎是用來處理非同步網路請求或某種非同步操作
        StartCoroutine(Request(message, _callback));
    }

    //給繼承LLM的那些模型覆寫他們要怎麼傳入AI端，postWord是包含自己內部設定的訊息(語言設定和角色設定那些)
    public virtual IEnumerator Request(string _postWord, Action<string> _callback)
    {
        //前面的操作都結束後，才callback
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// 設置保留的上下文條數，防止太長
    /// </summary>
    public virtual void CheckHistory()
    {
        if (m_DataList.Count > m_HistoryKeepCount)
        {
            m_DataList.RemoveAt(0);
        }
    }

    [Serializable]
    public class SendData
    {
        [SerializeField] public string role;
        [SerializeField] public string content;

        //無參數的建構函式
        public SendData()
        {

        }

        //有參數的建構函式
        public SendData(string _role, string _content)
        {
            role = _role;
            content = _content;
        }

    }

}

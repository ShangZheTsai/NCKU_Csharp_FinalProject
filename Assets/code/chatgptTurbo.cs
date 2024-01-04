using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class chatgptTurbo : LLM
{
    public chatgptTurbo()
    {
        url = "https://api.openai.com/v1/chat/completions";
    }

    /// <summary>
    /// api key
    /// </summary>
    [SerializeField] private string api_key;
    /// <summary>
    /// AI設定
    /// </summary>
    public string m_SystemSetting = "";
    /// <summary>
    /// gpt-3.5-turbo
    /// </summary>
    public string m_gptModel = "gpt-3.5-turbo";

    private void Start()
    {
        //運行時，添加AI設定
        m_DataList.Add(new SendData("system", m_SystemSetting));
    }

    /// <summary>
    /// 發送消息
    /// </summary>
    /// <returns></returns>
    public override void PostMsg(string _msg, Action<string> _callback)
    {
        base.PostMsg(_msg, _callback);
    }

    /// <summary>
    /// 調用接口
    /// </summary>
    /// <param name="_postWord"></param>
    /// <param name="_callback"></param>
    /// <returns></returns>
    public override IEnumerator Request(string _postWord, System.Action<string> _callback)
    {
        stopwatch.Restart();
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))      //這裡創建了一個UnityWebRequest物件用於發送POST請求。using語句確保請求物件在使用完畢後正確地被釋放。
        {
            PostData _postData = new PostData           //PostDat在再下面有定義class
            {
                model = m_gptModel,
                messages = m_DataList,
    
            };

            string _jsonText = JsonUtility.ToJson(_postData).Trim();                    //將_postData物件轉換成JSON格式的字串，並去除字串兩端的空白。JSON（JavaScript Object Notation）是一種輕量級的資料交換格式，它是語言無關的，這意味著幾乎所有程式語言都可以解析和產生JSON資料。在Web應用和API（特別是RESTful API）中，JSON是發送和接收資料的主流格式。它使得資料在客戶端和伺服器之間的傳輸變得簡單和一致。
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);        //將JSON字串轉換成UTF-8格式的位元組數組。
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);      //設定請求的uploadHandler為UploadHandlerRaw的實例，用於上傳位元組數組
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();     //設定請求的downloadHandler為DownloadHandlerBuffer的實例，用於接收伺服器的回應資料。

            request.SetRequestHeader("Content-Type", "application/json");               //為請求新增HTTP頭部，指定內容類型為JSON。
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", api_key));        //增加授權頭部，使用Bearer令牌進行認證。

            yield return request.SendWebRequest();  //發送請求並等待回應。這是協程的掛起點。

            if (request.responseCode == 200)    //檢查回應代碼是否為200，即請求成功。
            {
                string _msgBack = request.downloadHandler.text;     //取得伺服器的回應文字。
                MessageBack _textback = JsonUtility.FromJson<MessageBack>(_msgBack);        //將回應文字從JSON格式解析為MessageBack類型的物件。
                if (_textback != null && _textback.choices.Count > 0)   //檢查解析後的物件是否非空且其choices屬性包含至少一個元素。
                {

                    string _backMsg = _textback.choices[0].message.content;
                    //添加紀錄
                    m_DataList.Add(new SendData("assistant", _backMsg));
                    _callback(_backMsg);        //收到訊息後處理輸入在文字欄上的文字
                }

            }
            else
            {
                string _msgBack = request.downloadHandler.text;
                Debug.LogError(_msgBack);
            }

            stopwatch.Stop();
            Debug.Log("chatgpt耗時："+ stopwatch.Elapsed.TotalSeconds);
        }
    }
    #region 數據包

    [Serializable]
    public class PostData
    {
        [SerializeField]public string model;
        [SerializeField] public List<SendData> messages;
        [SerializeField] public float temperature = 0.7f;
    }

    [Serializable]
    public class MessageBack
    {
        public string id;
        public string created;
        public string model;
        public List<MessageBody> choices;
    }
    [Serializable]
    public class MessageBody
    {
        public Message message;
        public string finish_reason;
        public string index;
    }
    [Serializable]
    public class Message
    {
        public string role;
        public string content ;
    }

    #endregion






    //切換角色
    public void Change_Character_button1(GameObject _settingPanel)
    {
        m_Prompt = "";
        lan = "繁體中文回答";
        _settingPanel.SetActive(false);
    }

    public void Change_Character_button2(GameObject _settingPanel)
    {
        m_Prompt = "";
        lan = "英文回答";
        _settingPanel.SetActive(false);
    }

}

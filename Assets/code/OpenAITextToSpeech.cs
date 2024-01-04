using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LLM;
using UnityEngine.Networking;

public class OpenAITextToSpeech : TTS
{
    #region 参数定义

    [SerializeField] private string api_key=string.Empty;   //apikey
    [SerializeField] private ModelType m_ModelType = ModelType.tts_1;   //模型
    [SerializeField] private VoiceType m_Voice = VoiceType.onyx;    //聲音

    #endregion
    private void Awake()
    {
        m_PostURL = "https://api.openai.com/v1/audio/speech";
    }

    /// <summary>
    /// 語音合成，返回合成文本
    /// 在ChatSample中的callback將AI回傳的訊息與將PlayVoice這個方法傳入到這個Speak
    /// </summary>
    /// <param name="_msg"></param>
    /// <param name="_callback"></param>
    public override void Speak(string _msg, Action<AudioClip, string> _callback) //Action是C# 中的一個內建委託類型，就是方法中的宣告，像是整數前面要加int那樣。它用於封裝一個方法，該方法沒有傳回值（即void 類型）。<AudioClip, string>指的是這個Action委託可以封裝的方法的參數型別。在這個例子中，它表示這個Action可以封裝一個接受一個AudioClip和一個string作為參數的方法。
    {
        StartCoroutine(GetVoice(_msg, _callback));          //開啟協程，傳入AI回覆的訊息和callback方法


        /*      此為傳入的方法
         
        private void PlayVoice(AudioClip _clip, string _response)
        {
            m_AudioSource.clip = _clip;
            m_AudioSource.Play();
            Debug.Log("音頻時長：" + _clip.length);
            //開始逐個顯示返回的文本
            StartTypeWords(_response);
            //切換到說話動作
            SetAnimator("state", 2);
            SetAnimator2("state", 2);
        }

        */

    }

    private IEnumerator GetVoice(string _msg, Action<AudioClip, string> _callback)
    {
        stopwatch.Restart();
        using (UnityWebRequest request = UnityWebRequest.Post(m_PostURL, new WWWForm()))    //建立一個UnityWebRequest物件用於發送POST請求。這裡使用using語句確保請求結束後資源被正確釋放。
        {
            PostData _postData = new PostData   //建立並初始化一個PostData類型的物件_postData。此物件包含模型名稱、輸入訊息和聲音類型。
            {
                model = m_ModelType.ToString().Replace('_','-'),    //模型名稱
                input = _msg,       //要轉換的訊息
                voice= m_Voice.ToString()       //模型中可以選的聲音
            };

            string _jsonText = JsonUtility.ToJson(_postData).Trim();    //將_postData物件轉換成JSON格式的字串並移除兩端的空白字元。
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);    //將JSON字串轉換為UTF-8編碼的位元組數組。
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);  //為請求設定上傳處理程序，用於上傳先前準備的位元組資料。
            request.downloadHandler = new DownloadHandlerAudioClip(m_PostURL, AudioType.MPEG);  //為請求設定下載處理程序，這裡是用於下載音訊剪輯。

            request.SetRequestHeader("Content-Type", "application/json");   //設定HTTP請求頭，聲明發送的內容類型為JSON。
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", api_key));    //設定授權頭部，使用Bearer令牌認證。

            yield return request.SendWebRequest();  //發送請求並暫停協程，直到網路請求完成。

            if (request.responseCode == 200)    //檢查HTTP回應碼是否為200，即請求成功。
            {
                AudioClip audioClip = ((DownloadHandlerAudioClip)request.downloadHandler).audioClip;    //從下載處理程序中取得AudioClip物件。
                _callback(audioClip, _msg);

                /*      此為傳入的方法(在這個函式裡面為callback

                //此用來開始播放從openai端回傳回來的語音
                private void PlayVoice(AudioClip _clip, string _response)
                {
                    m_AudioSource.clip = _clip;     //AudioSource組件的clip屬性用於指定要播放的音訊
                    m_AudioSource.Play();            //開始播放
                    Debug.Log("音頻時長：" + _clip.length);
                    //開始逐個顯示返回的文本
                    StartTypeWords(_response);
                    //切換到說話動作
                    SetAnimator("state", 2);
                    SetAnimator2("state", 2);
                }

                */

            }
            else
            {
                Debug.LogError("語音合成失敗: " + request.error);
            }

            stopwatch.Stop();
            Debug.Log("openAI語音合成：" + stopwatch.Elapsed.TotalSeconds);
        }
    }

    #region 数据定义

    /// <summary>
    /// 发送的报文
    /// </summary>
    [Serializable]
    public class PostData
    {
        public string model = string.Empty;//模型名稱
        public string input = string.Empty;//文本内容
        public string voice = string.Empty;//聲音
    }
    /// <summary>
    /// 模型類型
    /// </summary>
    public enum ModelType
    {
        tts_1,
        tts_1_hd
    }
    /// <summary>
    /// 聲音類型
    /// </summary>
    public enum VoiceType
    {
        alloy,
        echo,
        fable,
        onyx,
        nova,
        shimmer
    }

    #endregion


    public void Change_voice_機器人()
    {
        m_Voice = VoiceType.alloy;
    }

    public void Change_voice_女生()
    {
        m_Voice = VoiceType.nova;
    }
}

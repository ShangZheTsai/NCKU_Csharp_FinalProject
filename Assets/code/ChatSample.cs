using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WebGLSupport;

public class ChatSample : MonoBehaviour
{
    //使用///這個就可以註解很多行並可以摺疊，<summary>和</summary>之間是對這個方法的描述，下面還可以再輸入，只要添加///就好
    /// <summary>
    /// 聊天配置
    /// </summary>
    [SerializeField] private ChatSetting m_ChatSettings;        //前面加m_表示為class的變數，而不是在method中的變數，用以區分是class的成員變數還是局部變數
    #region UI定義         //在#region和#endregion區間的程式碼可以用隱藏選單選擇隱藏和顯示
    /// <summary>
    /// 聊天UI窗
    /// </summary>
    [SerializeField] private GameObject m_ChatPanel;
    /// <summary>
    /// 輸入的訊息
    /// </summary>
    [SerializeField] public InputField m_InputWord;
    /// <summary>
    /// 返回的訊息
    /// </summary>
    [SerializeField] private Text m_TextBack;
    /// <summary>
    /// 播放聲音
    /// </summary>
    [SerializeField] private AudioSource m_AudioSource;
    /// <summary>
    /// 發送訊息的按鈕
    /// </summary>
    [SerializeField] private Button m_CommitMsgBtn;

    #endregion

    #region 參數定義
    /// <summary>
    /// 動畫控制器
    /// </summary>
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Animator m_Animator2;
    /// <summary>
    /// 語音模式，設置為false,則不通過語音合成
    /// </summary>
    [Header("設置是否通過語音合成播放文本")]
    [SerializeField] private bool m_IsVoiceMode = true;

    #endregion

    //Awake是一個特殊的方法，它是Unity的生命週期方法之一。這個方法不需要手動呼叫。Unity引擎會在腳本實例化時自動呼叫Awake方法
    private void Awake()
    {
        m_CommitMsgBtn.onClick.AddListener(delegate { SendData(); });       //這行程式碼為名為m_CommitMsgBtn的按鈕新增了一個點擊事件監聽器。當按鈕被點擊時，它會自動呼叫SendData方法。這裡使用了匿名委託（delegate），它允許你在不創建新方法的情況下直接定義要執行的操作。在這個例子中，當按鈕被點擊時，SendData方法將被執行
        RegistButtonEvent();        //把語音的按鈕添加事件
        InputSettingWhenWebgl();
    }

    #region 消息发送

    /// <summary>
    /// webgl時處理，支持中文输入
    /// </summary>
    private void InputSettingWhenWebgl()
    {
#if UNITY_WEBGL
        m_InputWord.gameObject.AddComponent<WebGLSupport.WebGLInput>();
#endif
    }


    /// <summary>
    /// 整理輸入欄的發送訊息，傳送到Chatgpt
    /// 這裡指介面的那個輸入欄位，文字輸入的
    /// 這裡為按下提交時會做的
    /// </summary>
    public void SendData()
    {
        // 如果輸入欄為空則不做事
        if (m_InputWord.text.Equals(""))
            return;

        //輸入欄有字，則添加紀錄聊天
        m_ChatHistory.Add(m_InputWord.text);

        //_msg為輸入欄的文字
        string _msg = m_InputWord.text ;

        //發送數據給在上面的m_ChatSettings中的m_ChatModel裡面的PostMsg方法，也就是語言模型的傳送給AI那
        m_ChatSettings.m_ChatModel.PostMsg(_msg, CallBack);

        //清空輸入欄
        m_InputWord.text = "";
        m_TextBack.text = "正在思考中...";

        //人物切換思考動作
        SetAnimator("state", 1);
        SetAnimator2("state", 1);
    }
    /// <summary>
    /// 還不知道這要幹嘛
    /// </summary>
    /// <param name="_postWord"></param>
    public void SendData(string _postWord)
    {
        if (_postWord.Equals(""))
            return;


        m_ChatHistory.Add(_postWord);

        string _msg = _postWord;

        m_ChatSettings.m_ChatModel.PostMsg(_msg, CallBack);

        m_InputWord.text = "";
        m_TextBack.text = "正在思考中...";

        //切换思考動作
        SetAnimator("state", 1);
        SetAnimator2("state", 1);
    }

    /// <summary>
    /// AI回覆的訊息的回調,CallBack
    /// </summary>
    /// <param name="_response"></param>
    private void CallBack(string _response)
    {
        _response = _response.Trim();
        //AI回覆的那個文字格清空
        m_TextBack.text = "";

        
        Debug.Log("收到AI回复："+ _response);

        //紀錄聊天
        m_ChatHistory.Add(_response);

        //如果選擇不用語音回覆或者文字轉語音沒有放入模型，則直接將回傳的文字顯示在文字欄上，就是為一般的文字回覆
        if (!m_IsVoiceMode||m_ChatSettings.m_TextToSpeech == null)
        {
            //開始逐個顯示返回的文本
            StartTypeWords(_response);
            return;
        }

        //發送數據給在上面的m_ChatSettings中的m_TextToSpeech裡面的Speak方法，也就是把文字轉語音的模型
        m_ChatSettings.m_TextToSpeech.Speak(_response, PlayVoice);
    }

#endregion

#region 語音輸入
    /// <summary>
    /// 語音辨識返回的文本是否直接發送至LLM
    /// </summary>
    [SerializeField] private bool m_AutoSend = true;
    /// <summary>
    /// 語音输入的按鈕
    /// </summary>
    [SerializeField] private Button m_VoiceInputBotton;
    /// <summary>
    /// 錄音按鈕的文本
    /// </summary>
    [SerializeField]private Text m_VoiceBottonText;
    /// <summary>
    /// 錄音的提示信息
    /// </summary>
    [SerializeField] private Text m_RecordTips;
    /// <summary>
    /// 語音輸入處理類
    /// </summary>
    [SerializeField] private VoiceInputs m_VoiceInputs;
    /// <summary>
    /// 注册按鈕事件
    /// </summary>
    private void RegistButtonEvent()
    {
        //如果語音按鈕不是空且沒有 EventTrigger 元件，則程式碼會繼續執行後續的操作，因為如果按鈕物件為空或者已經有了 EventTrigger 元件（表示已經設置了事件觸發），就不需要再進行按鈕事件的註冊，因此直接返回，避免重複設置按鈕事件
        if (m_VoiceInputBotton == null || m_VoiceInputBotton.GetComponent<EventTrigger>())
            return;

        EventTrigger _trigger = m_VoiceInputBotton.gameObject.AddComponent<EventTrigger>();

        //添加按鈕按下的事件
        EventTrigger.Entry _pointDown_entry = new EventTrigger.Entry(); //這一行建立了一個EventTrigger.Entry的新實例。EventTrigger.Entry是Unity中用來定義特定事件及其回呼的類別。變數_pointDown_entry被用來儲存這個新建立的實例
        _pointDown_entry.eventID = EventTriggerType.PointerDown;        //這行程式碼設定_pointDown_entry的eventID屬性。EventTriggerType.PointerDown是一個枚舉值，表示一個指標（例如滑鼠遊標或觸控螢幕上的手指）按下的事件。這表示此條目將用於當使用者按下指標時觸發
        _pointDown_entry.callback = new EventTrigger.TriggerEvent();     //這行程式碼初始化_pointDown_entry的callback屬性。EventTrigger.TriggerEvent是一個Unity中的事件類型，用於註冊和處理事件回調。在這裡，它被用來設定當觸發PointerDown事件時應該呼叫的方法或動作。在這個階段，callback還沒有具體的方法關聯，通常你需要後續添加如_pointDown_entry.callback.AddListener(...)來指定具體的反應函數

        //添加按鈕鬆開事件
        EventTrigger.Entry _pointUp_entry = new EventTrigger.Entry();
        _pointUp_entry.eventID = EventTriggerType.PointerUp;
        _pointUp_entry.callback = new EventTrigger.TriggerEvent();

        //添加委託事件
        _pointDown_entry.callback.AddListener(delegate { StartRecord(); });     //這行程式碼為的_pointDown_entry屬性callback增加了一個匿名委託（delegate）。當與_pointDown_entry相關聯的事件（如先前設定的EventTriggerType.PointerDown，即指標按下事件）被觸發時，這個匿名委託會自動執行
        _pointUp_entry.callback.AddListener(delegate { StopRecord(); });

        _trigger.triggers.Add(_pointDown_entry);    //这行代码将_pointDown_entry对象添加到_trigger对象的triggers列表中。_trigger应该是一个EventTrigger组件的实例，而triggers是一个包含所有事件触发器条目（EventTrigger.Entry对象）的列表。添加_pointDown_entry到triggers列表意味着当与此条目相关联的事件（如之前设置的EventTriggerType.PointerDown）发生时，_trigger组件会处理它并调用相应的回调函数（在这个例子中是StartRecord）
        _trigger.triggers.Add(_pointUp_entry);
    }

    /// <summary>
    /// 開始錄製
    /// </summary>
    public void StartRecord()
    {
        m_VoiceBottonText.text = "正在錄音中..."; 
        m_VoiceInputs.StartRecordAudio();
    }
    /// <summary>
    /// 结束錄製
    /// </summary>
    public void StopRecord()
    {
        m_VoiceBottonText.text = "按住按鈕，開始錄音"; 
        m_RecordTips.text = "錄音結束，正在識別...";
        //傳入AcceptClip這個方法進入到StopRecordAudio，然後在StopRecordAudio裡面會把錄好的音檔使用callback帶入執行
        m_VoiceInputs.StopRecordAudio(AcceptClip);
    }

    /// <summary>
    /// 处理录制的音频数据
    /// 目前找不到用途
    /// </summary>
    /// <param name="_data"></param>
    private void AcceptData(byte[] _data)
    {
        if (m_ChatSettings.m_SpeechToText == null)
            return;

        m_ChatSettings.m_SpeechToText.SpeechToText(_data, DealingTextCallback);
    }

    /// <summary>
    /// 處理錄製的音檔數據
    /// </summary>
    /// <param name="_data"></param>
    private void AcceptClip(AudioClip _audioClip)
    {
        if (m_ChatSettings.m_SpeechToText == null)
            return;

        //將錄好的音檔傳入SpeechToText去轉換，帶入音檔和DealingTextCallback這個方法的參數(在下面)
        m_ChatSettings.m_SpeechToText.SpeechToText(_audioClip, DealingTextCallback);
    }
    /// <summary>
    /// 處理辨識到的文本
    /// </summary>
    /// <param name="_msg"></param>
    private void DealingTextCallback(string _msg)
    {
        //m_RecordTips.text語音辨識文字顯示的欄位，將辨識出來的文字顯示在上面
        m_RecordTips.text = _msg;
        StartCoroutine(SetTextVisible(m_RecordTips));
        //是否自動發送
        if (m_AutoSend)
        {
            SendData(_msg);
            return;
        }

        //如果沒有選擇自動發送，則把語音的訊息放在文字欄上
        m_InputWord.text = _msg;
    }

    private IEnumerator SetTextVisible(Text _textbox)
    {
        //讓文字顯示在語音辨識的文字欄上面3秒後再消失
        yield return new WaitForSeconds(3f);
        _textbox.text = "";
    }

#endregion

#region 語音合成

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

#endregion

#region 文字逐個顯示
    //逐字顯示的時間間隔
    [SerializeField] private float m_WordWaitTime = 0.2f;
    //是否顯示完成
    [SerializeField] private bool m_WriteState = false;

    /// <summary>
    ///開始逐個打印
    /// </summary>
    /// <param name="_msg"></param>
    private void StartTypeWords(string _msg)
    {
        if (_msg == "")
            return;

        m_WriteState = true;
        StartCoroutine(SetTextPerWord(_msg));
    }

    //打印
    private IEnumerator SetTextPerWord(string _msg)
    {
        int currentPos = 0;

        //每次迴圈印一個字，直到印完為止
        while (m_WriteState)
        {
            yield return new WaitForSeconds(m_WordWaitTime);
            currentPos++;
            //更新顯示的內容
            m_TextBack.text = _msg.Substring(0, currentPos);

            //沒打印完全部的字就繼續
            m_WriteState = currentPos < _msg.Length;

        }

        //切换到等待動作
        SetAnimator("state",0);
        SetAnimator2("state", 0);
    }

#endregion

#region 聊天紀錄
    //保存聊天紀錄
    [SerializeField] private List<string> m_ChatHistory;
    //缓存已創建的聊天氣泡
    [SerializeField] private List<GameObject> m_TempChatBox;
    //聊天紀錄顯示層
    [SerializeField] private GameObject m_HistoryPanel;
    //聊天文本放置的層
    [SerializeField] private RectTransform m_rootTrans;
    //發送聊天氣泡
    [SerializeField] private ChatPrefab m_PostChatPrefab;
    //回覆的聊天氣泡
    [SerializeField] private ChatPrefab m_RobotChatPrefab;
    //滾動條
    [SerializeField] private ScrollRect m_ScroTectObject;
    //獲取聊天紀錄
    public void OpenAndGetHistory()
    {
        m_ChatPanel.SetActive(false);
        m_HistoryPanel.SetActive(true);

        ClearChatBox();
        StartCoroutine(GetHistoryChatInfo());
    }
    //返回
    public void BackChatMode()
    {
        m_ChatPanel.SetActive(true);
        m_HistoryPanel.SetActive(false);
    }

    //清空已創建的對話框
    private void ClearChatBox()
    {
        while (m_TempChatBox.Count != 0)
        {
            if (m_TempChatBox[0])
            {
                Destroy(m_TempChatBox[0].gameObject);
                m_TempChatBox.RemoveAt(0);
            }
        }
        m_TempChatBox.Clear();
    }

    //獲取聊天紀錄列表
    private IEnumerator GetHistoryChatInfo()
    {

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < m_ChatHistory.Count; i++)
        {
            if (i % 2 == 0)
            {
                ChatPrefab _sendChat = Instantiate(m_PostChatPrefab, m_rootTrans.transform);
                _sendChat.SetText(m_ChatHistory[i]);
                m_TempChatBox.Add(_sendChat.gameObject);
                continue;
            }

            ChatPrefab _reChat = Instantiate(m_RobotChatPrefab, m_rootTrans.transform);
            _reChat.SetText(m_ChatHistory[i]);
            m_TempChatBox.Add(_reChat.gameObject);
        }

        //重新計算容器尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
    }

    private IEnumerator TurnToLastLine()
    {
        yield return new WaitForEndOfFrame();
        //滾動到最近的消息
        m_ScroTectObject.verticalNormalizedPosition = 0;
    }


#endregion

    //切換人物的動畫
    private void SetAnimator(string _para,int _value)
    {
        if (m_Animator == null)
            return;

        m_Animator.SetInteger(_para, _value);
    }

    private void SetAnimator2(string _para, int _value)
    {
        if (m_Animator2 == null)
            return;

        m_Animator2.SetInteger(_para, _value);
    }



}

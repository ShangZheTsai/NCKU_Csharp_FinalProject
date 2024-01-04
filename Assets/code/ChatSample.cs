using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WebGLSupport;

public class ChatSample : MonoBehaviour
{
    //ʹ��///�@���Ϳ����]��ܶ��ЁK����ߡ�B��<summary>��</summary>֮�g�ǌ��@������������������߀������ݔ�룬ֻҪ���///�ͺ�
    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField] private ChatSetting m_ChatSettings;        //ǰ���m_��ʾ��class��׃������������method�е�׃�������ԅ^����class�ĳɆT׃��߀�Ǿֲ�׃��
    #region UI���x         //��#region��#endregion�^�g�ĳ�ʽ�a�������[���x���x���[�غ��@ʾ
    /// <summary>
    /// ����UI��
    /// </summary>
    [SerializeField] private GameObject m_ChatPanel;
    /// <summary>
    /// ݔ���ӍϢ
    /// </summary>
    [SerializeField] public InputField m_InputWord;
    /// <summary>
    /// ���ص�ӍϢ
    /// </summary>
    [SerializeField] private Text m_TextBack;
    /// <summary>
    /// ������
    /// </summary>
    [SerializeField] private AudioSource m_AudioSource;
    /// <summary>
    /// �l��ӍϢ�İ��o
    /// </summary>
    [SerializeField] private Button m_CommitMsgBtn;

    #endregion

    #region �������x
    /// <summary>
    /// �Ӯ�������
    /// </summary>
    [SerializeField] private Animator m_Animator;
    [SerializeField] private Animator m_Animator2;
    /// <summary>
    /// �Z��ģʽ���O�Þ�false,�t��ͨ�^�Z���ϳ�
    /// </summary>
    [Header("�O���Ƿ�ͨ�^�Z���ϳɲ����ı�")]
    [SerializeField] private bool m_IsVoiceMode = true;

    #endregion

    //Awake��һ������ķ���������Unity�������L�ڷ���֮һ���@����������Ҫ�քӺ��С�Unity��������_���������r�ԄӺ���Awake����
    private void Awake()
    {
        m_CommitMsgBtn.onClick.AddListener(delegate { SendData(); });       //�@�г�ʽ�a������m_CommitMsgBtn�İ��o������һ���c���¼��O ���������o���c���r�������ԄӺ���SendData�������@�eʹ��������ίӚ��delegate���������S���ڲ������·�������r��ֱ�Ӷ��xҪ���еĲ��������@�������У������o���c���r��SendData������������
        RegistButtonEvent();        //���Z���İ��o����¼�
        InputSettingWhenWebgl();
    }

    #region ��Ϣ����

    /// <summary>
    /// webgl�r̎��֧����������
    /// </summary>
    private void InputSettingWhenWebgl()
    {
#if UNITY_WEBGL
        m_InputWord.gameObject.AddComponent<WebGLSupport.WebGLInput>();
#endif
    }


    /// <summary>
    /// ����ݔ��ڵİl��ӍϢ�����͵�Chatgpt
    /// �@�eָ������ǂ�ݔ���λ������ݔ���
    /// �@�e�鰴���ύ�r������
    /// </summary>
    public void SendData()
    {
        // ���ݔ��ڞ�Մt������
        if (m_InputWord.text.Equals(""))
            return;

        //ݔ������֣��t��Ӽo�����
        m_ChatHistory.Add(m_InputWord.text);

        //_msg��ݔ��ڵ�����
        string _msg = m_InputWord.text ;

        //�l�͔����o�������m_ChatSettings�е�m_ChatModel�e���PostMsg������Ҳ�����Z��ģ�͵Ă��ͽoAI��
        m_ChatSettings.m_ChatModel.PostMsg(_msg, CallBack);

        //���ݔ���
        m_InputWord.text = "";
        m_TextBack.text = "����˼����...";

        //�����ГQ˼������
        SetAnimator("state", 1);
        SetAnimator2("state", 1);
    }
    /// <summary>
    /// ߀��֪���@Ҫ����
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
        m_TextBack.text = "����˼����...";

        //�л�˼������
        SetAnimator("state", 1);
        SetAnimator2("state", 1);
    }

    /// <summary>
    /// AI�ظ���ӍϢ�Ļ��{,CallBack
    /// </summary>
    /// <param name="_response"></param>
    private void CallBack(string _response)
    {
        _response = _response.Trim();
        //AI�ظ����ǂ����ָ����
        m_TextBack.text = "";

        
        Debug.Log("�յ�AI�ظ���"+ _response);

        //�o�����
        m_ChatHistory.Add(_response);

        //����x�����Z���ظ����������D�Z���]�з���ģ�ͣ��tֱ�ӌ��؂��������@ʾ�����֙��ϣ����Ǟ�һ������ֻظ�
        if (!m_IsVoiceMode||m_ChatSettings.m_TextToSpeech == null)
        {
            //�_ʼ�����@ʾ���ص��ı�
            StartTypeWords(_response);
            return;
        }

        //�l�͔����o�������m_ChatSettings�е�m_TextToSpeech�e���Speak������Ҳ���ǰ������D�Z����ģ��
        m_ChatSettings.m_TextToSpeech.Speak(_response, PlayVoice);
    }

#endregion

#region �Z��ݔ��
    /// <summary>
    /// �Z�����R���ص��ı��Ƿ�ֱ�Ӱl����LLM
    /// </summary>
    [SerializeField] private bool m_AutoSend = true;
    /// <summary>
    /// �Z������İ��o
    /// </summary>
    [SerializeField] private Button m_VoiceInputBotton;
    /// <summary>
    /// ������o���ı�
    /// </summary>
    [SerializeField]private Text m_VoiceBottonText;
    /// <summary>
    /// �������ʾ��Ϣ
    /// </summary>
    [SerializeField] private Text m_RecordTips;
    /// <summary>
    /// �Z��ݔ��̎���
    /// </summary>
    [SerializeField] private VoiceInputs m_VoiceInputs;
    /// <summary>
    /// ע�ᰴ�o�¼�
    /// </summary>
    private void RegistButtonEvent()
    {
        //����Z�����o���ǿ��қ]�� EventTrigger Ԫ�����t��ʽ�a���^�m�������m�Ĳ��������������o�����ջ����ѽ����� EventTrigger Ԫ������ʾ�ѽ��O�����¼��|�l�����Ͳ���Ҫ���M�а��o�¼����]�ԣ����ֱ�ӷ��أ��������}�O�ð��o�¼�
        if (m_VoiceInputBotton == null || m_VoiceInputBotton.GetComponent<EventTrigger>())
            return;

        EventTrigger _trigger = m_VoiceInputBotton.gameObject.AddComponent<EventTrigger>();

        //��Ӱ��o���µ��¼�
        EventTrigger.Entry _pointDown_entry = new EventTrigger.Entry(); //�@һ�н�����һ��EventTrigger.Entry��������EventTrigger.Entry��Unity���Á��x�ض��¼�����غ���e��׃��_pointDown_entry���Á탦���@���½����Č���
        _pointDown_entry.eventID = EventTriggerType.PointerDown;        //�@�г�ʽ�a�O��_pointDown_entry��eventID���ԡ�EventTriggerType.PointerDown��һ��ö�eֵ����ʾһ��ָ�ˣ����绬���[�˻��|��ΞĻ�ϵ���ָ�����µ��¼����@��ʾ�˗lĿ����춮�ʹ���߰���ָ�˕r�|�l
        _pointDown_entry.callback = new EventTrigger.TriggerEvent();     //�@�г�ʽ�a��ʼ��_pointDown_entry��callback���ԡ�EventTrigger.TriggerEvent��һ��Unity�е��¼���ͣ�����]�Ժ�̎���¼����{�����@�e�������Á��O�����|�lPointerDown�¼��r��ԓ���еķ�������������@���A�Σ�callback߀�]�о��w�ķ����P��ͨ������Ҫ���m�����_pointDown_entry.callback.AddListener(...)��ָ�����w�ķ�������

        //��Ӱ��o��_�¼�
        EventTrigger.Entry _pointUp_entry = new EventTrigger.Entry();
        _pointUp_entry.eventID = EventTriggerType.PointerUp;
        _pointUp_entry.callback = new EventTrigger.TriggerEvent();

        //���ίӚ�¼�
        _pointDown_entry.callback.AddListener(delegate { StartRecord(); });     //�@�г�ʽ�a���_pointDown_entry����callback������һ������ίӚ��delegate�������c_pointDown_entry���P���¼�������ǰ�O����EventTriggerType.PointerDown����ָ�˰����¼������|�l�r���@������ίӚ���Ԅӈ���
        _pointUp_entry.callback.AddListener(delegate { StopRecord(); });

        _trigger.triggers.Add(_pointDown_entry);    //���д��뽫_pointDown_entry������ӵ�_trigger�����triggers�б��С�_triggerӦ����һ��EventTrigger�����ʵ������triggers��һ�����������¼���������Ŀ��EventTrigger.Entry���󣩵��б����_pointDown_entry��triggers�б���ζ�ŵ������Ŀ��������¼�����֮ǰ���õ�EventTriggerType.PointerDown������ʱ��_trigger����ᴦ������������Ӧ�Ļص��������������������StartRecord��
        _trigger.triggers.Add(_pointUp_entry);
    }

    /// <summary>
    /// �_ʼ��u
    /// </summary>
    public void StartRecord()
    {
        m_VoiceBottonText.text = "���������..."; 
        m_VoiceInputs.StartRecordAudio();
    }
    /// <summary>
    /// ������u
    /// </summary>
    public void StopRecord()
    {
        m_VoiceBottonText.text = "��ס���o���_ʼ���"; 
        m_RecordTips.text = "����Y���������R�e...";
        //����AcceptClip�@�������M�뵽StopRecordAudio��Ȼ����StopRecordAudio�e�����䛺õ����nʹ��callback�������
        m_VoiceInputs.StopRecordAudio(AcceptClip);
    }

    /// <summary>
    /// ����¼�Ƶ���Ƶ����
    /// Ŀǰ�Ҳ�����;
    /// </summary>
    /// <param name="_data"></param>
    private void AcceptData(byte[] _data)
    {
        if (m_ChatSettings.m_SpeechToText == null)
            return;

        m_ChatSettings.m_SpeechToText.SpeechToText(_data, DealingTextCallback);
    }

    /// <summary>
    /// ̎����u�����n����
    /// </summary>
    /// <param name="_data"></param>
    private void AcceptClip(AudioClip _audioClip)
    {
        if (m_ChatSettings.m_SpeechToText == null)
            return;

        //��䛺õ����n����SpeechToTextȥ�D�Q���������n��DealingTextCallback�@�������ą���(������)
        m_ChatSettings.m_SpeechToText.SpeechToText(_audioClip, DealingTextCallback);
    }
    /// <summary>
    /// ̎����R�����ı�
    /// </summary>
    /// <param name="_msg"></param>
    private void DealingTextCallback(string _msg)
    {
        //m_RecordTips.text�Z�����R�����@ʾ�ę�λ�������R����������@ʾ������
        m_RecordTips.text = _msg;
        StartCoroutine(SetTextVisible(m_RecordTips));
        //�Ƿ��ԄӰl��
        if (m_AutoSend)
        {
            SendData(_msg);
            return;
        }

        //����]���x���ԄӰl�ͣ��t���Z����ӍϢ�������֙���
        m_InputWord.text = _msg;
    }

    private IEnumerator SetTextVisible(Text _textbox)
    {
        //׌�����@ʾ���Z�����R�����֙�����3��������ʧ
        yield return new WaitForSeconds(3f);
        _textbox.text = "";
    }

#endregion

#region �Z���ϳ�

    //���Á��_ʼ���ŏ�openai�˻؂��؁���Z��
    private void PlayVoice(AudioClip _clip, string _response)
    {
        m_AudioSource.clip = _clip;     //AudioSource�M����clip�������ָ��Ҫ���ŵ���Ӎ
        m_AudioSource.Play();            //�_ʼ����
        Debug.Log("���l�r�L��" + _clip.length);
        //�_ʼ�����@ʾ���ص��ı�
        StartTypeWords(_response);
        //�ГQ���fԒ����
        SetAnimator("state", 2);
        SetAnimator2("state", 2);
    }

#endregion

#region ���������@ʾ
    //�����@ʾ�ĕr�g�g��
    [SerializeField] private float m_WordWaitTime = 0.2f;
    //�Ƿ��@ʾ���
    [SerializeField] private bool m_WriteState = false;

    /// <summary>
    ///�_ʼ������ӡ
    /// </summary>
    /// <param name="_msg"></param>
    private void StartTypeWords(string _msg)
    {
        if (_msg == "")
            return;

        m_WriteState = true;
        StartCoroutine(SetTextPerWord(_msg));
    }

    //��ӡ
    private IEnumerator SetTextPerWord(string _msg)
    {
        int currentPos = 0;

        //ÿ��ޒȦӡһ���֣�ֱ��ӡ���ֹ
        while (m_WriteState)
        {
            yield return new WaitForSeconds(m_WordWaitTime);
            currentPos++;
            //�����@ʾ�ă���
            m_TextBack.text = _msg.Substring(0, currentPos);

            //�]��ӡ��ȫ�����־��^�m
            m_WriteState = currentPos < _msg.Length;

        }

        //�л����ȴ�����
        SetAnimator("state",0);
        SetAnimator2("state", 0);
    }

#endregion

#region ����o�
    //��������o�
    [SerializeField] private List<string> m_ChatHistory;
    //�����ф������������
    [SerializeField] private List<GameObject> m_TempChatBox;
    //����o��@ʾ��
    [SerializeField] private GameObject m_HistoryPanel;
    //�����ı����õČ�
    [SerializeField] private RectTransform m_rootTrans;
    //�l���������
    [SerializeField] private ChatPrefab m_PostChatPrefab;
    //�ظ����������
    [SerializeField] private ChatPrefab m_RobotChatPrefab;
    //�L�ӗl
    [SerializeField] private ScrollRect m_ScroTectObject;
    //�@ȡ����o�
    public void OpenAndGetHistory()
    {
        m_ChatPanel.SetActive(false);
        m_HistoryPanel.SetActive(true);

        ClearChatBox();
        StartCoroutine(GetHistoryChatInfo());
    }
    //����
    public void BackChatMode()
    {
        m_ChatPanel.SetActive(true);
        m_HistoryPanel.SetActive(false);
    }

    //����ф����Č�Ԓ��
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

    //�@ȡ����o��б�
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

        //����Ӌ�������ߴ�
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
    }

    private IEnumerator TurnToLastLine()
    {
        yield return new WaitForEndOfFrame();
        //�L�ӵ��������Ϣ
        m_ScroTectObject.verticalNormalizedPosition = 0;
    }


#endregion

    //�ГQ����ĄӮ�
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

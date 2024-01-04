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
    /// AI�O��
    /// </summary>
    public string m_SystemSetting = "";
    /// <summary>
    /// gpt-3.5-turbo
    /// </summary>
    public string m_gptModel = "gpt-3.5-turbo";

    private void Start()
    {
        //�\�Еr�����AI�O��
        m_DataList.Add(new SendData("system", m_SystemSetting));
    }

    /// <summary>
    /// �l����Ϣ
    /// </summary>
    /// <returns></returns>
    public override void PostMsg(string _msg, Action<string> _callback)
    {
        base.PostMsg(_msg, _callback);
    }

    /// <summary>
    /// �{�ýӿ�
    /// </summary>
    /// <param name="_postWord"></param>
    /// <param name="_callback"></param>
    /// <returns></returns>
    public override IEnumerator Request(string _postWord, System.Action<string> _callback)
    {
        stopwatch.Restart();
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))      //�@�e������һ��UnityWebRequest�����춰l��POSTՈ��using�Z��_��Ո�������ʹ���ꮅ�����_�ر�ጷš�
        {
            PostData _postData = new PostData           //PostDat���������ж��xclass
            {
                model = m_gptModel,
                messages = m_DataList,
    
            };

            string _jsonText = JsonUtility.ToJson(_postData).Trim();                    //��_postData����D�Q��JSON��ʽ���ִ����Kȥ���ִ��ɶ˵Ŀհס�JSON��JavaScript Object Notation����һ�N�p�������Y�Ͻ��Q��ʽ�������Z�ԟo�P�ģ��@��ζ���׺����г�ʽ�Z�Զ����Խ����ͮa��JSON�Y�ϡ���Web���ú�API���؄e��RESTful API���У�JSON�ǰl�ͺͽ����Y�ϵ�������ʽ����ʹ���Y���ڿ͑��˺��ŷ���֮�g�Ă�ݔ׃�ú��κ�һ�¡�
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);        //��JSON�ִ��D�Q��UTF-8��ʽ��λԪ�M���M��
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);      //�O��Ո���uploadHandler��UploadHandlerRaw�Č���������ς�λԪ�M���M
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();     //�O��Ո���downloadHandler��DownloadHandlerBuffer�Č�������춽����ŷ����Ļؑ��Y�ϡ�

            request.SetRequestHeader("Content-Type", "application/json");               //��Ո������HTTP�^����ָ��������͞�JSON��
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", api_key));        //�����ڙ��^����ʹ��Bearer�����M���J�C��

            yield return request.SendWebRequest();  //�l��Ո��K�ȴ��ؑ����@�ǅf�̵Ē����c��

            if (request.responseCode == 200)    //�z��ؑ����a�Ƿ��200����Ո��ɹ���
            {
                string _msgBack = request.downloadHandler.text;     //ȡ���ŷ����Ļؑ����֡�
                MessageBack _textback = JsonUtility.FromJson<MessageBack>(_msgBack);        //���ؑ����֏�JSON��ʽ������MessageBack��͵������
                if (_textback != null && _textback.choices.Count > 0)   //�z������������Ƿ�ǿ�����choices���԰�������һ��Ԫ�ء�
                {

                    string _backMsg = _textback.choices[0].message.content;
                    //��Ӽo�
                    m_DataList.Add(new SendData("assistant", _backMsg));
                    _callback(_backMsg);        //�յ�ӍϢ��̎��ݔ�������֙��ϵ�����
                }

            }
            else
            {
                string _msgBack = request.downloadHandler.text;
                Debug.LogError(_msgBack);
            }

            stopwatch.Stop();
            Debug.Log("chatgpt�ĕr��"+ stopwatch.Elapsed.TotalSeconds);
        }
    }
    #region ������

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






    //�ГQ��ɫ
    public void Change_Character_button1(GameObject _settingPanel)
    {
        m_Prompt = "";
        lan = "���w���Ļش�";
        _settingPanel.SetActive(false);
    }

    public void Change_Character_button2(GameObject _settingPanel)
    {
        m_Prompt = "";
        lan = "Ӣ�Ļش�";
        _settingPanel.SetActive(false);
    }

}

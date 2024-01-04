using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LLM;
using UnityEngine.Networking;

public class OpenAITextToSpeech : TTS
{
    #region ��������

    [SerializeField] private string api_key=string.Empty;   //apikey
    [SerializeField] private ModelType m_ModelType = ModelType.tts_1;   //ģ��
    [SerializeField] private VoiceType m_Voice = VoiceType.onyx;    //��

    #endregion
    private void Awake()
    {
        m_PostURL = "https://api.openai.com/v1/audio/speech";
    }

    /// <summary>
    /// �Z���ϳɣ����غϳ��ı�
    /// ��ChatSample�е�callback��AI�؂���ӍϢ�c��PlayVoice�@���������뵽�@��Speak
    /// </summary>
    /// <param name="_msg"></param>
    /// <param name="_callback"></param>
    public override void Speak(string _msg, Action<AudioClip, string> _callback) //Action��C# �е�һ���Ƚ�ίӚ��ͣ����Ƿ����е����棬��������ǰ��Ҫ��int�ǘӡ�����춷��bһ��������ԓ�����]�Ђ���ֵ����void ��ͣ���<AudioClip, string>ָ�����@��ActionίӚ���Է��b�ķ����ą����̈́e�����@�������У�����ʾ�@��Action���Է��bһ������һ��AudioClip��һ��string���酢���ķ�����
    {
        StartCoroutine(GetVoice(_msg, _callback));          //�_���f�̣�����AI�ظ���ӍϢ��callback����


        /*      �˞����ķ���
         
        private void PlayVoice(AudioClip _clip, string _response)
        {
            m_AudioSource.clip = _clip;
            m_AudioSource.Play();
            Debug.Log("���l�r�L��" + _clip.length);
            //�_ʼ�����@ʾ���ص��ı�
            StartTypeWords(_response);
            //�ГQ���fԒ����
            SetAnimator("state", 2);
            SetAnimator2("state", 2);
        }

        */

    }

    private IEnumerator GetVoice(string _msg, Action<AudioClip, string> _callback)
    {
        stopwatch.Restart();
        using (UnityWebRequest request = UnityWebRequest.Post(m_PostURL, new WWWForm()))    //����һ��UnityWebRequest�����춰l��POSTՈ���@�eʹ��using�Z��_��Ո��Y�����YԴ�����_ጷš�
        {
            PostData _postData = new PostData   //�����K��ʼ��һ��PostData��͵����_postData�����������ģ�����Q��ݔ��ӍϢ������͡�
            {
                model = m_ModelType.ToString().Replace('_','-'),    //ģ�����Q
                input = _msg,       //Ҫ�D�Q��ӍϢ
                voice= m_Voice.ToString()       //ģ���п����x����
            };

            string _jsonText = JsonUtility.ToJson(_postData).Trim();    //��_postData����D�Q��JSON��ʽ���ִ��K�Ƴ��ɶ˵Ŀհ���Ԫ��
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);    //��JSON�ִ��D�Q��UTF-8���a��λԪ�M���M��
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);  //��Ո���O���ς�̎���������ς���ǰ�ʂ��λԪ�M�Y�ϡ�
            request.downloadHandler = new DownloadHandlerAudioClip(m_PostURL, AudioType.MPEG);  //��Ո���O�����d̎������@�e��������d��Ӎ��݋��

            request.SetRequestHeader("Content-Type", "application/json");   //�O��HTTPՈ���^�����l�͵ă�����͞�JSON��
            request.SetRequestHeader("Authorization", string.Format("Bearer {0}", api_key));    //�O���ڙ��^����ʹ��Bearer�����J�C��

            yield return request.SendWebRequest();  //�l��Ո��K��ͣ�f�̣�ֱ���W·Ո����ɡ�

            if (request.responseCode == 200)    //�z��HTTP�ؑ��a�Ƿ��200����Ո��ɹ���
            {
                AudioClip audioClip = ((DownloadHandlerAudioClip)request.downloadHandler).audioClip;    //�����d̎�������ȡ��AudioClip�����
                _callback(audioClip, _msg);

                /*      �˞����ķ���(���@����ʽ�e���callback

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

                */

            }
            else
            {
                Debug.LogError("�Z���ϳ�ʧ��: " + request.error);
            }

            stopwatch.Stop();
            Debug.Log("openAI�Z���ϳɣ�" + stopwatch.Elapsed.TotalSeconds);
        }
    }

    #region ���ݶ���

    /// <summary>
    /// ���͵ı���
    /// </summary>
    [Serializable]
    public class PostData
    {
        public string model = string.Empty;//ģ�����Q
        public string input = string.Empty;//�ı�����
        public string voice = string.Empty;//��
    }
    /// <summary>
    /// ģ�����
    /// </summary>
    public enum ModelType
    {
        tts_1,
        tts_1_hd
    }
    /// <summary>
    /// �����
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


    public void Change_voice_�C����()
    {
        m_Voice = VoiceType.alloy;
    }

    public void Change_voice_Ů��()
    {
        m_Voice = VoiceType.nova;
    }
}

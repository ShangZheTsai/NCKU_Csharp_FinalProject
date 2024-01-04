using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using UnityEngine;

public class LLM : MonoBehaviour
{
    /// <summary>      
    /// api��ַ
    /// </summary>
    [SerializeField] protected string url;
    /// <summary>
    /// ��ʾ�~���cʹ���ߵ�ݔ��һ��l��
    /// </summary>
    [Header("�l�͵���ʾ�~�O��")]
    [SerializeField] protected string m_Prompt = string.Empty;      //string.Empty��ͬ�""����string.Empty������ߴ��a�Ŀ��x��
    /// <summary>
    /// �Z��
    /// </summary
    [Header("�O�ûظ����Z��")]
    [SerializeField] public string lan = "���w����";
    /// <summary>
    /// �����ı����l��
    /// </summary>
    [Header("�����ı����l��")]
    [SerializeField] protected int m_HistoryKeepCount = 15;
    /// <summary>
    /// ���挦Ԓ
    /// �Á����oAI�������ģ���ChatSample��m_ChatHistory��������o�
    /// SendData��ӍϢ������ɫ��ݔ���ӍϢ
    /// </summary>
    [SerializeField] public List<SendData> m_DataList = new List<SendData>();
    /// <summary>
    /// Ӌ�㷽���{�õĕr�g
    /// ֮���^�Е��{��
    /// </summary>
    [SerializeField] protected Stopwatch stopwatch = new Stopwatch();
    /// <summary>
    /// �l��ӍϢ
    /// ʹ��virtual׌֮���^��LLM�@��e��ģ�Ϳ���override
    /// Action<T>��һ�N�Ƚ���ίӚ��͡����㿴��Action<string>���@��ζ������һ������һ��������ίӚ���@����������һ��string��͵ą����K�қ]�л؂�ֵ��ίӚ���|���ǌ����������á������S�㌢�������酢�����f�����ǵ�ͬ춂���һ����string����������Ȼ�����@����ʽ�e���callbackȡ��֮��
    /// </summary>
    public virtual void PostMsg(string _msg, Action<string> _callback)
    {
        //�����ėl���O��
        CheckHistory();
        //��ʾ�~̎��
        string message = //"��ǰ���ɫ�������O����" + m_Prompt +
                                _msg + " Ո�ã�" + lan + "�K�ظ����֔�Ո������60��������";

        //����l�͵�ӍϢ�б�
        m_DataList.Add(new SendData("user", message));

        //�_ʼһ���f�̣���춈���Request����(����)���@�������ƺ����Á�̎���ͬ���W·Ո���ĳ�N��ͬ������
        StartCoroutine(Request(message, _callback));
    }

    //�o�^��LLM����Щģ�͸�������Ҫ���N����AI�ˣ�postWord�ǰ����Լ��Ȳ��O����ӍϢ(�Z���O���ͽ�ɫ�O����Щ)
    public virtual IEnumerator Request(string _postWord, Action<string> _callback)
    {
        //ǰ��Ĳ������Y���ᣬ��callback
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// �O�ñ����������ėl������ֹ̫�L
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

        //�o�����Ľ�����ʽ
        public SendData()
        {

        }

        //�Ѕ����Ľ�����ʽ
        public SendData(string _role, string _content)
        {
            role = _role;
            content = _content;
        }

    }

}

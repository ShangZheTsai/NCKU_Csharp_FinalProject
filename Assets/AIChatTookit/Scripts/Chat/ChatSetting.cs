using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]      //ʹ�������л���׌��һ����ʽ����׌�����F��inspector�ϣ�������ߡ�B�x���_�����[���@�e���ֵȥ���޸ģ��@��ζ�����Č���������Unity��݋�����@ʾ���޸�
public class ChatSetting
{
    /// <summary>
    /// ����ģ��
    /// </summary>
    [Header("������Ҫ���ϲ�ͬ��LLM")]
    [SerializeField] public LLM m_ChatModel;    //[SerializeField] ��׌�@��׃����ߡ�B�x�΃�ʹ���Ծ�݋������]�ӕ�ֱ���ܵ��x������һ��
    /// <summary>
    /// �Z���ϳ�
    /// </summary>
    [Header("�Z���ϳ��_��")]
    public TTS m_TextToSpeech;
    /// <summary>
    /// �Z�����R
    /// </summary>
    [Header("�Z�����R�_��")]
    public STT m_SpeechToText;
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TTS : MonoBehaviour
{
    /// <summary>
    /// �Z���ϳɵ�api��ַ
    /// </summary>
    [SerializeField] protected string m_PostURL = string.Empty;
    /// <summary>
    /// Ӌ�㷽���{�õĕr�g
    /// ֮���^�Е��{��
    /// </summary>
    [SerializeField] protected Stopwatch stopwatch = new Stopwatch();
    /// <summary>
    /// �Z���ϳɣ�������Ƶ
    /// </summary>
    /// ���μ��f������
    /// <param name="_msg"></param>
    /// <param name="_callback"></param>
    public virtual void Speak(string _msg,Action<AudioClip> _callback) {}
    /// <summary>
    /// �ϳ��Z��������Ƶ��ͬ�r���غϳɵ��ı�
    /// </summary>
    /// <param name="_msg"></param>
    /// <param name="_callback"></param>
    public virtual void Speak(string _msg, Action<AudioClip,string> _callback) { }


}

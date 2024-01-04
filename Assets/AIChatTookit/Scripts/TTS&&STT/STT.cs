using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class STT : MonoBehaviour
{

    /// <summary>
    /// 語音辨識api地址
    /// </summary>
    [SerializeField] protected string m_SpeechRecognizeURL = String.Empty;
    /// <summary>
    /// 計算方法調用的時間
    /// 之後繼承會調用
    /// </summary>
    [SerializeField] protected Stopwatch stopwatch = new Stopwatch();
    /// <summary>
    /// 語音辨識
    /// </summary>
    /// <param name="_clip"></param>
    /// <param name="_callback"></param>
    public virtual void SpeechToText(AudioClip _clip,Action<string> _callback)
    {
       
    }

    /// <summary>
    /// 語音辨識
    /// </summary>
    /// <param name="_audioData"></param>
    /// <param name="_callback"></param>
    public virtual void SpeechToText(byte[] _audioData, Action<string> _callback)
    {

    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]      //使可以序列化，讓另一個程式呼叫讓它出現在inspector上，可以用摺疊選單開啟或隱藏這裡面的值去做修改，這意味著它的實例可以在Unity編輯器中顯示和修改
public class ChatSetting
{
    /// <summary>
    /// 聊天模型
    /// </summary>
    [Header("根據需要掛上不同的LLM")]
    [SerializeField] public LLM m_ChatModel;    //[SerializeField] 是讓這個變數到摺疊選單內使可以編輯，如果沒加會直接跑到選單下面一個
    /// <summary>
    /// 語音合成
    /// </summary>
    [Header("語音合成腳本")]
    public TTS m_TextToSpeech;
    /// <summary>
    /// 語音辨識
    /// </summary>
    [Header("語音辨識腳本")]
    public STT m_SpeechToText;
}

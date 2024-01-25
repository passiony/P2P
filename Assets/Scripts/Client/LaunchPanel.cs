using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LaunchPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_IpInput;
    [SerializeField] private TMP_InputField m_PathInput;
    [SerializeField] private Button m_SaveBtn;
    [SerializeField] private Button m_TestBtn;

    private Queue<string> m_Queue = new Queue<string>();
    
    void Start()
    {
        var address = PlayerPrefs.GetString("IP_ADDRESS");
        var appPath = PlayerPrefs.GetString("APP_PATH");   

        m_IpInput.text = address;
        m_PathInput.text = appPath;

        m_SaveBtn.onClick.AddListener(OnSaveClick);
        m_TestBtn.onClick.AddListener(OnTestClick);
        if (!string.IsNullOrEmpty(address))
        {
            TCPClient.Instance.StartConnect(address);
        }

        TCPClient.Instance.OnMessageHandler = OnReceiveMsg;
    }

    private void OnSaveClick()
    {
        PlayerPrefs.SetString("APP_PATH", m_PathInput.text);
        PlayerPrefs.SetString("IP_ADDRESS", m_IpInput.text);
    }

    private void OnTestClick()
    {
        var path = m_PathInput.text;
        ProcessUtility.OpenApp(path);
    }
    
    private void OnReceiveMsg(string obj)
    {
        //接受服务器消息
        Debug.Log("OnReceiveMsg:"+obj);
        m_Queue.Enqueue(obj);
    }

    private void Update()
    {
        if (m_Queue.Count > 0)
        {
            var obj = m_Queue.Dequeue();
            OnTestClick();
        }
    }
}
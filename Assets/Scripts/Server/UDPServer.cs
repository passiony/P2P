using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Packet
{
    public string Ip;
    public IPEndPoint iPEndPoint;
    public string Msg;

    public Packet(string address, IPEndPoint port, string msg)
    {
        this.Ip = address;
        this.iPEndPoint = port;
        this.Msg = msg;
    }
}

public class UDPServer : MonoSingleton<UDPServer>
{
    // public int port = 1234;
    private Socket serverSocket;
    private EndPoint epSender;

    private UdpClient udpclient;
    
    //接收数据的字符串
    private byte[] ReceiveData = new byte[1024];

    private Queue<Packet> m_MsgQueue = new Queue<Packet>();

    public Action<Packet> OnMessgeHandler;

    void Start()
    {
        //服务器Socket对实例化
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //Socket对象服务器的IP和端口固定
        serverSocket.Bind(new IPEndPoint(IPAddress.Any, 1234));
        //监听的端口和地址
        epSender = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
        //开始异步接收数据
        serverSocket.BeginReceiveFrom(ReceiveData, 0, ReceiveData.Length, SocketFlags.None, ref epSender,
            new AsyncCallback(ReceiveFromClients), epSender);
    }

    /// <summary>
    /// 异步加载，处理数据
    /// </summary>
    /// <param name="iar"></param>
    void ReceiveFromClients(IAsyncResult iar)
    {
        int reve = serverSocket.EndReceiveFrom(iar, ref epSender);
        //数据处理
        string str = System.Text.Encoding.UTF8.GetString(ReceiveData, 0, reve);
        //把得到的数据传给数据处理中心
        serverSocket.BeginReceiveFrom(ReceiveData, 0, ReceiveData.Length, SocketFlags.None, ref epSender,
            ReceiveFromClients, epSender);

        var ipend = (IPEndPoint)epSender;
        m_MsgQueue.Enqueue(new Packet(ipend.Address.ToString(), ipend, str));
    }

    public void Send(string ip, int port, string msg)
    {
        Debug.Log($"Send:{ip}:{port}" + msg);

        var bytes = Encoding.UTF8.GetBytes(msg);
        var endPoint = (EndPoint)new IPEndPoint(IPAddress.Parse(ip), port);
        serverSocket.BeginSendTo(bytes, 0, bytes.Length, SocketFlags.None, endPoint,
            new AsyncCallback((x) =>
            {
                Debug.Log("aaa");
            }), endPoint);
    }

    private void Update()
    {
        if (m_MsgQueue.Count > 0)
        {
            var msg = m_MsgQueue.Dequeue();
            OnMessgeHandler?.Invoke(msg);
        }
    }

    void OnDestroy()
    {
        // 在对象被销毁时关闭UDP客户端
        serverSocket?.Close();
    }
}
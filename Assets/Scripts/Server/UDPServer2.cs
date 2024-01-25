using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class UDPServer2 : MonoSingleton<UDPServer2>
{
    public int serverPort = 1234;
    UdpSer udpServer;

    public Queue<Packet> m_MsgQueue = new Queue<Packet>();

    public Action<Packet> OnMessgeHandler;

    void Start()
    {
        // 创建并启动 UDP 服务器
        udpServer = new UdpSer(serverPort);
        udpServer.StartListening();
    }

    private void Update()
    {
        if (m_MsgQueue.Count > 0)
        {
            var msg = m_MsgQueue.Dequeue();
            OnMessgeHandler?.Invoke(msg);
        }
    }

    public void Send(IPEndPoint ip, string msg)
    {
        udpServer.SendMessage(msg, ip);
    }

    private void OnDestroy()
    {
        udpServer.StopServer();
    }
}

class UdpSer
{
    private UdpClient udpServer;
    private IPEndPoint clientEndpoint;

    public UdpSer(int port)
    {
        udpServer = new UdpClient(port);
        clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
    }

    public void StartListening()
    {
        Debug.Log("UDP Server is listening...");

        // 异步接收数据
        udpServer.BeginReceive(ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // 接收数据并获取远程客户端的终结点
            byte[] receivedBytes = udpServer.EndReceive(ar, ref clientEndpoint);

            // 将接收到的数据转换为字符串
            string receivedMessage = Encoding.UTF8.GetString(receivedBytes);

            // 显示接收到的消息和客户端信息
            Debug.Log($"Received from {clientEndpoint}: {receivedMessage}");

            SendMessage("test", clientEndpoint);
            
            // 继续异步接收下一条消息
            udpServer.BeginReceive(ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            Debug.Log($"Error during receive: {ex.Message}");
        }
    }

    public void SendMessage(string message, IPEndPoint targetEndpoint)
    {
        // 将字符串消息转换为字节数组
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        try
        {
            // 发送消息给指定的目标终结点
            udpServer.Send(messageBytes, messageBytes.Length, targetEndpoint);
            Debug.Log($"Sent to {targetEndpoint}: {message}");
        }
        catch (Exception ex)
        {
            Debug.Log($"Error during send: {ex.Message}");
        }
    }

    public void StopServer()
    {
        udpServer.Close();
        Debug.Log("UDP Server stopped.");
    }
}
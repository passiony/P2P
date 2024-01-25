using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TCPServer : MonoSingleton<TCPServer>
{
    private string ip = "0.0.0.0";
    private int port = 1234;

    private TcpListener server;
    private CancellationTokenSource cancellationTokenSource;

    public Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
    public Dictionary<string, float> clientHeart = new Dictionary<string, float>();
    public Queue<Packet> m_MsgQueue = new Queue<Packet>();

    public Action<string> OnClientConnect;
    public Action<string> OnClientDisconnect;
    public Action<Packet> OnMessgeHandler;

    private List<string> temp = new List<string>();
    
    private void Update()
    {
        if (m_MsgQueue.Count > 0)
        {
            var msg = m_MsgQueue.Dequeue();
            OnMessgeHandler?.Invoke(msg);
        }

        foreach (var pair in clientHeart)
        {
            if (Time.time - pair.Value > 6)
            {
                temp.Add(pair.Key);
            }
        }

        foreach (var value in temp)
        {
            clients.Remove(value);
            clientHeart.Remove(value);
            OnClientDisconnect.Invoke(value);
        }
        temp.Clear();
    }

    private async void Start()
    {
        await StartServerAsync();
    }

    private async Task StartServerAsync()
    {
        IPAddress localAddr = IPAddress.Parse(ip);
        server = new TcpListener(localAddr, port);
        server.Start();

        Debug.Log($"Server started on {localAddr}:{port}");

        cancellationTokenSource = new CancellationTokenSource();

        await AcceptClientsAsync(cancellationTokenSource.Token);
    }

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Debug.Log("Waiting for a connection...");

                TcpClient client = await server.AcceptTcpClientAsync();
                var iep = client.Client.RemoteEndPoint as IPEndPoint;
                if (!clients.ContainsKey(iep.Address.ToString()))
                {
                    var address = iep.Address.ToString();
                    clients.Add(address, client);
                    clientHeart.Add(address,Time.time);
                    OnClientConnect.Invoke(address);
                }

                Debug.Log($"Client connected: {((IPEndPoint)client.Client.RemoteEndPoint).Address}");

                _ = HandleClientAsync(client, cancellationToken);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
    }

    public async Task HandleClientAsync(TcpClient tcpClient, CancellationToken cancellationToken)
    {
        try
        {
            NetworkStream clientStream = tcpClient.GetStream();
            byte[] message = new byte[4096];
            int bytesRead;

            while (!cancellationToken.IsCancellationRequested)
            {
                bytesRead = await clientStream.ReadAsync(message, 0, message.Length, cancellationToken);
                if (bytesRead == 0)
                    break;

                string data = Encoding.ASCII.GetString(message, 0, bytesRead);
                Debug.Log($"Received from client: {data}");
                var iep = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                if (clientHeart.ContainsKey(iep.Address.ToString()))
                {
                    var address = iep.Address.ToString();
                    clientHeart[address] = Time.time;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
        finally
        {
            tcpClient.Close();
        }
    }

    public void Send(string address)
    {
        Debug.Log("Sending OpenApp command to the client...");
        if (clients.TryGetValue(address, out TcpClient client))
        {
            // 这里可以添加向客户端发送"OpenApp"指令的代码
            byte[] commandBytes = Encoding.ASCII.GetBytes("OpenApp");
            client.GetStream().WriteAsync(commandBytes, 0, commandBytes.Length, cancellationTokenSource.Token);
        }
    }

    private void OnApplicationQuit()
    {
        cancellationTokenSource?.Cancel();
        server?.Stop();
    }
}
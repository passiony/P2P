using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TCPClient : MonoSingleton<TCPClient>
{
    private TcpClient client;
    private NetworkStream clientStream;
    private CancellationTokenSource cancellationTokenSource;

    public int port = 1234;
    public int HearInterval = 3;
    public Action<string> OnMessageHandler;

    public async void StartConnect(string ip)
    {
        cancellationTokenSource = new CancellationTokenSource();
        await ConnectToServerAsync(ip, cancellationTokenSource.Token);
    }

    private async Task ConnectToServerAsync(string serverIP, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (client == null || !client.Connected)
            {
                try
                {
                    client = new TcpClient();
                    await client.ConnectAsync(serverIP, port);

                    if (client.Connected)
                    {
                        clientStream = client.GetStream();
                        Debug.Log($"Connected to server: {serverIP}:{port}");

                        // 在这里添加从服务器接收指令的逻辑
                        ReceiveCommandAsync(cancellationToken);
                        // 启动循环发送消息给服务器
                        SendMessagesToServerAsync(cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"Error: {e.Message}");
                }
            }
            await Task.Delay(HearInterval * 1000, cancellationToken); // 等待2秒后重试
        }
    }

    private async void ReceiveCommandAsync(CancellationToken cancellationToken)
    {
        byte[] message = new byte[1024];
        int bytesRead;
        try
        {
            while (client.Connected && !cancellationToken.IsCancellationRequested)
            {
                bytesRead = await clientStream.ReadAsync(message, 0, message.Length, cancellationToken);
                if (bytesRead == 0)
                    break;

                string command = Encoding.ASCII.GetString(message, 0, bytesRead);
                Debug.Log($"Received command from server: {command}");
                OnMessageHandler?.Invoke(command);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
        }
        finally
        {
            client?.Close();
        }
    }

    private async Task SendMessagesToServerAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (client.Connected && !cancellationToken.IsCancellationRequested)
            {
                // 模拟发送消息给服务器
                string message = "Hello, server!";
                byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                await clientStream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);

                Debug.Log($"Sent message to server: {message}");

                // 每隔一秒发送一次消息
                await Task.Delay(HearInterval * 1000, cancellationToken);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending message to server: {e.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        cancellationTokenSource?.Cancel();
        client?.Close();
    }
}

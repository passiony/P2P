using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;

public class PeerCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private Button m_OpenBtn;

    private string address;

    public void Awake()
    {
        m_OpenBtn.onClick.AddListener(OnOpenClick);
    }

    public void Init(string address)
    {
        this.address = address;
        m_Title.text = $"ip:{address}";
    }

    private void OnOpenClick()
    {
        Debug.Log("SendMessage");
        TCPServer.Instance.Send(address);
    }
}
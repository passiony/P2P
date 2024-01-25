using System.Collections.Generic;
using UnityEngine;

public class ControlPanel : MonoBehaviour
{
    [SerializeField] private Transform m_Content;
    [SerializeField] private GameObject m_CellPrefab;

    private Dictionary<string, GameObject> m_Cells;

    void Start()
    {
        TCPServer.Instance.OnClientConnect = OnConnect;
        TCPServer.Instance.OnClientDisconnect = OnDisconnect;
        
        m_Cells = new Dictionary<string, GameObject>();
    }

    private void OnConnect(string address)
    {
        var go = GameObject.Instantiate(m_CellPrefab, m_Content);
        go.SetActive(true);
        go.GetComponent<PeerCell>().Init(address);
        m_Cells.Add(address, go);
    }
    
    private void OnDisconnect(string address)
    {
        if (m_Cells.TryGetValue(address, out GameObject go))
        {
            Destroy(go);
        }

        m_Cells.Remove(address);
    }
    
}
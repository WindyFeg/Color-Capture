using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class rpcChat : NetworkBehaviour
{
    public Button button;
    public TMPro.TMP_InputField input;
    public TMPro.TMP_Text text;
    void Start()
    {
        button.onClick.AddListener(TaskOnClick);
    }

    private void TaskOnClick()
    {
        if (IsOwner && !IsServer)
        {
            sendMessageServerRpc(input.text, NetworkObjectId);
        }
        if (IsServer)
        {
            UpdateMessageClientRpc(input.text, NetworkObjectId);
        }
    }

    [ServerRpc]
    void sendMessageServerRpc(string mess, ulong sourceNetworkObjectId)
    {
        UpdateMessageClientRpc(mess, sourceNetworkObjectId);
    }

    [ClientRpc]
    void UpdateMessageClientRpc(string mess, ulong sourceNetworkObjectId)
    {

        text.text += mess + "\n";
    }
}

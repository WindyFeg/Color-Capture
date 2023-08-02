using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SubmitName : NetworkBehaviour
{
    #region client Side
    public TMPro.TMP_InputField inputField;
    public TMPro.TMP_Text myName;
    public GameObject mapBlockPrefab;
    public Material emptyMaterial;
    public Material wallMaterial;
    #endregion

    #region Server Side
    public UniteData.Color[] mapData;
    public string player1Name = "";
    public string player2Name = "";
    public GameObject genMapButton;
    #endregion
    void Start()
    {
        if (IsServer)
        {
            // CallServerToGenMapServerRpc();
            genMapButton.SetActive(true);
        }

        if (!IsLocalPlayer)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            // SpawnMap();
        }

    }

    public void SpawnMap()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Instantiate(mapBlockPrefab, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SubmitNameToSever()
    {
        var _name = inputField.text;
        Debug.Log(_name + " summited");
        SubmitNameServerRpc(_name, NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubmitNameServerRpc(string name, ulong sourceNetworkObjectId)
    {
        if (player1Name == "")
            player1Name = name;
        if (player2Name == "")
            player2Name = name;

        UpdateNameClientRpc(name, sourceNetworkObjectId);
    }

    [ClientRpc]
    private void UpdateNameClientRpc(string name, ulong sourceNetworkObjectId)
    {
        if (NetworkObjectId == sourceNetworkObjectId)
        {
            myName.text = name;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void CallServerToGenMapServerRpc()
    {
        mapData = new UniteData.Color[100];
        // 0->49
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                mapData[x * 10 + y] = RandomWall(15);
            }
        }
        //50->99 
        for (int x = 5; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                mapData[x * 10 + y] = mapData[100 - (x * 10 + y) - 1];
            }
        }
        CallClientToGenMapClientRpc(mapData);
    }

    [ClientRpc]
    private void CallClientToGenMapClientRpc(UniteData.Color[] mapData)
    {
        var AllBlockParent = GameObject.Find("AllBlock");
        foreach (Transform block in AllBlockParent.transform)
        {
            GameObject.Destroy(block.gameObject);
        }


        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                var block = Instantiate(mapBlockPrefab, new Vector3(x, y, 0), Quaternion.identity);

                if (mapData[x * 10 + y] == UniteData.Color.Empty)
                {
                    block.GetComponent<Renderer>().material = emptyMaterial;
                }
                else
                {
                    block.GetComponent<Renderer>().material = wallMaterial;
                }

                block.transform.SetParent(AllBlockParent.transform);
            }
        }
    }

    private UniteData.Color RandomWall(int wallRate)
    {
        int percent = UnityEngine.Random.Range(0, 100);
        if (wallRate > percent)
        {
            return UniteData.Color.Wall;
        }
        else
        {
            return UniteData.Color.Empty;
        }
    }
}

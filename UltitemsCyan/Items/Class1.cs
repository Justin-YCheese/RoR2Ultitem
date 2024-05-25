using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [ClientRpc]
    public void TestMe(NetworkConnection conn, NetworkReader reader)
    {
        Debug.Log("How could you not?");
    }
}

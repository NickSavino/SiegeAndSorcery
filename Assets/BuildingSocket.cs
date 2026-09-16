using System;
using UnityEngine;

public class BuildingSocket : MonoBehaviour
{

    public BuildableStructure Owner { get; private set; }
    public BuildingSocket ConnectedSocket { get; private set; }
    
    public bool IsOccupied => ConnectedSocket != null;

    public void Initialize(BuildableStructure owner)
    {
        Owner = owner;
    }

    public bool CanConnectTo(BuildingSocket otherSocket)
    {
        return
            otherSocket != null
            && otherSocket != this
            && Owner != null
            && otherSocket.Owner != null
            && Owner != otherSocket.Owner
            && !IsOccupied
            && !otherSocket.IsOccupied;
    }

    public bool TryConnect(BuildingSocket otherSocket)
    {
        if (!CanConnectTo(otherSocket))
        {
            return false;
        }

        ConnectedSocket = otherSocket;
        otherSocket.ConnectedSocket = this;
        
        return true;
    }

    public void Disconnect()
    {
        var other = ConnectedSocket;
        ConnectedSocket = null;

        if (other != null && other.ConnectedSocket == this)
        {
            other.ConnectedSocket = null;
        }
    }

    void OnDestroy()
    {
        Disconnect();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.DrawRay(transform.position, transform.forward * 0.75f);
    }
}

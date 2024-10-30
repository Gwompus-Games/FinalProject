using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeartbeat 
{
    public void AddHeartbeat(PlayerController playerController)
    {
        if (playerController == null)
        {
            return;
        }
        if (playerController.CheckHeartbeatInList(this))
        {
            return;
        }
        playerController.AddHeartBeat(this);
    }

    public void RemoveHeartbeat(PlayerController playerController)
    {
        if (playerController == null)
        {
            return;
        }
        if (!playerController.CheckHeartbeatInList(this))
        {
            return;
        }
        playerController.RemoveHeartBeat(this);
    }
}

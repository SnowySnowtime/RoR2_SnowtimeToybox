using IL.RoR2.Achievements.Railgunner;
using RoR2;
using SnowtimeToybox.FriendlyTurrets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterBody))]
public class SwarmPlayerSwarmlingTracker : NetworkBehaviour
{
    public CharacterBody self;
    public List<CharacterBody> SwarmlingBodies = [];

    public void Start()
    {
        self = gameObject.GetComponent<CharacterBody>();
        GetSwarmlings();
        CharacterBody.onBodyDestroyGlobal += OnSwarmlingBodyDestroyGlobal;
        CharacterBody.onBodyStartGlobal += OnSwarmlingBodyStartGlobal;
        //hasAuthority = Util.HasEffectiveAuthority(this.gameObject);
    }
    
    public void FixedUpdate()
    {
        if(SwarmlingBodies.Count == 0)
        {
            GetSwarmlings();
        }
    }
    
    public List<CharacterBody> GetSwarmlingBodies()
    {
        return SwarmlingBodies;
    }
    
    public void GetSwarmlings()
    {
        //if (!hasAuthority) return;
        //if (!hasAuthority) return;
        CharacterBody[] minionBodies = this.self.GetMinionBodies();
        foreach(CharacterBody characterBody in minionBodies)
        {
            if(characterBody.name.Contains("_SwarmTurretling"))
            {
                OnSwarmlingFound(characterBody);
            }
        }
    }
    
    public void OnSwarmlingFound(CharacterBody body)
    {
        if(!SwarmlingBodies.Contains(body))
        {
            SwarmlingBodies.Add(body);
            //DroneInfo Swarmling = new DroneInfo(body);
            //Swarmlings.Add(Swarmling);
        }
    }
    
    private void OnSwarmlingBodyStartGlobal(CharacterBody body)
    {
        if (body.name.Contains("_SwarmTurretling") && !(body.GetOwnerBody() != self))
        {
            OnSwarmlingDiscovered(body);
        }
    }
    
    private void OnSwarmlingDiscovered(CharacterBody body)
    {
        if (!SwarmlingBodies.Contains(body))
        {
            SwarmlingBodies.Add(body);
            //DroneInfo Swarmling = new DroneInfo(body);
            //Swarmlings.Add(Swarmling);
        }
    }
    
    private void OnSwarmlingBodyDestroyGlobal(CharacterBody body)
    {
        if (body.name.Contains("_SwarmTurretling"))
        {
            OnSwarmlingLost(body);
        }
    }
    
    private void OnSwarmlingLost(CharacterBody body)
    {
        if (SwarmlingBodies.Contains(body))
        {
            SwarmlingBodies.Remove(body);
            //DroneInfo Swarmling = new DroneInfo(body);
            //Swarmlings.Remove(Swarmling);
        }
    }
    
    public void teleportSwarmling(Vector3 position)
    {
        if (NetworkServer.active)
        {
            StartTeleporting(position);
        }
        else
        {
            CmdTeleportSwarmlings(position);
        }
    }
    
    [Command]
    public void CmdTeleportSwarmlings(Vector3 position)
    {
        StartTeleporting(position);
    }
    
    [Server]
    public void StartTeleporting(Vector3 position)
    {
        //Log.Debug($"tel;eporting swarmlings as server !! {SwarmlingBodies.Count}");
        foreach (CharacterBody swarmlingBody in SwarmlingBodies)
        {
            if (!swarmlingBody?.GetComponent<SwarmMinionSwarmlingTeleportHandler>()) return;
            swarmlingBody.GetComponent<SwarmMinionSwarmlingTeleportHandler>().StartTeleporting(position);
        }
    }
}
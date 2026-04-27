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
public class SwarmPlayerSwarmlingTracker : MonoBehaviour
{
    public CharacterBody self;
    public static List<DroneInfo> Swarmlings = new List<DroneInfo>();
    public static List<CharacterBody> SwarmlingBodies = new List<CharacterBody>();
    private new bool hasAuthority => Util.HasEffectiveAuthority(base.gameObject);

    public void Start()
    {
        self = gameObject.GetComponent<CharacterBody>();
        GetSwarmlings();
        CharacterBody.onBodyDestroyGlobal += OnSwarmlingBodyDestroyGlobal;
        CharacterBody.onBodyStartGlobal += OnSwarmlingBodyStartGlobal;
    }
    public void FixedUpdate()
    {
        if(Swarmlings.Count == 0)
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
        if (!hasAuthority) return;
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
            DroneInfo Swarmling = new DroneInfo(body);
            Swarmlings.Add(Swarmling);
        }
    }
    private void OnSwarmlingBodyStartGlobal(CharacterBody body)
    {
        if (hasAuthority && body.name.Contains("_SwarmTurretling") && !(body.GetOwnerBody() != self))
        {
            OnSwarmlingDiscovered(body);
        }
    }
    private void OnSwarmlingDiscovered(CharacterBody body)
    {
        if (!SwarmlingBodies.Contains(body))
        {
            SwarmlingBodies.Add(body);
            DroneInfo Swarmling = new DroneInfo(body);
            Swarmlings.Add(Swarmling);
        }
    }
    private void OnSwarmlingBodyDestroyGlobal(CharacterBody body)
    {
        if (hasAuthority && body.name.Contains("_SwarmTurretling"))
        {
            OnSwarmlingLost(body);
        }
    }
    private void OnSwarmlingLost(CharacterBody body)
    {
        if (SwarmlingBodies.Contains(body))
        {
            SwarmlingBodies.Remove(body);
            DroneInfo Swarmling = new DroneInfo(body);
            Swarmlings.Remove(Swarmling);
        }
    }
}
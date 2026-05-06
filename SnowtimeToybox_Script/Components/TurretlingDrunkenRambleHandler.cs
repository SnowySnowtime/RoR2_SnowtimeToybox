
using RoR2;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterBody))]
public class TurretlingDrunkenRamblingHandler : NetworkBehaviour
{
    public CharacterBody self;
    public CharacterBody ownerBody;
    public CharacterMaster charMaster;
    public bool bodiesLogged;
    public string ownerName;
    public bool timeToRamble;
    public float rambleChance;
    public float timeSinceLastRamble = 999f;
    public float timeBetweenRambles = 12f;

    public void Awake()
    {
        self = gameObject.GetComponent<CharacterBody>();
        bodiesLogged = false;
        rambleChance = 0.05f;
        timeSinceLastRamble = 12f;
    }

    public void FixedUpdate()
    {
        if (!NetworkServer.active) return;
        
        timeSinceLastRamble += Time.fixedDeltaTime;
        
        if (!bodiesLogged)
        {
            if (!self.master) return;
            charMaster = self.master;
            
            if (!charMaster?.minionOwnership?.ownerMaster?.GetBody()) return;
            ownerBody = charMaster.minionOwnership.ownerMaster.GetBody();

            //Log.Debug(ownerBody.name + " turretling ");
            ownerName = ownerBody.name.Replace("(Clone)", "");
            bodiesLogged = true;
        }
        
        if (ownerName.Contains("Demolisher") && (gameObject.name.Contains("_DManTurretlingBody")) || gameObject.name.Contains("_DemoTurretlingBody"))
        {
            if (timeSinceLastRamble < timeBetweenRambles) return;

            if(SnowtimeToyboxMod.TurretlingGibberishChance.Value >= Run.instance.runRNG.RangeFloat(0, 100))
            {
                Ramble();
                RpcRamble();
            }

            timeSinceLastRamble = 0f;
        }
    }

    private void Ramble()
    {
        Log.Debug("playings sound .,.");
        Util.PlaySound("Play_Demoman_Gibberish", gameObject);
    }
    
    [ClientRpc]
    private void RpcRamble()
    {
        Ramble();
    }
}
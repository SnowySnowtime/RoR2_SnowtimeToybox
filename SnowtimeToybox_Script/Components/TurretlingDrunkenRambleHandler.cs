using IL.RoR2.Achievements.Railgunner;
using RoR2;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterBody))]
public class TurretlingDrunkenRamblingHandler : MonoBehaviour
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
        timeSinceLastRamble = 999f;
    }

    public void FixedUpdate()
    {
        timeSinceLastRamble += Time.fixedDeltaTime;
        if (!bodiesLogged)
        {
            if (!self.master) return;
            charMaster = self.master;
            if (!charMaster.minionOwnership.ownerMaster) return;
            if (!charMaster.minionOwnership.ownerMaster.GetBody()) return;
            //Log.Debug(charBody.master.minionOwnership.ownerMaster.GetBody().name);
            ownerBody = charMaster.minionOwnership.ownerMaster.GetBody();

            //Log.Debug(ownerBody.name + " turretling ");
            ownerName = ownerBody.name.Replace("(Clone)", "");
            bodiesLogged = true;
        }
        if (ownerName.Contains("Demolisher") && (gameObject.name.Contains("_DManTurretlingBody")) || gameObject.name.Contains("_DemoTurretlingBody"))
        {
            if (timeSinceLastRamble < timeBetweenRambles) return;

            timeToRamble = SnowtimeToyboxMod.TurretlingRainbowChance.Value >= Run.instance.runRNG.RangeFloat(0, 100);
            if(timeToRamble)
            {
                Ramble();
            }
        }
    }

    private void Ramble()
    {
        timeSinceLastRamble = 0f;
        Util.PlaySound("Play_Demoman_Gibberish", gameObject);
    }
}
using IL.RoR2.Achievements.Railgunner;
using RoR2;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterMaster))]
public class PassiveTurretlingUpdateNamePerCharacter : MonoBehaviour
{
    public CharacterMaster self;
    public CharacterBody ownerBody;
    public CharacterBody charBody;
    public bool nameUpdated;

    public void Awake()
    {
        self = GetComponent<CharacterMaster>();
        nameUpdated = false;
    }

    public void FixedUpdate()
    {
        if (!self.GetBody()) return;
        charBody = self.GetBody();
        if (!charBody.master.minionOwnership.ownerMaster) return;
        if (!charBody.master.minionOwnership.ownerMaster.GetBody()) return;
        //Log.Debug(charBody.master.minionOwnership.ownerMaster.GetBody().name);
        ownerBody = charBody.master.minionOwnership.ownerMaster.GetBody();
        if (ownerBody.gameObject.name.Contains("RailgunnerBody"))
        {
            nameUpdated = true;
            charBody.baseNameToken = "TURRETLING_RAIL_NAME";
        }
        else if(ownerBody.gameObject.name.Contains("MercBody"))
        {
            nameUpdated = true;
            charBody.baseNameToken = "TURRETLING_MERC_NAME";
        }
        else if(ownerBody.gameObject.name.Contains("MageBody"))
        {
            nameUpdated = true;
            charBody.baseNameToken = "TURRETLING_ARTI_NAME_IG";
        }
        else
        {
            nameUpdated = true;
            //Log.Debug("We are someone who shouldnt be/have this turretling.");
        }
    }
}
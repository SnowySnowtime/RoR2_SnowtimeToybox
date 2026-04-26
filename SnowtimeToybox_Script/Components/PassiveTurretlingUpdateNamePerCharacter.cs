using IL.RoR2.Achievements.Railgunner;
using RoR2;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterBody))]
public class PassiveTurretlingUpdateNamePerCharacter : MonoBehaviour
{
    public CharacterBody self;
    public CharacterBody ownerBody;
    public CharacterMaster charMaster;
    public bool nameUpdated;

    public void Awake()
    {
        self = gameObject.GetComponent<CharacterBody>();
        nameUpdated = false;
    }

    public void FixedUpdate()
    {
        if (nameUpdated) return;
        if (!self.master) return;
        charMaster = self.master;
        if (!charMaster.minionOwnership.ownerMaster) return;
        if (!charMaster.minionOwnership.ownerMaster.GetBody()) return;
        //Log.Debug(charBody.master.minionOwnership.ownerMaster.GetBody().name);
        ownerBody = charMaster.minionOwnership.ownerMaster.GetBody();
        
        Log.Debug(ownerBody.name + " turretling ");
        string ownerName = ownerBody.name.Replace("(Clone)", "");
        self.baseNameToken = $"TURRETLING_{ownerName.ToUpper()}_NAME";
        nameUpdated = true;
    }
}
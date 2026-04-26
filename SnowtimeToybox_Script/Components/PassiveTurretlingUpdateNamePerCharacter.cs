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
        if (nameUpdated) return;
        if (!self.GetBody()) return;
        charBody = self.GetBody();
        if (!charBody.master.minionOwnership.ownerMaster) return;
        if (!charBody.master.minionOwnership.ownerMaster.GetBody()) return;
        //Log.Debug(charBody.master.minionOwnership.ownerMaster.GetBody().name);
        ownerBody = charBody.master.minionOwnership.ownerMaster.GetBody();
        
        Log.Debug(ownerBody.name + " turretling ");
        charBody.baseNameToken = $"TURRETLING_{ownerBody.name.ToUpper()}_NAME";
        nameUpdated = true;
    }
}
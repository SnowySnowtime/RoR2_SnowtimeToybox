using RoR2;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterMaster))]
public class PlayerTurretlingRainbowHandler : MonoBehaviour
{
    public CharacterMaster self;
    public CharacterBody charBody;
    public GameObject charBodyObject;
    public bool addedRainbowizer;

    public void Awake()
    {
        enabled = NetworkServer.active;
        self = GetComponent<CharacterMaster>();
        addedRainbowizer = false;
    }

    public void FixedUpdate()
    {
        if (!NetworkServer.active) return;
        if (!self.GetBody()) return;
        charBody = self.GetBody();
        charBodyObject = charBody.gameObject;
        if (!charBodyObject.name.Contains("Turretling")) return;
        if (charBodyObject.name.Contains("Body_")) return;

        if(addedRainbowizer == false)
        {
            addedRainbowizer = true;
            Log.Debug("Added Turretling Rainbowizer to... " + self + " | " + self.playerCharacterMasterController.GetDisplayName());
            self.gameObject.AddComponent<TurretlingRainbow>();
        }
    }
}
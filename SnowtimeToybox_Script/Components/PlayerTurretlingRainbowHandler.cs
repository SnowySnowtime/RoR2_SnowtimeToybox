using IL.RoR2.UI;
using RoR2;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterMaster))]
[RequireComponent(typeof(PlayerCharacterMasterController))]
public class PlayerTurretlingRainbowHandler : NetworkBehaviour
{
    public CharacterMaster self;
    public PlayerCharacterMasterController pcmc;
    public CharacterBody charBody;
    public GameObject charBodyObject;
    public bool addedRainbowizer;
    public bool idfound;
    [SyncVar]
    private string steamid = "";
    public string steamid_;

    public void Awake()
    {
        self = gameObject.GetComponent<CharacterMaster>();
        pcmc = gameObject.GetComponent<PlayerCharacterMasterController>();
    }

    public void FixedUpdate()
    {
        GetSteamID();
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
    public void GetSteamID()
    {
        if(NetworkServer.active && Run.instance)
        {
            if (!idfound)
            {
                Log.Debug("Attempting to get SteamID");
                if (!gameObject.GetComponent<PlayerCharacterMasterController>()) return;
                steamid = pcmc.networkUser.id.steamId.ToSteamID();
                steamid_ = steamid;
                Log.Debug("Player " + pcmc.GetDisplayName() + " SteamID: " + steamid);
                idfound = true;
            }
        }
    }
    public string SendSteamID()
    {
        return steamid;
    }
}
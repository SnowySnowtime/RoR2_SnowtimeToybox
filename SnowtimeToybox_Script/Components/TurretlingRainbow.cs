using System;
using System.Collections.Generic;
using BepInEx;
using EntityStates.AffixVoid;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using UnityEngine.TextCore.Text;
using Object = UnityEngine.Object;
using HG;
using System.Net;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterMaster))]
public class TurretlingRainbow : NetworkBehaviour
{
    [SyncVar]
    private float turretlingHue;
    [SyncVar]
    private float turretlingSat;
    [SyncVar]
    private float turretlingShade;
    [SyncVar]
    public bool turretlingRainbow;
    [SyncVar]
    public bool DTRainbowActive;
    [SyncVar]
    private string steamid = "";
    private bool applyTurretlingVisuals = true;

    private CharacterMaster turretlingPlayerMaster;
    private PlayerCharacterMasterController turretlingPlayer;
    private CharacterMaster master;
    private CharacterBody charBody;
    private string steamidToApply;
    public static List<CharacterBody> DTActiveTurretlings = [];
    public float myHue;
    
    public static Dictionary<string, string> turretlingRecolors = new()
    {
        // SteamID             | Hue | Sat | Shade | Prefix( + Halo/Unusual)
        { "STEAM_1:1:146751517", "0.5,0,0,Snowtime" }, // snowy
        { "STEAM_1:0:615574887", "0.05,0,0,Shortcake" }, //shortcake
        { "STEAM_1:1:60493073", "0,1,0,Acanthi" }, // canthi 
        { "STEAM_1:1:174533492", "0.87,0.87,0,Bread" }, // bread
        { "STEAM_1:0:64329810", "0.71,0.27,0.27,Illusive" }, // illusive 
        { "STEAM_1:1:891275848", "0.85,0.45,0.8,Sentry" }, // illusive 
        { "STEAM_1:1:502654116", "0.83,0,0,Anartoast" }, // anar
        { "STEAM_1:0:131809264", "0.43,0.97,0,Lukas" }, // lukas
        // Contributors!!!
        { "STEAM_1:1:35743795", "0.9,0.5,0,PlNK" }, // plnk (logbook)
        { "STEAM_1:0:24377756", "0.014,0.52,0.4,Saucy" }, // ebbysauce (logbook)
        // Indirect Contributor
        { "STEAM_1:1:48521507", "0.014,0.52,0.4,Score" }, // .score (passive skill fix, missiles, and more...)
        // Testers!!!!!
        { "STEAM_1:0:502558120", "0.32,0.3,0.99,Green" }, // green 
        { "STEAM_1:0:75897289", "0.5,0.3,0.99,Regi" }, // Regigigas 
        { "STEAM_1:1:158283592", "0.0125,0.25,0.5,VCR" }, // VCR
        { "STEAM_1:1:59278323", "0.037,0.1,0.01,F4lx" }, // F4lx/Gadreel
        { "STEAM_1:0:98120944", "0.6,0.27,0.27,Samuel" }, // Samuel
        // Mod support stuffs
        { "STEAM_1:1:33573234", "0.34,0.65,0.6,DTEE" }, // DTEE
    };
    
    public void Start()
    {
        if (gameObject.name.Contains("Broken")) return;
        
        //Log.Debug("master spawned !!");
        //Log.Debug("Object Name: " + gameObject.name);
        
        master = gameObject.GetComponent<CharacterMaster>();
        
        master.onBodyStart += MasterOnonBodyStart;  
        master.onBodyDeath.AddListener(MasterOnonBodyDeath);

        if (!NetworkServer.active) return;
        
        turretlingHue = Run.instance.runRNG.RangeFloat(0, 1);
        turretlingSat = Run.instance.runRNG.RangeFloat(0, 1);
        turretlingShade = Run.instance.runRNG.RangeFloat(0, 1);

        if (!gameObject.name.Contains("_DT") && !gameObject.name.Contains("PlayerMaster") && !gameObject.name.Contains("_Holy") && !gameObject.name.Contains("_DMan"))
        {
            turretlingRainbow = SnowtimeToyboxMod.TurretlingRainbowChance.Value >= Run.instance.runRNG.RangeFloat(0, 100);  
        }
        
        if (turretlingRainbow)
        {
            giveItems(true);
        }
    }
    
    public void FixedUpdate()
    {
        if (steamid != "" && steamidToApply != "-1")
        {
            applyTurretlingVisuals = true;
        }
        if (!master.GetBody()) return;

        if (gameObject.name.StartsWith("_DT"))
        {
            if (DTRainbowActive && !turretlingRainbow)
            {
                applyTurretlingVisuals = true;
                turretlingRainbow = true;
                DTActiveTurretlings.Add(master.GetBody());
            }
            else if(!DTRainbowActive && turretlingRainbow)
            {
                applyTurretlingVisuals = true;
                turretlingRainbow = false;
                DTActiveTurretlings.Remove(master.GetBody());
            }
        }
        
        
        if (!applyTurretlingVisuals) return;
        if (gameObject.name.Contains("PlayerMaster") && !master.GetBody().gameObject.name.Contains("Turretling"))
        {
            applyTurretlingVisuals = false;
            //Log.Debug($"{master.GetBody().name} is not a turretling !! continue ,.,.");
            return;
        };
        //Log.Debug($"running fixed updatre 9on {master.GetBody().name} dt rainbow {DTRainbowActive} rainbow {turretlingRainbow}");
        if (NetworkServer.active && Run.instance && !turretlingPlayerMaster)
        {
            if(gameObject.name.Contains("_DT") || gameObject.name.Contains("_Holy") || gameObject.name.Contains("_SwarmTurretling") || gameObject.name.Contains("_DMan"))
            {
                //Log.Debug("Operator/Artificer Turretling Found... Defining Turretling Owner Master...");
                turretlingPlayerMaster = master.minionOwnership.ownerMaster;
                if (!turretlingPlayer)
                {
                    //Log.Debug("Defining Player Controller of Owner Master...");
                    turretlingPlayer = turretlingPlayerMaster.playerCharacterMasterController;
                    //Log.Debug(turretlingPlayer);
                }
                
                if (turretlingPlayer != null)
                {
                    steamid = turretlingPlayer.networkUser.id.steamId.ToSteamID();
                    //Log.Debug($"steam id !! {steamid}");
                }
                
                // Just in case...
                if (gameObject.name.Contains("Broken"))
                {
                    if (master.inventory.GetItemCountEffective(ItemCatalog.FindItemIndex("RainbowizerPowerUp")) != 0)
                    {
                        master.inventory.RemoveItemPermanent(ItemCatalog.FindItemIndex("RainbowizerPowerUp"), master.inventory.GetItemCountEffective(RoR2Content.Items.ScrapRed));
                    }
                }
            }
            else if (gameObject.name.Contains("PlayerMaster"))
            {
                turretlingPlayerMaster = gameObject.GetComponent<CharacterMaster>();
                Log.Debug("Player found possessing Turretling, defining SteamID directly.");
                if (!gameObject.GetComponent<PlayerCharacterMasterController>()) return;
                steamid = turretlingPlayerMaster.playerCharacterMasterController.networkUser.id.steamId.ToSteamID();
                //Log.Debug("Player" + gameObject.GetComponent<PlayerCharacterMasterController>().GetDisplayName() + " SteamID: " + steamid);
                Log.Debug($"steam id !! {steamid} from player: " + gameObject.GetComponent<PlayerCharacterMasterController>().GetDisplayName());
            }
            
            if (gameObject.name.Contains("_DT") && turretlingPlayer != null || gameObject.name.Contains("_Holy") && !steamid.IsNullOrWhiteSpace() || gameObject.name.Contains("PlayerMaster") || gameObject.name.Contains("_SwarmTurretling") || gameObject.name.Contains("_DMan"))
            {
                if (turretlingRecolors.TryGetValue(steamid, out string turretlingColors))
                {
                    string[] turretlingParams = turretlingColors.Split(",");
                    
                    turretlingHue = float.Parse(turretlingParams[0]);
                    turretlingSat = float.Parse(turretlingParams[1]);
                    turretlingShade = float.Parse(turretlingParams[2]);
                }
            }
        }

        if (steamidToApply.IsNullOrWhiteSpace() && !steamid.IsNullOrWhiteSpace() && turretlingRecolors.ContainsKey(steamid))
        {
            //Log.Debug($"appling steam id {steamid} !!");
            if(gameObject.name.Contains("PlayerMaster"))
            {
                //Log.Debug("The ID was applied from" + gameObject.GetComponent<PlayerCharacterMasterController>().GetDisplayName());
            }
            steamidToApply = steamid;
            applyTurretlingVisuals = true;
        }
        
        ApplyVisuals();
    }

    public void ApplyVisuals()
    {
        //if (!gameObject?.GetComponent<CharacterMaster>()) return;
        //if (!gameObject?.GetComponent<CharacterMaster>().GetBody()) return;
        //if (!gameObject?.GetComponent<CharacterMaster>().GetBody().gameObject?.GetComponent<CharacterBody>()) return;
        if (gameObject.name.Contains("PlayerMaster") && !master.GetBody().gameObject.name.Contains("Turretling")) return;
        if (!applyTurretlingVisuals) return;
        applyTurretlingVisuals = false;

        if (gameObject.name.Contains("PlayerMaster"))
        {
            //Log.Debug("Applying visuals to player controlled turretling Player: " + gameObject.GetComponent<PlayerCharacterMasterController>().GetDisplayName());
        }
        //Log.Debug($"current steam id {steamid} !!");

        if (!charBody)
        {
            charBody = gameObject.name.Contains("PlayerMaster") ? gameObject.GetComponent<CharacterMaster>().GetBody().gameObject.GetComponent<CharacterBody>() : master.GetBody();
        }
        if (!charBody) return;
        //Log.Debug("Character Body: " + charBody);
        if (gameObject.name.Contains("PlayerMaster"))
        {
            //Log.Debug("Character Body Player Name:" + gameObject.GetComponent<PlayerCharacterMasterController>().GetDisplayName());
        }
        if (charBody.name.Contains("Broken")) return;

        if (charBody.modelLocator._modelTransform.gameObject.AsValidOrNull() == null) return;
        if (charBody.modelLocator._modelTransform?.gameObject.TryGetComponent(out ChildLocator childLocator) != true) return;
        if (childLocator == null) return;
        //Log.Debug("ChildLocator: " + childLocator.gameObject.name);

        if (!childLocator.TryFindChild("Turretling_Overlay", out Transform overlay) ||
            !childLocator.TryFindChild("Turretling_Light", out Transform light) ||
            !childLocator.TryFindChild("Turretling_RainbowFX", out Transform fx)) return;

        if (!overlay.gameObject.TryGetComponent(out Animator overlayAnimator) ||
            !light.gameObject.TryGetComponent(out Animator lightAnimator) ||
            !fx.gameObject.TryGetComponent(out Animator fxAnimator)) return;

        //Log.Debug("Overlay: " + overlay.gameObject.name);
        //Log.Debug("Light: " + light.gameObject.name);
        //Log.Debug("Fx: " + fx.gameObject.name);
        Animator[] animators =
        [
            overlayAnimator,
            lightAnimator,
            fxAnimator
        ];

        myHue = turretlingHue;
        //does this have to be like this? no ,.., but its silyl .,. ,
        //Log.Debug("Applying visuals to animators");
        foreach (var animator in animators)
        {
            //Log.Debug(animator + " is being applied with...");
            //Log.Debug(turretlingHue);
            //Log.Debug(turretlingSat);
            //Log.Debug(turretlingShade);
            animator.SetFloat("hue", turretlingRainbow ? 0 : turretlingHue);
            animator.SetFloat("sat", turretlingRainbow ? 0 : turretlingSat);
            animator.SetFloat("shade", turretlingRainbow ? 0 : turretlingShade);
            animator.SetBool("shift", turretlingRainbow);
        }

        if (steamidToApply != "-1" && turretlingRecolors.TryGetValue(steamid, out string turretling))
        {
            string[] turretlingParams = turretling.Split(",");
            //Log.Debug("Applying Halo or Unusual");
            if (turretlingParams.Length == 4)
            {
                string turretlingName = turretlingParams[^1].Trim();
                if(charBody && charBody.modelLocator?.modelTransform?.gameObject.TryGetComponent(out ChildLocator childLocatorSteamUnusualHolder) == true)
                {
                    if (childLocatorSteamUnusualHolder.FindChild("DevTesterEffectsPrefab").gameObject.TryGetComponent(out ChildLocator childLocatorSteamUnusual) == true)
                    {
                        Log.Debug(childLocatorSteamUnusual.gameObject.name);
                        childLocatorSteamUnusual.FindChild($"{turretlingName}Halo")?.gameObject.SetActive(true);
                        childLocatorSteamUnusual.FindChild($"{turretlingName}Unusual")?.gameObject.SetActive(true);
                        //Log.Debug(turretlingName + " has been applied");
                    }
                }
            }

            steamidToApply = "-1";
            //Log.Debug("applied steam id !!");
        }
    }

    public void giveItems(bool takeRemove)
    {
        if (gameObject.name.Contains("PlayerMaster")) return;
        // Do not give operator turretlings the defined rainbow turret items, item is handled separately in the case it is in revive state.
        // However, do remove the item in the case it dies during its rainbow powerup.
        if (gameObject.name.Contains("_DT"))
        {
            if (takeRemove)
            {
                if (master.inventory.GetItemCountEffective(ItemCatalog.FindItemIndex("RainbowizerPowerUp")) == 0)
                {
                    master.inventory.GiveItemPermanent(ItemCatalog.FindItemIndex("RainbowizerPowerUp"));
                }
            }
            else
            {
                if (master.inventory.GetItemCountEffective(ItemCatalog.FindItemIndex("RainbowizerPowerUp")) != 0)
                {
                    master.inventory.RemoveItemPermanent(ItemCatalog.FindItemIndex("RainbowizerPowerUp"), master.inventory.GetItemCountEffective(ItemCatalog.FindItemIndex("RainbowizerPowerUp")));
                }
            }
            return;
        }
        try
        {
            string[] bonusItems = SnowtimeToyboxMod.TurretlingRainbowBonusItems.Value.Split(",");
            for (int i = 0; i < bonusItems.Length; i += 2)
            {
                if (takeRemove)
                {
                    master.inventory.GiveItemPermanent(ItemCatalog.FindItemIndex(bonusItems[i]), int.Parse(bonusItems[i + 1]));
                    //Log.Debug($"gave turretling {bonusItems[i + 1]} {bonusItems[i]} !!!");
                }
                else
                {
                    master.inventory.RemoveItemPermanent(ItemCatalog.FindItemIndex(bonusItems[i]), int.Parse(bonusItems[i + 1]));
                    //Log.Debug($"removed turretling {bonusItems[i + 1]} {bonusItems[i]} !!!");
                }
            }
        }
        catch (Exception e)
        {
            Log.Error("something bad happened when giving turretlings extra items!!");
            Log.Error(e);
        }
    }

    private void MasterOnonBodyDeath()
    {
        if (gameObject.name.Contains("PlayerMaster")) return;
        // enough said.
        if (gameObject.name.Contains("_DT") || gameObject.name.Contains("Broken") || gameObject.name.Contains("_Holy") || gameObject.name.Contains("_SwarmTurretling") || gameObject.name.Contains("_DMan")) return;
        int extralives = master.inventory.GetItemCountPermanent(RoR2Content.Items.ExtraLife);
        ChildLocator childLocator = master.GetBody().modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
        if (turretlingRainbow && extralives != 0)
        {
            childLocator.FindChild("Turretling_RainbowFX").gameObject.SetActive(false);
        }
        if (extralives == 0 && master.GetBody() && NetworkServer.active)
        {
            if(gameObject.name.Contains("Demo"))
            {
                GameObject newTurretling = Object.Instantiate(Content.DemoTurretlingBroken, master.GetBody().transform.position, master.GetBody().transform.rotation);
                newTurretling.GetComponent<PurchaseInteraction>().cost = (int)(Run.instance.GetDifficultyScaledCost(newTurretling.GetComponent<PurchaseInteraction>().cost) * SnowtimeToyboxMod.TurretlingReviveCostMult.Value);
                NetworkServer.Spawn(newTurretling);
            }
            else
            {
                GameObject newTurretling = Object.Instantiate(Content.FriendlyTurretTurretlingBroken, master.GetBody().transform.position, master.GetBody().transform.rotation);
                newTurretling.GetComponent<PurchaseInteraction>().cost = (int)(Run.instance.GetDifficultyScaledCost(newTurretling.GetComponent<PurchaseInteraction>().cost) * SnowtimeToyboxMod.TurretlingReviveCostMult.Value);
                NetworkServer.Spawn(newTurretling);
            }
        }
    }

    public void MasterOnonBodyStart(CharacterBody body)
    {
        applyTurretlingVisuals = true;
        steamidToApply = ""; 
        //Log.Debug($"ran body start on {body.name} !!");
    }
    
    public void DTRainbowize(bool enterExit)
    {
        //turretlingRainbow = enterExit;
        DTRainbowActive = enterExit;
        //applyTurretlingVisuals = true;
        giveItems(enterExit);
    }
}
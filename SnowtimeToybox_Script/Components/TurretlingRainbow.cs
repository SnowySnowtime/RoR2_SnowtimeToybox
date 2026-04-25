using System;
using System.Collections.Generic;
using BepInEx;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using Object = UnityEngine.Object;

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
    private string steamid;

    private CharacterMaster turretlingPlayerMaster;
    private PlayerCharacterMasterController turretlingPlayer;
    private CharacterMaster master;
    private CharacterBody charBody;
    private bool firstSpawnPassed;
    
    public static Dictionary<string, string> turretlingRecolors = new()
    {
        { "STEAM_1:1:146751517", "0.55,0,0,Snowtime" }, // snowy 
        { "STEAM_0:0:615574887", "0.045,0,0,Shortcake" }, //shortcake
        { "STEAM_0:1:60493073", "0,1,0,Acanthi" }, // canthi 
        { "STEAM_0:1:174533492", "0.87,0.87,0,Bread" }, // bread
        { "STEAM_0:0:64329810", "0.71,0.27,0.27,Illusive" }, // illusive 
        { "STEAM_0:1:502654116", "0.83,0,0,Anartoast" }, // anar
        { "STEAM_0:0:131809264", "0.43,0.97,0,Lukas" }, // lukas
    };
    
    public void OnEnable()
    {
        if (!gameObject.name.Contains("Turretling")) return;
        firstSpawnPassed = false;
        Log.Debug("turretling master spawned !!");
        Log.Debug("Object Name: " + gameObject.name);
        if (gameObject.name.Contains("Broken")) return;
        master = gameObject.GetComponent<CharacterMaster>();
        Log.Debug(master);
        master.onBodyStart += MasterOnonBodyStart;
        master.onBodyDeath.AddListener(MasterOnonBodyDeath);

        if (NetworkServer.active)
        {
            turretlingHue = Run.instance.runRNG.RangeFloat(0, 1);
            turretlingSat = Run.instance.runRNG.RangeFloat(0, 1);
            turretlingShade = Run.instance.runRNG.RangeFloat(0, 1);

            Log.Debug(gameObject.name);
            if (!gameObject.name.Contains("_DT") && !gameObject.name.Contains("PlayerMaster") && !gameObject.name.Contains("_Holy"))
            {
                turretlingRainbow = SnowtimeToyboxMod.TurretlingRainbowChance.Value >= Run.instance.runRNG.RangeFloat(0, 100);
            }

            if (turretlingRainbow)
            {
                giveItems(true);
            }
        }

    }
    public void FixedUpdate()
    {
        if (NetworkServer.active && Run.instance)
        {
            //Log.Debug("erm...");
            if (!turretlingPlayerMaster)
            {
                if(gameObject.name.Contains("_DT") || gameObject.name.Contains("_Holy"))
                {
                    Log.Debug("Operator/Artificer Turretling Found... Defining Turretling Owner Master...");
                    turretlingPlayerMaster = master.minionOwnership.ownerMaster;
                    if (!turretlingPlayer)
                    {
                        Log.Debug("Defining Player Controller of Owner Master...");
                        turretlingPlayer = turretlingPlayerMaster.playerCharacterMasterController;
                        Log.Debug(turretlingPlayer);
                    }
                    if (turretlingPlayer != null)
                    {
                        steamid = turretlingPlayer.networkUser.id.steamId.ToSteamID();
                        Log.Debug($"steam id !! {steamid}");
                        Log.Debug($"steam id 2!! {turretlingPlayer.networkUser.id.steamId}");
                        Log.Debug($"steam id !3! {turretlingPlayer.networkUser.id}");
                        Log.Debug($"steam id !4! {turretlingPlayer.networkUser.id.steamId.value}");
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
                    //Log.Debug("Player found possessing Turretling, defining SteamID directly.");
                    if (!gameObject.GetComponent<PlayerCharacterMasterController>()) return;
                    steamid = gameObject.GetComponent<PlayerCharacterMasterController>().networkUser.id.steamId.ToSteamID();
                    //Log.Debug("Player" + gameObject.GetComponent<PlayerCharacterMasterController>().GetDisplayName() + " SteamID: " + steamid);
                }
                
                if (gameObject.name.Contains("_DT") && turretlingPlayer != null || gameObject.name.Contains("_Holy") && !steamid.IsNullOrWhiteSpace() || gameObject.name.Contains("PlayerMaster"))
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
        }

        if (firstSpawnPassed) return;
        
        Log.Debug("This firstSpawnPassed check ran!");
        if (gameObject.name.Contains("PlayerMaster"))
        {
            charBody = gameObject.GetComponent<CharacterMaster>().GetBody().gameObject.GetComponent<CharacterBody>();
        }
        else
        {
            charBody = master.GetBody();
        }

        if (!charBody) return;
        if (charBody.name.Contains("Broken")) return;
        
        ChildLocator childLocator = charBody.modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
        if (childLocator == null) return;

        if (!childLocator.TryFindChild("Turretling_Overlay", out Transform overlay) ||
            !childLocator.TryFindChild("Turretling_Light", out Transform light) ||
            !childLocator.TryFindChild("Turretling_RainbowFX", out Transform fx)) return;

        if (!overlay.gameObject.TryGetComponent(out Animator overlayAnimator) ||
            !light.gameObject.TryGetComponent(out Animator lightAnimator) ||
            !fx.gameObject.TryGetComponent(out Animator fxAnimator)) return;

        Animator[] animators =
        [
            overlayAnimator,
            lightAnimator,
            fxAnimator
        ];

        //does this have to be like this? no ,.., but its silyl .,. ,
        foreach (var animator in animators)
        {
            animator.SetFloat("hue", turretlingRainbow ? 0 : turretlingHue);
            animator.SetFloat("sat", turretlingRainbow ? 0 : turretlingSat);
            animator.SetFloat("shade", turretlingRainbow ? 0 : turretlingShade);
            animator.SetBool("shift", turretlingRainbow);
        }

        if (turretlingRecolors.TryGetValue(steamid, out string turretling))
        {
            string[] turretlingParams = turretling.Split(",");
            if (turretlingParams.Length == 4)
            {
                string turretlingName = turretlingParams[^1].Trim();
                
                childLocator.FindChild($"{turretlingName}Halo")?.gameObject.SetActive(true);
                childLocator.FindChild($"{turretlingName}Unusual")?.gameObject.SetActive(true);
            }
        }
        
        firstSpawnPassed = true;
    }

    public void giveItems(bool takeRemove)
    {
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
                    Log.Debug($"gave turretling {bonusItems[i + 1]} {bonusItems[i]} !!!");
                }
                else
                {
                    master.inventory.RemoveItemPermanent(ItemCatalog.FindItemIndex(bonusItems[i]), int.Parse(bonusItems[i + 1]));
                    Log.Debug($"removed turretling {bonusItems[i + 1]} {bonusItems[i]} !!!");
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
        // enough said.
        if (gameObject.name.Contains("_DT") || gameObject.name.Contains("Broken") || gameObject.name.Contains("_Holy")) return;
        int extralives = master.inventory.GetItemCountPermanent(RoR2Content.Items.ExtraLife);
        ChildLocator childLocator = master.GetBody().modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
        if (turretlingRainbow && extralives != 0)
        {
            childLocator.FindChild("Turretling_RainbowFX").gameObject.SetActive(false);
        }
        if (extralives == 0 && master.GetBody() && NetworkServer.active)
        {
            GameObject newTurretling = Object.Instantiate(SnowtimeToyboxMod.FriendlyTurretTurretlingBroken, master.GetBody().transform.position, master.GetBody().transform.rotation);
            newTurretling.GetComponent<PurchaseInteraction>().cost = (int)(Run.instance.GetDifficultyScaledCost(newTurretling.GetComponent<PurchaseInteraction>().cost) * SnowtimeToyboxMod.TurretlingReviveCostMult.Value);
            NetworkServer.Spawn(newTurretling);
        }
    }

    public void MasterOnonBodyStart(CharacterBody body)
    {
        firstSpawnPassed = false;
        /*// try to prevent it from keeping the item on map change or revive
        if (master.inventory.GetItemCountEffective(ItemCatalog.FindItemIndex("RainbowizerPowerUp")) != 0)
        {
            master.inventory.RemoveItemPermanent(ItemCatalog.FindItemIndex("RainbowizerPowerUp"), master.inventory.GetItemCountEffective(ItemCatalog.FindItemIndex("RainbowizerPowerUp")));
        }
        // dont run code if we're operator turretlings and we're being revived.
        if (body.name.Contains("Broken")) return;
        ChildLocator childLocator = body.modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
        if (childLocator == null) return;
        GameObject overlay = childLocator.FindChild("Turretling_Overlay").gameObject;
        Animator overlayAnimator = overlay.GetComponent<Animator>();
        GameObject light = childLocator.FindChild("Turretling_Light").gameObject;
        Animator lightAnimator = light.GetComponent<Animator>();
        GameObject fx = childLocator.FindChild("Turretling_RainbowFX").gameObject;
        Animator fxAnimator = fx.GetComponent<Animator>();

        Animator[] animators =
        [
            overlayAnimator,
            lightAnimator,
            fxAnimator
        ];

        //does this have to be like this? no ,.., but its silyl .,. ,
        foreach (var animator in animators)
        {
            if (turretlingRainbow)
            {
                animator.SetFloat("hue", 0);
                animator.SetFloat("sat", 0);
                animator.SetFloat("shade", 0);
                animator.SetBool("shift", turretlingRainbow);
            }
            else
            {
                animator.SetFloat("hue", turretlingHue);
                animator.SetFloat("sat", turretlingSat);
                animator.SetFloat("shade", turretlingShade);
                animator.SetBool("shift", turretlingRainbow);
            }

        }*/
    }
}
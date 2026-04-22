using System;
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
    public bool Snowtime;
    [SyncVar]
    public bool Acanthi;
    [SyncVar]
    public bool Bread;
    [SyncVar]
    public bool Shortcake;


    private CharacterMaster turretlingPlayerMaster;
    private PlayerCharacterMasterController turretlingPlayer;
    private CharacterMaster master;
    private CharacterBody charBody;
    private string steamid;
    public void OnEnable()
    {
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

            if (!gameObject.name.Contains("_DT"))
            {
                turretlingRainbow = SnowtimeToyboxMod.TurretlingRainbowChance.Value >= Run.instance.runRNG.RangeFloat(0, 100);
            }
            
            if (Snowtime)
            {
                if (turretlingHue != 0.55f)
                {
                    turretlingHue = 0.55f;
                    turretlingSat = 0f;
                    turretlingShade = 0f;
                }
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
            if (!turretlingPlayerMaster)
            {
                Log.Debug("Defining Turretling Owner Master...");
                turretlingPlayerMaster = master.minionOwnership.ownerMaster;
                Log.Debug(turretlingPlayerMaster);
                if (!turretlingPlayer)
                {
                    Log.Debug("Defining Player Controller of Owner Master...");
                    turretlingPlayer = turretlingPlayerMaster.playerCharacterMasterController;
                    Log.Debug(turretlingPlayer);
                }

                if (gameObject.name.Contains("_DT") && turretlingPlayer != null)
                {
                    steamid = turretlingPlayer.networkUser.id.steamId.ToSteamID();

                    // Snowy Snowtime
                    if (steamid == "STEAM_1:1:146751517" && Snowtime == false)
                    {
                        Log.Debug("Snowy Snowtime -> Operator Turretling!!!!");
                        Snowtime = true;
                        if(turretlingHue != 0.55f)
                        {
                            turretlingHue = 0.55f;
                            turretlingSat = 0f;
                            turretlingShade = 0f;
                        }
                    }
                }
            }
        }
    }
    public void giveItems(bool takeRemove)
    {
        // Do not give operator turretlings items, item is handled separately in the case it is in revive state.
        // However, do remove the item in the case it dies during its rainbow powerup.
        if (gameObject.name.Contains("Broken"))
        {
            if(master.inventory.GetItemCountEffective(RoR2Content.Items.ScrapRed) != 0)
            {
                master.inventory.RemoveItemPermanent(RoR2Content.Items.ScrapRed, master.inventory.GetItemCountEffective(RoR2Content.Items.ScrapRed));
            }
        }
        if (gameObject.name.Contains("_DT")) return;
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
        if (gameObject.name.Contains("_DT") || gameObject.name.Contains("Broken")) return;
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
        charBody = body;
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
            
        }
        
        if(Snowtime == true)
        {
            childLocator.FindChild("SnowtimeHalo").gameObject.SetActive(true);
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;

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
    
    private CharacterMaster master;
    public void OnEnable()
    {
        Log.Debug("turretling master spawned !!");
        
        master = gameObject.GetComponent<CharacterMaster>();
        master.onBodyStart += MasterOnonBodyStart;
        master.onBodyDeath.AddListener(MasterOnonBodyDeath);

        if (NetworkServer.active)
        {
            turretlingHue = Run.instance.runRNG.RangeFloat(0, 1);
            turretlingSat = Run.instance.runRNG.RangeFloat(0, 1);
            turretlingShade = Run.instance.runRNG.RangeFloat(0, 1);
            if(!master.name.Contains("_DT"))
            {
                turretlingRainbow = SnowtimeToyboxMod.TurretlingRainbowChance.Value >= Run.instance.runRNG.RangeFloat(0, 100);
            }
            
            if (turretlingRainbow)
            {
                turretlingHue = 0f;
                turretlingSat = 0f;
                turretlingShade = 0f;
                giveItems(true);
            }
        }

    }

    public void giveItems(bool takeRemove)
    {
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
        ChildLocator childLocator = body.modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
        GameObject overlay = childLocator.FindChild("Turretling_Overlay").gameObject;
        Animator overlayAnimator = overlay.GetComponent<Animator>();
        GameObject light = childLocator.FindChild("Turretling_Light").gameObject;
        Animator lightAnimator = light.GetComponent<Animator>();
        
        Animator[] animators =
        [
            overlayAnimator,
            lightAnimator
        ];
        
        //does this have to be like this? no ,.., but its silyl .,. ,
        foreach (var animator in animators)
        {
            if (turretlingRainbow)
            {
                animator.SetFloat("hue", turretlingHue);
                animator.SetFloat("sat", turretlingSat);
                animator.SetFloat("shade", turretlingShade);
                animator.SetBool("shift", turretlingRainbow);
            }
            else
            {
                animator.SetFloat("hue", 0);
                animator.SetFloat("sat", 0);
                animator.SetFloat("shade", 0);
                animator.SetBool("shift", turretlingRainbow);
            }
            
        }
        
        childLocator.FindChild("Turretling_RainbowFX").gameObject.SetActive(turretlingRainbow);
    }
}
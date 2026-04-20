using System;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;

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
    private bool turretlingEpicWin;
    public bool turretlingRainbow;
    
    private CharacterMaster master;
    public void OnEnable()
    {
        Log.Debug("turretling master spawned !!");
        
        master = gameObject.GetComponent<CharacterMaster>();
        master.onBodyStart += MasterOnonBodyStart;

        if (NetworkServer.active)
        {
            turretlingHue = Run.instance.runRNG.RangeFloat(0, 1);
            turretlingSat = Run.instance.runRNG.RangeFloat(0, 1);
            turretlingShade = Run.instance.runRNG.RangeFloat(0, 1);
            turretlingEpicWin = SnowtimeToyboxMod.TurretlingRainbowChance.Value >= Run.instance.runRNG.RangeFloat(0, 100);
            turretlingRainbow = false;
            
            if (turretlingEpicWin)
            {
                turretlingSat = 0f;
                turretlingShade = 0f;
                turretlingRainbow = true;
                try
                {
                    string[] bonusItems = SnowtimeToyboxMod.TurretlingRainbowBonusItems.Value.Split(",");
                    for (int i = 0; i < bonusItems.Length; i += 2)
                    {
                        master.inventory.GiveItemPermanent(ItemCatalog.FindItemIndex(bonusItems[i]), int.Parse(bonusItems[i + 1]));
                        Log.Debug($"gave turretling {bonusItems[i + 1]} {bonusItems[i]} !!!");
                    }
                }
                catch (Exception e)
                {
                    Log.Error("something bad happened when giving turretlings extra items!!");
                    Log.Error(e);
                }
            }
        }

    }

    private void MasterOnonBodyStart(CharacterBody body)
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
            animator.SetFloat("hue", turretlingHue);
            animator.SetFloat("sat", turretlingSat);
            animator.SetFloat("shade", turretlingShade);
            animator.SetBool("shift", turretlingEpicWin);
        }
    }
}
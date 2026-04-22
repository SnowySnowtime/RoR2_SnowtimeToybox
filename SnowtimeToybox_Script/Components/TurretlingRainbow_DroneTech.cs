using System;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using Object = UnityEngine.Object;
using System.Net;

namespace SnowtimeToybox.Components;

public class DTTurretlingRainbow : NetworkBehaviour
{
    [SyncVar]
    private float turretlingHue;
    [SyncVar]
    private float turretlingSat;
    [SyncVar]
    private float turretlingShade;
    [SyncVar]
    private bool turretlingEmpowered;
    
    private CharacterMaster master;
    private CharacterBody characterBody;
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
            turretlingEmpowered = false;
        }
    }

    // Initial Shenanigans
    private void MasterOnonBodyStart(CharacterBody body)
    {
        characterBody = body;
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
        }
    }

    public void UpdateItPLEASE()
    {
        ChildLocator childLocator = characterBody.modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
        GameObject overlay = childLocator.FindChild("Turretling_Overlay").gameObject;
        Animator overlayAnimator = overlay.GetComponent<Animator>();
        GameObject light = childLocator.FindChild("Turretling_Light").gameObject;
        Animator lightAnimator = light.GetComponent<Animator>();
        // Null check and apply; should only try to apply once.
        Log.Debug("Updating...");
        Log.Debug("Bruh");
        if (master.inventory.GetItemCountEffective(RoR2Content.Items.ScrapRed) != 0)
        {
            turretlingEmpowered = true;
        }
        else
        {
            turretlingEmpowered = false;
        }

        Animator[] animators =
        [
            overlayAnimator,
            lightAnimator
        ];

        //does this have to be like this? no ,.., but its silyl .,. ,
        //Log.Debug(turretlingHue);
        //Log.Debug(turretlingSat);
        //Log.Debug(turretlingShade);
        foreach (var animator in animators)
        {
            animator.SetBool("shift", turretlingEmpowered);
            if (turretlingEmpowered == true)
            {

                animator.SetFloat("hue", 0f);
                animator.SetFloat("sat", 0f);
                animator.SetFloat("shade", 0f);
            }
            else if (animator.GetFloat("hue") != turretlingHue && turretlingEmpowered == false)
            {
                animator.SetFloat("hue", turretlingHue);
                animator.SetFloat("sat", turretlingSat);
                animator.SetFloat("shade", turretlingShade);
            }
        }

        if (turretlingEmpowered == true)
        {
            childLocator.FindChild("Turretling_RainbowFX").gameObject.SetActive(true);
        }
        else
        {
            childLocator.FindChild("Turretling_RainbowFX").gameObject.SetActive(false);
        }
    }
}
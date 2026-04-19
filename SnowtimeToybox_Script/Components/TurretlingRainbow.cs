using System;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;
using static RoR2.UI.CarouselController;
using RoR2.Hologram;
namespace SnowtimeToybox.Components;

public class TurretlingRainbow : NetworkBehaviour
{
    [SyncVar]
    private Color turretlingLights;
    [SyncVar]
    private Color turretlingOther;

    private CharacterMaster master;
    public void OnEnable()
    {
        Log.Debug("turretling master spawned !! i think this one is kept through stages .,.,");
        
        if (NetworkServer.active)
        {
            turretlingLights = new Color(Run.instance.runRNG.RangeFloat(0, 1), Run.instance.runRNG.RangeFloat(0, 1), Run.instance.runRNG.RangeFloat(0, 1), 1);
            turretlingOther = new Color(Run.instance.runRNG.RangeFloat(0, 1), Run.instance.runRNG.RangeFloat(0, 1), Run.instance.runRNG.RangeFloat(0, 1), 1);
        }

        master = gameObject.GetComponent<CharacterMaster>();
        master.onBodyStart += MasterOnonBodyStart;
    }

    private void MasterOnonBodyStart(CharacterBody body)
    {
        Log.Debug($"turretling body spawneds !! {turretlingLights} lights and {turretlingOther} other ,.,");
        ChildLocator childLocator = body.modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
        GameObject overlay = childLocator.FindChild("Turretling_Overlay").gameObject;
        overlay.GetComponent<SkinnedMeshRenderer>().GetMaterial().color = turretlingOther;
        GameObject light = childLocator.FindChild("Turretling_Light").gameObject;
        Log.Debug(light);
        Log.Debug(light.GetComponent<SkinnedMeshRenderer>());
        Log.Debug(light.GetComponent<SkinnedMeshRenderer>().GetMaterial());
        Log.Debug(light.GetComponent<SkinnedMeshRenderer>().GetMaterial().color);
        light.GetComponent<SkinnedMeshRenderer>().GetMaterial().color = turretlingOther;
        Log.Debug(light.GetComponent<SkinnedMeshRenderer>().GetMaterial().color);
        //body.modelLocator.GetComponent<CharacterModel>().baseRendererInfos[0] // 0 = weapon, 1 = leg (base mat, 2 = base (borbolight mat 
    }
}
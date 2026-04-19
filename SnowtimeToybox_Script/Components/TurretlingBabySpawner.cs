using System;
using System.Collections;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using Stage = On.RoR2.Stage;

namespace SnowtimeToybox.Components;

public class TurretlingBabySpawner : MonoBehaviour
{
    public GameObject turretlingPrefab;
    private CharacterMaster turretlingMaster;
    private CharacterMaster master;
    private bool enableTurretlings;
    public void OnEnable()
    {
        master = this.gameObject.GetComponent<CharacterMaster>();
        master.onBodyStart += MasterOnonBodyStart;
        if (NetworkServer.active && SnowtimeToybox.SnowtimeToyboxMod.TurretlingSpawnChance.Value >= Run.instance.runRNG.RangeFloat(0, 100))
        {
            enableTurretlings = true;
        }
    }

    private void MasterOnonBodyStart(CharacterBody body)
    {
        if (turretlingMaster || !enableTurretlings) return;
        
        Log.Debug("sppasnwing  turretling !! ");
        //stolen froms OpenSummonReturnMaster .., 
        turretlingMaster = new MasterSummon
        {
            masterPrefab = turretlingPrefab,
            position = base.transform.position + Vector3.up * 10,
            rotation = base.transform.rotation,
            summonerBodyObject = master.GetBodyObject(),
            ignoreTeamMemberLimit = true,
            useAmbientLevel = true,
            teamIndexOverride = master.teamIndex
        }.Perform();
            
        if (turretlingMaster)
        {
            turretlingMaster.inventory.GiveItemPermanent(DLC3Content.Items.DroneUpgradeHidden, master.inventory.GetItemCountEffective(DLC3Content.Items.DroneUpgradeHidden));
            GameObject bodyObject = turretlingMaster.GetBodyObject();
            if (bodyObject)
            {
                ModelLocator modelLocator = bodyObject.GetComponent<ModelLocator>();
                if (modelLocator && modelLocator.modelTransform)
                {
                    TemporaryOverlayInstance temporaryOverlayInstance = TemporaryOverlayManager.AddOverlay(modelLocator.modelTransform.gameObject);
                    temporaryOverlayInstance.duration = 0.5f;
                    temporaryOverlayInstance.animateShaderAlpha = true;
                    temporaryOverlayInstance.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlayInstance.destroyComponentOnEnd = true;
                    temporaryOverlayInstance.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matSummonDrone");
                    temporaryOverlayInstance.AddToCharacterModel(modelLocator.modelTransform.GetComponent<CharacterModel>());
                }
            }
        }
    }
}
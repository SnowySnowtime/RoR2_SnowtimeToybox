using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace SnowtimeToybox.Components;

public class TurretlingKillNormalTurrets : MonoBehaviour
{
    public void OnEnable()
    {
        if (!SnowtimeToyboxMod.TurretlingKillOriginalTurrets.Value || !NetworkServer.active) return;
        
        GameObject newReplacementTurretling = Instantiate(SnowtimeToyboxMod.FriendlyTurretTurretlingBroken, gameObject.transform.position, gameObject.transform.rotation);
        newReplacementTurretling.GetComponent<PurchaseInteraction>().automaticallyScaleCostWithDifficulty = true;
        NetworkServer.Spawn(newReplacementTurretling);
        Destroy(gameObject);
    }
}
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
        
        GameObject newTurretling = Instantiate(SnowtimeToyboxMod.FriendlyTurretTurretlingBroken, gameObject.transform.position, gameObject.transform.rotation);
        newTurretling.GetComponent<PurchaseInteraction>().cost = Run.instance.GetDifficultyScaledCost(newTurretling.GetComponent<PurchaseInteraction>().cost);
        NetworkServer.Spawn(newTurretling);
        Destroy(gameObject);
    }
}
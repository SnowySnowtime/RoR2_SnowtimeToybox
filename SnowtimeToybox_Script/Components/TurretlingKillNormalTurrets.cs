using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace SnowtimeToybox.Components;

public class TurretlingKillNormalTurrets : MonoBehaviour
{
    public bool rareReplaceDemoman;
    public void OnEnable()
    {
        if (!NetworkServer.active) return;
        if (gameObject.name.Contains("Turretling"))
        {
            rareReplaceDemoman = SnowtimeToyboxMod.TurretlingDemoChance.Value >= Run.instance.runRNG.RangeFloat(0, 100);
            if (rareReplaceDemoman)
            {
                GameObject demoTurretling = Instantiate(SnowtimeToyboxMod.DemoTurretlingBroken, gameObject.transform.position, gameObject.transform.rotation);
                demoTurretling.GetComponent<PurchaseInteraction>().cost = Run.instance.GetDifficultyScaledCost(demoTurretling.GetComponent<PurchaseInteraction>().cost);
                NetworkServer.Spawn(demoTurretling);
                Destroy(gameObject);
            }
        }
        else if (!gameObject.name.Contains("Turretling"))
        {
            if (!SnowtimeToyboxMod.TurretlingKillOriginalTurrets.Value) return;
            GameObject newTurretling = Instantiate(SnowtimeToyboxMod.FriendlyTurretTurretlingBroken, gameObject.transform.position, gameObject.transform.rotation);
            newTurretling.GetComponent<PurchaseInteraction>().cost = Run.instance.GetDifficultyScaledCost(newTurretling.GetComponent<PurchaseInteraction>().cost);
            NetworkServer.Spawn(newTurretling);
            Destroy(gameObject);
        }

    }
}
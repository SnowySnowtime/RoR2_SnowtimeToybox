using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;

public class TurretlingKillNormalTurrets : MonoBehaviour
{
    public void OnEnable()
    {
        if (!SnowtimeToyboxMod.TurretlingKillOriginalTurrets.Value) return;
        
        Instantiate(SnowtimeToyboxMod.FriendlyTurretTurretlingBroken, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(gameObject);
    }
}
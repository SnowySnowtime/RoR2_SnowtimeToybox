
using RoR2;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterBody))]
public class SwarmPlayerOSPHandler : MonoBehaviour
{
    public CharacterBody self;

    public void Awake()
    {
        self = gameObject.GetComponent<CharacterBody>();
    }
    public void OnEnable()
    {
        self.onRecalculateStats += onBodyRecalculateStats;
    }
    public void OnDisable()
    {
        self.onRecalculateStats -= onBodyRecalculateStats;
    }
    public void OnDestroy()
    {
        self.onRecalculateStats -= onBodyRecalculateStats;
    }
    private void onBodyRecalculateStats(CharacterBody body)
    {
        //Log.Debug("ughhhhhh");
        if(body.isPlayerControlled)
        {
            if (!body.GetComponent<SwarmPlayerOSPHandler>()) return;
            body.hasOneShotProtection = SnowtimeToyboxMod.SwarmlingOSP.Value;
        }
    }
}
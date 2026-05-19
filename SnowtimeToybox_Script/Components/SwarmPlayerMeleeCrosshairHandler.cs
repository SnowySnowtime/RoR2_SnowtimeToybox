
using RoR2;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterBody))]
public class SwarmPlayerMeleeCrosshairHandler : MonoBehaviour
{
    public CharacterBody self;

    public void Awake()
    {
        self = gameObject.GetComponent<CharacterBody>();
    }
    private void FixedUpdate()
    {
        //erm
    }
}
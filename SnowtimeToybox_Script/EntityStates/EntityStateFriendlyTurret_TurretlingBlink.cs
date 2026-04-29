using EntityStates.Huntress;
using RoR2;
using SnowtimeToybox.Components;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingMiniBlinkState : BlinkState
    {
        public SwarmPlayerSwarmlingTracker swarmlingTracker;
        public List<CharacterBody> Swarmlings = new List<CharacterBody>();
        public override void OnEnter()
        {
            duration = 0.1f;
            speedCoefficient = 15f;
            beginSoundString = "Play_huntress_shift_mini_blink";
            endSoundString = "Play_huntress_shift_end";
            base.OnEnter();
        }
        public override void OnExit()
        {
            swarmlingTracker = gameObject.GetComponent<SwarmPlayerSwarmlingTracker>();
            Swarmlings = swarmlingTracker.GetSwarmlingBodies();
            foreach(CharacterBody body in Swarmlings)
            {
                if (!body) return;
                if (!body.gameObject) return;
                if (!body.gameObject?.GetComponent<SwarmMinionSwarmlingTeleportHandler>()) return;
                body.gameObject.GetComponent<SwarmMinionSwarmlingTeleportHandler>().StartTeleporting();
            }
            base.OnExit();
        }
        public override Vector3 GetBlinkVector()
        {
            return ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
        }
    }
}
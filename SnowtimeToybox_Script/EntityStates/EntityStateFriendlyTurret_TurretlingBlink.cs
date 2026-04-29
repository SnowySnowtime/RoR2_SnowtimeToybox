using EntityStates.Huntress;
using RoR2;
using SnowtimeToybox.Components;
using System.Collections.Generic;
using SnowtimeToybox;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingMiniBlinkState : BlinkState
    {
        public SwarmPlayerSwarmlingTracker swarmlingTracker;
        public List<CharacterBody> Swarmlings = [];
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
            if (!isAuthority)
            {
                Log.Debug("wasnt authority when tried to blink !! nuh uh .,,. ");
            };
            
            Log.Debug("1");
            swarmlingTracker = gameObject.GetComponent<SwarmPlayerSwarmlingTracker>();
            Swarmlings = swarmlingTracker.GetSwarmlingBodies();
            Log.Debug("2");
            swarmlingTracker.teleportSwarmling(gameObject.transform.position);
            //foreach(CharacterBody body in Swarmlings)
            //{
            //    if (body?.gameObject.TryGetComponent(out SwarmMinionSwarmlingTeleportHandler teleportHandler) != true) continue;
            //    Log.Debug("3");
            //    teleportHandler.teleportSwarmling();
            //}
            base.OnExit();
        }

       
        public override Vector3 GetBlinkVector()
        {
            return ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
        }
    }
}
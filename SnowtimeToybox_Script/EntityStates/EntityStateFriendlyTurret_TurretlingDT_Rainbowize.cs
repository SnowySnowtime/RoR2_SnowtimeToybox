using EntityStates;
using RoR2;
using RoR2.Orbs;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class DTTurretlingRainbowize : BaseState
    {
        public float baseDuration = 5f;

        private float duration;

        private TurretlingRainbow rainbowComponent;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration;
            //Log.Debug("DT rainbowize enter ");
            
            if (characterBody.master.TryGetComponent(out rainbowComponent) && NetworkServer.active)
            {
                rainbowComponent.DTRainbowize(true);
            }
            else
            {
                Log.Debug("rainbow null !! ");
            }
            
            if (base.gameObject.name.Contains("RemoteOp")) return;
            characterBody.GetComponent<DroneCommandReceiver>().droneState = DroneCommandReceiver.DroneState.Idle;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            if (rainbowComponent != null && NetworkServer.active)
            {
                Log.Debug($"setting dt rainbow false !!");
                rainbowComponent.DTRainbowize(false);
            }
            else
            {
                Log.Debug("rainbow null !! exit");
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.gameObject.name.Contains("RemoteOp") || base.gameObject.name.Contains("_Holy") || base.gameObject.name.Contains("PlayerMaster"))
            {
                outer.SetNextStateToMain();
            }
            else if (base.fixedAge > duration && base.isAuthority)
            {
                characterBody.GetComponent<DroneCommandReceiver>().droneState = DroneCommandReceiver.DroneState.Idle;
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
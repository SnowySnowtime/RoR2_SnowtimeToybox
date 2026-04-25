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
            characterBody.master.TryGetComponent(out rainbowComponent);
            
            if (rainbowComponent != null)
            {
                rainbowComponent.turretlingRainbow = true;
                rainbowComponent.MasterOnonBodyStart(characterBody);
                rainbowComponent.giveItems(true);
            }
            else
            {
                Log.Debug("rainboe null !! ");
            }
            if (base.gameObject.name.Contains("RemoteOp")) return;
            characterBody.GetComponent<DroneCommandReceiver>().droneState = DroneCommandReceiver.DroneState.Idle;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            if (rainbowComponent != null)
            {
                rainbowComponent.turretlingRainbow = false;
                rainbowComponent.MasterOnonBodyStart(characterBody);
                rainbowComponent.giveItems(false);
            }
            else
            {
                Log.Debug("rainboe null !!exyt  ");
            }
            
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.gameObject.name.Contains("RemoteOp"))
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
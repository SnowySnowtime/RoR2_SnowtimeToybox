// RoR2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// EntityStates.Huntress.MiniBlinkState
using EntityStates.Huntress;
using UnityEngine;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingMiniBlinkState : BlinkState
    {
        public override void OnEnter()
        {
            duration = 0.1f;
            speedCoefficient = 10f;
            beginSoundString = "Play_huntress_shift_mini_blink";
            endSoundString = "Play_huntress_shift_end";
            base.OnEnter();
        }
        public override Vector3 GetBlinkVector()
        {
            return ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
        }
    }
}
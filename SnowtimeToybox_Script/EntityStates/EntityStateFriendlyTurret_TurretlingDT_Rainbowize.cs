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
        public float baseDuration = 7f;

        private float duration;

        private string item = "ScrapRed";

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration;
            base.characterBody.master.inventory.GiveItemPermanent(ItemCatalog.FindItemIndex(item));
        }

        public override void OnExit()
        {
            base.OnExit();
            base.characterBody.master.inventory.RemoveItemPermanent(ItemCatalog.FindItemIndex(item));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
using R2API;
using RoR2;
using SnowtimeToybox;
using System;
using UnityEngine;

namespace SnowtimeToybox.Buffs
{
    public class AcanthiBleeding : BuffBase<AcanthiBleeding>
    {
        public override BuffDef Buff => SnowtimeToyboxMod.AcanthiTurretDebuff;
        public static DamageAPI.ModdedDamageType Acanthi_ButHeresTheBleeder = DamageAPI.ReserveDamageType();
        public DotController.DotDef AcanthiDot;
        public DotController.DotIndex AcanthiDotIndex;

        public override void PostCreation()
        {
            AcanthiDot = new();
            AcanthiDot.associatedBuff = Buff;
            AcanthiDot.interval = 0.2f;
            AcanthiDot.damageCoefficient = 0.1f;
            AcanthiDot.terminalTimedBuff = Buff;
            AcanthiDot.terminalTimedBuffDuration = 1f;
            AcanthiDotIndex = DotAPI.RegisterDotDef(AcanthiDot);

            On.RoR2.CharacterBody.FixedUpdate += AcanthiDebuffOverlayManager;
            On.RoR2.GlobalEventManager.OnHitEnemy += AcanthiApplyDebuff;
        }

        private void AcanthiApplyDebuff(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);

            if (damageInfo.attacker && damageInfo.HasModdedDamageType(Acanthi_ButHeresTheBleeder))
            {
                InflictDotInfo info = new();
                info.damageMultiplier = 1f;
                info.totalDamage = damageInfo.attacker.GetComponent<CharacterBody>().damage / 2f;
                info.duration = 3f;
                info.victimObject = victim;
                info.attackerObject = damageInfo.attacker;
                info.preUpgradeDotIndex = AcanthiDotIndex;
                info.dotIndex = AcanthiDotIndex;

                DotController.InflictDot(ref info);
            }
        }

        private void AcanthiDebuffOverlayManager(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
        {
            if (self.modelLocator && self.modelLocator.modelTransform && self.HasBuff(Buff))
            {
                CharacterModel waow = self.modelLocator?.modelTransform?.GetComponent<CharacterModel>(); // this had an nre somewhere .,,.. 

                var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.modelLocator.modelTransform.gameObject);
                temporaryOverlay.duration = 0.5f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Acanthi/acanthidebuffoverlay.mat");
                temporaryOverlay.inspectorCharacterModel = waow;
                temporaryOverlay.AddToCharacterModel(waow);
            }
            orig(self);
        }
    }
}
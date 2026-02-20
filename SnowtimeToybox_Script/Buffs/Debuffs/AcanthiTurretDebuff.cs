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
            AcanthiDot.damageCoefficient = 0.25f;
            AcanthiDot.terminalTimedBuff = Buff;
            AcanthiDot.terminalTimedBuffDuration = 1f;
            AcanthiDot.damageColorIndex = DamageColorIndex.SuperBleed;
            AcanthiDotIndex = DotAPI.RegisterDotDef(AcanthiDot);

            On.RoR2.CharacterBody.FixedUpdate += AcanthiDebuffOverlayManager;
            On.RoR2.GlobalEventManager.OnHitEnemy += AcanthiApplyDebuff;
        }

        private void AcanthiApplyDebuff(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (damageInfo == null) return;
            if (victim == null) return;
            if (damageInfo.attacker == null) return;

            CharacterBody characterBody = (damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null);
            if (characterBody == null) return;
            CharacterMaster master = characterBody.master;
            if(master == null) return;
            bool bleedRoll = Util.CheckRoll(damageInfo.procCoefficient*100, master.luck, master);

            if (damageInfo.attacker && damageInfo.HasModdedDamageType(Acanthi_ButHeresTheBleeder))
            {
                if (!bleedRoll) return;
                InflictDotInfo info = new();
                info.damageMultiplier = 1f;
                info.totalDamage = damageInfo.attacker.GetComponent<CharacterBody>().damage;
                info.duration = 3f;
                info.victimObject = victim;
                info.attackerObject = damageInfo.attacker;
                info.preUpgradeDotIndex = AcanthiDotIndex;
                info.dotIndex = AcanthiDotIndex;

                // waow...
                DotController.InflictDot(ref info);
                // Inflict it a second time if bread turret is buffing acanthi...
                if (!characterBody.HasBuff(SnowtimeToyboxMod.BreadTurretBuffFortune)) return;
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
using R2API;
using RoR2;
using R2API.ContentManagement;
using SnowtimeToybox;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.RecalculateStatsAPI;
using EntityStates.BrotherMonster;
using EntityStates.AffixVoid;
using R2API.AddressReferencedAssets;

namespace SnowtimeToybox.Buffs
{
    public class BreadTurretBuff : BuffBase<BreadTurretBuff>
    {
        public override BuffDef Buff => SnowtimeToyboxMod.BreadTurretBuffFortune;
        public CharacterBody buffedSelf;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBreadTurretBuff;
            On.RoR2.CharacterBody.FixedUpdate += BreadBuffFortuneOverlayManager;
        }

        // I want the damage increase to only occur if the victim has a specific buff. I dont know if this code even works.
        // I also dont know if sender is LITERALLY the character sender (looks like it should be obvious but im stupiD), or the API itself. No clue. Just gonna try this anyways.
        private void AddBreadTurretBuff(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;
            var damageIncrease = sender.baseDamage;

            args.luckAdd += 1f;
            if (!buffedSelf) return;
            if (!buffedSelf.HasBuff(SnowtimeToyboxMod.AcanthiTurretBuff)) return;
            args.damageMultAdd += (damageIncrease * 0.25f);
        }

        // aetherium being based again
        private void BreadBuffFortuneOverlayManager(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
        {
            if(self.modelLocator && self.modelLocator.modelTransform && self.HasBuff(Buff))
            {
                CharacterModel waow = self.modelLocator?.modelTransform?.GetComponent<CharacterModel>(); // this had an nre somewhere .,,.. 

                var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.modelLocator.modelTransform.gameObject);
                temporaryOverlay.duration = 0.5f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/borboturretdebuffoverlay.mat");
                temporaryOverlay.inspectorCharacterModel = waow;
                temporaryOverlay.AddToCharacterModel(waow);
            }
            buffedSelf = self;
            orig(self);
        }
    }
}
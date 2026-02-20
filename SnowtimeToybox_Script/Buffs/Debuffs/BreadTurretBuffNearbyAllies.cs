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
    public class BreadTurretBuffNearbyAllies : BuffBase<BreadTurretBuffNearbyAllies>
    {
        public override BuffDef Buff => SnowtimeToyboxMod.BreadTurretBuffNearbyAllies;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBreadTurretBuffNearby;
            On.RoR2.CharacterBody.FixedUpdate += BreadBuffNearbyOverlayManager;
        }

        private void AddBreadTurretBuffNearby(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;

            args.attackSpeedMultAdd += 1.2f;
            args.barrierDecayMult += 0.4f;
        }

        // this probably shouldnt be used for a common buff but w/e
        private void BreadBuffNearbyOverlayManager(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
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
            orig(self);
        }
    }
}
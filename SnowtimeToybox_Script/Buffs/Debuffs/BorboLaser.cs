using R2API;
using RoR2;
using R2API.ContentManagement;
using SnowtimeToybox;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.RecalculateStatsAPI;
using EntityStates.BrotherMonster;

namespace SnowtimeToybox.Buffs
{
    public class BorboTurretDebuff : BuffBase<BorboTurretDebuff>
    {
        public override BuffDef Buff => SnowtimeToyboxMod.BorboTurretDebuff;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBorboTurretDebuff;
            On.RoR2.CharacterBody.FixedUpdate += BorboSuperDebuffOverlayManager;
        }
        
        private void AddBorboTurretDebuff(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;
            
            args.armorAdd += -120f;
            args.damageTotalMult *= 0.2f;
            args.jumpPowerTotalMult *= 0f;
            args.moveSpeedTotalMult *= 0.2f;
        }

        // aetherium being based again
        private void BorboSuperDebuffOverlayManager(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
        {
            if(self.modelLocator && self.modelLocator.modelTransform && self.HasBuff(Buff))
            {
                CharacterModel waow = self.modelLocator?.modelTransform?.GetComponent<CharacterModel>(); // this had an nre somewhere .,,.. 

                var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.modelLocator.modelTransform.gameObject);
                temporaryOverlay.duration = 3;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/borboturretdebuffoverlay.mat");
                temporaryOverlay.inspectorCharacterModel = waow;
                temporaryOverlay.AddToCharacterModel(waow);
            }
            orig(self);
        }
    }
}
using EntityStates;
using IL.RoR2.UI;
using R2API;
using RoR2.ContentManagement;
using RoR2;
using RoR2.Orbs;
using RoR2.Skills;
using SnowtimeToybox;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace SnowtimeToybox
{
    public static class SnowtimeDamageTypes
    {
        public static DamageAPI.ModdedDamageType HaloRicochetOnHit = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType BorboSuperDebuffOnHit = DamageAPI.ReserveDamageType();

        public static void GlobalEventManager_onServerDamageDealt(DamageReport report)
        {
            if (report.damageInfo.HasModdedDamageType(HaloRicochetOnHit))
            {
                SnowtimeHaloRicochetOrb.CreateHaloRicochetOrb(report.damageInfo, report.attackerTeamIndex, report.victimBody);
            }
            if (report.damageInfo.HasModdedDamageType(BorboSuperDebuffOnHit))
            {
                CharacterModel attackedModel = report.victimBody?.modelLocator?.modelTransform?.GetComponent<CharacterModel>(); // this had an nre somewhere .,,.. 
                if (!attackedModel) return;
                
                report.victimBody.AddTimedBuff(SnowtimeToybox.SnowtimeToyboxMod.BorboTurretDebuff, 3);
                
                var temporaryOverlay = TemporaryOverlayManager.AddOverlay(report.victimBody.modelLocator.modelTransform.gameObject);
                temporaryOverlay.duration = 3;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/borboturretdebuffoverlay.mat");
                temporaryOverlay.inspectorCharacterModel = attackedModel;
                temporaryOverlay.AddToCharacterModel(attackedModel);
            }
        }
    }
}
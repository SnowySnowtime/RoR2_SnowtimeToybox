using EntityStates;
using RoR2;
using RoR2.Projectile;
using RoR2BepInExPack.GameAssetPaths;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using UnityEngine;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingGrenadeLauncher : GenericProjectileBaseState
    {
        public static GameObject grenadeObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/TurretlingDemoGrenadeProjectile.prefab");
        public static GameObject grenadeGhostObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/DemoGrenadeGhost.prefab");
        public static GameObject grenadeImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/GrenadeImpact.prefab");

        private static int FireHash = Animator.StringToHash("turretling_fire");
        private static int FireParamHash = Animator.StringToHash("turretling_fire.playbackRate");
        public float myHue;
        public override void OnEnter()
        {
            duration = 1.5f / attackSpeedStat;
            damageCoefficient = 2f;
            force = 0f;
            minSpread = 0.5f;
            maxSpread = 0.5f;
            projectilePitchBonus = -2f;
            projectilePrefab = grenadeObject;
            targetMuzzle = "Muzzle_Primary";
            attackSoundString = "Play_DemoTF2_GL";
            base.OnEnter();
            stopwatch = 0f;
            delayBeforeFiringProjectile = baseDelayBeforeFiringProjectile / attackSpeedStat;
            PlayAnimation(duration);
        }

        public override void PlayAnimation(float duration)
        {
            if ((bool)GetModelAnimator())
            {
                PlayAnimation("Gesture", FireHash, FireParamHash, duration);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void ModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            base.ModifyProjectileInfo(ref fireProjectileInfo);
            fireProjectileInfo.damageTypeOverride = DamageTypeCombo.GenericSpecial;
        }
    }
}

using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using SnowtimeToybox;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class SnowtimeCryoGaussFire : BaseState
    {
        public static GameObject muzzleflashEffectObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Snowtime/Skills/CryoGaussMuzzleFlash.prefab");
        public static GameObject muzzleflashEffectPrefab = muzzleflashEffectObject;
        public static GameObject projectileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Snowtime/Skills/CryoGaussProjectile.prefab");
        public static GameObject projectilePrefab = projectileObject;
        public static GameObject projectileGhostObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Snowtime/Skills/CryoGaussProjectileGhost.prefab");
        public static GameObject projectileExplosionObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Snowtime/Skills/CryoGaussExplosion.prefab");
        public static Component TracerWaow;
        public static float damageCoefficient = 4.0f;
        public static float blastRadius = 4f;
        public static float force = 400f;
        public static float minSpread = 0f;
        public static float maxSpread = 0f;
        public static int bulletCount = 1;
        public static float baseDuration = 0.9f;
        public static string attackSoundString = "Play_Snowtime_Gauss";
        public Vector3 laserDirection;
        public float selfKnockbackForce = 1f;
        private float duration;
        private Ray modifiedAimRay;
        private static int FireLaserStateHash = Animator.StringToHash("FireWeapon");

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration;
            Ray ray = GetAimRay();
            modifiedAimRay.direction = laserDirection;
            GetModelAnimator();
            base.characterBody.characterMotor.ApplyForce((0f - selfKnockbackForce) * laserDirection);
            Transform modelTransform = GetModelTransform();
            Util.PlaySound(attackSoundString, base.gameObject);
            string text = "Muzzle";
            projectileObject.GetComponent<ProjectileController>().ghostPrefab = projectileGhostObject;
            PlayAnimation("Gesture", FireLaserStateHash);
            if ((bool)muzzleflashEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, text, transmit: false);
            }
            if (base.isAuthority)
            {
                TrajectoryAimAssist.ApplyTrajectoryAimAssist(ref ray, projectilePrefab, base.gameObject);
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    projectilePrefab = projectilePrefab,
                    position = ray.origin,
                    rotation = Util.QuaternionSafeLookRotation(ray.direction),
                    owner = base.gameObject,
                    damage = damageCoefficient * (damageStat + (attackSpeedStat - 1)),
                    force = force,
                    crit = Util.CheckRoll(critStat, base.characterBody.master),
                    damageTypeOverride = new DamageTypeCombo(DamageType.SlowOnHit, DamageTypeExtended.Frost, DamageSource.Primary)
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration && base.isAuthority)
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
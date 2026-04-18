using EntityStates;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingBlaster : BaseState
    {
        public static GameObject muzzlefx = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.Muzzleflash1_prefab).WaitForCompletion();
        public static GameObject effectPrefab = muzzlefx;

        public static GameObject hitfx = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.Hitspark1_prefab).WaitForCompletion();
        public static GameObject hitEffectPrefab = hitfx;

        public static GameObject tracerfx = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.TracerNoSmoke_prefab).WaitForCompletion();
        public static GameObject tracerEffectPrefab = tracerfx;

        public static string attackSoundString = "Play_Turretling_Fire";

        public static float damageCoefficient = 1.0f;

        public static float force = 50.0f;

        public static float minSpread = 0f;

        public static float maxSpread = 0.5f;

        public static int bulletCount = 1;

        public static float baseDuration = 2f;

        public int bulletCountCurrent = 1;

        private float duration = 1f;

        private static int FireHash = Animator.StringToHash("turretling_fire");

        private static int FireParamHash = Animator.StringToHash("turretling_fire.playbackRate");

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Util.PlaySound(attackSoundString, base.gameObject);
            Ray aimRay = GetAimRay();
            StartAimMode(aimRay);
            PlayAnimation("Gesture", FireHash, FireParamHash, duration);
            string muzzleName = "Muzzle_Primary";
            if ((bool)effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, muzzleName, transmit: false);
            }
            if (base.isAuthority)
            {
                BulletAttack bulletAttack = new BulletAttack();
                bulletAttack.owner = base.gameObject;
                bulletAttack.weapon = base.gameObject;
                bulletAttack.origin = aimRay.origin;
                bulletAttack.aimVector = aimRay.direction;
                bulletAttack.minSpread = minSpread;
                bulletAttack.maxSpread = maxSpread;
                bulletAttack.bulletCount = 1u;
                bulletAttack.damage = damageCoefficient * damageStat;
                bulletAttack.force = force;
                bulletAttack.tracerEffectPrefab = tracerEffectPrefab;
                bulletAttack.muzzleName = muzzleName;
                bulletAttack.hitEffectPrefab = hitEffectPrefab;
                bulletAttack.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
                bulletAttack.HitEffectNormal = false;
                bulletAttack.radius = 0.15f;
                bulletAttack.damageType.damageSource = DamageSource.Primary;
                bulletAttack.Fire();
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
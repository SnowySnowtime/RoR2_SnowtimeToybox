using EntityStates;
using RoR2;
using RoR2.ContentManagement;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingBlaster : BaseState
    {
        public static GameObject muzzlefx = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.Muzzleflash1_prefab).WaitForCompletion();
        public static GameObject effectPrefab;

        public static GameObject hitfx = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.Hitspark1_prefab).WaitForCompletion();
        public static GameObject hitEffectPrefab;

        public static GameObject tracerfx = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.TracerNoSmoke_prefab).WaitForCompletion();
        public static GameObject tracerEffectPrefab;

        public static GameObject muzzlefx_acanthi = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Muzzleflash_Acanthiling.prefab");
        public static GameObject hitfx_acanthi = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Hitspark_Acanthiling.prefab");
        public static GameObject tracerfx_acanthi = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Tracer_Acanthiling.prefab");

        public static GameObject muzzlefx_borbo = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Muzzleflash_Borboling.prefab");
        public static GameObject hitfx_borbo = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Hitspark_Borboling.prefab");
        public static GameObject tracerfx_borbo = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Tracer_Borboling.prefab");

        public static GameObject hitfx_bread = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Hitspark_Breadling.prefab");

        public static GameObject muzzlefx_shortcake = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Muzzleflash_Shortcakeling.prefab");
        public static GameObject hitfx_shortcake = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Hitspark_Shortcakeling.prefab");
        public static GameObject tracerfx_shortcake = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Tracer_Shortcakeling.prefab");

        public static GameObject muzzlefx_snowtime = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Muzzleflash_Snowtimeling.prefab");
        public static GameObject hitfx_snowtime = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Hitspark_Snowtimeling.prefab");
        public static GameObject tracerfx_snowtime = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Tracer_Snowtimeling.prefab");

        public static GameObject muzzlefx_rainbow = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Muzzleflash__Rainbow.prefab");
        public static GameObject hitfx_rainbow = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Hitspark__Rainbow.prefab");
        public static GameObject tracerfx_rainbow = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx_Tracer__Rainbow.prefab");

        public static string attackSoundString = "Play_Turretling_Fire";

        public static float damageCoefficient = 1.0f;

        public static float force = 50.0f;

        public static float minSpread = 0f;

        public static float maxSpread = 2f;

        public static int bulletCount = 1;

        public static float baseDuration = 2f;

        public int bulletCountCurrent = 1;

        private float duration = 1f;

        private static int FireHash = Animator.StringToHash("turretling_fire");

        private static int FireParamHash = Animator.StringToHash("turretling_fire.playbackRate");

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.SetAimTimer(0f);
            duration = baseDuration / attackSpeedStat;
            Util.PlaySound(attackSoundString, base.gameObject);
            Ray aimRay = GetAimRay();
            StartAimMode(aimRay);
            PlayAnimation("Gesture", FireHash, FireParamHash, duration);
            string muzzleName = "Muzzle_Primary";
            if(base.gameObject.name.Contains("Acanthi"))
            {
                effectPrefab = muzzlefx_acanthi;
                hitEffectPrefab = hitfx_acanthi;
                tracerEffectPrefab = tracerfx_acanthi;
            }
            else if (base.gameObject.name.Contains("Borbo"))
            {
                effectPrefab = muzzlefx_borbo;
                hitEffectPrefab = hitfx_borbo;
                tracerEffectPrefab = tracerfx_borbo;
            }
            else if (base.gameObject.name.Contains("Bread"))
            {
                effectPrefab = muzzlefx;
                hitEffectPrefab = hitfx_bread;
                tracerEffectPrefab = tracerfx;
            }
            else if (base.gameObject.name.Contains("Shortcake"))
            {
                effectPrefab = muzzlefx_shortcake;
                hitEffectPrefab = hitfx_shortcake;
                tracerEffectPrefab = tracerfx_shortcake;
            }
            else if (base.gameObject.name.Contains("Snowtime"))
            {
                effectPrefab = muzzlefx_snowtime;
                hitEffectPrefab = hitfx_snowtime;
                tracerEffectPrefab = tracerfx_snowtime;
            }
            else if (characterBody.master.gameObject.TryGetComponent(out TurretlingRainbow rainbowCheck) && rainbowCheck.turretlingRainbow)
            {
                effectPrefab = muzzlefx_rainbow;
                hitEffectPrefab = hitfx_rainbow;
                tracerEffectPrefab = tracerfx_rainbow;
            }
            else
            {
                effectPrefab = muzzlefx;
                hitEffectPrefab = hitfx;
                tracerEffectPrefab = tracerfx;
            }
            if ((bool)effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, muzzleName, transmit: false);
            }
            if (base.isAuthority)
            {
                BulletAttack bulletAttack = new BulletAttack();
                // Snowtimeling Turret fires a shotgun blaster
                if (base.gameObject.name.Contains("Snowtime"))
                {
                    bulletAttack.minSpread = 0f;
                    bulletAttack.maxSpread = 4f;
                    bulletAttack.bulletCount = 15u;
                    bulletAttack.damage = (damageCoefficient/5) * damageStat;
                }
                // Rainbow Turretling and Turretling Variants should be more accurate than default
                else if (characterBody.master.gameObject.TryGetComponent(out TurretlingRainbow rainbowCheck) && rainbowCheck.turretlingRainbow || base.gameObject.name.Contains("Acanthi") || base.gameObject.name.Contains("Bread") || base.gameObject.name.Contains("Borbo") || base.gameObject.name.Contains("Shortcake"))
                {
                    bulletAttack.minSpread = minSpread;
                    bulletAttack.maxSpread = maxSpread/4;
                    bulletAttack.bulletCount = 1u;
                    bulletAttack.damage = damageCoefficient * damageStat;
                }
                else
                {
                    bulletAttack.minSpread = minSpread;
                    bulletAttack.maxSpread = maxSpread;
                    bulletAttack.bulletCount = 1u;
                    bulletAttack.damage = damageCoefficient * damageStat;
                }
                bulletAttack.owner = base.gameObject;
                bulletAttack.weapon = base.gameObject;
                bulletAttack.origin = aimRay.origin;
                bulletAttack.aimVector = aimRay.direction;
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
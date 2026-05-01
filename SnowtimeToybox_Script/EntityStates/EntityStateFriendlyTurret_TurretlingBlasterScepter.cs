using EntityStates;
using RoR2;
using RoR2.ContentManagement;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingBlasterScepter : BaseState
    {
        public static GameObject hitfx_kinetic = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/vfx_Hitspark__Prismatic_Kinetic.prefab");
        public static GameObject hitfx_corrosive = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/vfx_Hitspark__Prismatic_Corrosive.prefab");
        public static GameObject hitfx_energy = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/vfx_Hitspark__Prismatic_Energy.prefab");
        public static GameObject tracerfx_kinetic = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/vfx_Tracer__Prismatic_Kinetic.prefab");
        public static GameObject tracerfx_corrosive = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/vfx_Tracer__Prismatic_Corrosive.prefab");
        public static GameObject tracerfx_energy = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/vfx_Tracer__Prismatic_Energy.prefab");
        public static GameObject muzzlefx_kinetic = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/vfx_Muzzleflash__Prismatic_Kinetic.prefab");
        public static GameObject muzzlefx_corrosive = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/vfx_Muzzleflash__Prismatic_Corrosive.prefab");
        public static GameObject muzzlefx_energy = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/vfx_Muzzleflash__Prismatic_Energy.prefab");

        public static string attackSoundString = "Play_TurretlingScepter_Fire";

        public static float damageCoefficient = 1.0f;
        public static float procCoefficient = 0.5f;

        public static float force = 50.0f;

        public static float minSpread = 0f;

        public static float maxSpread = 2f;

        public static int bulletCount = 1;

        public int bulletCountCurrent = 1;

        private float duration = 1f;

        private static int FireHash = Animator.StringToHash("turretling_fire");

        private static int FireParamHash = Animator.StringToHash("turretling_fire.playbackRate");

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(attackSoundString, base.gameObject);
            Ray aimRay = GetAimRay();
            StartAimMode(aimRay);
            PlayAnimation("Gesture", FireHash, FireParamHash, duration);
            string muzzleName = "Muzzle_Primary";
            EffectManager.SimpleMuzzleFlash(muzzlefx_kinetic, base.gameObject, muzzleName, transmit: false);
            EffectManager.SimpleMuzzleFlash(muzzlefx_corrosive, base.gameObject, muzzleName, transmit: false);
            EffectManager.SimpleMuzzleFlash(muzzlefx_energy, base.gameObject, muzzleName, transmit: false);
            if (base.isAuthority)
            {
                BulletAttack bulletAttack = new BulletAttack();
                BulletAttack bulletAttack1 = new BulletAttack();
                BulletAttack bulletAttack2 = new BulletAttack();
                // Kinetic
                bulletAttack.minSpread = 0f;
                bulletAttack.maxSpread = 3f;
                bulletAttack.bulletCount = 3u;
                bulletAttack.damage = (damageCoefficient / 1.5f) * (damageStat * (Mathf.Clamp(((attackSpeedStat - 2.5f)), 1f, 9999f)));
                bulletAttack.procCoefficient = (procCoefficient / 3);
                bulletAttack.owner = base.gameObject;
                bulletAttack.weapon = base.gameObject;
                bulletAttack.origin = aimRay.origin;
                bulletAttack.aimVector = aimRay.direction;
                bulletAttack.force = force;
                bulletAttack.muzzleName = muzzleName;
                bulletAttack.hitEffectPrefab = hitfx_kinetic;
                bulletAttack.tracerEffectPrefab = tracerfx_kinetic;
                bulletAttack.damageColorIndex = SnowtimeToyboxMod.BlasterScepterColor1;
                bulletAttack.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
                bulletAttack.HitEffectNormal = false;
                bulletAttack.radius = 0.15f;
                bulletAttack.damageType.damageSource = DamageSource.Primary;
                bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
                bulletAttack.damageType = DamageType.BypassArmor;

                // Corrosive
                bulletAttack1.minSpread = 0f;
                bulletAttack1.maxSpread = 3f;
                bulletAttack1.bulletCount = 3u;
                bulletAttack1.damage = (damageCoefficient / 1.5f) * (damageStat * (Mathf.Clamp(((attackSpeedStat - 2.5f)), 1f, 9999f)));
                bulletAttack1.procCoefficient = (procCoefficient / 3);
                bulletAttack1.owner = base.gameObject;
                bulletAttack1.weapon = base.gameObject;
                bulletAttack1.origin = aimRay.origin;
                bulletAttack1.aimVector = aimRay.direction;
                bulletAttack1.force = force;
                bulletAttack1.muzzleName = muzzleName;
                bulletAttack1.hitEffectPrefab = hitfx_corrosive;
                bulletAttack1.tracerEffectPrefab = tracerfx_corrosive;
                bulletAttack1.damageColorIndex = SnowtimeToyboxMod.BlasterScepterColor2;
                bulletAttack1.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
                bulletAttack1.HitEffectNormal = false;
                bulletAttack1.radius = 0.15f;
                bulletAttack1.damageType.damageSource = DamageSource.Primary;
                bulletAttack1.damageType = DamageType.PoisonOnHit;

                // Energy
                bulletAttack2.minSpread = 0f;
                bulletAttack2.maxSpread = 3f;
                bulletAttack2.bulletCount = 3u;
                bulletAttack2.damage = (damageCoefficient / 1.5f) * (damageStat * (Mathf.Clamp(((attackSpeedStat - 2.5f)), 1f, 9999f)));
                bulletAttack2.procCoefficient = (procCoefficient / 3);
                bulletAttack2.owner = base.gameObject;
                bulletAttack2.weapon = base.gameObject;
                bulletAttack2.origin = aimRay.origin;
                bulletAttack2.aimVector = aimRay.direction;
                bulletAttack2.force = force;
                bulletAttack2.muzzleName = muzzleName;
                bulletAttack2.hitEffectPrefab = hitfx_energy;
                bulletAttack2.tracerEffectPrefab = tracerfx_energy;
                bulletAttack2.damageColorIndex = SnowtimeToyboxMod.BlasterScepterColor3;
                bulletAttack2.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
                bulletAttack2.HitEffectNormal = false;
                bulletAttack2.radius = 0.15f;
                bulletAttack2.damageType.damageSource = DamageSource.Primary;
                bulletAttack2.damageType = DamageType.CrippleOnHit;

                bulletAttack.Fire();
                bulletAttack1.Fire();
                bulletAttack2.Fire();
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
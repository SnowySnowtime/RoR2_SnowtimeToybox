using EntityStates;
using RoR2;
using RoR2.ContentManagement;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingBlaster : BaseState
    {
        public GameObject muzzlefx = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.Muzzleflash1_prefab).WaitForCompletion();
        public GameObject effectPrefab;

        public GameObject hitfx = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.Hitspark1_prefab).WaitForCompletion();
        public GameObject hitEffectPrefab;

        public GameObject tracerfx = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.TracerNoSmoke_prefab).WaitForCompletion();
        public GameObject tracerEffectPrefab;

        public static string attackSoundString = "Play_Turretling_Fire";

        public static float damageCoefficient = 1.0f;
        public static float procCoefficient = 0.5f;

        public static float force = 50.0f;

        public static float minSpread = 0f;

        public static float maxSpread = 2f;

        public static int bulletCount = 1;

        public static float baseDuration = 2f;

        public int bulletCountCurrent = 1;

        private float duration = 1f;

        private int firecount;

        private static int FireHash = Animator.StringToHash("turretling_fire");

        private static int FireParamHash = Animator.StringToHash("turretling_fire.playbackRate");

        private float firingTime;
        private float refireTime;

        public override void OnEnter()
        {
            base.OnEnter();
            firecount = 0;
            firingTime = 0f;
            base.characterBody.SetAimTimer(0f);
            duration = (baseDuration) / attackSpeedStat;
            refireTime = duration / 8;
            AttackWaow();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            firingTime += Time.fixedDeltaTime;
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
            if (base.gameObject.name.Contains("Survivor"))
            {
                if(firingTime > refireTime && firecount < 3)
                {
                    AttackWaow();
                }
            }
        }

        public void AttackWaow()
        {
            firecount++;
            firingTime = 0f;
            //Log.Debug(firecount);
            Util.PlaySound(attackSoundString, base.gameObject);
            Ray aimRay = GetAimRay();
            PlayAnimation("Gesture", FireHash, FireParamHash, duration);
            string muzzleName = "Muzzle_Primary";
            if (base.gameObject.name.Contains("Acanthi"))
            {
                effectPrefab = SnowtimeToyboxMod.muzzlefx_acanthi;
                hitEffectPrefab = SnowtimeToyboxMod.hitfx_acanthi;
                tracerEffectPrefab = SnowtimeToyboxMod.tracerfx_acanthi;
            }
            else if (base.gameObject.name.Contains("Borbo"))
            {
                effectPrefab = SnowtimeToyboxMod.muzzlefx_borbo;
                hitEffectPrefab = SnowtimeToyboxMod.hitfx_borbo;
                tracerEffectPrefab = SnowtimeToyboxMod.tracerfx_borbo;
            }
            else if (base.gameObject.name.Contains("Bread"))
            {
                effectPrefab = muzzlefx;
                hitEffectPrefab = SnowtimeToyboxMod.hitfx_bread;
                tracerEffectPrefab = tracerfx;
            }
            else if (base.gameObject.name.Contains("Shortcake"))
            {
                effectPrefab = SnowtimeToyboxMod.muzzlefx_shortcake;
                hitEffectPrefab = SnowtimeToyboxMod.hitfx_shortcake;
                tracerEffectPrefab = SnowtimeToyboxMod.tracerfx_shortcake;
            }
            else if (base.gameObject.name.Contains("Snowtime"))
            {
                effectPrefab = SnowtimeToyboxMod.muzzlefx_snowtime;
                hitEffectPrefab = SnowtimeToyboxMod.hitfx_snowtime;
                tracerEffectPrefab = SnowtimeToyboxMod.tracerfx_snowtime;
            }
            else if (characterBody.master.gameObject.TryGetComponent(out TurretlingRainbow rainbowCheck) && rainbowCheck.turretlingRainbow)
            {
                effectPrefab = SnowtimeToyboxMod.muzzlefx_rainbow;
                hitEffectPrefab = SnowtimeToyboxMod.hitfx_rainbow;
                tracerEffectPrefab = SnowtimeToyboxMod.tracerfx_rainbow;
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
                    bulletAttack.damage = (damageCoefficient / 5) * damageStat;
                    bulletAttack.procCoefficient = (procCoefficient / 5);
                }
                // Rainbow Turretling and Turretling Variants should be more accurate than default
                else if (characterBody.master.gameObject.TryGetComponent(out TurretlingRainbow rainbowCheck) && rainbowCheck.turretlingRainbow || base.gameObject.name.Contains("Acanthi") || base.gameObject.name.Contains("Bread") || base.gameObject.name.Contains("Borbo") || base.gameObject.name.Contains("Shortcake"))
                {
                    bulletAttack.minSpread = minSpread;
                    bulletAttack.maxSpread = maxSpread / 4;
                    bulletAttack.bulletCount = 1u;
                    bulletAttack.damage = (damageCoefficient * 1f) * damageStat;
                    bulletAttack.procCoefficient = procCoefficient * 4;
                }
                else if (base.gameObject.name.Contains("Survivor"))
                {
                    bulletAttack.minSpread = minSpread;
                    bulletAttack.maxSpread = maxSpread / 4;
                    bulletAttack.bulletCount = 1u;
                    bulletAttack.damage = (damageCoefficient/1.25f) * damageStat;
                    bulletAttack.procCoefficient = procCoefficient * 4;
                }
                else
                {
                    bulletAttack.minSpread = minSpread;
                    bulletAttack.maxSpread = maxSpread;
                    bulletAttack.bulletCount = 1u;
                    bulletAttack.damage = damageCoefficient * damageStat;
                    bulletAttack.procCoefficient = procCoefficient;
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

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
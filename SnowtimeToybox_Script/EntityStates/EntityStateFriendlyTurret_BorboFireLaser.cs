using R2API;
using RoR2;
using SnowtimeToybox;
using System;
using UnityEngine;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class FireBorboLaser : BaseState
    {
        public static GameObject effectPrefabObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Skills/MuzzleflashBorbo.prefab");
        public static GameObject hitEffectPrefabObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Skills/ExplosionBorbo.prefab");
        public static GameObject tracerEffectPrefabObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Skills/TracerBorbo.prefab");
        public static GameObject effectPrefab = effectPrefabObject;
        public static GameObject hitEffectPrefab = hitEffectPrefabObject;
        public static GameObject tracerEffectPrefab = tracerEffectPrefabObject;
        public static float damageCoefficient = 10.0f;
        public static float blastRadius = 4f;
        public static float force = 5000f;
        public static float minSpread = 0f;
        public static float maxSpread = 0f;
        public static int bulletCount = 1;
        public static float baseDuration = 5f;
        public static string attackSoundString = "Play_Borbo_Laser_Fire";
        public static string attackFoleySoundString = "Play_Borbo_Laser_Overheat";
        public Vector3 laserDirection;
        public float selfKnockbackForce = 10f;
        private float duration;
        private Ray modifiedAimRay;
        private static int FireLaserStateHash = Animator.StringToHash("FireLaser");
        private static int FireLaserParamHash = Animator.StringToHash("FireLaser.playbackRate");

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            modifiedAimRay = GetAimRay();
            modifiedAimRay.direction = laserDirection;
            GetModelAnimator();
            base.characterBody.characterMotor.ApplyForce((0f - selfKnockbackForce) * laserDirection);
            Transform modelTransform = GetModelTransform();
            Util.PlaySound(attackSoundString, base.gameObject);
            Util.PlaySound(attackFoleySoundString, base.gameObject);
            string text = "Muzzle";
            PlayAnimation("Gesture", FireLaserStateHash, FireLaserParamHash, duration);
            if ((bool)effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, text, transmit: false);
            }
            if (!base.isAuthority)
            {
                return;
            }
            float num = 1000f;
            Vector3 vector = modifiedAimRay.origin + modifiedAimRay.direction * num;
            if (Physics.Raycast(modifiedAimRay, out var hitInfo, num, LayerIndex.CommonMasks.laser))
            {
                vector = hitInfo.point;
            }
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.attacker = base.gameObject;
            blastAttack.inflictor = base.gameObject;
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
            blastAttack.baseDamage = damageStat * damageCoefficient;
            blastAttack.baseForce = force * 0.2f;
            blastAttack.position = vector;
            blastAttack.radius = blastRadius;
            blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            blastAttack.bonusForce = force * modifiedAimRay.direction;
            blastAttack.damageType = new DamageTypeCombo((DamageTypeCombo)DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Primary);
            DamageAPI.AddModdedDamageType(blastAttack, SnowtimeDamageTypes.BorboSuperDebuffOnHit);
            blastAttack.Fire();
            _ = modifiedAimRay.origin;
            if (!modelTransform)
            {
                return;
            }
            ChildLocator component = modelTransform.GetComponent<ChildLocator>();
            if ((bool)component)
            {
                int childIndex = component.FindChildIndex(text);
                if ((bool)tracerEffectPrefab)
                {
                    EffectData effectData = new EffectData
                    {
                        origin = vector,
                        start = modifiedAimRay.origin
                    };
                    effectData.SetChildLocatorTransformReference(base.gameObject, childIndex);
                    EffectManager.SpawnEffect(tracerEffectPrefab, effectData, transmit: true);
                    EffectManager.SpawnEffect(hitEffectPrefab, effectData, transmit: true);
                }
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
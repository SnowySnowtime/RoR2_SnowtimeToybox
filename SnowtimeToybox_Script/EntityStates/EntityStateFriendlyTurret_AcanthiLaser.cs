// RoR2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// EntityStates.EngiTurret.EngiTurretWeapon.FireBeam
using EntityStates;
using EntityStates.EngiTurret.EngiTurretWeapon;
using HG;
using System;
using R2API;
using RoR2;
using SnowtimeToybox;
using UnityEngine;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class FireAcanthiBeam : BaseState
    {
        private struct LerpHelper
        {
            private float startTime;

            private float endTime;

            private float invTimeSpan;

            private float timeSpan;

            private bool hasStartTime;

            private bool hasEndTime;

            public void Push(float currentTime)
            {
                startTime = endTime;
                endTime = currentTime;
                hasStartTime = hasEndTime;
                hasEndTime = true;
                float num = endTime - startTime;
                if (hasStartTime && num != timeSpan)
                {
                    timeSpan = num;
                    invTimeSpan = 0.5f / timeSpan;
                }
            }

            public readonly float CalcLerpValue(float currentTime)
            {
                if (hasStartTime)
                {
                    return (currentTime - startTime) * invTimeSpan;
                }
                return 0.5f;
            }
        }

        //public GameObject effectPrefab;

        public static GameObject laserObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Acanthi/Skills/laseracanthi.prefab");
        public GameObject laserPrefab = laserObject;

        public string muzzleString = "Muzzle";

        public string attackSoundPlayString = "Play_HWLocust_Fire";
        public string attackSoundStopString = "Stop_HWLocust_Fire";

        public float damageCoefficient = 1f;

        public float procCoefficient = 0.4f;

        public float force = 0f;

        public float minSpread = 0f;

        public float maxSpread = 0f;

        public int bulletCount = 1;

        public float fireFrequency = 5f;

        public float maxDistance = 80f;

        private float fireStopwatch;

        private Ray laserRay;

        private Transform modelTransform;

        private GameObject laserVfxInstance;

        private Transform laserVfxInstanceEndTransform;

        private Vector3? newestRaycastHitPoint;

        private Vector3? previousRaycastHitPoint;

        private float raycastLerpValue;

        private LerpHelper vfxLerpHelper;

        private BulletAttack bulletAttack;

        private EffectManagerHelper _emh_laserEffect;

        private BulletAttack.HitCallback hitCallback;

        private Vector3? bulletEndPos;

        private static int FireLaserStateHash = Animator.StringToHash("acanthi_firing7s");
        private static int FireLaser1StateHash = Animator.StringToHash("acanthi_firingloop");
        private static int FireLaser2StateHash = Animator.StringToHash("acanthi_firingend");
        private float firingTime;

        public override void Reset()
        {
            base.Reset();
            fireStopwatch = 0f;
            firingTime = 0f;
            newestRaycastHitPoint = null;
            previousRaycastHitPoint = null;
            modelTransform = null;
            laserVfxInstance = null;
            laserVfxInstanceEndTransform = null;
            laserRay = default(Ray);
            vfxLerpHelper = default(LerpHelper);
            if (bulletAttack != null)
            {
                bulletAttack.Reset();
            }
            _emh_laserEffect = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(attackSoundPlayString, base.gameObject);
            PlayAnimation("Gesture", FireLaserStateHash);
            fireStopwatch = 0f;
            firingTime = 0f;
            modelTransform = GetModelTransform();
            if (!modelTransform)
            {
                return;
            }
            ChildLocator component = modelTransform.GetComponent<ChildLocator>();
            if (!component)
            {
                return;
            }
            Transform transform = component.FindChild(muzzleString);
            if ((bool)transform && (bool)laserPrefab)
            {
                if (!EffectManager.ShouldUsePooledEffect(laserPrefab))
                {
                    laserVfxInstance = UnityEngine.Object.Instantiate(laserPrefab, transform.position, transform.rotation);
                }
                else
                {
                    _emh_laserEffect = EffectManager.GetAndActivatePooledEffect(laserPrefab, transform.position, transform.rotation);
                    laserVfxInstance = _emh_laserEffect.gameObject;
                }
                if ((bool)laserVfxInstance)
                {
                    ChildLocator component2 = laserVfxInstance.GetComponent<ChildLocator>();
                    laserVfxInstanceEndTransform = component2.FindChild("LaserEnd");
                }
                laserVfxInstance.transform.parent = transform;
            }
        }

        public override void OnExit()
        {
            if ((bool)laserVfxInstance)
            {
                if (_emh_laserEffect != null && _emh_laserEffect.OwningPool != null)
                {
                    _emh_laserEffect.OwningPool.ReturnObject(_emh_laserEffect);
                }
                else
                {
                    EntityState.Destroy(laserVfxInstance);
                }
            }
            laserVfxInstance = null;
            laserVfxInstanceEndTransform = null;
            _emh_laserEffect = null;
            Util.PlaySound(attackSoundStopString, base.gameObject);
            PlayAnimation("Gesture", FireLaser2StateHash);
            base.OnExit();
        }

        private void UpdateBeamVFX(float time)
        {
            laserVfxInstance.AsValidOrNull()?.SetActive(newestRaycastHitPoint.HasValue);
            if ((bool)laserVfxInstanceEndTransform && newestRaycastHitPoint.HasValue)
            {
                Ray ray = GetLaserRay();
                float t = vfxLerpHelper.CalcLerpValue(time);
                float magnitude = (Vector3.Lerp((previousRaycastHitPoint ?? newestRaycastHitPoint).Value, newestRaycastHitPoint.Value, t) - ray.origin).magnitude;
                laserVfxInstanceEndTransform.position = ray.GetPoint(magnitude);
            }
        }

        public override void Update()
        {
            base.Update();
            UpdateBeamVFX(Time.time);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (Time.fixedDeltaTime > 0f)
            {
                laserRay = GetLaserRay();
                fireStopwatch += Time.fixedDeltaTime;
                firingTime += Time.fixedDeltaTime;
                if (firingTime > 7f)
                {
                    firingTime = 7f;
                }
                float numfirefreq = Math.Clamp((0.7f / (1f + (firingTime / 7f))), 0.35f, 0.6f);
                procCoefficient = Math.Clamp((firingTime / 14f), 0.05f, 0.5f);
                float num = fireFrequency;
                float num2 = numfirefreq / num;
                if (fireStopwatch > num2)
                {
                    fireStopwatch = 0f;
                    FireBullet(laserRay, muzzleString, Time.fixedTime);
                }
                if (base.isAuthority && !ShouldFireLaser())
                {
                    outer.SetNextState(GetNextState());
                }
                //Log.Debug(firingTime);
                //Log.Debug("Potential procCoefficient: " + (Math.Clamp((firingTime / 21f), 0.05f, 0.33f) ) );
                //Log.Debug("Potential fireFrequency:" + (Math.Clamp((0.6f / ( 1f + (firingTime/10f) ) ), 0.36f, 0.6f) ) );
            }
        }

        protected Vector3 GetBeamEndPoint()
        {
            Vector3 point = laserRay.GetPoint(maxDistance);
            if (Util.CharacterRaycast(base.gameObject, laserRay, out var hitInfo, maxDistance, (int)LayerIndex.world.mask | (int)LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
            {
                point = hitInfo.point;
            }
            return point;
        }

        protected virtual EntityState GetNextState()
        {
            return EntityStateCatalog.InstantiateState(ref outer.mainStateType);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public virtual bool ShouldFireLaser()
        {
            if ((bool)base.inputBank)
            {
                return base.inputBank.skill1.down;
            }
            return false;
        }

        public virtual Ray GetLaserRay()
        {
            return GetAimRay();
        }

        private void FireBullet(Ray laserRay, string targetMuzzle, float time)
        {
            //if ((bool)effectPrefab)
            //{
            //    EffectManager.SimpleMuzzleFlash(effectPrefab, base.gameObject, targetMuzzle, transmit: false);
            //}
            previousRaycastHitPoint = newestRaycastHitPoint;
            RaycastHit hitInfo;
            if (base.isAuthority)
            {
                if (bulletAttack == null)
                {
                    bulletAttack = new BulletAttack();
                }
                bulletAttack.owner = base.gameObject;
                bulletAttack.weapon = base.gameObject;
                bulletAttack.origin = laserRay.origin;
                bulletAttack.aimVector = laserRay.direction;
                bulletAttack.minSpread = minSpread;
                bulletAttack.maxSpread = maxSpread;
                bulletAttack.bulletCount = 1u;
                bulletAttack.damage = damageCoefficient * damageStat + attackSpeedStat;
                bulletAttack.procCoefficient = procCoefficient;
                bulletAttack.force = force;
                bulletAttack.muzzleName = targetMuzzle;
                bulletAttack.hitEffectPrefab = null;
                bulletAttack.isCrit = Util.CheckRoll(critStat, base.characterBody.master);
                bulletAttack.HitEffectNormal = false;
                bulletAttack.radius = 0f;
                bulletAttack.maxDistance = maxDistance;
                bulletAttack.hitCallback = hitCallback;
                bulletAttack.damageType = (DamageTypeCombo)DamageType.Generic;
                bulletAttack.damageType.damageSource = DamageSource.Primary;
                bulletAttack.AddModdedDamageType(SnowtimeToybox.Buffs.AcanthiBleeding.Acanthi_ButHeresTheBleeder);
                bulletAttack.Fire();
                newestRaycastHitPoint = bulletEndPos;
                bulletEndPos = null;
            }
            else if ((bool)laserVfxInstanceEndTransform && Util.CharacterRaycast(base.gameObject, laserRay, out hitInfo, maxDistance, (int)LayerIndex.world.mask | (int)LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
            {
                newestRaycastHitPoint = hitInfo.point;
            }
            if (!newestRaycastHitPoint.HasValue)
            {
                newestRaycastHitPoint = laserRay.GetPoint(maxDistance);
            }
            vfxLerpHelper.Push(time + Time.fixedDeltaTime);
        }

        public FireAcanthiBeam()
        {
            hitCallback = OnBulletHitAuthority;
        }

        private bool OnBulletHitAuthority(BulletAttack bulletAttack, ref BulletAttack.BulletHit hitInfo)
        {
            bool result = BulletAttack.defaultHitCallback(bulletAttack, ref hitInfo);
            bulletEndPos = bulletEndPos ?? hitInfo.point;
            return result;
        }
    }
}
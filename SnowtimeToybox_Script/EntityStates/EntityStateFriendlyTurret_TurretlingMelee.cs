// RoR2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// EntityStates.EngiTurret.EngiTurretWeapon.FireBeam
using EntityStates;
using EntityStates.EngiTurret.EngiTurretWeapon;
using HG;
using R2API;
using RoR2;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using System;
using UnityEngine;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class FireTurretlingMeleeBeam : BaseState
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

        public GameObject laserPrefab = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/Skills/KineticPhaseBlade_BeamL.prefab");
        public GameObject laserPrefab2 = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/Skills/KineticPhaseBlade_BeamR.prefab");

        public string muzzleString = "Blade_L";
        public string muzzleString2 = "Blade_R";

        public string attackSoundPlayString = "Play_HWLocust_Fire";
        public string attackSoundStopString = "Stop_HWLocust_Fire";

        public float damageCoefficient = 0.3f;

        public float procCoefficient = 0.6f;

        public float force = 0f;

        public float minSpread = 0f;

        public float maxSpread = 0f;

        public int bulletCount = 1;

        public float fireFrequency = 1f;

        public float maxDistance = 10f;

        private float fireStopwatch;
        private float numFireFrequency;

        private Ray laserRay;

        private Transform modelTransform;
        private Transform modelTransform2;

        private GameObject laserVfxInstance;
        private GameObject laserVfxInstance2;

        private Transform laserVfxInstanceEndTransform;
        private Transform laserVfxInstanceEndTransform2;

        private Vector3? newestRaycastHitPoint;

        private Vector3? previousRaycastHitPoint;

        private float raycastLerpValue;

        private LerpHelper vfxLerpHelper;

        private BulletAttack bulletAttack;

        private EffectManagerHelper _emh_laserEffect;
        private EffectManagerHelper _emh_laserEffect2;

        private BulletAttack.HitCallback hitCallback;

        private Vector3? bulletEndPos;

        public override void Reset()
        {
            base.Reset();
            fireStopwatch = 0f;
            newestRaycastHitPoint = null;
            previousRaycastHitPoint = null;
            modelTransform = null;
            modelTransform2 = null;
            laserVfxInstance = null;
            laserVfxInstance2 = null;
            laserVfxInstanceEndTransform = null;
            laserVfxInstanceEndTransform2 = null;
            laserRay = default(Ray);
            vfxLerpHelper = default(LerpHelper);
            if (bulletAttack != null)
            {
                bulletAttack.Reset();
            }
            _emh_laserEffect = null;
            _emh_laserEffect2 = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(attackSoundPlayString, base.gameObject);
            fireStopwatch = 0f;
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
            Transform transform2 = component.FindChild(muzzleString2);
            if ((bool)transform && (bool)laserPrefab)
            {
                // Left
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
                // Right
                if (!EffectManager.ShouldUsePooledEffect(laserPrefab2))
                {
                    laserVfxInstance2 = UnityEngine.Object.Instantiate(laserPrefab2, transform2.position, transform2.rotation);
                }
                else
                {
                    _emh_laserEffect2 = EffectManager.GetAndActivatePooledEffect(laserPrefab2, transform2.position, transform2.rotation);
                    laserVfxInstance2 = _emh_laserEffect2.gameObject;
                }
                if ((bool)laserVfxInstance)
                {
                    ChildLocator component4 = laserVfxInstance2.GetComponent<ChildLocator>();
                    laserVfxInstanceEndTransform2 = component4.FindChild("LaserEnd");
                }
                laserVfxInstance2.transform.parent = transform2;

                float myHue = base.gameObject.GetComponent<CharacterBody>().master.gameObject.GetComponent<TurretlingRainbow>().myHue;
                bool rainbow = base.gameObject.GetComponent<CharacterBody>().master.gameObject.GetComponent<TurretlingRainbow>().turretlingRainbow;
                if (rainbow)
                {
                    laserVfxInstance.GetComponent<Animator>().SetFloat("hue", 0);
                    laserVfxInstance2.GetComponent<Animator>().SetFloat("hue", 0);
                    laserVfxInstance.GetComponent<Animator>().SetBool("shift", rainbow);
                    laserVfxInstance2.GetComponent<Animator>().SetBool("shift", rainbow);
                }
                else
                {
                    laserVfxInstance.GetComponent<Animator>().SetFloat("hue", myHue);
                    laserVfxInstance2.GetComponent<Animator>().SetFloat("hue", myHue);
                }
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
                if (_emh_laserEffect2 != null && _emh_laserEffect2.OwningPool != null)
                {
                    _emh_laserEffect2.OwningPool.ReturnObject(_emh_laserEffect2);
                }
                else
                {
                    EntityState.Destroy(laserVfxInstance2);
                }
            }
            laserVfxInstance = null;
            laserVfxInstance2 = null;
            laserVfxInstanceEndTransform = null;
            laserVfxInstanceEndTransform2 = null;
            _emh_laserEffect = null;
            _emh_laserEffect2 = null;
            Util.PlaySound(attackSoundStopString, base.gameObject);
            base.OnExit();
        }

        private void UpdateBeamVFX(float time)
        {
            laserVfxInstance.AsValidOrNull()?.SetActive(newestRaycastHitPoint.HasValue);
            laserVfxInstance2.AsValidOrNull()?.SetActive(newestRaycastHitPoint.HasValue);
            if ((bool)laserVfxInstanceEndTransform && newestRaycastHitPoint.HasValue)
            {
                Ray ray = GetLaserRay();
                float t = vfxLerpHelper.CalcLerpValue(time);
                float magnitude = (Vector3.Lerp((previousRaycastHitPoint ?? newestRaycastHitPoint).Value, newestRaycastHitPoint.Value, t) - ray.origin).magnitude;
                laserVfxInstanceEndTransform.position = ray.GetPoint(magnitude);
                laserVfxInstanceEndTransform2.position = ray.GetPoint(magnitude);
            }

            if (laserVfxInstance.AsValidOrNull() != null && laserVfxInstance2.AsValidOrNull() != null)
            {
                float myHue = base.gameObject.GetComponent<CharacterBody>().master.gameObject.GetComponent<TurretlingRainbow>().myHue;
                bool rainbow = base.gameObject.GetComponent<CharacterBody>().master.gameObject.GetComponent<TurretlingRainbow>().turretlingRainbow;
                laserVfxInstance.GetComponent<Animator>().SetBool("rainbow", rainbow);
                laserVfxInstance2.GetComponent<Animator>().SetBool("rainbow", rainbow);
                if (rainbow)
                {
                    laserVfxInstance.GetComponent<Animator>().SetFloat("hue", 0);
                    laserVfxInstance2.GetComponent<Animator>().SetFloat("hue", 0);
                }
                else
                {
                    laserVfxInstance.GetComponent<Animator>().SetFloat("hue", myHue);
                    laserVfxInstance2.GetComponent<Animator>().SetFloat("hue", myHue);
                }
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
                Inventory inventory = characterBody.inventory;
                if (!SnowtimeToyboxMod.scepterLoaded)
                {
                    numFireFrequency = 0.11f;
                }
                else
                {
                    int itemCountScepter = inventory.GetItemCountEffective(ItemCatalog.FindItemIndex("ITEM_ANCIENT_SCEPTER"));
                    if (itemCountScepter != 0)
                    {
                        numFireFrequency = 0.055f;
                    }
                    else
                    {
                        numFireFrequency = 0.11f;
                    }
                }
                procCoefficient = 0.5f;
                float num = fireFrequency;
                float num2 = numFireFrequency / num;
                if (fireStopwatch > num2)
                {
                    fireStopwatch = 0f;
                    FireBullet(laserRay, muzzleString, Time.fixedTime);
                }
                if (base.isAuthority && !ShouldFireLaser())
                {
                    outer.SetNextState(GetNextState());
                }
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
            previousRaycastHitPoint = newestRaycastHitPoint;
            RaycastHit hitInfo;
            base.characterBody.AddTimedBuff(Content.SwarmlingMeleeBarrierHandler, 1.5f, 1);
            if (base.isAuthority)
            {
                //Log.Debug("Firing...");
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
                bulletAttack.damage = (damageCoefficient * damageStat) + attackSpeedStat;
                bulletAttack.procCoefficient = procCoefficient;
                bulletAttack.force = force;
                bulletAttack.muzzleName = targetMuzzle;
                bulletAttack.hitEffectPrefab = null;
                bulletAttack.isCrit = Util.CheckRoll(characterBody.crit, base.characterBody.master);
                //Log.Debug(bulletAttack.isCrit);
                bulletAttack.HitEffectNormal = false;
                bulletAttack.radius = 0f;
                bulletAttack.maxDistance = maxDistance;
                bulletAttack.hitCallback = hitCallback;
                bulletAttack.damageType = (DamageTypeCombo)DamageType.Generic;
                bulletAttack.damageType.damageSource = DamageSource.Primary;
                DamageAPI.AddModdedDamageType(bulletAttack, SnowtimeToyboxMod.SwarmlingArmorStripOnHit);
                bulletAttack.Fire();
                newestRaycastHitPoint = bulletEndPos;
                bulletEndPos = null;
                bulletAttack = null;
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

        public FireTurretlingMeleeBeam()
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
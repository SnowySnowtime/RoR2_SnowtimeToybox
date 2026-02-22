// RoR2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// EntityStates.Drone.DroneWeapon.HealBeam
using EntityStates;
using HG;
using RoR2;
using RoR2.Items;
using SnowtimeToybox;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class FireBreadBeam : BaseState
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

        public static float baseDuration = 0.1f;

        public static GameObject healBeamObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Bread/Skills/BreadFortuneBeamL.prefab");
        public static GameObject healBeamObjectR = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Bread/Skills/BreadFortuneBeamR.prefab");
        public static GameObject healBeamPrefab = healBeamObject;
        public static GameObject healBeamPrefabR = healBeamObjectR;

        public HurtBox target;

        private float duration;

        private Transform modelTransform;
        private EffectManagerHelper _emh_laserEffect;
        private EffectManagerHelper _emh_laserEffect2;
        private GameObject laserVfxInstance;
        private GameObject laserVfxInstance2;
        private Transform laserVfxInstanceEndTransform;
        private Transform laserVfxInstanceEndTransform2;
        private LerpHelper vfxLerpHelper;
        private Ray laserRay;
        private UnityEngine.Vector3? newestRaycastHitPoint;
        private UnityEngine.Vector3? previousRaycastHitPoint;

        public override void Reset()
        {
            base.Reset();
            modelTransform = null;
            laserVfxInstance = null;
            laserVfxInstance2 = null;
            laserVfxInstanceEndTransform = null;
            laserVfxInstanceEndTransform2 = null;
            laserRay = default(Ray);
            vfxLerpHelper = default(LerpHelper);
            _emh_laserEffect = null;
            _emh_laserEffect2 = null;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration;

            /// straight up lasering it
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
            Transform transformL = component.FindChild("Muzzle_L");
            Transform transformR = component.FindChild("Muzzle_R");

            // fire it.
            Ray ray = GetAimRay();
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.teamMaskFilter = TeamMask.none;
            if ((bool)base.teamComponent)
            {
                bullseyeSearch.teamMaskFilter.AddTeam(base.teamComponent.teamIndex);
            }
            bullseyeSearch.filterByLoS = false;
            bullseyeSearch.maxDistanceFilter = 50f;
            bullseyeSearch.maxAngleFilter = 180f;
            bullseyeSearch.searchOrigin = ray.origin;
            bullseyeSearch.searchDirection = ray.direction;
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(base.gameObject);
            target = bullseyeSearch.GetResults().FirstOrDefault();
            if (NetworkServer.active && (bool)transform && (bool)target)
            {
                if (!target) return;
                if (!target?.healthComponent) return;
                if (!target?.healthComponent?.body) return;
                target.healthComponent.body.AddTimedBuff(SnowtimeToyboxMod.BreadTurretBuffFortune, 0.15f);
            }

            if (_emh_laserEffect != null || _emh_laserEffect2 != null) return;
            if ((bool)transform && (bool)healBeamPrefab && (bool)healBeamPrefabR)
            {
                // Left
                if (!EffectManager.ShouldUsePooledEffect(healBeamPrefab))
                {
                    laserVfxInstance = Object.Instantiate(healBeamPrefab, transformL.position, transformL.rotation);
                }
                else
                {
                    _emh_laserEffect = EffectManager.GetAndActivatePooledEffect(healBeamPrefab, transformL.position, transformL.rotation);
                    laserVfxInstance = _emh_laserEffect.gameObject;
                }
                // Right
                if (!EffectManager.ShouldUsePooledEffect(healBeamPrefabR))
                {
                    laserVfxInstance2 = Object.Instantiate(healBeamPrefabR, transformR.position, transformR.rotation);
                }
                else
                {
                    _emh_laserEffect2 = EffectManager.GetAndActivatePooledEffect(healBeamPrefabR, transformR.position, transformR.rotation);
                    laserVfxInstance2 = _emh_laserEffect2.gameObject;
                }

                if ((bool)laserVfxInstance)
                {
                    ChildLocator component2 = laserVfxInstance.GetComponent<ChildLocator>();
                    laserVfxInstanceEndTransform = component2.FindChild("LaserEnd");
                }
                if ((bool)laserVfxInstance2)
                {
                    ChildLocator component3 = laserVfxInstance2.GetComponent<ChildLocator>();
                    laserVfxInstanceEndTransform2 = component3.FindChild("LaserEnd");
                }
                laserVfxInstance.transform.parent = transform;
                laserVfxInstance2.transform.parent = transform;
            }
        }

        private void UpdateBeamVFX(float time)
        {
            laserVfxInstance.AsValidOrNull()?.SetActive(newestRaycastHitPoint.HasValue);
            laserVfxInstance2.AsValidOrNull()?.SetActive(newestRaycastHitPoint.HasValue);
            if ((bool)laserVfxInstanceEndTransform && newestRaycastHitPoint.HasValue)
            {
                Ray ray = GetAimRay();
                float t = vfxLerpHelper.CalcLerpValue(time);
                float magnitude = (UnityEngine.Vector3.Lerp((previousRaycastHitPoint ?? newestRaycastHitPoint).Value, newestRaycastHitPoint.Value, t) - ray.origin).magnitude;
                laserVfxInstanceEndTransform.position = ray.GetPoint(magnitude);
                laserVfxInstanceEndTransform2.position = ray.GetPoint(magnitude);
            }
        }

        private void UpdateEndPoint(Ray laserRay)
        {
            previousRaycastHitPoint = newestRaycastHitPoint;
            if (NetworkServer.active && (bool)transform && (bool)target)
            {
                var targetpos = target.transform.position;
                newestRaycastHitPoint = targetpos;
            }
            vfxLerpHelper.Push(0.01f + Time.fixedDeltaTime);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            UpdateEndPoint(laserRay);
            if ((base.fixedAge >= duration || !target) && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        public override void Update()
        {
            base.Update();
            UpdateBeamVFX(Time.time);
        }

        public override void OnExit()
        {
            //PlayCrossfade("Gesture", "Empty", 0.2f);
            if ((bool)laserVfxInstance)
            {
                if (_emh_laserEffect != null && _emh_laserEffect.OwningPool != null)
                {
                    _emh_laserEffect.OwningPool.ReturnObject(_emh_laserEffect);
                    _emh_laserEffect2.OwningPool.ReturnObject(_emh_laserEffect2);
                }
                else
                {
                    EntityState.Destroy(laserVfxInstance);
                    EntityState.Destroy(laserVfxInstance2);
                }
            }
            laserVfxInstance = null;
            laserVfxInstance2 = null;
            laserVfxInstanceEndTransform = null;
            laserVfxInstanceEndTransform2 = null;
            _emh_laserEffect = null;
            _emh_laserEffect2 = null;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            HurtBoxReference.FromHurtBox(target).Write(writer);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            HurtBoxReference hurtBoxReference = default(HurtBoxReference);
            hurtBoxReference.Read(reader);
            target = hurtBoxReference.ResolveGameObject()?.GetComponent<HurtBox>();
        }
    }
}
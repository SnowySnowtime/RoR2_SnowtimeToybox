// RoR2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// EntityStates.Drone.DroneWeapon.HealBeam

using System;
using EntityStates;
using HG;
using RoR2;
using RoR2.Items;
using SnowtimeToybox;
using System.Linq;
using System.Numerics;
using SnowtimeToybox.FriendlyTurretChecks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem.PlaybackState;
using Object = UnityEngine.Object;

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

        public static float baseDuration = 0.5f;

        public static GameObject healBeamPrefab = SnowtimeToyboxMod.FriendlyTurretBreadBeamL;
        public static GameObject healBeamPrefabR = SnowtimeToyboxMod.FriendlyTurretBreadBeamR;

        public HurtBox target;

        private float duration;
        
        protected ref InputBankTest.ButtonState skillButtonState => ref base.inputBank.skill1;

        private Transform modelTransform;
        private EffectManagerHelper _emh_laserEffect;
        private EffectManagerHelper _emh_laserEffect2;
        private GameObject laserVfxInstance;
        private GameObject laserVfxInstance2;
        private Transform laserVfxInstanceStartTransform;
        private Transform laserVfxInstanceStartTransform2;
        private Transform laserVfxInstanceEndTransform;
        private Transform laserVfxInstanceEndTransform2;
        private LerpHelper vfxLerpHelper;
        private Ray laserRay;
        private UnityEngine.Vector3? newestRaycastHitPoint;
        private UnityEngine.Vector3? previousRaycastHitPoint;
        private Transform muzzleL;
        private Transform muzzleR;
        private ChildLocator childLocator;

        public override void Reset()
        {
            Log.Debug("redsrt");
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
            childLocator = modelTransform.GetComponent<ChildLocator>();
            if (!childLocator)
            {
                return;
            }
            muzzleL = childLocator.FindChild("Muzzle_L");
            muzzleR = childLocator.FindChild("Muzzle_R");

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
            

            if (_emh_laserEffect != null || _emh_laserEffect2 != null) return;
            if ((bool)transform && (bool)healBeamPrefab && (bool)healBeamPrefabR)
            {
                //Log.Debug(transformL);
                // Left
                if (!EffectManager.ShouldUsePooledEffect(healBeamPrefab))
                {
                    laserVfxInstance = Object.Instantiate(healBeamPrefab, muzzleL.position, muzzleL.rotation);
                }
                else
                {
                    _emh_laserEffect = EffectManager.GetAndActivatePooledEffect(healBeamPrefab, muzzleL.position, muzzleL.rotation);
                    laserVfxInstance = _emh_laserEffect.gameObject;
                }
                // Right
                if (!EffectManager.ShouldUsePooledEffect(healBeamPrefabR))
                {
                    laserVfxInstance2 = Object.Instantiate(healBeamPrefabR, muzzleR.position, muzzleR.rotation);
                }
                else
                {
                    _emh_laserEffect2 = EffectManager.GetAndActivatePooledEffect(healBeamPrefabR, muzzleR.position, muzzleR.rotation);
                    laserVfxInstance2 = _emh_laserEffect2.gameObject;
                }

                if ((bool)laserVfxInstance)
                {
                    ChildLocator component2 = laserVfxInstance.GetComponent<ChildLocator>();
                    laserVfxInstanceEndTransform = component2.FindChild("LaserEnd");
                    laserVfxInstanceStartTransform = laserVfxInstance.transform.Find("HealBeamStartPoint");
                }
                if ((bool)laserVfxInstance2)
                {
                    ChildLocator component3 = laserVfxInstance2.GetComponent<ChildLocator>();
                    laserVfxInstanceEndTransform2 = component3.FindChild("LaserEnd");
                    laserVfxInstanceStartTransform2 = laserVfxInstance2.transform.Find("HealBeamStartPoint");
                }
                laserVfxInstance.transform.parent = transform;
                laserVfxInstance2.transform.parent = transform;
                
                //if(NetworkServer.active)
                //{
                //    NetworkServer.Spawn(laserVfxInstance);
                 //   NetworkServer.Spawn(laserVfxInstance2);
                //}
                
            }
        }

        private void UpdateBeamVFX(float time)
        {
            laserVfxInstance.AsValidOrNull()?.SetActive(newestRaycastHitPoint.HasValue);
            laserVfxInstance2.AsValidOrNull()?.SetActive(newestRaycastHitPoint.HasValue);
            if (laserVfxInstanceEndTransform && laserVfxInstanceEndTransform2 && newestRaycastHitPoint.HasValue)
            {
                laserVfxInstanceEndTransform.position = target.transform.position;
                laserVfxInstanceEndTransform2.position = target.transform.position;
                laserVfxInstanceStartTransform.position = muzzleL.position;
                laserVfxInstanceStartTransform2.position = muzzleR.position;
            }
        }

        private void UpdateEndPoint(Ray laserRay)
        {
            previousRaycastHitPoint = newestRaycastHitPoint;
            if (target)
            {
                newestRaycastHitPoint = target.transform.position;
            }
            vfxLerpHelper.Push(0.01f + Time.fixedDeltaTime);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((base.fixedAge >= duration || !target) && base.isAuthority)
            {
                //Log.Debug("fixed = " + base.fixedAge);
            }
            if (base.isAuthority && !skillButtonState.down)
            {
                outer.SetNextStateToMain();
            }
            
            if ((bool)transform && (bool)target)
            {
                if (!target) return;
                if (!target?.healthComponent) return;
                if (!target?.healthComponent?.body) return;
                CharacterBody targetBody = target.healthComponent.body;
                
                if (targetBody.HasBuff(SnowtimeToyboxMod.BreadTurretBuffFortune) && NetworkServer.active)
                {
                    foreach (var timedBuff in target.healthComponent.body.timedBuffs)
                    {
                        if (timedBuff.buffIndex != SnowtimeToyboxMod.BreadTurretBuffFortune.buffIndex)
                        {
                            continue;
                        }

                        timedBuff.timer = 0.5f;
                    }
                }
                else
                {
                    if (NetworkServer.active)
                    {
                        targetBody.AddTimedBuff(SnowtimeToyboxMod.BreadTurretBuffFortune, 0.5f);
                    }
                    
                    /*FriendlyTurretOverlayManager overlayManager = targetBody.gameObject.GetComponent<FriendlyTurretOverlayManager>();
                    if (overlayManager == null)
                    {
                        overlayManager = targetBody.gameObject.AddComponent<FriendlyTurretOverlayManager>();
                        overlayManager.Body = targetBody;
                    }
                    CharacterModel waow = targetBody.modelLocator?.modelTransform?.GetComponent<CharacterModel>();
                    
                    var temporaryOverlay = TemporaryOverlayManager.AddOverlay(targetBody.modelLocator.modelTransform.gameObject);
                    temporaryOverlay.duration = Single.PositiveInfinity;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Bread/matBreadFortune.mat");
                    temporaryOverlay.inspectorCharacterModel = waow;
                    temporaryOverlay.AddToCharacterModel(waow);
                    overlayManager.Overlay.Add(temporaryOverlay);
                    Log.Debug("addings overlay !!");*/
                }
            }
        }
        public override void Update()
        {
            base.Update();
            UpdateEndPoint(laserRay);
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
            return InterruptPriority.PrioritySkill;
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
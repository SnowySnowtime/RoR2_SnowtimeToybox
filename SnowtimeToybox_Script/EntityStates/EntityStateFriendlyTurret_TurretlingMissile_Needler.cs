using EntityStates;
using R2API;
using RoR2;
using RoR2.Orbs;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using SnowtimeToybox.Items;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingNeedler : BaseState
    {
        public float orbDamageCoefficient = 0.25f;

        public float orbProcCoefficient = 1f;

        public string muzzleString = "Muzzle_Secondary";

        public float baseDuration = 0.3125f;

        private float duration;

        protected bool isCrit;

        private HurtBox initialOrbTarget;

        private ChildLocator childLocator;
        
        private Animator animator;

        private static int fireMissileHash = Animator.StringToHash("turretling_missile_fire");
        private static int fireMissileParamHash = Animator.StringToHash("turretling_missile_fire.playbackRate");
        private bool missileCheckPassed = false;

        public override void OnEnter()
        {
            base.OnEnter();
            if (TryGetComponent(out TurretlingMissileTracker missileTracker) != true) return;
            if(isAuthority)
            {
                initialOrbTarget = missileTracker?.GetTrackingTarget();
            }
            if(initialOrbTarget == null)
            {
                if(isAuthority)
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }
            Transform modelTransform = GetModelTransform();
            if ((bool)modelTransform)
            {
                childLocator = modelTransform.GetComponent<ChildLocator>();
                animator = modelTransform.GetComponent<Animator>();
            }
            missileCheckPassed = true;
            if (NetworkServer.active && characterBody.inventory && characterBody.inventory.GetItemCountEffective(DLC2Content.Items.IncreasePrimaryDamage) > 0)
            {
                characterBody.AddIncreasePrimaryDamageStack();
            }
            //Log.Debug(missileTracker.GetTrackingTarget().gameObject);
            //base.skillLocator.secondary.DeductStock(base.skillLocator.secondary.maxStock);
            skillLocator.secondary.DeductStock(1);
            duration = baseDuration / attackSpeedStat;
            isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
            Inventory inventory = characterBody.inventory;
            int itemCountEffective = inventory.GetItemCountEffective(DLC1Content.Items.MoreMissile);
            if (itemCountEffective > 0)
            {
                FireOrbMissile();
                FireOrbMissile();
            }
            EffectManager.SimpleMuzzleFlash(Content.SwarmNeedlerMuzzle, base.gameObject, muzzleString, transmit: false);
            FireOrbMissile();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireOrbMissile()
        {
            if (NetworkServer.active)
            {
                //Log.Debug($"bwa 6");
                SnowtimeOrbs snowtimeOrb = new();
                snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingNeedler;
                
                snowtimeOrb.isCrit = isCrit;
                snowtimeOrb.teamIndex = TeamComponent.GetObjectTeam(gameObject);
                snowtimeOrb.damageValue = characterBody.damage * orbDamageCoefficient;
                snowtimeOrb.attacker = gameObject;
                snowtimeOrb.procCoefficient = gameObject.name.Contains("Survivor") ? orbProcCoefficient : orbProcCoefficient / 2;
                snowtimeOrb.damageType.damageSource = DamageSource.Secondary;

                HurtBox hurtBox = initialOrbTarget;
                if (hurtBox)
                {
                    Transform transform = childLocator.FindChild(muzzleString);
                    snowtimeOrb.origin = transform.position;
                    snowtimeOrb.target = hurtBox;
                    OrbManager.instance.AddOrb(snowtimeOrb);
                    PlayAnimation("Gesture", fireMissileHash, fireMissileParamHash, duration);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            writer.Write(HurtBoxReference.FromHurtBox(initialOrbTarget));
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            initialOrbTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
        }
    }
}
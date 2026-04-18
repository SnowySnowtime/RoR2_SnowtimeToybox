using EntityStates;
using RoR2;
using RoR2.Orbs;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingMissile : BaseState
    {
        public float orbDamageCoefficient = 2f;

        public float orbProcCoefficient = 1f;

        public string muzzleString = "Muzzle_Secondary";

        public float baseDuration = 0.25f;

        private float duration;

        protected bool isCrit;

        private HurtBox initialOrbTarget;

        private ChildLocator childLocator;

        private TurretlingMissileTracker missileTracker;

        private Animator animator;

        private static int fireMissileHash = Animator.StringToHash("turretling_missile_fire");

        private static int fireMissileParamHash = Animator.StringToHash("turretling_missile_fire.playbackRate");

        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = GetModelTransform();
            missileTracker = GetComponent<TurretlingMissileTracker>();
            if ((bool)modelTransform)
            {
                childLocator = modelTransform.GetComponent<ChildLocator>();
                animator = modelTransform.GetComponent<Animator>();
            }
            if ((bool)missileTracker && base.isAuthority)
            {
                initialOrbTarget = missileTracker.GetTrackingTarget();
            }
            duration = baseDuration / attackSpeedStat;
            PlayAnimation("Gesture", fireMissileHash, fireMissileParamHash, duration);
            isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
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
                SnowtimeOrbs snowtimeOrb = new SnowtimeOrbs();
                snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile;
                snowtimeOrb.damageValue = base.characterBody.damage * orbDamageCoefficient;
                snowtimeOrb.isCrit = isCrit;
                snowtimeOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
                snowtimeOrb.attacker = base.gameObject;
                snowtimeOrb.procCoefficient = orbProcCoefficient;
                snowtimeOrb.damageType.damageSource = DamageSource.Secondary;
                HurtBox hurtBox = initialOrbTarget;
                if ((bool)hurtBox)
                {
                    Transform transform = childLocator.FindChild(muzzleString);
                    snowtimeOrb.origin = transform.position;
                    snowtimeOrb.target = hurtBox;
                    OrbManager.instance.AddOrb(snowtimeOrb);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > duration && base.isAuthority)
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
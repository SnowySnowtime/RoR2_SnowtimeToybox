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

        public float baseDuration = 0.4f;

        private float duration;

        protected bool isCrit;

        private HurtBox initialOrbTarget;

        private ChildLocator childLocator;

        private TurretlingMissileTracker missileTracker;

        private Animator animator;

        private static int fireMissileHash = Animator.StringToHash("turretling_missile_fire");

        private static int fireMissileParamHash = Animator.StringToHash("turretling_missile_fire.playbackRate");
        private float firingTime;
        private int missilesFired;

        public override void OnEnter()
        {
            base.OnEnter();
            firingTime = 0f;
            missilesFired = 0;
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
            duration = baseDuration;
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
                missilesFired++;
                SnowtimeOrbs snowtimeOrb = new SnowtimeOrbs();
                if(base.gameObject.name.Contains("Acanthi"))
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Acanthi;
                }
                else if (base.gameObject.name.Contains("Borbo"))
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Borbo;
                }
                else if (base.gameObject.name.Contains("Bread"))
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Bread;
                }
                else if (base.gameObject.name.Contains("Shortcake"))
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Shortcake;
                }
                else if (base.gameObject.name.Contains("Snowtime"))
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Snowtime;
                }
                //else if (base.GetComponent<TurretlingRainbow>().turretlingRainbow)
                //{
                //    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Rainbow;
                //}
                else
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile;
                }
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
            firingTime += Time.fixedDeltaTime;
            Inventory inventory = base.characterBody.inventory;
            int itemCountEffective = inventory.GetItemCountEffective(DLC1Content.Items.MoreMissile);
            if (itemCountEffective > 0)
            {
                if (firingTime > 0.1f && missilesFired < 2)
                {
                    FireOrbMissile();
                }
                if (firingTime > 0.2f && missilesFired < 3)
                {
                    FireOrbMissile();
                }
                if (firingTime > 0.3f && missilesFired < 4)
                {
                    FireOrbMissile();
                }
            }
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
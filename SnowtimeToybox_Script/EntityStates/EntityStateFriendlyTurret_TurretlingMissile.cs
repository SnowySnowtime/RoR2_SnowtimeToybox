using EntityStates;
using RoR2;
using RoR2.Orbs;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using SnowtimeToybox.Items;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingMissile : BaseState
    {
        public float orbDamageCoefficient = 4f;

        public float orbProcCoefficient = 1f;

        public string muzzleString = "Muzzle_Secondary";

        public float baseDuration = 0.4f;

        private float duration;

        protected bool isCrit;

        private HurtBox initialOrbTarget;

        private ChildLocator childLocator;
        
        private Animator animator;

        private static int fireMissileHash = Animator.StringToHash("turretling_missile_fire");

        private static int fireMissileParamHash = Animator.StringToHash("turretling_missile_fire.playbackRate");
        private float firingTime;
        private float refireTime;
        private int missilesFired;
        private bool missileCheckPassed = false;

        public override void OnEnter()
        {
            base.OnEnter();
            if (!gameObject?.GetComponent<TurretlingMissileTracker>()) return;
            if (gameObject?.TryGetComponent(out TurretlingMissileTracker missileTracker) != true) return;
            if (missileTracker?.GetTrackingTarget()?.gameObject == null)
            if (missileTracker?.GetTrackingTarget() == null)
            {
                //base.skillLocator.secondary.AddOneStock();
                outer.SetNextStateToMain();
                return;
            }
            missileCheckPassed = true;
            //Log.Debug(missileTracker.GetTrackingTarget().gameObject);
            //base.skillLocator.secondary.DeductStock(base.skillLocator.secondary.maxStock);
            skillLocator.secondary.RemoveAllStocks();
            firingTime = 0f;
            missilesFired = 0;
            Transform modelTransform = GetModelTransform();
            if ((bool)modelTransform)
            {
                childLocator = modelTransform.GetComponent<ChildLocator>();
                animator = modelTransform.GetComponent<Animator>();
            }
            if ((bool)missileTracker && isAuthority)
            {
                initialOrbTarget = missileTracker.GetTrackingTarget();
            }
            duration = baseDuration;
            refireTime = duration / 4;
            isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
            Inventory inventory = characterBody.inventory;
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
                Log.Debug($"bwa 6");
                missilesFired++;
                firingTime = 0f;
                SnowtimeOrbs snowtimeOrb = new();
                if(base.gameObject.name.Contains("Acanthi"))
                if(gameObject.name.Contains("Acanthi"))
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Acanthi;
                }
                else if (gameObject.name.Contains("Borbo"))
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Borbo;
                }
                else if (gameObject.name.Contains("Bread"))
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Bread;
                }
                else if (gameObject.name.Contains("Shortcake"))
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Shortcake;
                }
                else if (gameObject.name.Contains("Snowtime"))
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Snowtime;
                }
                else if (characterBody.master.gameObject.TryGetComponent(out TurretlingRainbow rainbowCheck) && rainbowCheck.turretlingRainbow)
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile_Rainbow;
                }
                else
                {
                    snowtimeOrb.snowtimeOrbType = SnowtimeOrbs.OrbTypes.TurretlingMissile;
                }
                if(gameObject.name.Contains("SwarmTurretling"))
                {
                    snowtimeOrb.damageValue = (characterBody.damage * ((orbDamageCoefficient / 2) + skillLocator.secondary.bonusStockFromBody + skillLocator.secondary.bonusStockFromBody)) * ((Mathf.Clamp(((attackSpeedStat - 2.5f) / 2f), 1f, 9999f)));
                }
                else
                {
                    snowtimeOrb.damageValue = (characterBody.damage * (orbDamageCoefficient + skillLocator.secondary.bonusStockFromBody + skillLocator.secondary.bonusStockFromBody)) * ((Mathf.Clamp(((attackSpeedStat - 2.5f) / 2f), 1f, 9999f)));
                }
                
                snowtimeOrb.isCrit = isCrit;
                snowtimeOrb.teamIndex = TeamComponent.GetObjectTeam(gameObject);
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
            firingTime += Time.fixedDeltaTime;
            Inventory inventory = characterBody.inventory;
            int itemCountEffective = inventory.GetItemCountEffective(DLC1Content.Items.MoreMissile);
            int itemCountDTTurretlingPowerup = inventory.GetItemCountEffective(ItemCatalog.FindItemIndex("RainbowizerPowerUp"));
            if (itemCountEffective > 0 || itemCountDTTurretlingPowerup > 0 && RainbowizerPowerup.AdditionalMissiles.Value == true)
            {
                if (firingTime > refireTime && missilesFired < 4)
                {
                    FireOrbMissile();
                }
            }
            if (characterBody.isServer)
            {
                if (missileCheckPassed == true)
                {
                    CharacterBody obj = characterBody;
                    if ((object)obj != null && obj.inventory.GetItemCountEffective(DLC2Content.Items.IncreasePrimaryDamage) > 0)
                    {
                        characterBody.AddIncreasePrimaryDamageStack();
                    }
                }
            }
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
using EntityStates;
using RoR2;
using RoR2.Projectile;
using RoR2BepInExPack.GameAssetPaths;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using UnityEngine;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingGrenadeLauncher : GenericProjectileBaseState
    {
        private static int FireHash = Animator.StringToHash("turretling_fire");
        private static int FireParamHash = Animator.StringToHash("turretling_fire.playbackRate");
        public float myHue;
        public override void OnEnter()
        {
            duration = 1.5f / attackSpeedStat;
            damageCoefficient = 3f;
            force = 0f;
            projectilePitchBonus = -2f;
            if (gameObject.name.Contains("Survivor") || gameObject.name.Contains("PlayerMaster"))
            {
                projectilePrefab = Content.grenadePlayerObject;
            }
            else
            {
                projectilePrefab = Content.grenadeObject;
            }

            targetMuzzle = "Muzzle_Primary";
            attackSoundString = "Play_DemoTF2_GL";
            base.OnEnter();
            stopwatch = 0f;
            delayBeforeFiringProjectile = baseDelayBeforeFiringProjectile / attackSpeedStat;
            PlayAnimation(duration);

            // double up with icbm and scepter

            minSpread = 0.0f;
            maxSpread = 0.5f;
            Inventory inventory = characterBody.inventory;
            int itemCountEffective = inventory.GetItemCountEffective(DLC1Content.Items.MoreMissile);
            if (itemCountEffective != 0)
            {
                FireProjectile();
                minSpread = 0.5f;
                maxSpread = 1.25f;
            }
            if (!SnowtimeToyboxMod.scepterLoaded) return;
            int itemCountScepter = inventory.GetItemCountEffective(ItemCatalog.FindItemIndex("ITEM_ANCIENT_SCEPTER"));
            if(itemCountScepter != 0)
            {
                minSpread = 0.75f;
                maxSpread = 1.5f;
                FireProjectile();
                if (itemCountEffective != 0)
                {
                    minSpread = 1f;
                    maxSpread = 2f;
                    FireProjectile();
                }
            }
        }

        public override void PlayAnimation(float duration)
        {
            if ((bool)GetModelAnimator())
            {
                PlayAnimation("Gesture", FireHash, FireParamHash, duration);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void ModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            base.ModifyProjectileInfo(ref fireProjectileInfo);
            fireProjectileInfo.damageTypeOverride = DamageTypeCombo.GenericSpecial;
        }
    }
}

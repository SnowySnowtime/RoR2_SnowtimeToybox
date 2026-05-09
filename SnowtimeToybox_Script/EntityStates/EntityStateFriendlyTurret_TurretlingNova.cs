using EntityStates;
using EntityStates.AffixVoid;
using HG;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using SnowtimeToybox;
using SnowtimeToybox.Components;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class TurretlingEnergyNova : BaseState
    {
        public static GameObject novafx = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/Skills/turretling_novaeffect.prefab");
        public static GameObject novaPrefab = novafx;

        public static string attackSoundString = "Play_Turretling_Nova";

        public static float damageCoefficient = 3f;

        public static float force = 50.0f;

        public static float minSpread = 0f;

        public static float maxSpread = 2f;

        public static int bulletCount = 1;
        public static float radius = 15f;
        public static float newrad;

        public static float baseDuration = 0.4f;

        public int bulletCountCurrent = 1;

        private float duration = 1f;

        public float additionalStocks = 0;

        public override void OnEnter()
        {
            base.OnEnter();
            if(base.gameObject.name.Contains("Survivor"))
            {
                attackSoundString = "Play_Turretling_Nova_Player";
            }
            base.characterBody.SetAimTimer(0f);
            duration = baseDuration;
            additionalStocks = base.skillLocator.special.bonusStockFromBody;
            newrad = radius + (additionalStocks + additionalStocks + additionalStocks);
            //Log.Debug("Original Radius: " + radius + "... Now it is: " + newrad);
            Util.PlaySound(attackSoundString, base.gameObject);
            Ray aimRay = GetAimRay();
            TeamIndex myTeam = base.gameObject.GetComponent<TeamComponent>().teamIndex;
            Transform fxorigin = base.modelLocator.modelChildLocator.FindChild("HeadCenter").transform;
            var immunity = RoR2Content.Buffs.Immune;
            if (!base.HasBuff(immunity.buffIndex))
            {
                base.characterBody.AddTimedBuff(immunity.buffIndex, 0.15f);
            }
            if (base.isAuthority)
            {
                if ((bool)novaPrefab)
                {
			        EffectManager.SpawnEffect(novaPrefab, new EffectData
			        {
			        	origin = fxorigin.position,
			        	scale = newrad
                    }, transmit: true);
                }
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.position = fxorigin.position;
                blastAttack.radius = newrad;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.attacker = base.gameObject;
                blastAttack.inflictor = base.gameObject;
                blastAttack.damageColorIndex = DamageColorIndex.Electrocution;
                blastAttack.damageType.damageSource = DamageSource.Special;
                blastAttack.baseDamage = ((damageCoefficient + (additionalStocks/2)) * (Mathf.Clamp(((attackSpeedStat - 2.5f)), 1f, 9999f))) * damageStat;
                blastAttack.baseForce = 150f;
                blastAttack.bonusForce = Vector3.zero;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.procCoefficient = 0f;
                blastAttack.teamIndex = base.gameObject.GetComponent<TeamComponent>().teamIndex;
                blastAttack.Fire();
                RemoveNearbyProjectilesServer(myTeam, fxorigin.position, radius);
            }
        }

        private static void RemoveNearbyProjectilesServer(TeamIndex teamIndexToIgnore, Vector3 position, float radius)
        {
            float num = radius * radius;
            List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
            List<ProjectileController> list = CollectionPool<ProjectileController, List<ProjectileController>>.RentCollection();
            int i = 0;
            for (int count = instancesList.Count; i < count; i++)
            {
                ProjectileController projectileController = instancesList[i];
                if (!projectileController.cannotBeDeleted && projectileController.teamFilter.teamIndex != teamIndexToIgnore && (projectileController.transform.position - position).sqrMagnitude < num)
                {
                    list.Add(projectileController);
                }
            }
            int j = 0;
            for (int count2 = list.Count; j < count2; j++)
            {
                ProjectileController projectileController2 = list[j];
                if ((bool)projectileController2)
                {
                    Object.Destroy(projectileController2.gameObject);
                }
            }
            CollectionPool<ProjectileController, List<ProjectileController>>.ReturnCollection(list);
        }

        public override void OnExit()
        {
            base.OnExit();
            TeamIndex myTeam = base.gameObject.GetComponent<TeamComponent>().teamIndex;
            Transform fxorigin = base.modelLocator.modelChildLocator.FindChild("HeadCenter").transform;
            RemoveNearbyProjectilesServer(myTeam, fxorigin.position, radius);
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
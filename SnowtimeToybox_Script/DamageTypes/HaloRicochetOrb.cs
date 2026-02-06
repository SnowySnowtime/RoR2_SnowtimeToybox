using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using UnityEngine;


namespace SnowtimeToybox
{
    public class SnowtimeHaloRicochetOrb : GenericDamageOrb
    {
        public static GameObject orbEffectObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/PlasmaRifleOrbEffect.prefab");
        public GameObject orbEffectPrefab = orbEffectObject;

        public override void Begin()
        {
            this.speed = 220f;
            this.duration = Mathf.Max(this.distanceToTarget / this.speed, 0.167f);
            EffectData effectData = new EffectData()
            {
                scale = this.scale,
                origin = this.origin,
                genericFloat = this.duration
            };
            if ((bool)(UnityEngine.Object)this.target)
                effectData.SetHurtBoxReference(this.target);
            // EffectManager.SpawnEffect(snoworbEffectPrefab, effectData, true);
        }

        public override void OnArrival()
        {
            if (!(bool)(UnityEngine.Object)this.target || !(bool)(UnityEngine.Object)this.target.healthComponent || !this.target.healthComponent.alive)
                return;
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.damage = this.damageValue;
            damageInfo.attacker = this.attacker;
            damageInfo.inflictor = (GameObject)null;
            damageInfo.force = Vector3.zero;
            damageInfo.crit = this.isCrit;
            damageInfo.procChainMask = this.procChainMask;
            damageInfo.procCoefficient = this.procCoefficient;
            damageInfo.position = this.target.transform.position;
            damageInfo.damageColorIndex = this.damageColorIndex;
            damageInfo.damageType = this.damageType;
            damageInfo.inflictedHurtbox = this.target;
            this.target.healthComponent.TakeDamage(damageInfo);
            GlobalEventManager.instance.OnHitEnemy(damageInfo, this.target.healthComponent.gameObject);
            GlobalEventManager.instance.OnHitAll(damageInfo, this.target.healthComponent.gameObject);
        }

        // public override GameObject GetOrbEffect() => snoworbEffectPrefab;

        public static void CreateHaloRicochetOrb(
          DamageInfo damageInfo,
          TeamIndex attackerTeamIndex,
          CharacterBody victim,
          List<BodyIndex> killedBodies = null)
        {
            HurtBox hurtBox1 = (HurtBox)null;
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.searchOrigin = damageInfo.position;
            bullseyeSearch.searchDirection = Vector3.zero;
            bullseyeSearch.teamMaskFilter = TeamMask.GetEnemyTeams(attackerTeamIndex);
            bullseyeSearch.filterByLoS = false;
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.None;
            bullseyeSearch.maxAngleFilter = 180f;
            bullseyeSearch.maxDistanceFilter = 27f;
            bullseyeSearch.filterByDistinctEntity = true;
            bullseyeSearch.RefreshCandidates();
            if ((bool)(UnityEngine.Object)victim)
                bullseyeSearch.FilterOutGameObject(victim.gameObject);
            IEnumerable<HurtBox> results = bullseyeSearch.GetResults();
            float num = float.PositiveInfinity;
            foreach (HurtBox hurtBox2 in results)
            {
                if ((bool)(UnityEngine.Object)hurtBox2.healthComponent && (double)hurtBox2.healthComponent.combinedHealth < (double)num && hurtBox2.healthComponent.alive)
                {
                    num = hurtBox2.healthComponent.combinedHealth;
                    hurtBox1 = hurtBox2;
                }
            }
            if (!(bool)(UnityEngine.Object)hurtBox1)
                return;
            RicochetOrb ricochetOrb = new RicochetOrb();
            ricochetOrb.origin = damageInfo.position;
            ricochetOrb.effectPrefab = orbEffectObject;
            ricochetOrb.damageValue = damageInfo.damage * 0.75f;
            ricochetOrb.isCrit = damageInfo.crit;
            ricochetOrb.teamIndex = attackerTeamIndex;
            ricochetOrb.attacker = damageInfo.attacker;
            ricochetOrb.procChainMask = damageInfo.procChainMask;
            ricochetOrb.procCoefficient = 0.3f;
            ricochetOrb.damageType = (DamageTypeCombo)DamageType.Generic;
            ricochetOrb.damageType.damageSource = DamageSource.Primary;
            ricochetOrb.damageColorIndex = DamageColorIndex.Default;
            ricochetOrb.target = hurtBox1;
            OrbManager.instance.AddOrb((Orb)ricochetOrb);
        }
        public class RicochetOrb : GenericDamageOrb
        {
            public GameObject effectPrefab;

            public override GameObject GetOrbEffect() => this.effectPrefab;
        }
    }
}
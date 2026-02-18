using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace SnowtimeToybox
{
    public class SnowtimeOrbs : Orb
    {
        public enum OrbTypes
        {
            ShortcakeTaunt,
            ShortcakeRetaliate,
            ShortcakeRetaliateFriendly
        }

        public float speed = 200f;

        public float damageValue;

        public GameObject attacker;

        public GameObject inflictor;

        public TeamIndex teamIndex;

        public bool isCrit;

        public ProcChainMask procChainMask;

        public float procCoefficient = 1f;

        public DamageColorIndex damageColorIndex;

        public float range = 20f;

        public DamageTypeCombo damageType = DamageType.Generic;

        public OrbTypes snowtimeOrbType;

        private bool isElectric;
        private bool isHealing;

        public static GameObject orbShortcakeRetaliateObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcakeretaliate_orbeffect.prefab");
        public static GameObject orbShortcakeRetaliateImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcakeretaliate_impacteffect.prefab");
        public static GameObject orbShortcakeRetaliateFriendlyObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcakeretaliatefriendly_orbeffect.prefab");
        public static GameObject orbShortcakeRetaliateFriendlyImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcakeretaliatefriendly_impacteffect.prefab");
        public GameObject orbShortcakeRetaliatePrefab = orbShortcakeRetaliateObject;
        public GameObject orbShortcakeRetaliateFriendlyPrefab = orbShortcakeRetaliateFriendlyObject;
        public static GameObject orbShortcakeTauntObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcaketaunt_orbeffect.prefab");
        public static GameObject orbShortcakeTauntImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Shortcake/Skills/shortcaketaunt_impacteffect.prefab");
        public GameObject orbShortcakeTauntPrefab = orbShortcakeTauntObject;

        public override void Begin()
        {
            base.duration = Mathf.Max(this.distanceToTarget / this.speed, 0.1f);;
            GameObject orbasset = null;
            switch (snowtimeOrbType)
            {
                case OrbTypes.ShortcakeTaunt:
                    orbasset = orbShortcakeTauntPrefab;
                    isHealing = false;
                    isElectric = false;
                    break;
                case OrbTypes.ShortcakeRetaliate:
                    orbasset = orbShortcakeRetaliatePrefab;
                    isHealing = false;
                    isElectric = false;
                    break;
                case OrbTypes.ShortcakeRetaliateFriendly:
                    orbasset = orbShortcakeRetaliateFriendlyPrefab;
                    isHealing = true;
                    isElectric = false;
                    break;
            }
            EffectData effectData = new EffectData
            {
                origin = origin,
                genericFloat = (base.duration * 2f)
            };
            effectData.SetHurtBoxReference(target);
            EffectManager.SpawnEffect(orbasset, effectData, transmit: true);
        }

        public override void OnArrival()
        {
            if (!target)
            {
                return;
            }
            HealthComponent healthComponent = target.healthComponent;
            if ((bool)healthComponent)
            {
                DamageInfo damageInfo = new DamageInfo();
                if (!isHealing)
                {
                    damageInfo.damage = damageValue;
                }
                else
                {
                    healthComponent.Heal( (target.healthComponent.fullHealth * 0.025f) + damageValue, procChainMask);
                }
                damageInfo.attacker = attacker;
                damageInfo.inflictor = inflictor;
                damageInfo.force = Vector3.zero;
                damageInfo.crit = isCrit;
                damageInfo.procChainMask = procChainMask;
                damageInfo.procCoefficient = procCoefficient;
                damageInfo.position = target.transform.position;
                damageInfo.damageColorIndex = damageColorIndex;
                damageInfo.damageType = damageType;
                if (isElectric)
                {
                    damageInfo.damageType.damageTypeExtended = DamageTypeExtended.Electrical;
                }
                damageInfo.inflictedHurtbox = target;
                healthComponent.TakeDamage(damageInfo);
                GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
            }
        }
    }
}
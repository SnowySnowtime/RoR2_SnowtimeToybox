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
            ShortcakeRetaliateFriendly,
            TurretlingMissile,
            TurretlingMissile_Acanthi,
            TurretlingMissile_Borbo,
            TurretlingMissile_Bread,
            TurretlingMissile_Shortcake,
            TurretlingMissile_Snowtime
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

        public static GameObject orbTurretlingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_orbeffect.prefab");
        public static GameObject orbTurretlingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Skills/turretling_impacteffect.prefab");
        public GameObject orbTurretlingMissilePrefab = orbTurretlingMissileObject;

        public static GameObject orbAcanthilingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx__Missile_Orb_Acanthiling.prefab");
        public static GameObject orbAcanthilingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx__Missile_Impact_Acanthiling.prefab");
        public GameObject orbAcanthilingMissilePrefab = orbAcanthilingMissileObject;

        public static GameObject orbBorbolingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx__Missile_Orb_Borboling.prefab");
        public static GameObject orbBorbolingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx__Missile_Impact_Borboling.prefab");
        public GameObject orbBorbolingMissilePrefab = orbBorbolingMissileObject;

        public static GameObject orbBreadlingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx__Missile_Orb_Breadling.prefab");
        public static GameObject orbBreadlingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx__Missile_Impact_Breadling.prefab");
        public GameObject orbBreadlingMissilePrefab = orbBreadlingMissileObject;

        public static GameObject orbShortcakelingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx__Missile_Orb_Shortcakeling.prefab");
        public static GameObject orbShortcakelingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx__Missile_Impact_Shortcakeling.prefab");
        public GameObject orbShortcakelingMissilePrefab = orbShortcakelingMissileObject;

        public static GameObject orbSnowtimelingMissileObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx__Missile_Orb_Snowtimeling.prefab");
        public static GameObject orbSnowtimelingMissileImpactObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/vfx__Missile_Impact_Snowtimeling.prefab");
        public GameObject orbSnowtimelingMissilePrefab = orbSnowtimelingMissileObject;

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
                case OrbTypes.TurretlingMissile:
                    orbasset = orbTurretlingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    break;
                case OrbTypes.TurretlingMissile_Acanthi:
                    orbasset = orbAcanthilingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    break;
                case OrbTypes.TurretlingMissile_Borbo:
                    orbasset = orbBorbolingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    break;
                case OrbTypes.TurretlingMissile_Bread:
                    orbasset = orbBreadlingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    break;
                case OrbTypes.TurretlingMissile_Shortcake:
                    orbasset = orbShortcakelingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    break;
                case OrbTypes.TurretlingMissile_Snowtime:
                    orbasset = orbSnowtimelingMissilePrefab;
                    isHealing = false;
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
                    damageInfo.procChainMask = procChainMask;
                    damageInfo.procCoefficient = procCoefficient;
                }
                else
                {
                    healthComponent.Heal( (target.healthComponent.fullHealth * 0.025f) + damageValue, procChainMask);
                    damageInfo.procCoefficient = 0f;
                }
                damageInfo.attacker = attacker;
                damageInfo.inflictor = inflictor;
                damageInfo.force = Vector3.zero;
                damageInfo.crit = isCrit;
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
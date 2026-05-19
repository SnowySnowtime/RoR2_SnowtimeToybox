using R2API;
using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Linq;
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
            TurretlingMissile_Snowtime,
            TurretlingMissile_Player,
            TurretlingMissile_Rainbow,
            TurretlingNeedler
        }

        public float speed = 250f;

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
        private bool isRainbow;
        private bool isMissile;
        private bool isNeedler;

        public GameObject orbShortcakeRetaliatePrefab = Content.orbShortcakeRetaliateObject;
        public GameObject orbShortcakeRetaliateFriendlyPrefab = Content.orbShortcakeRetaliateFriendlyObject;
        public GameObject orbShortcakeTauntPrefab = Content.orbShortcakeTauntObject;

        public GameObject orbTurretlingMissilePrefab = Content.orbTurretlingMissileObject;
        public GameObject orbAcanthilingMissilePrefab = Content.orbAcanthilingMissileObject;
        public GameObject orbBorbolingMissilePrefab = Content.orbBorbolingMissileObject;
        public GameObject orbBreadlingMissilePrefab = Content.orbBreadlingMissileObject;
        public GameObject orbShortcakelingMissilePrefab = Content.orbShortcakelingMissileObject;
        public GameObject orbSnowtimelingMissilePrefab = Content.orbSnowtimelingMissileObject;
        public GameObject orbRainbowMissilePrefab = Content.orbRainbowMissileObject;
        public GameObject orbPlayerMissilePrefab = Content.orbPlayerMissileObject;
        public GameObject orbNeedlerPrefab = Content.SwarmNeedlerOrb;

        public override void Begin()
        {
            GameObject orbasset = null;
            switch (snowtimeOrbType)
            {
                case OrbTypes.ShortcakeTaunt:
                    orbasset = orbShortcakeTauntPrefab;
                    isHealing = false;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = false;
                    isNeedler = false;
                    break;
                case OrbTypes.ShortcakeRetaliate:
                    orbasset = orbShortcakeRetaliatePrefab;
                    isHealing = false;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = false;
                    isNeedler = false;
                    break;
                case OrbTypes.ShortcakeRetaliateFriendly:
                    orbasset = orbShortcakeRetaliateFriendlyPrefab;
                    isHealing = true;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = false;
                    isNeedler = false;
                    break;
                case OrbTypes.TurretlingMissile:
                    orbasset = orbTurretlingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = true;
                    isNeedler = false;
                    break;
                case OrbTypes.TurretlingMissile_Acanthi:
                    orbasset = orbAcanthilingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = true;
                    isNeedler = false;
                    break;
                case OrbTypes.TurretlingMissile_Borbo:
                    orbasset = orbBorbolingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = true;
                    isNeedler = false;
                    break;
                case OrbTypes.TurretlingMissile_Bread:
                    orbasset = orbBreadlingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = true;
                    isNeedler = false;
                    break;
                case OrbTypes.TurretlingMissile_Shortcake:
                    orbasset = orbShortcakelingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = true;
                    isNeedler = false;
                    break;
                case OrbTypes.TurretlingMissile_Snowtime:
                    orbasset = orbSnowtimelingMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = true;
                    isNeedler = false;
                    break;
                case OrbTypes.TurretlingMissile_Rainbow:
                    orbasset = orbRainbowMissilePrefab;
                    isHealing = false;
                    isElectric = true;
                    isRainbow = true;
                    isMissile = true;
                    isNeedler = false;
                    break;
                case OrbTypes.TurretlingMissile_Player:
                    orbasset = orbPlayerMissilePrefab;
                    isHealing = false;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = true;
                    isNeedler = false;
                    break;
                case OrbTypes.TurretlingNeedler:
                    orbasset = orbNeedlerPrefab;
                    isHealing = false;
                    isElectric = false;
                    isRainbow = false;
                    isMissile = false;
                    isNeedler = true;
                    speed = 125f;
                    break;
            }
            base.duration = Mathf.Max(this.distanceToTarget / this.speed, 0.1f);
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
                if (isNeedler)
                {
                    DamageAPI.AddModdedDamageType(damageInfo, SnowtimeToyboxMod.SwarmlingNeedleImpale);
                }
                damageInfo.inflictedHurtbox = target;
                healthComponent.TakeDamage(damageInfo);
                GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
            }
        }
    }
}
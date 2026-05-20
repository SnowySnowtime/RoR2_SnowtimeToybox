using R2API;
using RoR2;
using SnowtimeToybox;
using SnowtimeToybox.FriendlyTurrets;
using System;
using UnityEngine;
using static R2API.RecalculateStatsAPI;

namespace SnowtimeToybox.Buffs
{
    public class SwarmlingNeedlerDebuff : BuffBase<SwarmlingNeedlerDebuff>
    {
        public override BuffDef Buff => Content.SwarmlingNeedlerDebuff;
        public int currentStack;

        public override void PostCreation()
        {
            //RecalculateStatsAPI.GetStatCoefficients += SwarmlingArmorStealHook;
            On.RoR2.CharacterBody.HandleCascadingBuffs += NeedlerCascadingDebuff;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += NeedlerDebuffLost;
            //On.RoR2.CharacterBody.FixedUpdate += NeedlerDebuffHandler;
        }
        private void NeedlerCascadingDebuff(On.RoR2.CharacterBody.orig_HandleCascadingBuffs orig, CharacterBody self)
        {
            orig(self);
            if (self.HasBuff(Content.SwarmlingNeedlerDebuff) && self.GetBuffCount(Content.SwarmlingNeedlerDebuff) < currentStack)
            {
                SmallDetonation(1f, self);
            }
            currentStack = self.GetBuffCount(Content.SwarmlingNeedlerDebuff);
        }
        private void NeedlerDebuffLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            orig(self, buffDef);
            if (buffDef == Content.SwarmlingNeedlerDebuff)
            {
                SmallDetonation(1f, self);
            }
        }
        public void SmallDetonation(float explosionRadius, CharacterBody victimBody)
        {
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.radius = explosionRadius;
            blastAttack.baseDamage = 1f;
            blastAttack.procCoefficient = 0f;
            blastAttack.damageColorIndex = DamageColorIndex.Default;
            blastAttack.damageColorIndex = Content.NeedlerColor;
            blastAttack.attackerFiltering = AttackerFiltering.AlwaysHitSelf;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.attacker = victimBody.gameObject;
            blastAttack.position = victimBody.mainHurtBox.transform.position;
            blastAttack.damageType = DamageType.BypassArmor;
            blastAttack.baseForce = 1f;
            blastAttack.teamIndex = TeamIndex.Player;
            blastAttack.Fire();
            EffectManager.SpawnEffect(Content.SwarmNeedlerExpire, new EffectData
            {
                origin = victimBody.mainHurtBox.transform.position,
                scale = explosionRadius + victimBody.radius,
                rotation = Util.QuaternionSafeLookRotation(victimBody.transform.forward)
            }, transmit: true);
        }
    }
}
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
            //On.RoR2.CharacterBody.FixedUpdate += NeedlerDebuffHandler;
        }

        //private void NeedlerDebuffHandler(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        //{
        //    orig(self);
        //    if(self.HasBuff(Buff))
        //    {
        //        if(self.GetBuffCount(Buff) > currentStack)
        //        {
        //            SmallDetonation(1f, self);
        //        }
        //        currentStack = self.GetBuffCount(Buff);
        //    }
        //}

        private void NeedlerCascadingDebuff(On.RoR2.CharacterBody.orig_HandleCascadingBuffs orig, CharacterBody self)
        {
            orig(self);
            if(self.HasBuff(Content.SwarmlingNeedlerDebuff) && self.GetBuffCount(Content.SwarmlingNeedlerDebuff) >= 7)
            {
                self.ClearTimedBuffs(Content.SwarmlingNeedlerDebuff);
                SuperCombine(15f, self);
            }
            if (self.HasBuff(Content.SwarmlingNeedlerDebuff) && self.GetBuffCount(Content.SwarmlingNeedlerDebuff) < currentStack)
            {
                SmallDetonation(1f, self);
            }
            currentStack = self.GetBuffCount(Content.SwarmlingNeedlerDebuff);
        }

        public void SuperCombine(float explosionRadius, CharacterBody victimBody)
        {
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.radius = explosionRadius;
            blastAttack.baseDamage = 7f;
            blastAttack.procCoefficient = 1f;
            blastAttack.damageColorIndex = DamageColorIndex.Default;
            blastAttack.attackerFiltering = AttackerFiltering.AlwaysHitSelf;
            blastAttack.falloffModel = BlastAttack.FalloffModel.None;
            blastAttack.attacker = victimBody.gameObject;
            blastAttack.position = victimBody.mainHurtBox.transform.position;
            blastAttack.damageType = DamageType.BypassArmor;
            blastAttack.baseForce = 1f;
            blastAttack.teamIndex = TeamIndex.Player;
            blastAttack.Fire();
            EffectManager.SpawnEffect(Content.SwarmNeedlerSuperCombine, new EffectData
            {
                origin = victimBody.mainHurtBox.transform.position,
                scale = explosionRadius + victimBody.radius,
                rotation = Util.QuaternionSafeLookRotation(victimBody.transform.forward)
            }, transmit: true);
        }

        public void SmallDetonation(float explosionRadius, CharacterBody victimBody)
        {
            BlastAttack blastAttack = new BlastAttack();
            blastAttack.radius = explosionRadius;
            blastAttack.baseDamage = 1f;
            blastAttack.procCoefficient = 0f;
            blastAttack.damageColorIndex = DamageColorIndex.Default;
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
using BepInEx.Configuration;
using IL.RoR2.CharacterAI;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using AISkillDriver = RoR2.CharacterAI.AISkillDriver;

namespace SnowtimeToybox.Items;

public class MinionNeedlerHandler : ItemBase<MinionNeedlerHandler>
{
    public override ItemDef ItemDef => SnowtimeToyboxMod._stitemAssetBundle.LoadAsset<ItemDef>(@"Assets/SnowtimeMod/Assets/Items/Hidden/MinionNeedlerHandler.asset");

    public override ItemDisplayRuleDict CreateItemDisplayRules()
    {
        return new ItemDisplayRuleDict();
    }

    public override void Init(ConfigFile config)
    {
        CreateConfig(config);
        CreateLang();
        CreateItem();
        Hooks();
        // TODO - Mark as Hidden when done.
        Hidden = true;
    }

    public override void CreateConfig(ConfigFile config)
    {
    }

    public override void Hooks()
    {
        RecalculateStatsAPI.GetStatCoefficients += RecalculateStats;
    }
    private void RecalculateStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
    {
        if (sender)
        {
            if (sender.name.Contains("TurretlingSurvivor")) return;

            var itemCount = GetCount(sender);
            if (itemCount != 0 && sender.healthComponent != null)
            {
                GenericSkill[] genericSkills = sender.gameObject.GetComponents<GenericSkill>();
                //Log.Debug("Skill Moment...");
                foreach (GenericSkill genericSkill in genericSkills)
                {
                    //Log.Debug("Skill Found...");
                    if (genericSkill.skillFamily == Content.FriendlyTurretTurretlingSecondarySkillFamily)
                    {
                        genericSkill.SetSkillOverride(sender.skillLocator.secondary, Content.FriendlyTurretTurretlingSecondaryAltSkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                        //Log.Debug("Skill Replaced");
                    }
                }
                AISkillDriver[] skillDrivers = sender.master.gameObject.GetComponents<AISkillDriver>();
                foreach(AISkillDriver skillDriver in skillDrivers)
                {
                    if(skillDriver.customName == "Priority_FireSkill")
                    {
                        skillDriver.driverUpdateTimerOverride = 0.3f;
                        skillDriver.buttonPressType = 0;
                    }
                }
            }
        }
    }
}
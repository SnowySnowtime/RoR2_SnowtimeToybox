using RoR2;
using RoR2.EntityLogic;
using SnowtimeToybox;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace SnowtimeToybox.FriendlyTurretChecks
{
    public class BorboCheck : NetworkBehaviour
    {
        public PurchaseInteraction purchaseInteraction;
        public static GameObject turretUseEffect = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/TurretUseEffect.prefab");

        SummonMasterBehavior summonMasterBehavior;

        public void Awake()
        {
            if (NetworkServer.active && Run.instance)
            {
                purchaseInteraction.SetAvailable(true);
                //Log.Debug("Added BorboCheck");
            }

            if (!summonMasterBehavior)
            {
                summonMasterBehavior = GetComponent<SummonMasterBehavior>();
            }

            purchaseInteraction.onDetailedPurchaseServer.AddListener(OnDetailedPurchase);

            if (SnowtimeToyboxMod.ToggleSpawnMessages.Value)
            {
                switch (purchaseInteraction.displayNameToken)
                {
                    case "FRIENDLYTURRET_BORBO_BROKEN_NAME": // Borbo Turret Selected
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#A8B1FF>Friendly Turret: pleas repair me, i will update 2r4r if you do..!</color></style>" });
                        break;  
                    case "FRIENDLYTURRET_SHORTCAKE_BROKEN_NAME": // Strawberry Shortcake Turret Selected
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#D3A8FF>Friendly Turret: uwaah! pleas repair me, i have a cake for you if you do..!</color></style>" });
                        break;
                    case "FRIENDLYTURRET_SNOWTIME_BROKEN_NAME": // Snowtime Turret Selected
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#A8D3FF>Friendly Turret: bwaaaa! pleas repair me, i have a snowcone for you if you do..!</color></style>" });
                        break;
                    case "FRIENDLYTURRET_ACANTHI_BROKEN_NAME": // Snowtime Turret Selected
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#FF9C9C>Friendly Turret: hjelp! pleas repair me, i have a flower for you if you do..!</color></style>" });
                        break;
                }
            }
        }

        [Server]
        public void OnDetailedPurchase(CostTypeDef.PayCostContext payCostContext, CostTypeDef.PayCostResults payCostResult)
        {
            purchaseInteraction.SetAvailable(false);
            if (SnowtimeToyboxMod.ToggleSpawnMessages.Value)
            {
                switch (purchaseInteraction.displayNameToken)
                {
                    case "FRIENDLYTURRET_BORBO_BROKEN_NAME": // Borbo Turret Selected
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#30ff78>borbo turret: Thank you! We are friends now!</color></style>" });
                        break;
                    case "FRIENDLYTURRET_SHORTCAKE_BROKEN_NAME": // Strawberry Shortcake Turret Selected
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#30ff78>Strawberry Shortcake Turret: Thank you friend! Let's take ibuprofen together? No? Aw. I'll defend you instead!</color></style>" });
                        break;
                    case "FRIENDLYTURRET_SNOWTIME_BROKEN_NAME": // Snowtime Turret Selected
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#30ff78>Snowtime Turret: Thank you! We are friends now!</color></style>" });
                        break;
                    case "FRIENDLYTURRET_ACANTHI_BROKEN_NAME": // Acanthi Turret Selected
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#30ff78>Acanthi Turret: Thank you! We are friends now! :soyeyes:</color></style>" });
                        break;
                }
            }
            EffectManager.SpawnEffect(turretUseEffect, new EffectData()
            {
                origin = gameObject.transform.position,
                rotation = Quaternion.identity,
                scale = 1f,
                color = Color.white,
            }, true);

            summonMasterBehavior.OpenSummon(payCostContext.activator);

            EventFunctions.Destroy(purchaseInteraction);
        }
    }
}
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

            if (!SnowtimeToyboxMod.ToggleSpawnMessages.Value)
            {
                return;
            }
            else
            {
                // Borbo Turret Selected
                if (purchaseInteraction.displayNameToken == "FRIENDLYTURRET_BORBO_BROKEN_NAME")
                {
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#A8D3FF>Friendly Turret: pleas repair me, i will update 2r4r if you do..!</color></style>" });
                }
                // Strawberry Shortcake Turret Selected
            }
        }

        [Server]
        public void OnDetailedPurchase(CostTypeDef.PayCostContext payCostContext, CostTypeDef.PayCostResults payCostResult)
        {
            purchaseInteraction.SetAvailable(false);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#30ff78>Thank you! We are friends now!</color></style>" });
            summonMasterBehavior.OpenSummon(payCostContext.activator);
            EffectManager.SpawnEffect(turretUseEffect, new EffectData()
            {
                origin = gameObject.transform.position,
                rotation = Quaternion.identity,
                scale = 1f,
                color = Color.white,
            }, true);

            EventFunctions.Destroy(purchaseInteraction);
        }
    }
}
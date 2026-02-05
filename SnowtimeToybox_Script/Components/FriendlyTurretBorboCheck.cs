using RoR2;
using System.Collections.ObjectModel;
using UnityEngine.Networking;
using SnowtimeToybox;
using System.Linq;
using RoR2.EntityLogic;
using System.Collections;
using UnityEngine;

namespace SnowtimeToybox.FriendlyTurretChecks
{
    public class BorboCheck : NetworkBehaviour
    {
        public PurchaseInteraction purchaseInteraction;
        SummonMasterBehavior summonMasterBehavior;

        public void Awake()
        {
            if (NetworkServer.active && Run.instance)
            {
                purchaseInteraction.SetAvailable(true);
                Log.Debug("Added BorboCheck");
            }

            if (!summonMasterBehavior)
            {
                summonMasterBehavior = GetComponent<SummonMasterBehavior>();
            }

            purchaseInteraction.onDetailedPurchaseServer.AddListener(OnDetailedPurchase);

            if (purchaseInteraction.displayNameToken == "FRIENDLYTURRET_BORBO_NAME")
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#A8D3FF>Friendly Turret: pleas repair me, i will update 2r4r if you do..!</color></style>" });
            }
        }

        [Server]
        public void OnDetailedPurchase(CostTypeDef.PayCostContext payCostContext, CostTypeDef.PayCostResults payCostResult)
        {
            purchaseInteraction.SetAvailable(false);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#30ff78>Thank you! We are friends now!</color></style>" });
            summonMasterBehavior.OpenSummon(payCostContext.activator);
            EventFunctions.Destroy(purchaseInteraction);
        }
    }
}
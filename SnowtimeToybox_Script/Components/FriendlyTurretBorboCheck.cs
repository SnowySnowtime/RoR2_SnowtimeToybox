using RoR2;
using System.Collections.ObjectModel;
using UnityEngine.Networking;
using SnowtimeToybox;
using System.Linq;

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
            Log.Debug("Checking Player Minion");
            ReadOnlyCollection<PlayerCharacterMasterController> playerMasters = PlayerCharacterMasterController.instances;
            foreach (PlayerCharacterMasterController player in playerMasters)
            {
                Log.Debug(player.name);
                Log.Debug("foreach (PlayerCharacterMasterController player in playerMasters)");
                CharacterBody[] minionBodies = player.body.GetMinionBodies();
                Log.Debug(minionBodies);
                Log.Debug(minionBodies.Length);
                if (minionBodies.Length >= 0)
                {
                    Log.Debug("No minions found, skipping check!");
                    purchaseInteraction.SetAvailable(false);
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#30ff78>Thank you! We are friends now!</color></style>" });
                    summonMasterBehavior.OpenSummon(payCostContext.activator);
                    EventFunctions.Destroy(purchaseInteraction);
                }
                else
                foreach (CharacterBody body in minionBodies)
                {
                    Log.Debug("foreach (CharacterBody body in minionBodies)");
                    Log.Debug(body.name);
                    Log.Debug(body.GetOwnerBody());
                    Log.Debug(payCostContext.activator.name);
                    if (body.name == "FRIENDLYTURRET_BORBO_NAME")
                    {
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#ffa530>You cannot acquire another Borbo Turret!</color></style>" });
                    }
                    else
                    {
                        purchaseInteraction.SetAvailable(false);
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = "<style=cEvent><color=#30ff78>Thank you! We are friends now!</color></style>" });
                        summonMasterBehavior.OpenSummon(payCostContext.activator);
                        EventFunctions.Destroy(purchaseInteraction);
                    }
                }
            }
        }
    }
}
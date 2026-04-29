using IL.RoR2.Achievements.Railgunner;
using RoR2;
using RoR2.Navigation;
using SnowtimeToybox.FriendlyTurrets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SnowtimeToybox.Components;
[RequireComponent(typeof(CharacterBody))]
public class SwarmMinionSwarmlingTeleportHandler : MonoBehaviour
{
    public CharacterBody self;
    private RigidbodyMotor rigidbodyMotor;

    public void Start()
    {
        self = gameObject.GetComponent<CharacterBody>();
    }
    
    public void StartTeleporting(Vector3 position)
    {
        CharacterMaster master = self.master;
        CharacterMaster characterMaster = (master ? master.minionOwnership.ownerMaster : null);
        CharacterBody characterBody = (characterMaster ? characterMaster.GetBody() : null);
        if (self.hasEffectiveAuthority && (bool)characterBody && (((bool)self.characterMotor) || ((bool)rigidbodyMotor)))
        {
            StartCoroutine(Teleport(position));
        }
    }
    public IEnumerator Teleport(Vector3 ownerPosition)
    {
        //CharacterMaster master = self.master;
        //CharacterMaster characterMaster = (master ? master.minionOwnership.ownerMaster : null);
        //CharacterBody ownerBody = (characterMaster ? characterMaster.GetBody() : null);
        NodeGraph nodeGraph = SceneInfo.instance.GetNodeGraph(MapNodeGroup.GraphType.Ground);
        //if (!ownerBody)
        //{
        //    yield break;
        //}
        List<NodeGraph.NodeIndex> list = nodeGraph.FindNodesInRangeWithFlagConditions(ownerPosition, 3f, 20f, HullMask.None, NodeFlags.None, NodeFlags.NoCharacterSpawn, preventOverhead: false);
        while (list.Count == 0)
        {
            yield return new WaitForSeconds(1f);
            list = nodeGraph.FindNodesInRangeWithFlagConditions(ownerPosition, 3f, 20f, HullMask.None, NodeFlags.None, NodeFlags.NoCharacterSpawn, preventOverhead: false);
        }
        while (list.Count > 0)
        {
            int index = Random.Range(0, list.Count);
            NodeGraph.NodeIndex nodeIndex = list[index];
            if (nodeGraph.GetNodePosition(nodeIndex, out var position))
            {
                TeleportHelper.TeleportBody(self, position);
                GameObject teleportEffectPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Huntress.HuntressBlinkEffect_prefab).WaitForCompletion();
                if ((bool)teleportEffectPrefab)
                {
                    EffectManager.SimpleEffect(teleportEffectPrefab, position, Quaternion.identity, transmit: true);
                    Util.PlaySound("Play_huntress_shift_end", base.gameObject);
                }
                break;
            }
        }
    }
}
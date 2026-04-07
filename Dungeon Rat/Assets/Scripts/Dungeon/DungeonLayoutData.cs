using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLayout", menuName = "Dungeon/Dungeon Layout")]
public class DungeonLayoutData : ScriptableObject
{
    public List<BattleRoomEnemySetup> roomEnemySetups = new List<BattleRoomEnemySetup>();
}
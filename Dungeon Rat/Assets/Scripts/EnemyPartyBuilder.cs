using System.Collections.Generic;

public static class EnemyPartyBuilder
{
    public static List<EnemyCharacter> Build(BattleRoomEnemySetup setup)
    {
        List<EnemyCharacter> result = new List<EnemyCharacter>();

        if (setup == null || setup.enemies == null)
            return result;

        for (int i = 0; i < setup.enemies.Count; i++)
        {
            EnemySpawnInfo spawnInfo = setup.enemies[i];

            if (spawnInfo == null || spawnInfo.enemyData == null)
                continue;

            result.Add(new EnemyCharacter(spawnInfo.enemyData, spawnInfo.position));
        }

        return result;
    }
}
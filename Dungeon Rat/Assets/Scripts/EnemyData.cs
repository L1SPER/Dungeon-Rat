using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Dungeon/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public Sprite enemySprite;

    [Header("Stats")]
    public int maxHealth = 30;
    public int damage = 10;
    public int range = 1;
    public int armor = 0;
}
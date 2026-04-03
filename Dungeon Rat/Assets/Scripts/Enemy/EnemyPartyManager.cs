using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyPartyManager : MonoBehaviour
{
    private EnemyCharacter[] enemySlots = new EnemyCharacter[3];

    public event Action OnEnemyPartyChanged;

    public void SetEnemyParty(List<EnemyCharacter> enemies)
    {
        ClearParty(false);

        if (enemies == null || enemies.Count == 0)
        {
            Debug.LogWarning("Enemy party is null or empty.");
            OnEnemyPartyChanged?.Invoke();
            return;
        }

        for (int i = 0; i < enemies.Count && i < enemySlots.Length; i++)
        {
            EnemyCharacter enemy = enemies[i];

            if (enemy == null)
                continue;

            enemySlots[i] = enemy;
            enemy.position = i + 1;

            RegisterEnemy(enemy);
        }

        PrintParty();
        OnEnemyPartyChanged?.Invoke();
    }

    public bool AddEnemy(EnemyCharacter enemy)
    {
        if (enemy == null)
        {
            Debug.LogWarning("Enemy is null.");
            return false;
        }

        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i] == null)
            {
                enemySlots[i] = enemy;
                enemy.position = i + 1;

                RegisterEnemy(enemy);

                Debug.Log($"{enemy.EnemyName} added to enemy party. Position: {enemy.position}");
                OnEnemyPartyChanged?.Invoke();
                return true;
            }
        }

        Debug.Log("Enemy party is full.");
        return false;
    }

    private void RegisterEnemy(EnemyCharacter enemy)
    {
        if (enemy == null || enemy.health == null)
            return;

        enemy.health.OnDeath -= OnAnyEnemyDeath;
        enemy.health.OnDeath += OnAnyEnemyDeath;
    }

    private void OnAnyEnemyDeath()
    {
        CompactParty();
        PrintParty();
        OnEnemyPartyChanged?.Invoke();
    }

    public void CompactParty()
    {
        EnemyCharacter[] newSlots = new EnemyCharacter[enemySlots.Length];
        int writeIndex = 0;

        for (int i = 0; i < enemySlots.Length; i++)
        {
            EnemyCharacter enemy = enemySlots[i];

            if (enemy == null)
                continue;

            if (enemy.health != null && enemy.health.isDead)
                continue;

            newSlots[writeIndex] = enemy;
            newSlots[writeIndex].position = writeIndex + 1;
            writeIndex++;
        }

        enemySlots = newSlots;
    }

    public EnemyCharacter[] GetPartyMembers()
    {
        return enemySlots;
    }

    public List<EnemyCharacter> GetAliveMembers()
    {
        List<EnemyCharacter> result = new List<EnemyCharacter>();

        for (int i = 0; i < enemySlots.Length; i++)
        {
            EnemyCharacter enemy = enemySlots[i];

            if (enemy == null || enemy.health == null || enemy.health.isDead)
                continue;

            result.Add(enemy);
        }

        return result;
    }

    public List<EnemyCharacter> GetAllMembersForLog()
    {
        List<EnemyCharacter> result = new List<EnemyCharacter>();

        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i] != null)
                result.Add(enemySlots[i]);
        }

        return result;
    }

    public List<EnemyCharacter> GetAliveMembersInRange(int range)
    {
        List<EnemyCharacter> aliveMembers = GetAliveMembers();
        List<EnemyCharacter> result = new List<EnemyCharacter>();

        int clampedRange = Mathf.Clamp(range, 1, 99);

        for (int i = 0; i < aliveMembers.Count; i++)
        {
            if (i >= clampedRange)
                break;

            result.Add(aliveMembers[i]);
        }

        return result;
    }

    public EnemyCharacter GetFrontAliveMember()
    {
        List<EnemyCharacter> aliveMembers = GetAliveMembers();
        return aliveMembers.Count > 0 ? aliveMembers[0] : null;
    }

    public EnemyCharacter GetEnemyBehind(EnemyCharacter targetEnemy)
    {
        if (targetEnemy == null)
            return null;

        int targetPosition = targetEnemy.position;

        EnemyCharacter bestCandidate = null;
        int closestHigherPosition = int.MaxValue;

        for (int i = 0; i < enemySlots.Length; i++)
        {
            EnemyCharacter candidate = enemySlots[i];

            if (candidate == null || candidate.health == null || candidate.health.isDead)
                continue;

            if (candidate.position > targetPosition && candidate.position < closestHigherPosition)
            {
                closestHigherPosition = candidate.position;
                bestCandidate = candidate;
            }
        }

        return bestCandidate;
    }

    public EnemyCharacter GetLowestShieldAliveMemberExcept(EnemyCharacter source)
    {
        EnemyCharacter bestTarget = null;
        int lowestShield = int.MaxValue;

        for (int i = 0; i < enemySlots.Length; i++)
        {
            EnemyCharacter candidate = enemySlots[i];

            if (candidate == null || candidate.health == null || candidate.health.isDead || candidate == source)
                continue;

            if (candidate.shield != null && candidate.shield.currentShield < lowestShield)
            {
                lowestShield = candidate.shield.currentShield;
                bestTarget = candidate;
            }
        }

        return bestTarget;
    }

    public bool AreAllDead()
    {
        return GetAliveMembers().Count == 0;
    }

    public bool HasEmptySlot()
    {
        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i] == null)
                return true;
        }

        return false;
    }

    public bool IsPartyEmpty()
    {
        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i] != null)
                return false;
        }

        return true;
    }

    public EnemyCharacter GetEnemyBySlotIndex(int position)
    {
        int index = position - 1;

        if (index < 0 || index >= enemySlots.Length)
            return null;

        return enemySlots[index];
    }

    public void SwapEnemiesByPosition(int pos1, int pos2)
    {
        if (pos1 < 1 || pos1 > enemySlots.Length || pos2 < 1 || pos2 > enemySlots.Length)
        {
            Debug.LogWarning("Invalid positions for swapping.");
            return;
        }

        int index1 = pos1 - 1;
        int index2 = pos2 - 1;

        EnemyCharacter temp = enemySlots[index1];
        enemySlots[index1] = enemySlots[index2];
        enemySlots[index2] = temp;

        if (enemySlots[index1] != null)
            enemySlots[index1].position = index1 + 1;

        if (enemySlots[index2] != null)
            enemySlots[index2].position = index2 + 1;

        OnEnemyPartyChanged?.Invoke();
    }

    public void PrintParty()
    {
        Debug.Log("=== ENEMY PARTY ===");

        for (int i = 0; i < enemySlots.Length; i++)
        {
            EnemyCharacter enemy = enemySlots[i];

            if (enemy == null)
                Debug.Log($"Slot {i + 1}: Empty");
            else
                Debug.Log($"Slot {i + 1}: {enemy.EnemyName}");
        }
    }
    public void ClearEnemyParty()
    {
        ClearParty(true);
    }

    public void ClearParty(bool invokeEvent = true)
    {
        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (enemySlots[i] != null && enemySlots[i].health != null)
                enemySlots[i].health.OnDeath -= OnAnyEnemyDeath;

            enemySlots[i] = null;
        }

        if (invokeEvent)
            OnEnemyPartyChanged?.Invoke();
    }
}
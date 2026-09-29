using UnityEngine;
using System.Collections.Generic;

public class EliteMonsterAttackSellector : MonoBehaviour
{
    [SerializeField] private EliteMonsterAttack[] _attacks;

    private readonly List<EliteMonsterAttack> _bossSellectAttacks = new();

    public EliteMonsterAttack SellectAttack(float distance)
    {
        if (_attacks == null) return null;

        _bossSellectAttacks.Clear();

        int totalWeight = 0;

        foreach (EliteMonsterAttack attack in _attacks)
        {
            if (attack == null) continue;

            if (!attack.CanAttack(distance)) continue;

            int weight = attack.Data.Weight;

            if (weight <= 0) continue;

            _bossSellectAttacks.Add(attack);
            totalWeight += weight;
        }

        if (totalWeight <= 0) return null;

        int randomWeight = Random.Range(0, totalWeight);

        foreach (EliteMonsterAttack attack in _bossSellectAttacks)
        {
            int weight = attack.Data.Weight;

            if (weight > randomWeight)
            {
                return attack;
            }

            randomWeight -= weight;
        }


        return null;
    }
}
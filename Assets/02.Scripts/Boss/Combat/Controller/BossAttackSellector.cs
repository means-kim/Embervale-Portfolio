using UnityEngine;
using System.Collections.Generic;

public class BossAttackSellector : MonoBehaviour
{
    [SerializeField] private BossAttack[] _attacks;

    private readonly List<BossAttack> _bossSellectAttacks = new();

    public BossAttack SellectAttack(float distance)
    {
        if (_attacks == null) return null;

        _bossSellectAttacks.Clear();

        int totalWeight = 0;

        foreach (BossAttack attack in _attacks)
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

        foreach (BossAttack attack in _bossSellectAttacks)
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
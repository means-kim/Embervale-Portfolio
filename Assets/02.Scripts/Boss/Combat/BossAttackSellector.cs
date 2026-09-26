using UnityEngine;

public class BossAttackSellector : MonoBehaviour
{
    [SerializeField] private BossAttack[] _attacks;

    public BossAttack SellectAttack(float distance)
    {
        if (_attacks == null) return null;

        for (int i = 0; i < _attacks.Length; i++)
        {
            BossAttack attack = _attacks[i];

            if (attack == null) continue;

            if (attack.CanAttack(distance))
            {
                return attack;
            }
        }

        return null;
    }
}
using UnityEngine;
[CreateAssetMenu(fileName = "NewBuff", menuName = "Scriptable Objects/Buff")]
public class BuffSO : ScriptableObject
{
    public string BuffName;
    public string BuffDescription;
    public Buff BuffType;
    public float Value;
    public int Weight;
    public int MaxNumberOfBuffs;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (MaxNumberOfBuffs < 0) MaxNumberOfBuffs = 0;
        if (Weight < 0) Weight = 0;
    }
#endif
}
using UnityEngine;

public abstract class StatusEffectSO : ScriptableObject
{
    [Header("Base Info")]
    public string effectName;
    public Sprite effectIcon;
    public float duration;
    public float tickInterval = 1f;

    public abstract void OnTick(GameObject target, float elapsedSeconds);

    public virtual void OnApply(GameObject target) { }

    public virtual void OnRemove(GameObject target) { }
}

[CreateAssetMenu(fileName = "DamageOverTimeSO", menuName = "Status Effects/DamageOverTimeSO")]
public class DamageOverTimeEffect : StatusEffectSO
{
    public int damagePerTick;

    public override void OnTick(GameObject target, float elapsedSeconds)
    {
        if (target.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(damagePerTick);
            Debug.Log($"Эффект [{effectName}] нанес {damagePerTick} урона объекту {target.name}");
        }
    }
}

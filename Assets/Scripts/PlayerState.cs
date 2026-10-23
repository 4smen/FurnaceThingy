using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerState : MonoBehaviour
{
    [SerializeField] private List<ActiveEffect> activeEffects = new List<ActiveEffect>();

    [SerializeField] private PlayerLocal playerLocal;
    [SerializeField] private GameObject firstPersonCamera;
    [SerializeField] private GameObject thirdPersonCamera;

    bool deathActive = false;

    Animator animator;
    UniversalRendererData URPData;

    void Start()
    {
        animator = playerLocal.playerAnimator;
        URPData = PlayerGlobal.Singletone.URPData;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Death();
        }

        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            ActiveEffect effect = activeEffects[i];
            effect.duration -= Time.deltaTime;

            if (Time.time >= effect.nextTickTime)
            {
                effect.data.OnTick(gameObject, effect.data.duration - effect.duration);
                effect.nextTickTime = Time.time + effect.data.tickInterval;
            }

            if (effect.duration <= 0)
            {
                effect.data.OnRemove(gameObject);
                activeEffects.RemoveAt(i);
                Debug.Log($"Ёффект [{effect.data.effectName}] завершилс€ на {gameObject.name}");
            }
        }
    }

    public void ApplyEffect(StatusEffectSO effectData)
    {
        var existingEffect = activeEffects.Find(vr => vr.data.effectName == effectData.effectName);
        if (existingEffect != null)
        {
            existingEffect.duration = effectData.duration;
            return;
        }

        var newEffect = new ActiveEffect(effectData);
        activeEffects.Add(newEffect);
        effectData.OnApply(gameObject);
    }

    void Death()
    {
        deathActive = !deathActive;

        playerLocal.playerMovement.enabled = !deathActive;

        firstPersonCamera.SetActive(!deathActive);
        thirdPersonCamera.SetActive(deathActive);

        URPData.rendererFeatures.Find(vr => vr.name == "Black").SetActive(deathActive);
        URPData.rendererFeatures.Find(vr => vr.name == "White").SetActive(deathActive);
    }

    private class ActiveEffect
    {
        public StatusEffectSO data;
        public float duration;
        public float nextTickTime;

        public ActiveEffect(StatusEffectSO effectData)
        {
            data = effectData;
            duration = effectData.duration;
            nextTickTime = Time.time;
        }
    }
}

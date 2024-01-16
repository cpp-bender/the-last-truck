using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    public List<EffectStruct> effectStructs;

    private void Start()
    {
        foreach (EffectStruct effect in effectStructs)
        {
            ObjectPooler.Instance.CreatePool(effect.effectPrefab, effect.poolTag, 1);
        }
    }

    public GameObject ShowEffect(Vector3 pos, PoolTag poolTag, float duration = 1f)
    {
        var effectGO = ObjectPooler.Instance.DequeueFromPool(poolTag);

        effectGO.transform.position = pos;
        effectGO.SetActive(true);

        StartCoroutine(EffectGoPool(effectGO, poolTag, duration));

        return effectGO;
    }

    public GameObject ShowGainEffect(Vector3 pos, float count, float duration = 1f)
    {
        var effectGO = ObjectPooler.Instance.DequeueFromPool(PoolTag.PriceGain);
        effectGO.GetComponent<PriceGainer>()?.SetCount(count);
        effectGO.transform.position = pos;
        effectGO.SetActive(true);

        StartCoroutine(EffectGoPool(effectGO, PoolTag.PriceGain, duration));

        return effectGO;
    }

    public GameObject ShowGainEffect(Vector3 pos, float count, float scale, float duration)
    {
        var effectGO = ObjectPooler.Instance.DequeueFromPool(PoolTag.PriceGain);
        effectGO.GetComponent<PriceGainer>().SetCount(Mathf.RoundToInt(count));
        effectGO.GetComponent<PriceGainer>().SetScale(scale);
        effectGO.transform.position = pos;
        effectGO.SetActive(true);

        StartCoroutine(EffectGoPool(effectGO, PoolTag.PriceGain, duration));

        return effectGO;
    }

    public GameObject ShowLevelUpEffect(Vector3 pos, string text, float duration)
    {
        var effectGO = ObjectPooler.Instance.DequeueFromPool(PoolTag.TurretLevelUp);
        effectGO.GetComponent<TurretLevelUpEffect>().SetText(text);
        effectGO.transform.position = pos;
        effectGO.SetActive(true);

        StartCoroutine(EffectGoPool(effectGO, PoolTag.TurretLevelUp, duration));

        return effectGO;
    }

    private IEnumerator EffectGoPool(GameObject effectGO, PoolTag poolTag, float duration = 1f)
    {
        yield return new WaitForSeconds(duration);

        if (gameObject.activeSelf)
            ObjectPooler.Instance.EnqueueToPool(poolTag, effectGO);
    }
}

[Serializable]
public class EffectStruct
{
    public GameObject effectPrefab;
    public PoolTag poolTag;
}

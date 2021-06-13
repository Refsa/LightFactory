using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildItem : ScriptableObject
{
    [SerializeField] GameObject prefab;
    [SerializeField] Sprite icon;
    [SerializeField] string tooltip = "Placeholder...";
    [SerializeField] List<LightCurrency> cost;

    public GameObject Prefab => prefab;
    public Sprite Icon => icon;
    public string Tooltip => tooltip;
    public List<LightCurrency> Cost => cost;
}

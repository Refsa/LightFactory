using System;
using System.Collections;
using System.Collections.Generic;
using Refsa.EventBus;
using UnityEngine;

public class BuildMenuUI : MonoBehaviour
{
    [SerializeField] RectTransform buildItemsContainer;
    [SerializeField] GameObject buildItemPrefab;
    [SerializeField] BuildItem[] buildItems;

    [SerializeField] PlaceItemPreview placeItemPreview;

    BuildItem selectedBuildItem;

    void Awake()
    {
        foreach (var buildItem in buildItems)
        {
            var inst = GameObject.Instantiate(buildItemPrefab, buildItemsContainer);
            inst.GetComponent<BuildItemUI>().Setup(buildItem);
        }

        GlobalEventBus.Bus.Sub<BuildItemSelected>(OnBuildItemSelected);
    }

    private void OnBuildItemSelected(BuildItemSelected obj)
    {
        selectedBuildItem = obj.BuildItem;
        placeItemPreview.Show(selectedBuildItem);
    }

    void Update()
    {
        if (selectedBuildItem != null) HandleBuildItemPlacement();
    }

    private void HandleBuildItemPlacement()
    {
        Vector2 mousePos = WorldCamera.Instance.MouseInWorld;
        placeItemPreview.transform.position = mousePos.ToVector3();

        if (Input.GetKeyDown(GameInput.CancelAction))
        {
            selectedBuildItem = null;
            placeItemPreview.Hide();
        }
    }
}

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
    BuildItemUI selectedBuildItemUI;

    void Awake()
    {
        foreach (var buildItem in buildItems)
        {
            var inst = GameObject.Instantiate(buildItemPrefab, buildItemsContainer);
            inst.GetComponent<BuildItemUI>().Setup(buildItem);
        }

        GlobalEventBus.Bus.Sub<BuildItemSelected>(OnBuildItemSelected);
    }

    void OnBuildItemSelected(BuildItemSelected obj)
    {
        selectedBuildItem = obj.BuildItem;
        selectedBuildItemUI = obj.BuildItemUI;

        placeItemPreview.Show(selectedBuildItem);
        selectedBuildItemUI.Select();

        GlobalEventBus.Bus.Pub(new GridEnable(placeItemPreview.gameObject));
    }

    void DeselectBuildItem()
    {
        selectedBuildItem = null;
        placeItemPreview.Hide();

        selectedBuildItemUI.Deselect();
        selectedBuildItemUI = null;

        GlobalEventBus.Bus.Pub(new GridDisable());
    }

    void Update()
    {
        if (selectedBuildItem != null) HandleBuildItemPlacement();
    }

    private void HandleBuildItemPlacement()
    {
        if (Input.GetKeyDown(GameInput.CancelAction))
        {
            DeselectBuildItem();
            return;
        }

        Vector2 mousePos = WorldCamera.Instance.MouseInWorld
            .Snap(Input.GetKey(GameInput.PrecisionMode) ? GameConstants.GridMinorSnap : GameConstants.GridMajorSnap);

        placeItemPreview.transform.position = mousePos.ToVector3();

        var canPlaceHit = Physics2D.OverlapCircle(mousePos, 1f);
        if (canPlaceHit != null)
        {
            placeItemPreview.SetColor(Color.red);
            return;
        }

        placeItemPreview.SetColor(Color.green);

        if (Input.GetKeyDown(GameInput.Place))
        {
            var go = GameObject.Instantiate(selectedBuildItem.Prefab);
            go.transform.position = mousePos.ToVector3();
            DeselectBuildItem();

            GlobalEventBus.Bus.Pub(new SelectionSet(go));
            return;
        }
    }
}

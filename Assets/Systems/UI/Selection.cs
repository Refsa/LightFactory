using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    [SerializeField] GameObject transformHandlePrefab;
    [SerializeField] GameObject grid;

    int targetLayerMask;
    GameObject selectedGameObject;
    TransformHandle transformHandle;

    [AutoBind] new Camera camera;

    void Awake()
    {
        targetLayerMask = 1 << LayerMask.NameToLayer("Solid");

        transformHandle = GameObject.Instantiate(transformHandlePrefab).GetComponent<TransformHandle>();
        transformHandle.gameObject.SetActive(false);
    }

    void Update()
    {
        bool hadSelection = selectedGameObject != null;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var mouseRay = camera.ScreenPointToRay(Input.mousePosition);
            mouseRay.origin.Scale(new Vector3(1, 1, 0));

            var hit = Physics2D.OverlapCircle(mouseRay.origin, 0.25f, targetLayerMask);

            if (hit != null && (hit.gameObject.HasTagInParent("Selectable") || hit.gameObject.HasTag("Selectable")))
            {
                if (!hadSelection || selectedGameObject != hit.gameObject)
                {
                    transformHandle.SetData(hit.gameObject);
                }
                selectedGameObject = hit.gameObject;

            }
            else if (!transformHandle.gameObject.activeSelf || !transformHandle.Active)
            {
                selectedGameObject = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedGameObject = null;
        }

        if (selectedGameObject != null)
        {
            transformHandle.gameObject.SetActive(true);
            grid.gameObject.SetActive(true);

            grid.transform.position = selectedGameObject.transform.position + Vector3.forward * 20f;

            transformHandle.transform.position = selectedGameObject.transform.position;
            transformHandle.Tick();
        }
        else
        {
            transformHandle.gameObject.SetActive(false);
            grid.gameObject.SetActive(false);
        }
    }
}

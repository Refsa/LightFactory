using UnityEngine;

public class ResourcesTutorial : TutorialStep
{
    [SerializeField] GameObject resourcePrefab;

    GameObject spawned;

    protected override void InternalShow()
    {

    }

    public override void Show()
    {
        base.Show();

        spawned = GameObject.Instantiate(resourcePrefab);
        spawned.transform.position = Vector3.zero;
        spawned.transform.position = spawned.transform.position.Snap(GameConstants.GridMinorSnap);
    }

    public override void Hide()
    {
        base.Hide();

        GameObject.Destroy(spawned);
    }
}
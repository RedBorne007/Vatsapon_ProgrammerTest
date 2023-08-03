using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectInteractable : BaseInteractable
{
    [Header("Settings")]
    [Tooltip("Text that will display on popup")]
    [SerializeField] private GameObject previewObjectPrefab;

    private InspectManager inspectM;

    protected override void Start()
    {
        base.Start();

        inspectM = InspectManager.Instance;
    }

    // Function to execute when player interact with this interactable.
    public override void OnInteract()
    {
        base.OnInteract();

        isShow = false;
        inspectM.Inspect(this, previewObjectPrefab);
    }
}

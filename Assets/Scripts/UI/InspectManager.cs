using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InspectManager : Singleton<InspectManager>, IPointerDownHandler, IDragHandler
{
    [Tooltip("Minimum amount to zoom in/out")]
    [SerializeField] private float zoomMin = 20f;
    [Tooltip("Maximum amount to zoom in/out")]
    [SerializeField] private float zoomMax = 5f;
    [Tooltip("Amount to zoom in/out")]
    [SerializeField] private float zoomStep = 1.5f;
    [Tooltip("Lerp interval for zooming")]
    [SerializeField] private float zoomLerpInterval = 2f;

    [Header("References")]
    [Tooltip("Camera that display inspected object")]
    [SerializeField] private Camera inspectCamera;
    [Tooltip("Renderer that will render inspected object")]
    [SerializeField] private RawImage imageRenderer;

    private RenderTexture renderTexture;
    private float currentZoom;
    private float targetZoom;
    private Vector3 previousMousePos;
    private GameObject inspectObject;

    private void Start()
    {
        AssignRenderTexture();
        currentZoom = (zoomMin + zoomMax) / 2f;
        targetZoom = currentZoom;
    }

    private void Update()
    {
        // If resolution changes, re-assign render texture.
        if (renderTexture && (Screen.width != renderTexture.width || Screen.height != renderTexture.height))
        {
            AssignRenderTexture();
        }

        if (inspectObject)
        {
            ZoomHandler();
        }
    }

    // Function to inspect object.
    public void Inspect(BaseInteractable interactable, GameObject gameObject)
    {
        // If player is currently inspecting another object, return.
        if (inspectObject)
        {
            return;
        }

        UIManager uiM = UIManager.Instance;

        Transform camTransform = inspectCamera.transform;
        Vector3 position = camTransform.position + (camTransform.forward * currentZoom);
        interactable.IsShow = false;

        inspectObject = Instantiate(gameObject, position, Quaternion.identity);

        uiM.SetFocusObject(uiM.InspectScreen, () =>
        {
            GameManager.Instance.SetCameraLock(false);
            GameManager.Instance.SetCursorLock(true);
            UIManager.Instance.InspectScreen.SetActive(false);
            interactable.IsShow = true;

            Destroy(inspectObject);
        });
    }

    // Function to determine if player is inspecting object or not.
    public bool IsInspecting() => inspectObject;

    // Function to assign new render texture.
    private void AssignRenderTexture()
    {
        // If there's render texture, release and remove.
        if (renderTexture)
        {
            renderTexture.Release();
            renderTexture = null;
        }

        renderTexture = new RenderTexture(Screen.width, Screen.height, 32);
        renderTexture.name = "Inspect Render Texture";
        inspectCamera.targetTexture = renderTexture;
        imageRenderer.texture = renderTexture;
    }

    // Function to handle zooming.
    private void ZoomHandler()
    {
        float scrollValue = Input.mouseScrollDelta.y;

        if (scrollValue > 0f)
        {
            targetZoom -= zoomStep;
        }

        if (scrollValue < 0f)
        {
            targetZoom += zoomStep;
        }

        targetZoom = Mathf.Clamp(targetZoom, zoomMin, zoomMax);
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomLerpInterval * Time.deltaTime);

        Transform camTransform = inspectCamera.transform;
        Vector3 position = camTransform.position + (camTransform.forward * currentZoom);
        inspectObject.transform.position = position;
    }

    // Function to handle rotation.
    private void RotationHandler()
    {
        if (inspectObject)
        {
            Vector2 pos = Input.mousePosition - previousMousePos;
            previousMousePos = Input.mousePosition;

            Vector2 axis = Quaternion.AngleAxis(-90f, Vector3.forward) * pos;
            inspectObject.transform.rotation = Quaternion.AngleAxis(pos.magnitude * 0.1f, axis) * inspectObject.transform.rotation;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        RotationHandler();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        previousMousePos = Input.mousePosition;
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using System;

public class ViewSwitcher : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera firstPersonCamera;
    [SerializeField] private CinemachineCamera thirdPersonCamera;

    [Header("Character renderer")]
    [SerializeField] private SkinnedMeshRenderer characterMesh;

    [Header("Switching keyboard")]
    [SerializeField] private InputAction toggleAction = new InputAction(binding: "<Keyboard>/v");


    public bool isFirstPerson = true;


    [SerializeField] private float blendTime;

    private void OnEnable() => toggleAction.Enable();
    private void OnDisable() => toggleAction.Disable();

    private void Start()
    {
        UpdateView();
    }

    private void Update()
    {
        if (toggleAction.WasPressedThisFrame())
        {
            isFirstPerson = !isFirstPerson;
            UpdateView();
        }
    }

    private void UpdateView()
    {
        StopAllCoroutines();

        if (isFirstPerson)
        {
            firstPersonCamera.Priority = 10;
            thirdPersonCamera.Priority = 0;

            StartCoroutine(HideMeshDelayed(blendTime));
        }
        else
        {
            characterMesh.shadowCastingMode = ShadowCastingMode.On;
            
            firstPersonCamera.Priority = 0;
            thirdPersonCamera.Priority = 10;
        }
    }

    private IEnumerator HideMeshDelayed(float delay)
    {
        yield return new WaitForSeconds(delay * 0.8f);
        characterMesh.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
    }
}
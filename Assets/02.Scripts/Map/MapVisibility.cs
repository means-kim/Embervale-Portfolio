using System;
using UnityEngine;

public sealed class MapVisibility : MonoBehaviour
{
    [Serializable]
    public class Surface
    {
        public Renderer renderer;
        public int entranceRole;
        public int exitRole;
        public bool entranceMouth;
    }

    [Header("셰이더 입구 마스킹 위치(_PortalX) 기준")]
    [SerializeField] private Transform _entranceAnchor;
    [SerializeField] private float _entranceX = 34.5f;
    [SerializeField] private bool _bossExitEnabled = true;
    [SerializeField] private PortalState _initialState;
    [SerializeField] private Surface[] _surfaces = Array.Empty<Surface>();

    private bool _inside, _outside;
    private MaterialPropertyBlock _block;
    public bool IsInside => _inside;
    public bool IsOutside => _outside;

    [Serializable]
    public struct PortalState
    {
        public bool Inside;
        public bool Outside;
    }

    private void OnEnable()
    {
        _inside = _initialState.Inside;
        _outside = _initialState.Outside;
        Apply(true);
    }

    public void SetState(PortalState state)
    {
        _inside = state.Inside;
        _outside = state.Outside;
        Apply(true);
    }

    private void Apply(bool enabledMask)
    {
        if (_entranceAnchor != null) _entranceX = _entranceAnchor.position.x;
        if (_block == null) _block = new MaterialPropertyBlock();
        foreach (var surface in _surfaces)
        {
            if (surface == null || !surface.renderer) continue;
            var renderer = surface.renderer;
            renderer.GetPropertyBlock(_block);
            _block.SetFloat("_PortalEnabled", enabledMask ? 1 : 0);
            _block.SetFloat("_PortalInside", _inside ? 1 : 0);
            _block.SetFloat("_PortalX", _entranceX);
            _block.SetFloat("_PortalEntrance", surface.entranceMouth ? 1 : 0);
            _block.SetFloat("_PortalRole", surface.entranceRole);
            _block.SetFloat("_ExitEnabled", enabledMask && _bossExitEnabled ? 1 : 0);
            _block.SetFloat("_ExitOutside", _outside ? 1 : 0);
            _block.SetFloat("_ExitRole", surface.exitRole);
            renderer.SetPropertyBlock(_block);
        }
    }

    private void OnDisable()
    {
        Apply(false);
    }
}

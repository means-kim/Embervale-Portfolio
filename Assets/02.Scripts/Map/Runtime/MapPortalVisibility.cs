using System;
using UnityEngine;

namespace MapReview
{
    // Only controls interior/exterior visibility. Art and colliders are ordinary assets.
    [ExecuteAlways, DefaultExecutionOrder(200)]
    public sealed class MapPortalVisibility : MonoBehaviour
    {
        [Serializable]
        public class Surface
        {
            public Renderer renderer;
            public int entranceRole;
            public int exitRole;
            public bool entranceMouth;
        }

        public Transform player, entranceAnchor, bossExitAnchor;
        public float entranceX = 34.5f, bossExitX = 120f, hysteresis = .25f;
        public bool bossExitEnabled = true;
        public Surface[] surfaces = Array.Empty<Surface>();
        bool _inside, _outside;
        MaterialPropertyBlock _block;

        void OnEnable()
        {
            Refresh(true);
        }

        void LateUpdate()
        {
            Refresh(false);
        }

        public void Refresh(bool initialize)
        {
            if (!player) return;
            if (entranceAnchor) entranceX = entranceAnchor.position.x;
            if (bossExitAnchor) bossExitX = bossExitAnchor.position.x;
            if (initialize)
            {
                _inside = player.position.x >= entranceX;
                _outside = player.position.x >= bossExitX;
            }
            else
            {
                if (player.position.x > entranceX + hysteresis) _inside = true;
                else if (player.position.x < entranceX - hysteresis) _inside = false;
                if (player.position.x > bossExitX + hysteresis) _outside = true;
                else if (player.position.x < bossExitX - hysteresis) _outside = false;
            }

            Apply(true);
        }

        void Apply(bool enabledMask)
        {
            if (_block == null) _block = new MaterialPropertyBlock();
            foreach (var surface in surfaces)
            {
                if (surface == null || !surface.renderer) continue;
                var renderer = surface.renderer;
                renderer.GetPropertyBlock(_block);
                _block.SetFloat("_PortalEnabled", enabledMask ? 1 : 0);
                _block.SetFloat("_PortalInside", _inside ? 1 : 0);
                _block.SetFloat("_PortalX", entranceX);
                _block.SetFloat("_PortalEntrance", surface.entranceMouth ? 1 : 0);
                _block.SetFloat("_PortalRole", surface.entranceRole);
                _block.SetFloat("_ExitEnabled", enabledMask && bossExitEnabled ? 1 : 0);
                _block.SetFloat("_ExitOutside", _outside ? 1 : 0);
                _block.SetFloat("_ExitRole", surface.exitRole);
                renderer.SetPropertyBlock(_block);
            }
        }

        void OnDisable()
        {
            Apply(false);
        }
    }
}
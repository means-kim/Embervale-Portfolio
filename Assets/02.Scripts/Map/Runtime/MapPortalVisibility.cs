using System;
using UnityEngine;

namespace MapReview
{
    // Only controls interior/exterior visibility. Art and colliders are ordinary assets.
    [ExecuteAlways, DefaultExecutionOrder(200)]
    public sealed class MapPortalVisibility : MonoBehaviour
    {
        [Serializable] public class Surface
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
        bool inside, outside;
        MaterialPropertyBlock block;
        void OnEnable() { Refresh(true); }
        void LateUpdate() { Refresh(false); }
        public void Refresh(bool initialize)
        {
            if (!player) return;
            if (entranceAnchor) entranceX = entranceAnchor.position.x;
            if (bossExitAnchor) bossExitX = bossExitAnchor.position.x;
            if (initialize) { inside = player.position.x >= entranceX; outside = player.position.x >= bossExitX; }
            else
            {
                if (player.position.x > entranceX + hysteresis) inside = true;
                else if (player.position.x < entranceX - hysteresis) inside = false;
                if (player.position.x > bossExitX + hysteresis) outside = true;
                else if (player.position.x < bossExitX - hysteresis) outside = false;
            }
            Apply(true);
        }
        void Apply(bool enabledMask)
        {
            if (block == null) block = new MaterialPropertyBlock();
            foreach (var surface in surfaces)
            {
                if (surface == null || !surface.renderer) continue;
                var renderer = surface.renderer;
                renderer.GetPropertyBlock(block);
                block.SetFloat("_PortalEnabled", enabledMask ? 1 : 0);
                block.SetFloat("_PortalInside", inside ? 1 : 0);
                block.SetFloat("_PortalX", entranceX);
                block.SetFloat("_PortalEntrance", surface.entranceMouth ? 1 : 0);
                block.SetFloat("_PortalRole", surface.entranceRole);
                block.SetFloat("_ExitEnabled", enabledMask && bossExitEnabled ? 1 : 0);
                block.SetFloat("_ExitOutside", outside ? 1 : 0);
                block.SetFloat("_ExitRole", surface.exitRole);
                renderer.SetPropertyBlock(block);
            }
        }
        void OnDisable() { Apply(false); }
    }
}

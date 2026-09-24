# Cave exterior v08

- Added Cave_ExteriorFacade_v01 under 02_Climb/Cave_UpperBoundary in HyunseoScene.
- Reuses the existing EntryWall texture mirrored horizontally, with upper vertex-alpha blending. No source image edits.
- GA1_World order 31; map physics layer; no collider added or changed.
- MapPortalVisibility surface entranceRole 5 makes the facade visible outside and transparent inside. Existing entrance threshold remains 34.5.
- Saved Map_CompleteLayout and Decoration_CaveUpperBoundary prefabs. Standalone decoration requires registration with the scene portal controller when used separately.
- BEFORE/AFTER captures: 0 approach, 1 outside threshold, 2 inside threshold, 3 upper interior, 4 upper exterior. Player position restored after captures.
- Render comparison completed; continuous gameplay traversal was not tested.
- Rollback: pre-change scene and prefabs are in Before. New facade mesh/material can remain unused after restoring those files. Do not restore backups over later user edits without checking.

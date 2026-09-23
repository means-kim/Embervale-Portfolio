# Boss terrain joins v16

Built-in imagegen edit mode; references: Bg_Concept.png and BossGround_v15/After_191.png, After_239.png. Two separate transparent overlays join the center bridge to adjacent ground and the right cliff to its foot.

Assets: Backgrounds/BossMap1/BG_Boss1_CenterJoin_v01.png and BG_Boss1_WallFootJoin_v01.png. Native meshes and materials use the same names. Separate Decoration prefabs are in Assets/03.Prefabs/Map.

Applied to HyunseoScene and scoped updates to Ground_Arena_BreakableFloor_Center, Terrain_Wall_RightBoundary, Map_CompleteLayout prefabs. Center overlay world position (191,36.05,0), order 63; wall overlay (239,44.25,0), order 91. Quad dimensions 42.666667 x 24; mesh vertex alpha blends edges. SortingGroup is editable in Inspector.

Original center renderer is disabled, not deleted. New center art remains under Intact, and becomes inactive when Intact is disabled (checked and restored). Original right wall remains visible behind its overlay. Snow and collision components were not edited. PortalMask materials and explicit surfaces registration preserve entranceRole 0 / exitRole 2. Full gameplay destruction was not tested.

Unity was in Play mode during initial visual work. Prefab assets were persisted first; exited Play mode, applied the overlays again to the existing edit scene, and saved without reloading the scene. Final captures were repeated in edit mode at 1280x720 with orthographic size 12.

## Exact generation prompts

### Center

Edit image1 actual Unity screenshot into an isolated terrain replacement OVERLAY. Image2 is overall concept/style. Preserve exact 16:9 framing and pixel positions of foreground terrain, ideally1280x720. Remove ALL sky mountains pillars floating platform to genuine transparency, and remove snow (separate game asset will remain over this). Output only foreground rock below its original walkable top line around y440 of720. Repair both obvious vertical seams at x430 and850: make continuous interlocking basalt strata spanning left ground through central destructible bridge into right ground, SAME stone size/detail/brightness across seams. No rectangular cut edges, no side caps. Central bridge underside can have a shallow irregular rock arch opening centered x640 starting below y630, with natural rock haunches joining left/right walls; not a dangling detached strip. Same dark cold blue painterly basalt, NO glowing rim, NO pillars or background. Keep walkable top at precisely original height/curve including rightmost rising ramp; rock surface 5pixels under snow. Outer left/right terrain texture must match original screenshot to blend. Canvas left/right/bottom rock continue beyond edges, no framing margins. Transparent everywhere outside the rock. This is flat 2D connected terrain, not a perspective ledge.

### Wall foot

Edit image1 actual Unity right wall screenshot into a terrain-junction repair OVERLAY with exact same 16:9 framing ideally1280x720. Image2 concept reference. Preserve foreground silhouette: ground top passes from left y625 up to x200y560, then down toward x620y600 where it meets right cliff; cliff occupies right side with its irregular inner silhouette. Remove ALL sky mountains floating platform to genuine transparency. Remove snow because separate snow remains in game. Draw cohesive continuous rock at the floor-to-wall junction: floor and cliff are ONE contiguous cross-section of basalt in SAME depth plane, rather than huge foreground wall laid over distant ground. Replace foreground protruding bright shelf at lower right with a naturally curving concave rock root/buttress whose strata continue from ground into wall. Match stone sizes and subdued brightness between floor and wall, avoid bright illuminated bevel or foreground rim casting a sharp overlap shadow. Keep the traversable silhouette almost unchanged, NO new boulder obstructing ground left of original cliff, no extra steps. Fill rock through bottom/right edges. Retain original cliff upper rock texture so overlay blends at top. Transparent only outside terrain. No landscape, no characters, no labels.

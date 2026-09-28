# Swing2 detail correction — built-in image_gen

## Maximum reach frame

Use case: precise-object-edit.
Asset type: one production-ready transparent 2D game sprite frame.
Image 1 is the EDIT TARGET: the ice giant at maximum reach of a low arm sweep. Image 2 is the AUTHORITATIVE IDENTITY AND SURFACE REFERENCE: the exact same giant in Idle.

Edit image 1 to restore the surface detail and rendering of image 2. Preserve image 1's exact pose, silhouette proportions, head direction, limb placement, grounded feet, camera, and low sweeping attack. Do not change the attack into a punch or raise the hand. The long near arm crosses in front of the chest toward bottom-left with bent shoulder/elbow; preserve that gesture.

FUR: replace the soft airbrushed scallops with the reference's distinct overlapping tapered fur tufts. Keep sharp wispy tuft tips along forearm, elbow, wrist and legs, layered fine hair strokes with narrow cool shadows between clumps, no broad smooth cotton forms. The near forearm must remain clearly separated from the dark torso by a narrow contact shadow.
SNOW: reproduce reference's thin broken granular crust perched on upper shoulder, upper forearm and ice ledges. Irregular small snow clumps and dark undercuts, not continuous white frosting, not smeared highlights and not falling particles.
ICE AND HANDS: use the reference's thin angular blue ice shards with sharp pale edges, internal cracks, frost, and irregular snow caps; retain their identity and arrangement as the torso tilts. Restore rough cracked charcoal stone knuckles and frost-grained claws, not glossy metal. Preserve reference's blue-white-grey palette and its existing light direction.
Critical: this is the Idle character moving its arm, with the same head, crystal crown, materials, and painting density. No motion blur, smoothing, lens effects, halos, duplicated limbs, new armor, weapons, debris or scene.

Output ONLY the edited single frame, genuinely transparent RGBA, square 1024x1024 if possible. Keep the target's full 512x512 canvas composition scaled uniformly to 1024: same empty space above, all extremities visible, same ground baseline and exact relative positions. No labels, no grid, no background or cast shadow.

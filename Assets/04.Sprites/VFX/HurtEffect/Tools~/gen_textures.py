"""Generate hit-effect textures (Hollow Knight style player damage)."""
import math
import os
import sys

import numpy as np
from PIL import Image, ImageDraw, ImageFilter
from scipy.ndimage import gaussian_filter

OUT = sys.argv[1]
os.makedirs(OUT, exist_ok=True)
rng = np.random.default_rng(7)

# 흰색 마스크로 저장 → Unity에서 SpriteRenderer.color / 파티클 startColor로 틴트 (색상 SSoT = Settings)
MASK = (255, 255, 255)


def save_rgba(alpha: np.ndarray, rgb, name: str):
    alpha = np.clip(alpha, 0, 1)
    h, w = alpha.shape
    img = np.zeros((h, w, 4), np.uint8)
    img[..., 0], img[..., 1], img[..., 2] = rgb
    img[..., 3] = (alpha * 255).astype(np.uint8)
    Image.fromarray(img, "RGBA").save(os.path.join(OUT, name))


# ---------------------------------------------------------------- 1. Splat
def make_splat(size=512):
    S = size * 2  # supersample
    yy, xx = np.mgrid[0:S, 0:S]
    cx = cy = S / 2
    dx, dy = (xx - cx) / (S / 2), (yy - cy) / (S / 2)
    r = np.sqrt(dx * dx + dy * dy)
    ang = np.arctan2(dy, dx)

    # Outer ring with wobbly edge
    wobble = 0.03 * np.sin(ang * 7 + 1.3) + 0.02 * np.sin(ang * 13 + 0.4)
    outer = 0.80 + wobble
    ring = ((r < outer) & (r > 0.58)).astype(np.float32)

    # Inward teeth from ring (the "jaw" silhouette)
    teeth = Image.new("L", (S, S), 0)
    d = ImageDraw.Draw(teeth)
    n = 18
    for i in range(n):
        a = (i + rng.uniform(-0.25, 0.25)) / n * 2 * math.pi
        tip_r = rng.uniform(0.16, 0.36)
        base_r = 0.62
        half_w = rng.uniform(0.035, 0.065)
        tip = (cx + math.cos(a) * tip_r * S / 2, cy + math.sin(a) * tip_r * S / 2)
        b1 = (cx + math.cos(a - half_w) * base_r * S / 2, cy + math.sin(a - half_w) * base_r * S / 2)
        b2 = (cx + math.cos(a + half_w) * base_r * S / 2, cy + math.sin(a + half_w) * base_r * S / 2)
        d.polygon([b1, tip, b2], fill=255)
    teeth = np.asarray(teeth, np.float32) / 255

    # Center blob (irregular)
    core_r = 0.13 + 0.025 * np.sin(ang * 5 + 2.0) + 0.015 * np.sin(ang * 11)
    core = (r < core_r).astype(np.float32)

    # Interior tint so the inside darkens background a bit
    interior = (r < outer).astype(np.float32) * 0.40

    sharp = np.maximum.reduce([ring, teeth, core, interior])
    sharp = gaussian_filter(sharp, 11)

    # Soft smoky falloff outside the ring (blurred disc, no hard rim)
    halo = gaussian_filter((r < outer + 0.03).astype(np.float32), 24) * 0.95
    halo *= np.clip((r - (outer - 0.10)) / 0.10, 0, 1)  # only outside the ring
    alpha = np.maximum(sharp, halo)
    alpha = np.clip(alpha, 0, 1)

    img = Image.fromarray((alpha * 255).astype(np.uint8)).resize((size, size), Image.LANCZOS)
    save_rgba(np.asarray(img, np.float32) / 255, MASK, "HitSplat.png")


# ---------------------------------------------------------------- 2. Cracks
def jagged_line(d, start, direction, length, width, depth, step=14):
    x, y = start
    ang = direction
    travelled = 0
    pts = [(x, y)]
    while travelled < length:
        ang += rng.normal(0, 0.10)
        ang = direction + np.clip(ang - direction, -0.18, 0.18)  # stay roughly horizontal
        seg = step * rng.uniform(0.6, 1.4)
        x += math.cos(ang) * seg
        y += math.sin(ang) * seg
        travelled += seg
        pts.append((x, y))
        # Branches
        if depth > 0 and rng.random() < 0.06:
            b_dir = ang + rng.choice([-1, 1]) * rng.uniform(0.35, 0.8)
            jagged_line(d, (x, y), b_dir, length * rng.uniform(0.12, 0.3),
                        max(1, width - 2), depth - 1, step * 0.8)
    # taper: draw segments with decreasing width
    for i in range(len(pts) - 1):
        t = i / max(1, len(pts) - 1)
        w = max(1, int(round(width * (1 - t * 0.85))))
        d.line([pts[i], pts[i + 1]], fill=255, width=w)


def make_cracks(w=1024, h=256):
    S = 2
    W, H = w * S, h * S
    canvas = Image.new("L", (W, H), 0)
    d = ImageDraw.Draw(canvas)
    c = (W / 2, H / 2)
    for side in (math.pi, 0.0):  # left, right
        for k in range(4):
            spread = rng.uniform(-0.09, 0.09)
            jagged_line(d, (c[0] + rng.uniform(-10, 10), c[1] + rng.uniform(-12, 12)),
                        side + spread, W / 2 * rng.uniform(0.55, 1.0),
                        width=rng.integers(4, 8), depth=2)
    core = np.asarray(canvas.resize((w, h), Image.LANCZOS), np.float32) / 255
    glow = gaussian_filter(core, 4) * 2.2
    alpha = np.clip(np.maximum(core, glow * 0.6), 0, 1)
    # Fade toward the far ends
    xx = np.linspace(-1, 1, w)[None, :]
    alpha *= np.clip(1.25 - np.abs(xx) ** 2, 0, 1)
    save_rgba(alpha, (255, 252, 240), "HitCrack.png")


# ---------------------------------------------------------------- 3. Shade blobs (2x2 sheet)
def make_blobs(cell=64):
    sheet = np.zeros((cell * 2, cell * 2), np.float32)
    yy, xx = np.mgrid[0:cell, 0:cell]
    dx, dy = (xx - cell / 2 + 0.5) / (cell / 2), (yy - cell / 2 + 0.5) / (cell / 2)
    r = np.sqrt(dx * dx + dy * dy)
    ang = np.arctan2(dy, dx)
    for i in range(4):
        edge = 0.72
        for f in (3, 5, 7):
            edge = edge + rng.uniform(0.03, 0.08) * np.sin(ang * f + rng.uniform(0, 6.28))
        blob = (r < edge).astype(np.float32)
        blob = gaussian_filter(blob, 1.2)
        gy, gx = divmod(i, 2)
        sheet[gy * cell:(gy + 1) * cell, gx * cell:(gx + 1) * cell] = blob
    save_rgba(sheet, MASK, "ShadeBlob_2x2.png")


# ---------------------------------------------------------------- 4. White shards (2x2 sheet)
def make_shards(cell=64):
    sheet = Image.new("L", (cell * 2, cell * 2), 0)
    d = ImageDraw.Draw(sheet)
    for i in range(4):
        gy, gx = divmod(i, 2)
        ox, oy = gx * cell, gy * cell
        # crescent-like sliver
        bbox = [ox + 10, oy + 14, ox + cell - 10, oy + cell - 14]
        d.chord(bbox, start=200 + i * 10, end=340 - i * 8, fill=255)
        inner = [bbox[0] + 8, bbox[1] + 14, bbox[2] - 8, bbox[3] + 10]
        d.chord(inner, start=180, end=360, fill=0)
    a = np.asarray(sheet.filter(ImageFilter.GaussianBlur(0.8)), np.float32) / 255
    save_rgba(a, (255, 255, 255), "WhiteShard_2x2.png")


# ---------------------------------------------------------------- 5. Vignette
def make_vignette(size=256):
    yy, xx = np.mgrid[0:size, 0:size]
    dx, dy = (xx - size / 2) / (size / 2), (yy - size / 2) / (size / 2)
    r = np.sqrt(dx * dx + dy * dy)
    a = np.clip((r - 0.35) / 0.95, 0, 1) ** 1.6
    a = 0.35 + 0.65 * a  # whole screen dims a bit, edges more
    save_rgba(a, MASK, "HitVignette.png")


make_splat()
make_cracks()
make_blobs()
make_shards()
make_vignette()
print("done", os.listdir(OUT))

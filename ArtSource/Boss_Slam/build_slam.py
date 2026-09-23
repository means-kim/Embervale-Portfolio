"""Assemble the generated Slam poses at the established Unity sprite scale.

Run from any directory with Python, Pillow and NumPy. Existing output assets are
not overwritten; this is the reproducible initial assembly for these references.
"""
from pathlib import Path
import hashlib
import json
import re
import uuid

import numpy as np
from PIL import Image, ImageDraw

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[1]
ASSETS = ROOT / 'Assets'
OUT = ASSETS / '04.Sprites/Boss/Boss_Slam.png'
CLIP = ASSETS / '05.Animation/Boss/Boss_Slam.anim'
assert not OUT.exists() and not CLIP.exists(), 'Refusing to overwrite existing Slam assets'

idle_path = ASSETS / '04.Sprites/Boss/Boss_Idle.png'
idle = Image.open(idle_path).convert('RGBA').crop((0, 0, 512, 512))
source = Image.open(HERE / 'generation-source.png').convert('RGBA')
assert source.size == (1774, 887)
pixels = np.array(source)
# The generated image has faint disconnected alpha artifacts. Remove only values
# below 16/255 before measuring and resizing, so noise cannot define the baseline.
pixels[pixels[:, :, 3] < 16] = 0
source = Image.fromarray(pixels)

def bounds_at(image, threshold):
    alpha = np.array(image)[:, :, 3]
    ys, xs = np.where(alpha >= threshold)
    return (int(xs.min()), int(ys.min()), int(xs.max() + 1), int(ys.max() + 1))

# These capture regions follow empty seams in the generated sheet; it is not yet
# a regular Unity grid. Final assembly below always produces exact 512px cells.
x_seams = [0, 452, 887, 1331, 1774]
y_seams = [0, 490, 887]
raw0 = source.crop((0, 0, 452, 490))
idle_solid = bounds_at(idle, 128)
raw_solid = bounds_at(raw0, 128)
scale = (idle_solid[3] - idle_solid[1]) / (raw_solid[3] - raw_solid[1])
# Centers of the same planted front foot, measured at the opaque sole band.
# Values are in source pixels relative to the nominal 443.5px column origins.
front_foot_x = [189, 188.5, 183.5, 184.5, 194.5, 209.5, 193, 180.5]
target_foot_x = (149 + 272) / 2
frames = []
measurements = []
for i in range(8):
    col, row = i % 4, i // 4
    if i in (0, 7):
        frame = idle.copy()
        info = {'index': i, 'source': 'Boss_Idle_0 (exact copy)'}
    else:
        x0, x1 = x_seams[col:col + 2]
        y0, y1 = y_seams[row:row + 2]
        raw = source.crop((x0, y0, x1, y1))
        bbox = raw.getchannel('A').getbbox()
        cropped = raw.crop(bbox)
        size = tuple(round(v * scale) for v in cropped.size)
        resized = cropped.resize(size, Image.Resampling.LANCZOS)
        anchor_x = col * 443.5 + front_foot_x[i] - x0 - bbox[0]
        dx = round(target_foot_x - anchor_x * size[0] / cropped.width)
        dy = 512 - size[1]
        assert dx >= 0 and dy >= 0 and dx + size[0] <= 512
        frame = Image.new('RGBA', (512, 512))
        frame.paste(resized, (dx, dy))
        info = {'index': i, 'source_crop': [x0+bbox[0], y0+bbox[1], x0+bbox[2], y0+bbox[3]],
                'uniform_scale': scale, 'destination': [dx, dy], 'resized_size': list(size)}
    final_bbox = frame.getchannel('A').getbbox()
    assert final_bbox[3] == 512, (i, final_bbox)
    assert final_bbox[0] > 0 and final_bbox[2] < 512 and final_bbox[1] > 0
    info['final_bbox_top_left_exclusive'] = list(final_bbox)
    info['bottom_transparent_margin_px'] = 512-final_bbox[3]
    measurements.append(info)
    frames.append(frame)

sheet = Image.new('RGBA', (2048, 1024))
for i, frame in enumerate(frames):
    sheet.paste(frame, (i % 4 * 512, i // 4 * 512))

# Preserve the existing project's texture settings, while assigning NEW GUIDs
# and local sprite IDs to this new asset. Idle and Walk are not modified.
meta = (ASSETS / '04.Sprites/Boss/Boss_Idle.png.meta').read_text()
meta = meta.replace('Boss_Idle', 'Boss_Slam')
asset_guid = uuid.uuid4().hex
meta = re.sub(r'^guid: \w+$', 'guid: ' + asset_guid, meta, flags=re.M)
old_ids = re.findall(r'^      internalID: (-?\d+)$', meta, flags=re.M)
assert len(old_ids) == 8
new_ids = [str((uuid.uuid4().int & ((1 << 62)-1)) + 1) for _ in range(8)]
for old_id, new_id in zip(old_ids, new_ids):
    meta = re.sub(r'(?<![\d\w])' + re.escape(old_id) + r'(?![\d\w])', new_id, meta)
meta = re.sub(r'(spriteID: )\w+', lambda m: m.group(1)+uuid.uuid4().hex, meta)
assert len(re.findall(r'      name: Boss_Slam_\d', meta)) == 8
assert 'spritePixelsToUnits: 50' in meta

# 8 unique poses, with a repeated final key solely to retain the last pose until
# 1.65 seconds. Playback is non-looping; no gameplay events are baked into it.
times = [0, .1, .3, .5, .7, .8, .85, 1.35, 1.6]
indices = [0, 1, 2, 3, 4, 5, 6, 7, 7]
clip = (ASSETS / '05.Animation/Boss/Boss_Walk.anim').read_text().replace('Boss_Walk', 'Boss_Slam')
curve = '    curve:\n' + ''.join(
    f'    - time: {t:g}\n      value: {{fileID: {new_ids[i]}, guid: {asset_guid}, type: 3}}\n'
    for t, i in zip(times, indices))
clip, count = re.subn(r'    curve:\n.*?(?=    attribute:)', curve, clip, flags=re.S)
assert count == 1
mapping = '    pptrCurveMapping:\n' + ''.join(
    f'    - {{fileID: {new_ids[i]}, guid: {asset_guid}, type: 3}}\n' for i in indices)
clip, count = re.subn(r'    pptrCurveMapping:\n.*?(?=  m_AnimationClipSettings:)', mapping, clip, flags=re.S)
assert count == 1
clip = re.sub(r'  m_SampleRate: .*', '  m_SampleRate: 20', clip)
clip = re.sub(r'    m_StopTime: .*', '    m_StopTime: 1.65', clip)
clip = re.sub(r'    m_LoopTime: .*', '    m_LoopTime: 0', clip)
clip_meta = (ASSETS / '05.Animation/Boss/Boss_Walk.anim.meta').read_text()
clip_meta = re.sub(r'^guid: \w+$', 'guid: ' + uuid.uuid4().hex, clip_meta, flags=re.M)

# Write metadata before each new asset, to keep the first Unity import stable.
Path(str(OUT)+'.meta').write_text(meta)
sheet.save(OUT, optimize=True)
Path(str(CLIP)+'.meta').write_text(clip_meta)
CLIP.write_text(clip)

# Opaque-background preview avoids display-dependent handling of transparent RGB.
checker = Image.new('RGB', (512, 512))
draw = ImageDraw.Draw(checker)
for y in range(0, 512, 16):
    for x in range(0, 512, 16):
        draw.rectangle((x,y,x+15,y+15), fill='#394555' if (x//16+y//16)%2 else '#323d4c')
labels = ['Ready', 'Lift', 'Raise', 'Hold', 'Downswing', 'Impact', 'Recovery', 'Return to Idle']
preview_frames = []
contact = Image.new('RGB', (2048, 1104), '#222b39')
for i, frame in enumerate(frames):
    preview = Image.new('RGB', (512, 552), '#222b39')
    preview.paste(checker, (0, 28))
    preview.paste(frame, (0, 28), frame)
    d = ImageDraw.Draw(preview)
    d.text((12,8), f'{i} - {labels[i]}', fill='white')
    d.line((0,540,511,540), fill='#56ddaf', width=1)
    preview_frames.append(preview)
    contact.paste(preview, (i%4*512, i//4*552))
contact.save(HERE/'contact-sheet.png')
palette = contact.quantize(colors=255)
gif_frames = [im.quantize(palette=palette) for im in preview_frames]
durations = [100,200,200,200,100,50,500,300]
gif_frames[0].save(HERE/'preview.gif', save_all=True, append_images=gif_frames[1:],
                   duration=durations, loop=0, disposal=2)
report = {'sheet_size':[2048,1024], 'cell_size':[512,512], 'sprite_count':8,
          'pivot_normalized':[.5,0], 'ppu':50, 'clip_duration_seconds':1.65,
          'impact_frame':5, 'impact_time_seconds':.8, 'frame_durations_ms':durations,
          'generation_source_sha256':hashlib.sha256((HERE/'generation-source.png').read_bytes()).hexdigest(),
          'sheet_sha256':hashlib.sha256(OUT.read_bytes()).hexdigest(),
          'frames':measurements}
(HERE/'measurements.json').write_text(json.dumps(report,indent=2)+'\n')
print(json.dumps(report,indent=2))

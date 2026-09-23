"""Build the Charge atlas, stable Unity metadata, clips and review previews.

Requires Pillow and NumPy. Run from any directory after generation-source.png
and assembly.json have been saved beside this script. Existing Idle/Walk/Slam/
Swing assets are read-only references. asset-ids.json preserves references on rerun.
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
OUT = ASSETS / '04.Sprites/Boss/Boss_Charge.png'
ANIM = ASSETS / '05.Animation/Boss'
CONFIG = json.loads((HERE / 'assembly.json').read_text())
IDLE_PATH = ASSETS / '04.Sprites/Boss/Boss_Idle.png'
idle = Image.open(IDLE_PATH).convert('RGBA').crop((0, 0, 512, 512))
source = Image.open(HERE / 'generation-source.png').convert('RGBA')
assert list(source.size) == CONFIG['source_size']
pixels = np.array(source)
pixels[pixels[:, :, 3] < 16] = 0
source = Image.fromarray(pixels)

def bounds_at(im, threshold):
    ys, xs = np.where(np.array(im)[:, :, 3] >= threshold)
    return [int(xs.min()), int(ys.min()), int(xs.max()+1), int(ys.max()+1)]

raw0 = source.crop(CONFIG['regions'][0])
ib, rb = bounds_at(idle, 128), bounds_at(raw0, 128)
scale = (ib[3]-ib[1]) / (rb[3]-rb[1])
frames, measurements = [], []
for i in range(12):
    if i in (0, 11):
        frame = idle.copy()
        info = {'index': i, 'source': 'Boss_Idle_0 (exact copy)'}
    else:
        region = CONFIG['regions'][i]
        raw = source.crop(region)
        bbox = raw.getchannel('A').getbbox()
        cropped = raw.crop(bbox)
        size = [round(cropped.width*scale), round(cropped.height*scale)]
        resized = cropped.resize(size, Image.Resampling.LANCZOS)
        # A manually inspected pelvis/root anchor; running foot X is allowed to
        # change through the stride rather than pinning every foot to one spot.
        root_x = CONFIG['root_x_source'][i] - region[0] - bbox[0]
        dx = round(CONFIG['target_root_x'] - root_x * size[0]/cropped.width)
        bottom_gap = CONFIG['bottom_gap_px'][i]
        dy = 512 - bottom_gap - size[1]
        assert dx >= 4 and dx+size[0] <= 508, (i, dx, size)
        assert dy > 0, (i, dy, size)
        frame = Image.new('RGBA', (512, 512))
        frame.paste(resized, (dx, dy))
        info = {'index': i, 'source': 'generation-source.png',
                'source_crop': [region[0]+bbox[0], region[1]+bbox[1],
                                region[0]+bbox[2], region[1]+bbox[3]],
                'uniform_scale': scale, 'destination': [dx, dy],
                'resized_size': size, 'source_root_x': CONFIG['root_x_source'][i]}
    final_bbox = frame.getchannel('A').getbbox()
    assert 0 < final_bbox[0] < final_bbox[2] < 512 and final_bbox[1] > 0
    assert 512-final_bbox[3] == CONFIG['bottom_gap_px'][i]
    info['final_bbox_top_left_exclusive'] = list(final_bbox)
    info['solid_bbox_alpha128'] = bounds_at(frame, 128)
    info['bottom_transparent_margin_px'] = 512-final_bbox[3]
    measurements.append(info)
    frames.append(frame)

sheet = Image.new('RGBA', (2048, 1536))
for i, frame in enumerate(frames):
    sheet.paste(frame, (i%4*512, i//4*512))

durations = [150, 200, 350, 150, 100, 100, 100, 100, 150, 300, 350, 400]
clips = {
    'Boss_Charge': {'indices': list(range(12)), 'durations_ms': durations, 'loop': False},
    'Boss_Charge_Start': {'indices': [0, 1, 2, 3], 'durations_ms': durations[:4], 'loop': False},
    'Boss_Charge_Loop': {'indices': [4, 5, 6, 7], 'durations_ms': durations[4:8], 'loop': True},
    'Boss_Charge_End': {'indices': [8, 9, 10, 11], 'durations_ms': durations[8:], 'loop': False},
}
ids_path = HERE / 'asset-ids.json'
if ids_path.exists():
    ids = json.loads(ids_path.read_text())
else:
    assert not OUT.exists(), 'Refusing to overwrite an unrelated existing asset'
    assert all(not (ANIM/(name+'.anim')).exists() for name in clips)
    ids = {'texture_guid': uuid.uuid4().hex, 'sheet_sprite_id': uuid.uuid4().hex,
           'sprite_ids': [uuid.uuid4().hex for _ in range(12)],
           'local_ids': [str((uuid.uuid4().int & ((1 << 62)-1))+1) for _ in range(12)],
           'clip_guids': {name: uuid.uuid4().hex for name in clips}}
    ids_path.write_text(json.dumps(ids, indent=2)+'\n')
guid, local_ids = ids['texture_guid'], ids['local_ids']
if OUT.exists():
    assert 'guid: '+guid in Path(str(OUT)+'.meta').read_text()

# Keep the established import settings, replacing only identity and slice data.
meta = Path(str(IDLE_PATH)+'.meta').read_text()
meta = re.sub(r'^guid: \w+$', 'guid: '+guid, meta, flags=re.M)
table = '  internalIDToNameTable:\n'+''.join(
    f'  - first:\n      213: {local_ids[i]}\n    second: Boss_Charge_{i}\n' for i in range(12))
meta = re.sub(r'  internalIDToNameTable:\n.*?(?=  externalObjects:)', table, meta, flags=re.S)
template = re.search(r'(    - serializedVersion: 2\n      name: Boss_Idle_0\n.*?)(?=    - serializedVersion:)', meta, re.S).group(1)
blocks = []
for i in range(12):
    block = template.replace('Boss_Idle_0', f'Boss_Charge_{i}')
    block = re.sub(r'^        x: \d+$', f'        x: {i%4*512}', block, flags=re.M)
    block = re.sub(r'^        y: \d+$', f'        y: {(2-i//4)*512}', block, flags=re.M)
    block = re.sub(r'      spriteID: \w+', '      spriteID: '+ids['sprite_ids'][i], block)
    block = re.sub(r'      internalID: -?\d+', '      internalID: '+local_ids[i], block)
    blocks.append(block)
meta = re.sub(r'    sprites:\n.*?(?=^    outline:)', '    sprites:\n'+''.join(blocks), meta, flags=re.S | re.M)
meta = re.sub(r'^    spriteID: \w+$', '    spriteID: '+ids['sheet_sprite_id'], meta, flags=re.M)
meta = meta.replace('"gridCellCount":{"x":4.0,"y":2.0}', '"gridCellCount":{"x":4.0,"y":3.0}')
names = '    nameFileIdTable:\n'+''.join(f'      Boss_Charge_{i}: {local_ids[i]}\n' for i in range(12))
meta = re.sub(r'    nameFileIdTable:\n.*?(?=  mipmapLimitGroupName:)', names, meta, flags=re.S)
assert len(re.findall(r'^      name: Boss_Charge_\d+$', meta, re.M)) == 12
Path(str(OUT)+'.meta').write_text(meta)
sheet.save(OUT, optimize=True)

clip_template = (ANIM/'Boss_Walk.anim').read_text()
clip_meta_template = (ANIM/'Boss_Walk.anim.meta').read_text()
for name, data in clips.items():
    indices = list(data['indices'])
    times, elapsed = [], 0
    for duration in data['durations_ms']:
        times.append(elapsed/1000)
        elapsed += duration
    # Unity's final sprite key occupies one sample. Repeat it one sample before
    # the intended end so every pose keeps its specified hold duration.
    if elapsed/1000-.05 > times[-1]+.00001:
        times.append(elapsed/1000-.05)
        indices.append(indices[-1])
    clip = clip_template.replace('Boss_Walk', name)
    curve = '    curve:\n'+''.join(
        f'    - time: {t:g}\n      value: {{fileID: {local_ids[i]}, guid: {guid}, type: 3}}\n'
        for t, i in zip(times, indices))
    clip, count = re.subn(r'    curve:\n.*?(?=    attribute:)', curve, clip, flags=re.S)
    assert count == 1
    mapping = '    pptrCurveMapping:\n'+''.join(
        f'    - {{fileID: {local_ids[i]}, guid: {guid}, type: 3}}\n' for i in indices)
    clip = re.sub(r'    pptrCurveMapping:\n.*?(?=  m_AnimationClipSettings:)', mapping, clip, flags=re.S)
    clip = re.sub(r'  m_SampleRate: .*', '  m_SampleRate: 20', clip)
    clip = re.sub(r'    m_StopTime: .*', f'    m_StopTime: {elapsed/1000:g}', clip)
    clip = re.sub(r'    m_LoopTime: .*', f'    m_LoopTime: {int(data["loop"])}', clip)
    clip_meta = re.sub(r'^guid: \w+$', 'guid: '+ids['clip_guids'][name], clip_meta_template, flags=re.M)
    (ANIM/(name+'.anim.meta')).write_text(clip_meta)
    (ANIM/(name+'.anim')).write_text(clip)
    data['duration_seconds'] = elapsed/1000
    data['keys'] = [{'time': t, 'frame': i} for t, i in zip(times, indices)]

checker = Image.new('RGB', (512, 512))
d = ImageDraw.Draw(checker)
for y in range(0, 512, 16):
    for x in range(0, 512, 16):
        d.rectangle((x,y,x+15,y+15), fill='#394555' if (x//16+y//16)%2 else '#323d4c')
labels = ['Ready', 'Lower stance', 'Hold / warning', 'Launch', 'Stride A contact',
          'Stride A passing', 'Stride B contact', 'Stride B passing',
          'Brake', 'Absorb stop', 'Rise / recovery', 'Return to Idle']
previews = []
contact = Image.new('RGB', (2048, 1656), '#222b39')
for i, frame in enumerate(frames):
    preview = Image.new('RGB', (512, 552), '#222b39')
    preview.paste(checker, (0, 28))
    preview.paste(frame, (0, 28), frame)
    d = ImageDraw.Draw(preview)
    d.text((12,8), f'{i:02d} - {labels[i]}', fill='white')
    d.line((0,540,511,540), fill='#56ddaf', width=1)
    previews.append(preview)
    contact.paste(preview, (i%4*512, i//4*552))
contact.save(HERE/'contact-sheet.png')
palette = contact.quantize(colors=255)
gif_frames = [im.quantize(palette=palette) for im in previews]
sequence = [0,1,2,3]+[4,5,6,7]*3+[8,9,10,11]
gif_frames[0].save(HERE/'preview.gif', save_all=True,
    append_images=[gif_frames[i] for i in sequence[1:]],
    duration=[durations[i] for i in sequence], loop=0, disposal=2)
gif_frames[4].save(HERE/'run-loop.gif', save_all=True, append_images=gif_frames[5:8],
    duration=durations[4:8], loop=0, disposal=2)
report = {'sheet_size':[2048,1536], 'cell_size':[512,512], 'sprite_count':12,
          'pivot_normalized':[.5,0], 'ppu':50, 'uniform_generation_scale':scale,
          'frame_durations_ms':durations, 'clips':clips,
          'preview_run_repetitions':3,
          'generation_source_sha256':hashlib.sha256((HERE/'generation-source.png').read_bytes()).hexdigest(),
          'sheet_sha256':hashlib.sha256(OUT.read_bytes()).hexdigest(), 'frames':measurements}
(HERE/'measurements.json').write_text(json.dumps(report,indent=2)+'\n')
print(json.dumps({'scale':scale,'frames':measurements,'clips':clips},indent=2))

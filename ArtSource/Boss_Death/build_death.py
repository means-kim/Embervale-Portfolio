"""Assemble the ImageGen Death poses and a 1.40s Unity sprite animation.

Requires Pillow and NumPy. Run after generation-source.png and assembly.json
are present. asset-ids.json keeps references stable when rebuilding this asset.
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
OUT = ASSETS / '04.Sprites/Boss/Boss_Death.png'
CLIP = ASSETS / '05.Animation/Boss/Boss_Death.anim'
IDLE = ASSETS / '04.Sprites/Boss/Boss_Idle.png'
HIT = ASSETS / '04.Sprites/Boss/Boss_Hit.png'
CONFIG = json.loads((HERE/'assembly.json').read_text())
impact = Image.open(HIT).convert('RGBA').crop((512, 0, 1024, 512))
source = Image.open(HERE/'generation-source.png').convert('RGBA')
assert list(source.size) == CONFIG['source_size']
arr = np.array(source)
arr[arr[:, :, 3] < 16] = 0
source = Image.fromarray(arr)

def bounds_at(im, threshold):
    ys, xs = np.where(np.array(im)[:, :, 3] >= threshold)
    return [int(xs.min()), int(ys.min()), int(xs.max()+1), int(ys.max()+1)]

ib = bounds_at(impact, 128)
neutral = source.crop(CONFIG['regions'][0])
nb = bounds_at(neutral, 128)
scale = (ib[3]-ib[1]) / (nb[3]-nb[1])
frames, measurements = [], []
for i in range(8):
    if i == 0:
        frame = impact.copy()
        info = {'index': i, 'source': 'Boss_Hit_1 (exact copy)'}
    else:
        region = CONFIG['regions'][i]
        raw = source.crop(region)
        bbox = raw.getchannel('A').getbbox()
        cropped = raw.crop(bbox)
        size = [round(cropped.width*scale), round(cropped.height*scale)]
        resized = cropped.resize(size, Image.Resampling.LANCZOS)
        # Early frames use the planted foot; collapse frames use the inspected body anchor.
        anchor = CONFIG['anchor_x_source'][i] - region[0] - bbox[0]
        dx = round(CONFIG['target_anchor_x'][i] - anchor*size[0]/cropped.width)
        dy = 512-size[1]
        assert dx >= 4 and dx+size[0] <= 508 and dy > 0, (i, dx, dy, size)
        frame = Image.new('RGBA', (512, 512))
        frame.paste(resized, (dx, dy))
        info = {'index': i, 'source': 'generation-source.png',
                'source_crop': [region[0]+bbox[0], region[1]+bbox[1],
                                region[0]+bbox[2], region[1]+bbox[3]],
                'uniform_scale': scale, 'destination': [dx,dy], 'resized_size': size,
                'source_anchor_x': CONFIG['anchor_x_source'][i],
                'aligned_anchor_x': dx+anchor*size[0]/cropped.width}
    b = frame.getchannel('A').getbbox()
    assert 0 < b[0] < b[2] < 512 and 0 < b[1] and b[3] == 512
    info['final_bbox_top_left_exclusive'] = list(b)
    info['solid_bbox_alpha128'] = bounds_at(frame, 128)
    info['bottom_transparent_margin_px'] = 512-b[3]
    frames.append(frame)
    measurements.append(info)

sheet = Image.new('RGBA', (2048, 1024))
for i, frame in enumerate(frames):
    sheet.paste(frame, (i%4*512, i//4*512))

ids_path = HERE/'asset-ids.json'
if ids_path.exists():
    ids = json.loads(ids_path.read_text())
else:
    assert not OUT.exists() and not CLIP.exists(), 'Refusing to replace unrelated assets'
    ids = {'texture_guid':uuid.uuid4().hex, 'clip_guid':uuid.uuid4().hex,
           'sheet_sprite_id':uuid.uuid4().hex,
           'sprite_ids':[uuid.uuid4().hex for _ in range(8)],
           'local_ids':[str((uuid.uuid4().int & ((1<<62)-1))+1) for _ in range(8)]}
    ids_path.write_text(json.dumps(ids,indent=2)+'\n')
guid, local_ids = ids['texture_guid'], ids['local_ids']
if OUT.exists():
    assert 'guid: '+guid in Path(str(OUT)+'.meta').read_text()
if CLIP.exists():
    assert 'guid: '+ids['clip_guid'] in Path(str(CLIP)+'.meta').read_text()

meta = Path(str(IDLE)+'.meta').read_text()
meta = re.sub(r'^guid: \w+$', 'guid: '+guid, meta, flags=re.M)
table = '  internalIDToNameTable:\n'+''.join(
    f'  - first:\n      213: {local_ids[i]}\n    second: Boss_Death_{i}\n' for i in range(8))
meta = re.sub(r'  internalIDToNameTable:\n.*?(?=  externalObjects:)',table,meta,flags=re.S)
template = re.search(r'(    - serializedVersion: 2\n      name: Boss_Idle_0\n.*?)(?=    - serializedVersion:)',meta,re.S).group(1)
blocks = []
for i in range(8):
    block = template.replace('Boss_Idle_0',f'Boss_Death_{i}')
    block = re.sub(r'^        x: \d+$',f'        x: {i%4*512}',block,flags=re.M)
    block = re.sub(r'^        y: \d+$',f'        y: {(1-i//4)*512}',block,flags=re.M)
    block = re.sub(r'      spriteID: \w+','      spriteID: '+ids['sprite_ids'][i],block)
    block = re.sub(r'      internalID: -?\d+','      internalID: '+local_ids[i],block)
    blocks.append(block)
meta = re.sub(r'    sprites:\n.*?(?=^    outline:)','    sprites:\n'+''.join(blocks),meta,flags=re.S|re.M)
meta = re.sub(r'^    spriteID: \w+$','    spriteID: '+ids['sheet_sprite_id'],meta,flags=re.M)
# Preserve the original 4-column, 2-row grid settings.
names = '    nameFileIdTable:\n'+''.join(f'      Boss_Death_{i}: {local_ids[i]}\n' for i in range(8))
meta = re.sub(r'    nameFileIdTable:\n.*?(?=  mipmapLimitGroupName:)',names,meta,flags=re.S)
assert len(re.findall(r'^      name: Boss_Death_\d+$',meta,re.M)) == 8

times = [0, .1, .3, .5, .7, .85, .95, 1.05, 1.35]
indices = [0, 1, 2, 3, 4, 5, 6, 7, 7]
durations = [100, 200, 200, 200, 150, 100, 100, 350]
clip = (ASSETS/'05.Animation/Boss/Boss_Walk.anim').read_text().replace('Boss_Walk','Boss_Death')
curve = '    curve:\n'+''.join(
    f'    - time: {t:g}\n      value: {{fileID: {local_ids[i]}, guid: {guid}, type: 3}}\n'
    for t,i in zip(times,indices))
clip,count = re.subn(r'    curve:\n.*?(?=    attribute:)',curve,clip,flags=re.S)
assert count == 1
mapping = '    pptrCurveMapping:\n'+''.join(
    f'    - {{fileID: {n}, guid: {guid}, type: 3}}\n' for n in [local_ids[i] for i in indices])
clip = re.sub(r'    pptrCurveMapping:\n.*?(?=  m_AnimationClipSettings:)',mapping,clip,flags=re.S)
clip = re.sub(r'  m_SampleRate: .*','  m_SampleRate: 20',clip)
clip = re.sub(r'    m_StopTime: .*','    m_StopTime: 1.4',clip)
clip = re.sub(r'    m_LoopTime: .*','    m_LoopTime: 0',clip)
clip_meta = (ASSETS/'05.Animation/Boss/Boss_Walk.anim.meta').read_text()
clip_meta = re.sub(r'^guid: \w+$','guid: '+ids['clip_guid'],clip_meta,flags=re.M)
Path(str(OUT)+'.meta').write_text(meta)
sheet.save(OUT,optimize=True)
Path(str(CLIP)+'.meta').write_text(clip_meta)
CLIP.write_text(clip)

checker = Image.new('RGB',(512,512))
d = ImageDraw.Draw(checker)
for y in range(0,512,16):
    for x in range(0,512,16):
        d.rectangle((x,y,x+15,y+15),fill='#394555' if (x//16+y//16)%2 else '#323d4c')
labels = ['Fatal hit', 'Lose strength', 'Knees buckle', 'Kneel',
          'Brace on hands', 'Arms give way', 'Ground impact', 'Dead / settled']
previews = []
contact = Image.new('RGB',(2048,1104),'#222b39')
for i,frame in enumerate(frames):
    preview = Image.new('RGB',(512,552),'#222b39')
    preview.paste(checker,(0,28))
    preview.paste(frame,(0,28),frame)
    d = ImageDraw.Draw(preview)
    d.text((12,8),f'{i} - {labels[i]}',fill='white')
    d.line((0,540,511,540),fill='#56ddaf',width=1)
    previews.append(preview)
    contact.paste(preview,(i%4*512,i//4*552))
contact.save(HERE/'contact-sheet.png')
palette = contact.quantize(colors=255)
gif_frames = [im.quantize(palette=palette) for im in previews]
# Extra 1000ms final corpse hold is review-only; the Unity clip remains 1.40s.
gif_frames[0].save(HERE/'preview.gif',save_all=True,append_images=gif_frames[1:],
                   duration=durations[:-1]+[durations[-1]+1000],loop=0,disposal=2)
report = {'sheet_size':[2048,1024], 'cell_size':[512,512], 'sprite_count':8,
          'pivot_normalized':[.5,0], 'ppu':50, 'uniform_generation_scale':scale,
          'clip_duration_seconds':1.4, 'clip_loop':False, 'frame_durations_ms':durations,
          'key_times_seconds':times,'key_frame_indices':indices,'preview_extra_dead_hold_ms':1000,
          'generation_source_sha256':hashlib.sha256((HERE/'generation-source.png').read_bytes()).hexdigest(),
          'sheet_sha256':hashlib.sha256(OUT.read_bytes()).hexdigest(), 'frames':measurements}
(HERE/'measurements.json').write_text(json.dumps(report,indent=2)+'\n')
print(json.dumps(report,indent=2))

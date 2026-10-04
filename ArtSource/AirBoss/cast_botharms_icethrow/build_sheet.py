"""Pack generated artwork; no procedural poses, trimming, or per-frame fitting."""
from pathlib import Path
from PIL import Image, ImageDraw
import hashlib, json, re, shutil, uuid

ROOT = Path(__file__).resolve().parents[3]
HERE = Path(__file__).resolve().parent
CELL = 1024
NAME = 'AirBoss_Cast_BothArms_IceThrow'
master = Image.open(ROOT/'ArtSource/AirBoss/idle/AirBoss_Idle_Master_image2.png').convert('RGBA')
frames, report = [], []
for i in range(32):
    source = HERE/'source/v01'/f'frame{i:02}_safe.png'
    if not source.exists(): source = HERE/'source/v01'/f'frame{i:02}_final.png'
    if not source.exists(): source = HERE/'source/v01'/f'frame{i:02}_revised.png'
    if not source.exists(): source = HERE/'source/v01'/f'frame{i:02}_generated.png'
    im = master.copy() if i in (0,31) else Image.open(source).convert('RGBA')
    original_size = im.size
    # Uniform whole-canvas normalization, not silhouette fitting.
    if im.size != (1024,1024): im = im.resize((1024,1024),Image.Resampling.LANCZOS)
    cell = im.copy()  # No added padding, silhouette fitting, or re-centering.
    if i in (0,31): assert cell.tobytes() == master.tobytes()
    bounds = cell.getchannel('A').getbbox()
    # Preserve all generated alpha, including isolated alpha=1 specks.
    visible_bounds = cell.getchannel('A').point(lambda a: 255 if a > 2 else 0).getbbox()
    assert visible_bounds[0]>=64 and visible_bounds[1]>=64 and visible_bounds[2]<=CELL-64 and visible_bounds[3]<=CELL-64, (i, visible_bounds)
    path=HERE/'frames/v01'/f'{NAME}_v01_{i:02}.png'
    cell.save(path)
    frames.append(cell)
    report.append(dict(frame=i, source=str(source.relative_to(HERE)) if i not in (0,31) else '../idle/AirBoss_Idle_Master_image2.png',source_size=original_size,cell_size=cell.size,alpha_bounds=bounds,alpha_gt_2_bounds=visible_bounds,strict_64px_alpha_margin=(bounds[0]>=64 and bounds[1]>=64 and bounds[2]<=960 and bounds[3]<=960),sha256=hashlib.sha256(cell.tobytes()).hexdigest()))

template=(ROOT/'Assets/04.Sprites/AirBoss/AirBoss_Cast_SpinRoll360_Part2_4x2_1024_v02.png.meta').read_text()
prefix=template.split('  spriteSheet:')[0]
entry_start=template.index('    - serializedVersion:',template.index('  spriteSheet:'))
entry_end=template.index('    - serializedVersion:',entry_start+8)
entry_template=template[entry_start:entry_end]
tail_start=template.index('\n    outline:',entry_end)
tail=template[tail_start:]
tail=tail[:tail.index('    nameFileIdTable:')]+ '    nameFileIdTable:\n'
assets=ROOT/'Assets/04.Sprites/AirBoss'
for part,start,count,rows in [(1,0,16,4),(2,16,16,4)]:
    sheet=Image.new('RGBA',(CELL*4,CELL*rows))
    entries=[];names=[]
    filename=f'{NAME}_Part{part}_4x{rows}_{CELL}_v01.png'
    for j in range(count):
        i=start+j;x=(j%4)*CELL;yt=(j//4)*CELL;y=(rows-1-j//4)*CELL
        sheet.paste(frames[i],(x,yt))
        assert sheet.crop((x,yt,x+CELL,yt+CELL)).tobytes()==frames[i].tobytes()
        name=f'{NAME}_v01_{i:02}'
        sid=uuid.uuid5(uuid.NAMESPACE_URL,name).hex
        fid=int(sid[:15],16)
        e=re.sub(r'      name: .*',f'      name: {name}',entry_template)
        for key,val in [('x',x),('y',y),('width',CELL),('height',CELL)]:
            e=re.sub(rf'        {key}: .*',f'        {key}: {val}',e)
        e=re.sub(r'      spriteID: .*',f'      spriteID: {sid}',e)
        e=re.sub(r'      internalID: .*',f'      internalID: {fid}',e)
        entries.append(e);names.append(f'      {name}: {fid}\n')
    path=HERE/'sheets'/filename;sheet.save(path)
    shutil.copy2(path,assets/filename)
    p=re.sub(r'^guid: .*',f'guid: {uuid.uuid5(uuid.NAMESPACE_URL,filename).hex}',prefix,flags=re.M)
    p=re.sub(r'maxTextureSize: \d+','maxTextureSize: 4096',p)
    meta=p+'  spriteSheet:\n    serializedVersion: 2\n    sprites:\n'+''.join(entries)+tail+''.join(names)+'  mipmapLimitGroupName: \n  pSDRemoveMatte: 0\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'
    # Unity assigns internal sprite file IDs on first import; preserve those on rebuilds.
    if not (assets/(filename+'.meta')).exists():
        (assets/(filename+'.meta')).write_text(meta)
    shutil.copy2(assets/(filename+'.meta'),HERE/'sheets'/(filename+'.meta'))

contact=Image.new('RGB',(2048,1024),(28,31,42));draw=ImageDraw.Draw(contact)
for i,im in enumerate(frames):
    small=im.resize((256,256),Image.Resampling.LANCZOS);x=i%8*256;y=i//8*256
    contact.paste(small,(x,y),small);draw.text((x+8,y+8),f'{i:02}',fill='white')
contact.save(HERE/'contact_final_v01.jpg')
(HERE/'validation_v01.json').write_text(json.dumps(report,indent=2))
assert frames[0].tobytes()==frames[31].tobytes()
print('Packed 32 cells (30 drawn + identical Idle endpoints), 1024 cells, visible artwork margin PASS (alpha>2), all alpha preserved, master exact, sheet round-trip PASS.')

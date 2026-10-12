from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import json,hashlib,re,subprocess
base=Path(__file__).resolve().parent;root=base.parents[1]
plan=json.loads((base/'Plan.json').read_text(encoding='utf-8'))
changed=[];errors=[];auto_ids=[]
for item in plan:
    path=item['path']
    if hashlib.sha256((root/path).read_bytes()).hexdigest()!=item['pngSha256']:errors.append('PNG changed '+path)
    original=(base/'Before'/Path(path+'.meta')).read_text(encoding='utf-8-sig')
    current=(root/(path+'.meta')).read_text(encoding='utf-8-sig')
    strip=lambda text:[line for line in text.splitlines() if not re.match(r'\s*(spritePivot|pivot|alignment):',line)]
    a,b=strip(original),strip(current)
    if len(a)!=len(b):errors.append('Unexpected metadata structure changed '+path)
    else:
        for before,after in zip(a,b):
            if before==after:continue
            if re.fullmatch(r'\s*internalID: 0',before) and re.fullmatch(r'\s*internalID: -?\d+',after):
                auto_ids.append({'path':path,'before':before.strip(),'after':after.strip()})
            elif re.fullmatch(r'\s*spriteID: *',before) and re.fullmatch(r'\s*spriteID: [0-9a-f]{32}',after):
                auto_ids.append({'path':path,'before':before.strip(),'after':after.strip()})
            else:errors.append('Unexpected metadata changed '+path+' '+before+' -> '+after)
    changed.append(path)
font=ImageFont.truetype('C:/Windows/Fonts/malgun.ttf',21)
small=ImageFont.truetype('C:/Windows/Fonts/consola.ttf',15)
preview=Image.new('RGB',(1500,1070),'#202631');d=ImageDraw.Draw(preview)
d.text((25,15),'몸통 기준 피벗 비교 | 위: 기존 발 기준 / 아래: 변경 후 몸통 기준',font=font,fill='white')
selected=[('Idle',0),('DashLoop',0),('DashLoop',45),('DashLoop',90),('DashLoop',135)]
for column,(name,angle) in enumerate(selected):
    item=next(i for i in plan if i['path'].endswith('/'+name+'_00.png'));c=item['changes'][0]
    im=Image.open(root/item['path']).convert('RGBA')
    for row,mode in enumerate(['old','new']):
        tile=Image.new('RGBA',(300,470),'#202631'); td=ImageDraw.Draw(tile)
        # Fixed transform origin, scale 0.6; oversized transparent canvas prevents rotation clipping.
        layer=Image.new('RGBA',(1200,1200))
        px=c['oldX']*512 if mode=='old' else c['anchorTopLeft'][0]
        py=(1-c['oldY'])*512 if mode=='old' else c['anchorTopLeft'][1]
        layer.alpha_composite(im,(round(600-px),round(600-py)))
        rotated=layer.rotate(angle,resample=Image.Resampling.BICUBIC,center=(600,600))
        reduced=rotated.resize((720,720),Image.Resampling.LANCZOS)
        tile.alpha_composite(reduced,(-210,-90))
        td.line((150,0,150,430),fill='#384152');td.line((0,270,300,270),fill='#384152')
        td.ellipse((146,266,154,274),fill='#ff6480' if mode=='old' else '#6dffbd')
        td.text((10,440),f'{name}  {angle} deg  {mode}',font=small,fill='white')
        preview.paste(tile.convert('RGB'),(column*300,65+row*485))
preview.save(base/'Comparison.png')
report={'textures':len(plan),'sprites':sum(len(i['changes']) for i in plan),'pngOriginalsUnchanged':not any('PNG' in e for e in errors),'onlyRequestedPivotsAndUnityGeneratedInternalIDsChanged':not errors,'unityGeneratedInternalIDAssignments':auto_ids,'failures':errors}
(base/'FileVerification.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
assert not errors,errors
print(json.dumps(report))

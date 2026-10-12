from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import json,hashlib,shutil
base=Path(__file__).resolve().parent
root=base.parents[1]
items=json.loads((base/'Inventory.json').read_text(encoding='utf-8'))
# Manually reviewed anatomical anchors in the original 512 cell, top-left pixels.
# Flame drift loops share a fixed anchor; DoubleJump preserves its authored spin centre.
anchors={
 'Idle':[(270,286)],
 'Attack':[(260,318),(264,308),(268,308),(260,326),(260,326),(258,322),(272,292),(270,286)],
 'DashStart':[(268,315),(310,300)], 'DashLoop':[(310,300)],
 'DashEnd':[(270,315),(270,294),(270,286)],
 'JumpStart':[(270,297),(270,345),(270,300)],
 'JumpRise':[(267,270)], 'JumpApex':[(270,265)], 'Fall':[(267,285)],
 'Land':[(270,345),(270,317),(270,297),(270,286)],
 'DoubleJump':[(256,286)]*15+[(267,255)],
 'Run':[(270,286),(273,285),(274,285),(274,284),(270,286),(273,304),(270,300),(273,288),(275,290),(270,289)],
 'Hurt':[(282,320),(282,286),(264,330),(270,286)],
 'WallCling':[(265,285)], 'WallSlide':[(266,282)],
 'WallClimb':[(265,285),(270,302),(265,285),(270,302)],
 'WallJump':[(260,323),(278,273),(278,273)]
}
attack_anchors={
 'Attack_Grid512':[(260,318),(264,308),(268,308),(268,308),(265,300),(263,300),(260,326),(260,326),(260,326),(260,326),(258,322),(272,292),(270,310),(270,310),(270,286)],
 'Attack2_Grid512':[(260,318),(262,318),(262,318),(260,326),(260,326),(260,326),(266,327),(266,327),(280,314),(280,314),(273,309),(272,292),(270,286),(270,286),(270,286)],
 'Attack3_Grid512':[(260,318),(260,318),(260,318),(260,326),(260,326),(260,318),(270,310),(270,310),(270,286)],
 'Attack4_Grid512':[(260,318),(260,318),(260,318),(260,318),(260,318),(260,318),(260,318),(260,318),(260,318),(260,318),(258,322),(272,292),(270,310),(270,310),(270,286)]
}
plan=[]; rows=[]
font=ImageFont.truetype('C:/Windows/Fonts/consola.ttf',13)
for item in items:
    path=item['path']; image=Image.open(root/path).convert('RGBA'); changes=[]
    for r in item['sprites']:
        name=r['name']; key,index=name.rsplit('_',1); index=int(index)
        options=attack_anchors[key] if item['mode']=='Multiple' else anchors[key]
        ax,ay=options[min(index,len(options)-1)]
        if item['mode']=='Multiple':
            cx=int(r['x']//512)*512; cy=int(r['y']//512)*512
            nx=(cx+ax-r['x'])/r['width']; ny=(cy+512-ay-r['y'])/r['height']
            oldx=r['x']-cx+r['px']*r['width']; oldy=512-(r['y']-cy+r['py']*r['height'])
            crop=image.crop((cx,image.height-cy-512,cx+512,image.height-cy))
        else:
            nx=ax/r['width']; ny=1-ay/r['height']; oldx=r['px']*r['width']; oldy=(1-r['py'])*r['height']; crop=image
        assert 0<=nx<=1 and 0<=ny<=1,(path,name,nx,ny)
        changes.append({'id':r['id'],'name':name,'newX':nx,'newY':ny,'oldX':r['px'],'oldY':r['py'],'anchorTopLeft':[ax,ay],'deltaLocal':[(oldx-ax)/item['ppu'],(ay-oldy)/item['ppu']]})
        if index==0 or item['mode']=='Multiple' or key in ['DashStart','JumpStart','DashEnd','Land','Run','Hurt','DoubleJump','WallClimb','WallJump','Attack']:
            tile=Image.new('RGB',(220,242),'#202631'); resized=crop.resize((204,204)); tile.paste(resized,(8,8),resized.getchannel('A')); d=ImageDraw.Draw(tile)
            for x,y,c in [(oldx,oldy,'#ff6480'),(ax,ay,'#6dffbd')]:
                x=8+x/512*204;y=8+y/512*204;d.line((x-6,y,x+6,y),fill=c,width=2);d.line((x,y-6,x,y+6),fill=c,width=2)
            d.text((5,215),name,font=font,fill='white');rows.append(tile)
    backup=base/'Before'/Path(path+'.meta'); backup.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(root/(path+'.meta'),backup)
    plan.append({'path':path,'guid':item['guid'],'mode':item['mode'],'ppu':item['ppu'],'pngSha256':hashlib.sha256((root/path).read_bytes()).hexdigest(),'changes':changes})
(base/'Plan.json').write_text(json.dumps(plan,indent=2),encoding='utf-8')
for page in range((len(rows)+29)//30):
    grid=Image.new('RGB',(1320,1210),'#202631')
    for i,t in enumerate(rows[page*30:(page+1)*30]):grid.paste(t,((i%6)*220,(i//6)*242))
    grid.save(base/f'Anchors_{page+1:02}.png')
print('textures',len(plan),'sprites',sum(len(i['changes']) for i in plan),'anchorPages',(len(rows)+29)//30)

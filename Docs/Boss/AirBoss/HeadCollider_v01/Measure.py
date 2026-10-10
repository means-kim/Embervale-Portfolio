from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,json
root=Path(__file__).resolve().parent;project=root.parents[3];assets=project/'Assets/04.Sprites/AirBoss'
idle=Image.open(assets/'AirBoss_Idle_Float_4x4_1024_v02.png').crop((0,0,1536,1536)).convert('RGBA')
def gray(im):
 bg=Image.new('RGBA',im.size,(30,35,45,255));bg.alpha_composite(im);return bg.convert('L')
# The existing offset (.5,10) is source pixel (786,408), measured from top-left.
# Rotate matching patch around exactly this existing head anchor.
ref=gray(idle).crop((690,312,882,504)).resize((24,24),Image.Resampling.LANCZOS)
results={}
for d,name in [('Left','AirBoss_Move_Bank_Left_4x4_1024_v01.png'),('Right','AirBoss_Move_Bank_4x4_1024_v01.png')]:
 im=Image.open(assets/name).convert('RGBA');records=[];contact=Image.new('RGB',(1536,1536),(30,35,45))
 for i in range(16):
  f=im.crop((i%4*1536,i//4*1536,(i%4+1)*1536,(i//4+1)*1536));g=np.array(gray(f).resize((192,192)),dtype=np.float32);search=g[20:95,45:155];best=None
  for angle in range(-40,41,5):
   for size in [22,24,26]:
    t=np.array(ref.resize((size,size)).rotate(angle,resample=Image.Resampling.BICUBIC,expand=False,fillcolor=35),dtype=np.float32);t-=t.mean();t/=max(np.linalg.norm(t),1)
    w=np.lib.stride_tricks.sliding_window_view(search,t.shape);means=w.mean(axis=(-1,-2));norm=np.sqrt(np.maximum((w*w).sum(axis=(-1,-2))-means*means*t.size,1));s=np.einsum('ijkl,kl->ij',w,t)/norm;y,x=np.unravel_index(s.argmax(),s.shape);score=float(s[y,x])
    if best is None or score>best[0]:best=(score,(45+x+size/2)*8,(20+y+size/2)*8,angle,size)
  score,x,y,angle,size=best
  if i==0:x,y=786,408
  offset=[round((x-768)/36,4),round((768-y)/36,4)]
  records.append({'frame':i,'pixel':[x,y],'offset':offset,'score':score,'angle':angle,'templateSize':size})
  bg=Image.new('RGBA',(1536,1536),(30,35,45,255));bg.alpha_composite(f);p=bg.resize((384,384)).convert('RGB');draw=ImageDraw.Draw(p);cx,cy=x/4,y/4;draw.ellipse((cx-27,cy-27,cx+27,cy+27),outline=(255,90,90),width=2);draw.text((8,8),str(i)+' '+str(offset),fill='white');contact.paste(p,(i%4*384,i//4*384))
 contact.save(root/(d+'_Review.jpg'));results[d]=records
(root/'Offsets.json').write_text(json.dumps(results,indent=2));print(json.dumps(results))

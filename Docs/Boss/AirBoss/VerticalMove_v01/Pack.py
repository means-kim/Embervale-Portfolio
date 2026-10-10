from PIL import Image, ImageDraw, ImageFilter
import numpy as np
from pathlib import Path
import json
root=Path(__file__).resolve().parent
assets=root.parents[3]/'Assets/04.Sprites/AirBoss'
idle=Image.open(root/'IdleReference.png').convert('RGBA')
# Helmet crop is a stable registration landmark; no per-frame silhouette centering.
target=(695,325,839,482)
ref=idle.crop(target).convert('L')
report={}
for direction,seeds in [('Up',[(170+c*313,90+r*307) for r in range(4) for c in range(4)]),('Down',[(207,110),(499,110),(784,110),(1070,110),(195,405),(500,422),(780,443),(1070,443),(207,762),(500,776),(787,721),(1070,721),(207,1008),(500,1008),(786,1008),(1070,1008)])]:
 im=Image.open(root/(direction+'_Generated.png')).convert('RGBA'); a=np.array(im); gray=np.array(im.convert('L'),dtype=np.float32)
 sheet=Image.new('RGBA',(6144,6144)); previews=[]; records=[]
 for i,(sx,sy) in enumerate(seeds):
  if i in (0,15): out=idle.copy();records.append({'frame':i,'source':'IdleReference exact'} )
  else:
   # Match helmet translation/scale against Idle; restrict search to expected head.
   best=None
   for scale in [.28 if direction=="Up" else .25]:
    t=np.array(ref.resize((round(ref.width*scale),round(ref.height*scale)),Image.Resampling.LANCZOS),dtype=np.float32);t-=t.mean();t/=max(np.linalg.norm(t),1)
    x0=max(0,sx-65);y0=max(0,sy-65);search=gray[y0:sy+65,x0:sx+65]
    windows=np.lib.stride_tricks.sliding_window_view(search,t.shape)
    means=windows.mean(axis=(-1,-2));norm=np.sqrt(np.maximum((windows**2).sum(axis=(-1,-2))-means**2*t.size,1))
    scores=np.einsum('ijkl,kl->ij',windows,t)/norm
    y,x=np.unravel_index(scores.argmax(),scores.shape);score=float(scores[y,x])
    if best is None or score>best[0]:best=(score,x0+x,y0+y,scale)
   score,hx,hy,scale=best
   # Extract the connected figure independently of nonuniform generated grid gutters.
   mask=Image.fromarray((a[:,:,3]>=16).astype('uint8')*255).copy(); seed=(int(hx+72*scale),int(hy+78*scale));ImageDraw.floodfill(mask,seed,128)
   component=Image.fromarray((np.array(mask)==128).astype('uint8')*255)
   bbox=component.getbbox()
   if bbox is None:raise RuntimeError((direction,i,seed))
   component=component.filter(ImageFilter.MaxFilter(7)); alpha=np.array(im.getchannel('A'));alpha[np.array(component)==0]=0
   figure=im.copy();figure.putalpha(Image.fromarray(alpha));bbox=figure.getbbox();figure=figure.crop(bbox)
   factor=1/scale
   figure=figure.convert('RGBa').resize((round(figure.width*factor),round(figure.height*factor)),Image.Resampling.LANCZOS).convert('RGBA')
   ox=round(target[0]-(hx-bbox[0])*factor);oy=round(target[1]-(hy-bbox[1])*factor)
   out=Image.new('RGBA',(1536,1536));out.alpha_composite(figure,(ox,oy))
   records.append({'frame':i,'helmetMatch':round(score,3),'sourceScale':float(scale),'offset':[ox,oy],'bounds':out.getchannel('A').point(lambda x:255 if x>=16 else 0).getbbox()})
  sheet.paste(out,(i%4*1536,i//4*1536))
  bg=Image.new('RGBA',out.size,(35,43,54,255));bg.alpha_composite(out);previews.append(bg.resize((384,384),Image.Resampling.LANCZOS).convert('RGB'))
 name=f'AirBoss_Move_{direction}_4x4_1536_v01.png';sheet.save(assets/name)
 contact=Image.new('RGB',(1536,1536));
 for i,p in enumerate(previews):contact.paste(p,(i%4*384,i//4*384))
 contact.save(root/(direction+'_Contact.jpg'))
 previews[0].save(root/(direction+'_Preview.gif'),save_all=True,append_images=previews[1:],duration=[60,60,60,70]*4,loop=0)
 report[direction]=records
(root/'PackingReport.json').write_text(json.dumps(report,indent=2))
print(json.dumps(report,indent=2))

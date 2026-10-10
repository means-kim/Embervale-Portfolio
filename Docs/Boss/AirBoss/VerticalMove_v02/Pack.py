from PIL import Image,ImageDraw
from pathlib import Path
import numpy as np,json,hashlib
root=Path(__file__).resolve().parent
project=root.parents[3]
idle=Image.open(root.parent/'VerticalMove_v01/IdleReference.png').convert('RGBA')
ref=np.array(idle.convert('L').resize((384,384)),dtype=np.float32)[80:122,171:214]
ref-=ref.mean();ref/=np.linalg.norm(ref)
(root/'Prepared').mkdir(exist_ok=True)
report={}
for d in ['Up','Down']:
 poses=[];stats=[]
 for p in sorted((root/'Frames').glob(d+'_*.png')):
  im=Image.open(p).convert('RGBA');size=im.size
  im=im.convert('RGBa').resize((1536,1536),Image.Resampling.LANCZOS).convert('RGBA')
  gray=np.array(im.convert('L').resize((384,384)),dtype=np.float32)
  search=gray[65:140,154:232];w=np.lib.stride_tricks.sliding_window_view(search,ref.shape)
  mean=w.mean(axis=(-1,-2));norm=np.sqrt(np.maximum((w*w).sum(axis=(-1,-2))-mean*mean*ref.size,1))
  corr=np.einsum('ijkl,kl->ij',w,ref)/norm;y,x=np.unravel_index(corr.argmax(),corr.shape)
  dx=(171-(154+x))*4;dy=(80-(65+y))*4
  aligned=Image.new('RGBA',(1536,1536));aligned.paste(im,(dx,dy))
  poses.append(aligned);stats.append({'source':p.name,'sourceSize':size,'offset':[int(dx),int(dy)],'helmetCorrelation':float(corr[y,x])})
 assert len(poses)==7
 # Order by actual hand height, since generation prompts do not guarantee pose magnitudes.
 yy,xx=np.indices((1536,1536));scores=[]
 for f in poses:
  a=np.array(f);m=(a[:,:,3]>100)&(a[:,:,:3].mean(2)<100)&(yy>620)&(yy<1040)&(((xx>320)&(xx<650))|((xx>980)&(xx<1300)))
  scores.append(float(yy[m].mean()))
 order=sorted(range(7),key=lambda i:scores[i],reverse=d=='Down')
 poses=[poses[i] for i in order]
 stats=[dict(stats[i],handHeightScore=scores[i]) for i in order]
 # Seven outward poses, peak held for two frames, reversed recovery, exact Idle endpoints.
 frames=[idle]+poses+list(reversed(poses))+[idle]
 sheet=Image.new('RGBA',(6144,6144));preview=[]
 for i,f in enumerate(frames):
  sheet.paste(f,(i%4*1536,i//4*1536))
  bg=Image.new('RGBA',f.size,(35,43,54,255));bg.alpha_composite(f);preview.append(bg.resize((384,384),Image.Resampling.LANCZOS).convert('RGB'))
 path=root/'Prepared'/f'AirBoss_Move_{d}_4x4_1536_v01.png';sheet.save(path)
 preview[0].save(root/(d+'_Preview.gif'),save_all=True,append_images=preview[1:],duration=[60,60,60,70]*4,loop=0)
 contact=Image.new('RGB',(1536,1536))
 for i,f in enumerate(preview):contact.paste(f,(i%4*384,i//4*384))
 contact.save(root/(d+'_Contact.jpg'))
 bounds=[f.getchannel('A').point(lambda x:255 if x>=16 else 0).getbbox() for f in frames]
 report[d]={'sourceFrames':stats,'frameCount':16,'firstIdleExact':frames[0].tobytes()==idle.tobytes(),'lastIdleExact':frames[-1].tobytes()==idle.tobytes(),'bounds':bounds,'clippedFrames':[i for i,b in enumerate(bounds) if b[0]<=0 or b[1]<=0 or b[2]>=1536 or b[3]>=1536]}
# Matched-size comparison; actual alpha compositing, no recoloring.
items=[('Idle',idle),('Old Up',Image.open(root/'Before/AirBoss_Move_Up_4x4_1536_v01.png').crop((1536,1536,3072,3072))),('New Up',Image.open(root/'Prepared/AirBoss_Move_Up_4x4_1536_v01.png').crop((1536,1536,3072,3072))),('Old Down',Image.open(root/'Before/AirBoss_Move_Down_4x4_1536_v01.png').crop((1536,1536,3072,3072))),('New Down',Image.open(root/'Prepared/AirBoss_Move_Down_4x4_1536_v01.png').crop((1536,1536,3072,3072)))]
compare=Image.new('RGB',(1600,760),(35,43,54));draw=ImageDraw.Draw(compare)
for i,(label,im) in enumerate(items):
 bg=Image.new('RGBA',im.size,(35,43,54,255));bg.alpha_composite(im)
 compare.paste(bg.resize((320,320),Image.Resampling.LANCZOS).convert('RGB'),(i*320,30));draw.text((i*320+8,8),label,fill='white')
 crop=bg.crop((850,880,1170,1200)).convert('RGB');compare.paste(crop,(i*320,400));draw.text((i*320+8,370),'Cloth detail, 1:1 pixels',fill='white')
compare.save(root/'Comparison.jpg')
(root/'Validation.json').write_text(json.dumps(report,indent=2));print(json.dumps(report,indent=2))

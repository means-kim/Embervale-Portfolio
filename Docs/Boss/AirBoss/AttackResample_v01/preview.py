from pathlib import Path
from PIL import Image, ImageDraw
p=Path(__file__).resolve().parent
out=Image.new('RGB',(1200,1000),(25,32,43));draw=ImageDraw.Draw(out)
draw.text((20,12),'BEFORE (same display size)',fill='white');draw.text((620,12),'AFTER (same display size)',fill='white')
jobs=[('Attack_Lightning_VerticalGrowth_AirBoss_32F_v01.png',(1536,768,1920,1536),2,(190,610),0,'Large lightning: Scale 50 x 80'),('Attack_IcePush_IceMass_32F_v01.png',(768,256,1024,512),4,(300,300),640,'Ice mass: Scale 50 x 50')]
for name,box,factor,size,y,label in jobs:
    draw.text((20,y+40),label,fill='white')
    for side,folder in enumerate(['Original','Prepared']):
        with Image.open(p/folder/name) as src:
            b=box if side==0 else tuple(v*factor for v in box)
            im=src.convert('RGBA').crop(b).convert('RGBa').resize(size,Image.Resampling.BILINEAR).convert('RGBA')
            out.paste(im,(side*600+20,y+65),im)
out.save(p/'Comparison.png')


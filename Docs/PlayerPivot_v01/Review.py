from PIL import Image,ImageDraw,ImageFont
from pathlib import Path
import json
root=Path(__file__).resolve().parents[2]
out=Path(__file__).parent
items=json.loads((out/'Inventory.json').read_text(encoding='utf-8'))
font=ImageFont.truetype('C:/Windows/Fonts/consola.ttf',14)
tiles=[]
for item in items:
    if item['mode']=='Single' and 'Idle_v3' in item['path']:
        if not item['path'].endswith('Idle_00.png'):continue
    im=Image.open(root/item['path']).convert('RGBA')
    for r in item['sprites']:
        x,y,w,h=map(int,[r['x'],r['y'],r['width'],r['height']])
        if item['mode']=='Multiple':
            x=(x//512)*512; y=(y//512)*512; w=h=512
        crop=im.crop((x,im.height-y-h,x+w,im.height-y))
        tile=Image.new('RGB',(220,242),'#202631'); tile.paste(crop.resize((204,204)),(8,8),crop.resize((204,204)).getchannel('A'))
        d=ImageDraw.Draw(tile)
        for a in [128,256,384]:
            d.line((8+a/w*204,8,8+a/w*204,212),fill='#384152')
            d.line((8,8+a/h*204,212,8+a/h*204),fill='#384152')
        d.text((5,215),r['name'],font=font,fill='white')
        tiles.append(tile)
for page in range((len(tiles)+29)//30):
    grid=Image.new('RGB',(1320,1210),'#202631')
    for i,t in enumerate(tiles[page*30:(page+1)*30]):grid.paste(t,((i%6)*220,(i//6)*242))
    grid.save(out/f'Contact_{page+1:02}.png')
print('tiles',len(tiles),'pages',(len(tiles)+29)//30)
print([(i['path'],len(i['sprites'])) for i in items if i['mode']=='Multiple'])

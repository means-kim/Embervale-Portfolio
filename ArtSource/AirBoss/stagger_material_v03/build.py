"""Normalize whole canvases, pack reviewed drawn frames; no procedural poses."""
from pathlib import Path
from PIL import Image, ImageDraw
import hashlib
import json
import re
import shutil
import sys
import uuid
import numpy as np

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
CELL = 1536
COUNTS = {"Enter": 12, "Hold": 24, "Recover": 16}
master = Image.open(HERE / "reference/Idle_Current.png").convert("RGBA")

def resize_canvas(image, size):
    return image.convert("RGBa").resize((size, size), Image.Resampling.LANCZOS).convert("RGBA")

frames = {}
report = []
for phase, count in COUNTS.items():
    frames[phase] = []
    for i in range(count):
        endpoint = (phase == "Enter" and i == 0) or (phase == "Recover" and i == 15)
        source_name = f"{phase}_{i:02}.png"
        if (phase == "Enter" and i == 11) or (phase == "Recover" and i == 0):
            source_name = "Hold_00.png"
        source = HERE / "source" / source_name
        if endpoint:
            im = master.copy()
            original_size = master.size
            removed_noise = 0
        else:
            with Image.open(source) as generated:
                assert generated.width == generated.height
                original_size = generated.size
                rgba = np.array(generated.convert("RGBA"))
                alpha = rgba[:, :, 3]
                # Inspected distant alpha=1 generation noise only. Preserve all
                # partial alpha within a 24px buffer around all alpha>=2 pixels.
                bounds = Image.fromarray(np.uint8(alpha >= 2) * 255).getbbox()
                outside = np.ones(alpha.shape, dtype=bool)
                outside[max(0,bounds[1]-24):min(alpha.shape[0],bounds[3]+24),
                        max(0,bounds[0]-24):min(alpha.shape[1],bounds[2]+24)] = False
                assert int(alpha[outside].max(initial=0)) <= 1
                removed_noise = int(np.count_nonzero(alpha[outside]))
                rgba[outside] = 0
                im = resize_canvas(Image.fromarray(rgba), CELL)
        bbox = im.getchannel("A").getbbox()
        margins = [bbox[0], bbox[1], CELL - bbox[2], CELL - bbox[3]]
        item = dict(phase=phase, frame=i, source="reference/Idle_Current.png" if endpoint else "source/"+source_name,
                    source_size=original_size, cell_size=im.size, alpha_bounds=bbox, margins=margins,
                    safe_margin_pass=min(margins)>=96, distant_alpha1_noise_removed=removed_noise,
                    rgba_sha256=hashlib.sha256(im.tobytes()).hexdigest())
        report.append(item)
        im.save(HERE / "frames" / f"{phase}_{i:02}.png")
        frames[phase].append(im)
    contact = Image.new("RGB", (4*256, ((count+3)//4)*280), (25, 30, 42))
    draw = ImageDraw.Draw(contact)
    for i, im in enumerate(frames[phase]):
        small = resize_canvas(im, 256)
        x, y = (i%4)*256, (i//4)*280
        contact.paste(small, (x,y), small)
        draw.text((x+10,y+258),f"{phase} {i:02}",fill=(220,235,255))
    contact.save(HERE / f"Contact_{phase}.jpg",quality=93)
(HERE / "validation.json").write_text(json.dumps(report,indent=2)+"\n")
print("Frames:",len(report),"Safety failures:",[(x['phase'],x['frame'],x['margins']) for x in report if not x['safe_margin_pass']])
assert frames["Enter"][0].tobytes() == frames["Recover"][-1].tobytes() == master.tobytes()
assert frames["Enter"][-1].tobytes() == frames["Hold"][0].tobytes() == frames["Recover"][0].tobytes()
for phase in frames:
    assert len({hashlib.sha256(im.tobytes()).hexdigest() for im in frames[phase]}) == COUNTS[phase]

if "--pack" not in sys.argv:
    sys.exit(0)
assert all(x["safe_margin_pass"] for x in report), "Do not publish unsafe cells"
template = (ROOT / "Assets/04.Sprites/AirBoss/AirBoss_Idle_Float_4x4_1024_v02.png.meta").read_text()
prefix = template.split("  spriteSheet:")[0]
prefix = re.sub(r"  internalIDToNameTable:.*?  externalObjects:", "  internalIDToNameTable: []\n  externalObjects:", prefix, flags=re.S)
prefix = re.sub(r"maxTextureSize: \d+", "maxTextureSize: 8192", prefix)
prefix = re.sub(r"textureCompression: \d+", "textureCompression: 0", prefix)
start = template.index("    - serializedVersion:",template.index("  spriteSheet:"))
end = template.index("    - serializedVersion:",start+8)
entry_template = template[start:end]
tail = template[template.index("\n    outline:",end):]
tail = tail[:tail.index("    nameFileIdTable:")] + "    nameFileIdTable:\n"
assets = ROOT / "Assets/04.Sprites/AirBoss"
manifest = []
for phase, offset, count, rows, part in [("Enter",0,12,3,""),("Hold",0,16,4,"_Part1"),("Hold",16,8,2,"_Part2"),("Recover",0,16,4,"")]:
    filename = f"AirBoss_Stagger_{phase}{part}_4x{rows}_1536_v03.png"
    sheet = Image.new("RGBA",(CELL*4,CELL*rows))
    entries, names, sprites = [], [], []
    for j in range(count):
        i = offset+j
        x, yt, y = (j%4)*CELL, (j//4)*CELL, (rows-1-j//4)*CELL
        sheet.paste(frames[phase][i],(x,yt))
        assert sheet.crop((x,yt,x+CELL,yt+CELL)).tobytes() == frames[phase][i].tobytes()
        name = f"AirBoss_Stagger_{phase}_v03_{i:02}"
        sid = uuid.uuid5(uuid.NAMESPACE_URL,"2d-team08/"+name).hex
        fid = int(sid[:15],16)
        entry = re.sub(r"      name: .*",f"      name: {name}",entry_template)
        for key,value in [("x",x),("y",y),("width",CELL),("height",CELL)]:
            entry = re.sub(rf"        {key}: .*",f"        {key}: {value}",entry)
        entry = re.sub(r"      spriteID: .*",f"      spriteID: {sid}",entry)
        entry = re.sub(r"      internalID: .*",f"      internalID: {fid}",entry)
        entries.append(entry)
        names.append(f"      {name}: {fid}\n")
        sprites.append(dict(name=name,local_id=fid,index=i,rect=[x,y,CELL,CELL]))
    guid = uuid.uuid5(uuid.NAMESPACE_URL,"2d-team08/"+filename).hex
    p = re.sub(r"^guid: .*",f"guid: {guid}",prefix,flags=re.M)
    meta = p+"  spriteSheet:\n    serializedVersion: 2\n    sprites:\n"+"".join(entries)+tail+"".join(names)+"  mipmapLimitGroupName: \n  pSDRemoveMatte: 0\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n"
    sheet.save(HERE / "sheets" / filename)
    (HERE / "sheets" / (filename+".meta")).write_text(meta)
    shutil.copy2(HERE / "sheets" / filename, assets / filename)
    target_meta = assets / (filename+".meta")
    if not target_meta.exists():
        target_meta.write_text(meta)
    manifest.append(dict(phase=phase,path=str((assets/filename).relative_to(ROOT)),guid=guid,size=sheet.size,sprites=sprites))
(HERE/"sheets.json").write_text(json.dumps(manifest,indent=2)+"\n")
print("Packed 4 sheets; 52 sprites; exact sheet round-trip and endpoint pixels verified.")

(HERE/"preview").mkdir(exist_ok=True)
for phase, ims in frames.items():
    sheet=Image.new("RGBA",(384*4,384*((len(ims)+3)//4)))
    for i, im in enumerate(ims):
        sheet.paste(resize_canvas(im,384),((i%4)*384,(i//4)*384))
    sheet.save(HERE/"preview"/(phase+".png"))

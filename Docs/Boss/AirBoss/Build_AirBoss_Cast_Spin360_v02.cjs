const fs=require('fs'),path=require('path'),crypto=require('crypto'),sharp=require('sharp');
const root=path.resolve(__dirname,'../../..'),art=path.join(root,'ArtSource/AirBoss/cast_spin360'),name='AirBoss_Cast_Spin360';
const hash=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
for(const p of ['source/v02','frames/v02','sheets'])fs.mkdirSync(path.join(art,p),{recursive:true});
function cleanFaintNoise(d){
 const w=1024,n=w*w,seen=new Uint8Array(n),q=new Int32Array(n);let removed=0;
 for(let p=0;p<n;p++){if(seen[p]||!d[p*4+3])continue;let start=0,end=1,peak=0;q[0]=p;seen[p]=1;
 while(start<end){let a=q[start++],x=a%w,y=Math.floor(a/w);peak=Math.max(peak,d[a*4+3]);for(let dy=-1;dy<=1;dy++)for(let dx=-1;dx<=1;dx++){let xx=x+dx,yy=y+dy;if(xx<0||xx>=w||yy<0||yy>=w)continue;let b=yy*w+xx;if(!seen[b]&&d[b*4+3]){seen[b]=1;q[end++]=b;}}}
 if(peak>=8||end>512)continue;let near=false;
 for(let j=0;j<end&&!near;j++){let x=q[j]%w,y=Math.floor(q[j]/w);for(let yy=Math.max(0,y-16);yy<=Math.min(1023,y+16)&&!near;yy++)for(let xx=Math.max(0,x-16);xx<=Math.min(1023,x+16);xx++)if(d[(yy*w+xx)*4+3]>=8){near=true;break;}}
 if(!near)for(let j=0;j<end;j++){d.fill(0,q[j]*4,q[j]*4+4);removed++;}
 }return removed;
}
async function main(){
 if(process.argv.includes('--snapshot')){
 const roots=['Assets/01.Scenes','Assets/02.Scripts','Assets/03.Prefabs','Assets/05.Animation','Assets/04.Sprites/Boss','Assets/04.Sprites/AirBoss','ProjectSettings','ArtSource/AirBoss/idle'];
 const files=[]; function walk(p){if(!fs.existsSync(p))return;for(const e of fs.readdirSync(p,{withFileTypes:true})){const f=path.join(p,e.name);if(e.isDirectory())walk(f);else files.push({path:path.relative(root,f).replaceAll('\\','/'),sha256:hash(f)});}}
 roots.forEach(p=>walk(path.join(root,p)));fs.writeFileSync(path.join(__dirname,name+'_v02.protected_before.json'),JSON.stringify(files,null,2));console.log('Protected files',files.length);return;
 }
 if(process.argv.includes('--verify-protected')){const list=JSON.parse(fs.readFileSync(path.join(__dirname,name+'_v02.protected_before.json')));const changed=list.filter(f=>!fs.existsSync(path.join(root,f.path))||hash(path.join(root,f.path))!==f.sha256);const authorizedDoc='ArtSource/AirBoss/idle/Master Crafting Guide for Aerial Bosses (Idle) v1.md';const report={checked:list.length,expectedDocChanges:changed.filter(f=>f.path===authorizedDoc),changed:changed.filter(f=>f.path!==authorizedDoc)};fs.writeFileSync(path.join(__dirname,name+'_v02.protected_after.json'),JSON.stringify(report,null,2));console.log(report);return;}
 const frames=[];
 for(let i=0;i<24;i++){
 const id=String(i).padStart(2,'0'),src=i?path.join(art,'source/v02','frame'+id+'_generated.png'):path.join(root,'ArtSource/AirBoss/idle/AirBoss_Idle_Master_image2.png');if(!fs.existsSync(src))continue;
 const dst=path.join(art,'frames/v02',name+'_v02_'+id+'.png'),m=await sharp(src).metadata();
 let faintNoisePixelsRemoved=0;
 if(i===0)fs.copyFileSync(src,dst);else{if(m.width!==m.height)throw Error('Non-square '+src);const raw=await sharp(src).ensureAlpha().resize(1024,1024,{kernel:'lanczos3'}).raw().toBuffer();faintNoisePixelsRemoved=cleanFaintNoise(raw);await sharp(raw,{raw:{width:1024,height:1024,channels:4}}).png().toFile(dst);}
 const d=await sharp(dst).ensureAlpha().raw().toBuffer();let x0=1024,y0=1024,x1=-1,y1=-1,outside=0,alphaPixels=0,alphaSum=0;
 for(let y=0;y<1024;y++)for(let x=0;x<1024;x++){const a=d[(y*1024+x)*4+3];if(a){alphaPixels++;alphaSum+=a;x0=Math.min(x0,x);y0=Math.min(y0,y);x1=Math.max(x1,x);y1=Math.max(y1,y);if(x<64||x>959||y<64||y>959)outside++;}}
 frames.push({frame:i,path:path.relative(root,dst).replaceAll('\\','/'),sourceSize:[m.width,m.height],faintNoisePixelsRemoved,width:1024,height:1024,alphaBounds:[x0,y0,x1,y1],margins:[x0,y0,1023-x1,1023-y1],outside64:outside,alphaPixels,alphaSum,sha256:hash(dst)});
 }
 fs.writeFileSync(path.join(__dirname,name+'_v02.metrics.json'),JSON.stringify(frames,null,2));
 console.log(JSON.stringify(frames.map(f=>({frame:f.frame,margins:f.margins,outside:f.outside64,alpha:f.alphaPixels}))));
 for(let s=0;s<2;s++)for(const [bg,suffix,fg] of [['#202936','dark','#eef4ff'],['#e1e7ed','light','#17222f']]){
 const layers=[];for(const f of frames.filter(f=>Math.floor(f.frame/16)===s)){const j=f.frame%16,x=j%4*320,y=Math.floor(j/4)*348;layers.push({input:await sharp(path.join(root,f.path)).resize(320,320).toBuffer(),left:x,top:y+28},{input:Buffer.from('<svg width="320" height="28"><text x="12" y="20" fill="'+fg+'" font-family="sans-serif" font-size="16">'+String(f.frame).padStart(2,'0')+'</text></svg>'),left:x,top:y});}
 await sharp({create:{width:1280,height:s===0?1392:696,channels:4,background:bg}}).composite(layers).png().toFile(path.join(__dirname,name+'_v02_part'+(s+1)+'_contact_'+suffix+'.png'));
 }
 if(!process.argv.includes('--final'))return;
 if(frames.length!==24||frames.some(f=>f.outside64))throw Error('Incomplete/unsafe frames');
 if(new Set(frames.map(f=>f.sha256)).size!==24)throw Error('Duplicate frames');
 const sheets=[];
 for(let s=0;s<2;s++){
 const rows=s===0?4:2, height=rows*1024, atlas=Buffer.alloc(4096*height*4);
 for(const f of frames.slice(s*16,s*16+16)){const j=f.frame%16,d=await sharp(path.join(root,f.path)).raw().toBuffer();for(let y=0;y<1024;y++)d.copy(atlas,((Math.floor(j/4)*1024+y)*4096+j%4*1024)*4,y*4096,(y+1)*4096);}
 const sheet=path.join(art,'sheets',name+'_Part'+(s+1)+'_4x'+rows+'_1024_v02.png');await sharp(atlas,{raw:{width:4096,height,channels:4}}).png().toFile(sheet);
 for(const f of frames.slice(s*16,s*16+16)){const j=f.frame%16,d=await sharp(sheet).extract({left:j%4*1024,top:Math.floor(j/4)*1024,width:1024,height:1024}).raw().toBuffer();if(!d.equals(await sharp(path.join(root,f.path)).raw().toBuffer()))throw Error('Sheet mismatch '+f.frame);}
 const dst=path.join(root,'Assets/04.Sprites/AirBoss',path.basename(sheet));
 if(!fs.existsSync(dst+'.meta')){
 const uuid=()=>crypto.randomUUID().replaceAll('-','');
 let meta=fs.readFileSync(path.join(root,'Assets/04.Sprites/AirBoss/AirBoss_Cast_Spin360_4x4_1024_v01.png.meta'),'utf8');
 meta=meta.replaceAll('\r\n','\n');
 const section=meta.match(/    sprites:\n([\s\S]*?)(?=^    outline:)/m);
 if(!section)throw Error('Missing source sprite metadata');
 const blocks=section[1].match(/    - serializedVersion: 2\n[\s\S]*?(?=    - serializedVersion: 2\n|$)/g);
 if(!blocks||blocks.length!==16)throw Error('Unexpected source slice count');
 const slices=blocks.slice(0,rows*4).map((block,j)=>block.replace(/name: .+/, 'name: '+name+'_v02_'+String(s*16+j).padStart(2,'0')).replace(/        y: \d+/, '        y: '+(rows-1-Math.floor(j/4))*1024));
 meta=meta.replace(section[0],'    sprites:\n'+slices.join('')).replace(/^guid: .+$/m,'guid: '+uuid()).replace(/  internalIDToNameTable:[\s\S]*?(?=  externalObjects:)/,'  internalIDToNameTable: []\n').replace(/spriteID: [0-9a-f]+/g,()=> 'spriteID: '+uuid()).replace(/maxTextureSize: \d+/g,'maxTextureSize: 4096').replace(/textureCompression: \d+/g,'textureCompression: 0');
 meta=meta.replace(/    nameFileIdTable:[\s\S]*?(?=  mipmapLimitGroupName:|  pSDRemoveMatte:|$)/,'    nameFileIdTable: {}\n');
 fs.writeFileSync(dst+'.meta',meta);}
 fs.copyFileSync(sheet,dst);sheets.push({path:path.relative(root,dst),sha256:hash(dst),cellsMatch:true});
 }fs.writeFileSync(path.join(__dirname,name+'_v02.sheets.json'),JSON.stringify(sheets,null,2));console.log(sheets);
}
main().catch(e=>{console.error(e);process.exit(1)});



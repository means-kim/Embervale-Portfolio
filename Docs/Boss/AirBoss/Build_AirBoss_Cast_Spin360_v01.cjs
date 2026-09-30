// Run with Node.js and sharp available through NODE_PATH.
// Full-canvas normalization only. No trim, recenter, per-pose fit or alpha thresholding.
const fs = require('fs'), path = require('path'), crypto = require('crypto'), sharp = require('sharp');
const root = path.resolve(__dirname, '../../..');
const art = path.join(root, 'ArtSource/AirBoss/cast_spin360');
const name = 'AirBoss_Cast_Spin360';
const sha = p => crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
function cleanDetachedFaint(d,w,h){
  // Remove only entire disconnected alpha 1..7 components more than 16px
  // from alpha>=8 content. Connected semitransparent cloth/glow is untouched.
  const stride=w+1,integral=new Uint32Array((w+1)*(h+1));
  for(let y=0;y<h;y++){let sum=0;for(let x=0;x<w;x++){sum+=d[(y*w+x)*4+3]>=8?1:0;integral[(y+1)*stride+x+1]=integral[y*stride+x+1]+sum;}}
  const near=(x,y)=>{let x0=Math.max(0,x-16),y0=Math.max(0,y-16),x1=Math.min(w,x+17),y1=Math.min(h,y+17);return integral[y1*stride+x1]-integral[y0*stride+x1]-integral[y1*stride+x0]+integral[y0*stride+x0]>0;};
  const seen=new Uint8Array(w*h),queue=new Int32Array(w*h);let removed=0;
  for(let p=0;p<w*h;p++){if(seen[p]||!d[p*4+3])continue;let start=0,end=1,keep=false;queue[0]=p;seen[p]=1;
    while(start<end){const q=queue[start++],x=q%w,y=Math.floor(q/w);if(d[q*4+3]>=8||near(x,y))keep=true;
      for(let dy=-1;dy<=1;dy++)for(let dx=-1;dx<=1;dx++){const nx=x+dx,ny=y+dy;if(nx<0||ny<0||nx>=w||ny>=h)continue;const n=ny*w+nx;if(!seen[n]&&d[n*4+3]){seen[n]=1;queue[end++]=n;}}}
    if(!keep)for(let j=0;j<end;j++){d.fill(0,queue[j]*4,queue[j]*4+4);removed++;}
  }return removed;
}
async function stats(file) {
  const {data,info} = await sharp(file).ensureAlpha().raw().toBuffer({resolveWithObject:true});
  let minX=info.width,minY=info.height,maxX=-1,maxY=-1,outside=0;
  for(let y=0;y<info.height;y++) for(let x=0;x<info.width;x++) if(data[(y*info.width+x)*4+3]>0) {
    minX=Math.min(minX,x);minY=Math.min(minY,y);maxX=Math.max(maxX,x);maxY=Math.max(maxY,y);
    if(x<64||y<64||x>=info.width-64||y>=info.height-64)outside++;
  }
  return {width:info.width,height:info.height,alphaBounds:[minX,minY,maxX,maxY],margins:[minX,minY,info.width-1-maxX,info.height-1-maxY],outside64:outside,sha256:sha(file)};
}
async function main(){
  const frames=[];
  for(let i=0;i<16;i++){
    const id=String(i).padStart(2,'0'),src=i===0?path.join(root,'ArtSource/AirBoss/idle/AirBoss_Idle_Master_image2.png'):path.join(art,'source/v01',`frame${id}_generated.png`);
    if(!fs.existsSync(src)) continue;
    const dst=path.join(art,'frames/v01',`${name}_v01_${id}.png`),m=await sharp(src).metadata();
    let detachedFaintPixelsRemoved=0;
    if(i===0)fs.copyFileSync(src,dst);else {
      if(m.width!==m.height)throw Error('Non-square canvas '+src);
      const data=await sharp(src).ensureAlpha().resize(1024,1024,{kernel:'lanczos3'}).raw().toBuffer();
      detachedFaintPixelsRemoved=cleanDetachedFaint(data,1024,1024);
      await sharp(data,{raw:{width:1024,height:1024,channels:4}}).png().toFile(dst);
    }
    frames.push({frame:i,sourceSize:[m.width,m.height],detachedFaintPixelsRemoved,path:path.relative(root,dst),...await stats(dst)});
  }
  fs.writeFileSync(path.join(__dirname,`${name}_v01.metrics.json`),JSON.stringify(frames,null,2));
  console.log(JSON.stringify(frames.map(f=>({frame:f.frame,sourceSize:f.sourceSize,margins:f.margins,outside64:f.outside64})),null,2));
  for(const [bg,suffix,fg] of [['#202936','dark','#eef4ff'],['#e1e7ed','light','#17222f']]){
    const layers=[];
    for(const f of frames){const x=(f.frame%4)*384,y=Math.floor(f.frame/4)*412;layers.push({input:await sharp(path.join(root,f.path)).resize(384,384).toBuffer(),left:x,top:y+28});layers.push({input:Buffer.from(`<svg width="384" height="28"><text x="12" y="20" fill="${fg}" font-family="sans-serif" font-size="16">${String(f.frame).padStart(2,'0')}</text></svg>`),left:x,top:y});}
    await sharp({create:{width:1536,height:1648,channels:4,background:bg}}).composite(layers).png().toFile(path.join(__dirname,`${name}_v01_contact_${suffix}.png`));
  }
  if(process.argv.includes('--final')){
    if(frames.length!==16||frames.some(f=>f.outside64>0))throw Error('Incomplete or unsafe frames');
    if(new Set(frames.map(f=>f.sha256)).size!==16)throw Error('Duplicate frame');
    const atlas=Buffer.alloc(4096*4096*4);
    for(const f of frames){const d=await sharp(path.join(root,f.path)).ensureAlpha().raw().toBuffer();for(let y=0;y<1024;y++)d.copy(atlas,((Math.floor(f.frame/4)*1024+y)*4096+(f.frame%4)*1024)*4,y*1024*4,(y+1)*1024*4);}
    const sheet=path.join(art,'sheets',`${name}_4x4_1024_v01.png`);
    await sharp(atlas,{raw:{width:4096,height:4096,channels:4}}).png().toFile(sheet);
    for(const f of frames){const cell=await sharp(sheet).extract({left:(f.frame%4)*1024,top:Math.floor(f.frame/4)*1024,width:1024,height:1024}).raw().toBuffer(),expected=await sharp(path.join(root,f.path)).ensureAlpha().raw().toBuffer();if(!cell.equals(expected))throw Error('Atlas mismatch '+f.frame);}
    const unityPath=path.join(root,'Assets/04.Sprites/AirBoss',path.basename(sheet));
    if(!fs.existsSync(unityPath+'.meta')){
      let meta=fs.readFileSync(path.join(root,'Assets/04.Sprites/AirBoss/AirBoss_Move_Bank_Left_4x4_1024_v01.png.meta'),'utf8');
      const uuid=()=>crypto.randomUUID().replaceAll('-','');
      meta=meta.replace(/^guid: .+$/m,'guid: '+uuid())
        .replace(/  internalIDToNameTable:[\s\S]*?(?=  externalObjects:)/,'  internalIDToNameTable: []\n')
        .replaceAll('AirBoss_Move_Bank_Left',name)
        .replace(/spriteID: [0-9a-f]+/g,()=> 'spriteID: '+uuid())
        .replace(/maxTextureSize: \d+/g,'maxTextureSize: 4096')
        .replace(/textureCompression: \d+/g,'textureCompression: 0');
      fs.writeFileSync(unityPath+'.meta',meta);
    }
    fs.copyFileSync(sheet,unityPath);
    console.log('Sheet verified:',sha(sheet));
  }
}
main().catch(e=>{console.error(e);process.exit(1)});

const fs=require('fs'),path=require('path'),sharp=require('sharp'),crypto=require('crypto');
const ROOT=path.resolve(__dirname,'..');
async function main(){
 const manifest=JSON.parse(fs.readFileSync(path.join(ROOT,'Motions.json')));
 const findings=[],clips=[];let total=0,minPadding=512;
 for(const c of manifest.clips){
  if(c.frameCount!==c.frames.length||c.times.length!==c.frameCount||c.times[0]!==0||c.times.some((t,i)=>i>0&&t<=c.times[i-1])||c.duration<=c.times.at(-1))throw Error('Invalid timing '+c.name);
  const {data:sheet,info:si}=await sharp(path.join(ROOT,c.sheet.file)).ensureAlpha().raw().toBuffer({resolveWithObject:true});
  if(si.width!==c.sheet.width||si.height!==c.sheet.height||si.width%512||si.height%512)throw Error('Invalid sheet dimensions '+c.name);
  const hashes=[],bounds=[];
  for(let i=0;i<c.frameCount;i++){
   const {data,info}=await sharp(path.join(ROOT,c.frames[i])).ensureAlpha().raw().toBuffer({resolveWithObject:true});
   if(info.width!==512||info.height!==512||info.channels!==4)throw Error('Invalid sprite '+c.frames[i]);
   let x0=512,y0=512,x1=-1,y1=-1;
   for(let y=0;y<512;y++){
    const offset=((Math.floor(i/c.sheet.columns)*512+y)*si.width+(i%c.sheet.columns)*512)*4;
    if(!data.subarray(y*512*4,(y+1)*512*4).equals(sheet.subarray(offset,offset+512*4)))throw Error('Sheet mismatch '+c.name+i);
    for(let x=0;x<512;x++)if(data[(y*512+x)*4+3]>16){x0=Math.min(x0,x);y0=Math.min(y0,y);x1=Math.max(x1,x);y1=Math.max(y1,y)}
   }
   const pad=Math.min(x0,y0,511-x1,511-y1);minPadding=Math.min(minPadding,pad);if(pad<8)throw Error('Sprite too close to cell edge '+c.name+i);
   hashes.push(crypto.createHash('sha256').update(data).digest('hex'));bounds.push([x0,y0,x1,y1]);total++;
  }
  for(let i=c.frameCount;i<c.sheet.columns*c.sheet.rows;i++)for(let y=0;y<512;y++){
   const offset=((Math.floor(i/c.sheet.columns)*512+y)*si.width+(i%c.sheet.columns)*512)*4;
   for(let x=0;x<512;x++)if(sheet[offset+x*4+3]!==0)throw Error('Nonempty unused cell '+c.name);
  }
  clips.push({name:c.name,count:c.frameCount,uniqueRenders:new Set(hashes).size,duration:c.duration,loop:c.loop,bounds});
 }
 if(total!==101||manifest.clips.length!==16)throw Error('Unexpected clip/frame count');
 const result={passed:true,clipCount:16,totalFrames:total,cell:[512,512],pivot:manifest.pivot,ppu:manifest.pixelsPerUnit,atlasCellsByteIdentical:true,unusedCellsTransparent:true,minimumMeaningfulAlphaPaddingPx:minPadding,timingMonotonic:true,visualDirectionReview:'All selected upright action keyposes face right. Full somersault rotates the same drawing through 360 degrees. Mirrored run source poses excluded.',unityEditorExecuted:false,gameplayTransitionsExecuted:false,notes:['Sprite animation only: root motion, physical displacement, hitboxes and invulnerability are owned by the game.','Run GIF samples at 48 fps; overview and other GIFs at 20 fps. Authoritative clip timing is Motions.json.','A consistent head-area calibration corrects generated key-art scale; pose height is intentionally not normalized.'],clips};
 fs.writeFileSync(path.join(ROOT,'Verification.json'),JSON.stringify(result,null,2));
 console.log(JSON.stringify({passed:true,clipCount:16,frames:total,minPadding,atlasCellsByteIdentical:true}));
}
main().catch(e=>{console.error(e);process.exit(1)});

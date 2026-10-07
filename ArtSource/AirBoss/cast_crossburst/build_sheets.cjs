// Technical canvas normalization and packing only; all new poses are drawn by ImageGen.
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const sharp=require('sharp');
const here=__dirname,root=path.resolve(here,'../../..');
const CELL=1152,INNER=1024,PAD=64,NAME='AirBoss_Cast_CrossBurst';
const plan=JSON.parse(fs.readFileSync(path.join(here,'frame_plan_v01.json'),'utf8'));
const sha=b=>crypto.createHash('sha256').update(b).digest('hex');
function bounds(data,w,h,threshold=1){let x0=w,y0=h,x1=-1,y1=-1,count=0;for(let y=0;y<h;y++)for(let x=0;x<w;x++){if(data[(y*w+x)*4+3]>=threshold){x0=Math.min(x0,x);y0=Math.min(y0,y);x1=Math.max(x1,x);y1=Math.max(y1,y);count++;}}return count?{x0,y0,x1,y1,count}:null;}
async function main(){
 const available=[];for(let i=0;i<40;i++){
  const spec=plan.schedule[i];let source;
  if(spec.kind==='idle') source=path.join(root,'ArtSource/AirBoss/idle/frames/v02',`AirBoss_Idle_Float_v02_${String(spec.idle_frame).padStart(2,'0')}.png`);
  else {source=path.join(here,'source/v01',`frame${String(i).padStart(2,'0')}_revised.png`);if(!fs.existsSync(source))source=source.replace('_revised','_generated');}
  if(!fs.existsSync(source)){if(process.argv.includes('--partial'))continue;throw Error('Missing frame '+i);}
  const original=await sharp(source).metadata();
  if(original.width!==original.height)throw Error('Unexpected nonsquare canvas '+i);
  let pipe=sharp(source).ensureAlpha();
  // Same whole-canvas normalization for every generated frame, never silhouette fitting.
  if(original.width!==INNER)pipe=pipe.resize(INNER,INNER,{kernel:'lanczos3'});
  const inner=await pipe.raw().toBuffer();const cell=Buffer.alloc(CELL*CELL*4);
  for(let y=0;y<INNER;y++)inner.copy(cell,((y+PAD)*CELL+PAD)*4,y*INNER*4,(y+1)*INNER*4);
  const b=bounds(cell,CELL,CELL),v=bounds(cell,CELL,CELL,8);
  const margin=Math.min(b.x0,b.y0,CELL-1-b.x1,CELL-1-b.y1);
  if(margin<64)throw Error('64px margin failed '+i);
  let idleExact=null;
  if(spec.kind==='idle'){const originalPixels=await sharp(source).ensureAlpha().raw().toBuffer();idleExact=originalPixels.equals(inner);if(!idleExact)throw Error('Idle pixels changed '+i);}
  const framePath=path.join(here,'frames/v01',`${NAME}_v01_${String(i).padStart(2,'0')}.png`);
  await sharp(cell,{raw:{width:CELL,height:CELL,channels:4}}).png().toFile(framePath);
  available.push({frame:i,source:path.relative(root,source).replaceAll('\\','/'),sourceDimensions:[original.width,original.height],framePath,cell,inner,allAlphaBounds:b,visibleBounds:v,margin,idleExact,sha256:sha(cell),innerSHA256:sha(inner)});
 }
 const records=available.map(({cell,inner,framePath,...r})=>({...r,framePath:path.relative(root,framePath).replaceAll('\\','/')}));
 fs.writeFileSync(path.join(here,'validation_v01.json'),JSON.stringify({cell:CELL,inner:INNER,padding:PAD,frames:records},null,2));
 // Dark and light contact sheets are review renders, not game assets.
 const tile=256;let composites=[];
 for(const f of available){const thumb=await sharp(f.cell,{raw:{width:CELL,height:CELL,channels:4}}).resize(tile,tile).png().toBuffer();composites.push({input:thumb,left:(f.frame%8)*tile,top:Math.floor(f.frame/8)*tile});}
 for(const [label,bg] of [['dark','#171f31'],['light','#cfdae4']]){
  const base=await sharp({create:{width:tile*8,height:tile*5,channels:4,background:bg}}).composite(composites).png().toBuffer();
  await sharp(base).jpeg({quality:92}).toFile(path.join(here,'review',`contact_${label}.jpg`));
 }
 if(process.argv.includes('--partial')){console.log(JSON.stringify({partial:true,count:available.length,frames:available.map(f=>f.frame)}));return;}
 if(available.length!==40||new Set(records.map(r=>r.sha256)).size!==40)throw Error('Missing or duplicate cells');
 const sheets=[];
 for(const [part,start,count,rows] of [[1,0,16,4],[2,16,16,4],[3,32,8,2]]){
  const width=CELL*4,height=CELL*rows,buf=Buffer.alloc(width*height*4);
  for(let j=0;j<count;j++){const src=available[start+j].cell,x=(j%4)*CELL,y0=Math.floor(j/4)*CELL;for(let y=0;y<CELL;y++)src.copy(buf,((y+y0)*width+x)*4,y*CELL*4,(y+1)*CELL*4);}
  const filename=`${NAME}_Part${part}_4x${rows}_${CELL}_v01.png`,out=path.join(here,'sheets',filename);
  await sharp(buf,{raw:{width,height,channels:4}}).png().toFile(out);
  const decoded=await sharp(out).ensureAlpha().raw().toBuffer();if(!decoded.equals(buf))throw Error('Sheet encoding differs');
  for(let j=0;j<count;j++){const src=available[start+j].cell,x=(j%4)*CELL,y0=Math.floor(j/4)*CELL;for(let y=0;y<CELL;y++){if(!src.subarray(y*CELL*4,(y+1)*CELL*4).equals(decoded.subarray(((y+y0)*width+x)*4,((y+y0)*width+x+CELL)*4)))throw Error('Roundtrip mismatch');}}
  sheets.push({part,start,count,rows,width,height,filename,sha256:sha(fs.readFileSync(out)),roundtrip:true});
 }
 fs.writeFileSync(path.join(here,'sheets_v01.json'),JSON.stringify(sheets,null,2));
 console.log(JSON.stringify({count:40,distinct:40,idleExact:records.filter(r=>r.idleExact).length,minMargin:Math.min(...records.map(r=>r.margin)),sheets}));
}
main().catch(e=>{console.error(e);process.exit(1)});

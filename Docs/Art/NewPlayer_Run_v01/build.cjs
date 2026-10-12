const fs=require('fs'),path=require('path'),crypto=require('crypto');
const deps='C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules';
const sharp=require(path.join(deps,'sharp'));
const {createCanvas,loadImage}=require(path.join(deps,'@napi-rs/canvas'));
const project=path.resolve(__dirname,'../../..'),out=path.join(project,'Assets/04.Sprites/NewPlayer/Run_v01');
const old=path.join(project,'Assets/04.Sprites/Player/GA1_Player_Motions_v4/Frames/Run');
const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
function walk(dir){return fs.readdirSync(dir,{withFileTypes:true}).flatMap(e=>e.isDirectory()?walk(path.join(dir,e.name)):[path.join(dir,e.name)]);}
function face(d,w,h){
 const seen=new Uint8Array(w*h),queue=new Int32Array(w*h),parts=[];
 const yes=p=>d[p*4+3]>128&&d[p*4]>220&&d[p*4+1]>175&&d[p*4+2]>105;
 for(let p=0;p<w*h;p++)if(!seen[p]&&yes(p)){
  let a=0,b=1,sx=0,sy=0;seen[p]=1;queue[0]=p;
  while(a<b){const v=queue[a++],x=v%w,y=Math.floor(v/w);sx+=x;sy+=y;
   for(let yy=Math.max(0,y-1);yy<=Math.min(h-1,y+1);yy++)for(let xx=Math.max(0,x-1);xx<=Math.min(w-1,x+1);xx++){const q=yy*w+xx;if(!seen[q]&&yes(q)){seen[q]=1;queue[b++]=q;}}
  }
  if(b>500)parts.push({area:b,x:sx/b,y:sy/b});
 }return parts.sort((a,b)=>b.area-a.area)[0];
}
// Sprite separation: select the dominant connected alpha silhouette in each
// sheet cell, excluding neighboring-row fragments and detached source speckles.
// Preserve RGB and the original soft alpha on that silhouette and its 2px edge.
function extractBody(raw,w,h){
 const seen=new Uint8Array(w*h),queue=new Int32Array(w*h);let largest=[];
 for(let p=0;p<w*h;p++)if(!seen[p]&&raw[p*4+3]>16){
  let a=0,b=1;seen[p]=1;queue[0]=p;
  while(a<b){const v=queue[a++],x=v%w,y=Math.floor(v/w);
   for(let yy=Math.max(0,y-1);yy<=Math.min(h-1,y+1);yy++)for(let xx=Math.max(0,x-1);xx<=Math.min(w-1,x+1);xx++){const q=yy*w+xx;if(!seen[q]&&raw[q*4+3]>16){seen[q]=1;queue[b++]=q;}}
  }if(b>largest.length)largest=Array.from(queue.subarray(0,b));
 }
 if(largest.length<1000)throw Error('Dominant sprite silhouette missing');
 const keep=new Uint8Array(w*h);
 for(const v of largest){const x=v%w,y=Math.floor(v/w);for(let yy=Math.max(0,y-2);yy<=Math.min(h-1,y+2);yy++)for(let xx=Math.max(0,x-2);xx<=Math.min(w-1,x+2);xx++)keep[yy*w+xx]=1;}
 const out=Buffer.from(raw);for(let p=0;p<w*h;p++)if(!keep[p])out.fill(0,p*4,p*4+4);
 return out;
}
async function main(){
 for(const p of ['Frames','Source','Sheets'])fs.mkdirSync(path.join(out,p),{recursive:true});
 const baselinePath=path.join(__dirname,'PreservationBaseline.json');
 if(!fs.existsSync(baselinePath)){
  const keep=walk(path.join(project,'Assets/04.Sprites/Player')).concat(walk(path.join(project,'Assets/05.Animation/player')),walk(path.join(project,'Assets/05.Animation/PlayerAttack')));
  fs.writeFileSync(baselinePath,JSON.stringify(keep.map(p=>({path:path.relative(project,p).replaceAll('\\','/'),sha256:hash(fs.readFileSync(p))})),null,2));
  fs.copyFileSync(path.join(project,'Assets/05.Animation/player/Run.anim'),path.join(__dirname,'Run_Original.anim.txt'));
 }
 const sources=process.argv.slice(2);if(!sources.length)throw Error('generated input path missing');
 const sourceMetadata=await Promise.all(sources.map(src=>sharp(src).metadata()));
 sourceMetadata.forEach((m,i)=>{if(!m.hasAlpha)throw Error('Alpha missing');fs.copyFileSync(sources[i],path.join(out,'Source',sources.length===4?`Run_Group_${i+1}_Generated_v01.png`:'Run_Complete_Generated_v01.png'));});
 const plan=JSON.parse(fs.readFileSync(path.join(project,'Docs/PlayerPivot_v01/Plan.json'),'utf8'));
 const originals=[],raws=[],anchors=[];
 for(let i=0;i<8;i++){
  const f=`Run_${String(i).padStart(2,'0')}.png`,p=path.join(old,f),buf=fs.readFileSync(p);
  originals.push(buf);raws.push(await sharp(buf).ensureAlpha().raw().toBuffer());
  const entry=plan.find(e=>e.path.endsWith('/Frames/Run/'+f));anchors.push(entry.changes[0].anchorTopLeft);
 }
 const generated=[],qa=[];
 for(let i=0;i<16;i++){
  const phase=i/2,first=Math.floor(phase),next=(first+1)%8,blend=phase-first;
  const group=sources.length===4?Math.floor(i/4):0,src=sources[group],m=sourceMetadata[group],cols=sources.length===4?2:4,rows=cols,cellIndex=sources.length===4?i%4:i;
  const l=Math.round((cellIndex%cols)*m.width/cols),r=Math.round((cellIndex%cols+1)*m.width/cols),t=Math.round(Math.floor(cellIndex/cols)*m.height/rows),b=Math.round((Math.floor(cellIndex/cols)+1)*m.height/rows);
  const raw=extractBody(await sharp(src).extract({left:l,top:t,width:r-l,height:b-t}).ensureAlpha().raw().toBuffer(),r-l,b-t);
  const cell=await sharp(raw,{raw:{width:r-l,height:b-t,channels:4}}).png().toBuffer();
  const f=face(raw,r-l,b-t),a=face(raws[first],512,512),z=face(raws[next],512,512);
  if(!f||!a||!z)throw Error('Face missing '+i);
  const scale=Math.sqrt((a.area*(1-blend)+z.area*blend)/f.area),tx=a.x*(1-blend)+z.x*blend,ty=a.y*(1-blend)+z.y*blend;
  const canvas=createCanvas(512,512),ctx=canvas.getContext('2d');ctx.translate(tx,ty);ctx.scale(scale,scale);ctx.drawImage(await loadImage(cell),-f.x,-f.y);
  const buf=canvas.toBuffer('image/png');generated.push(buf);
  qa.push({index:i,sourceFace:f,scale,targetFace:[tx,ty]});
 }
 const frames=[];
 for(let i=0;i<8;i++)for(let j=0;j<2;j++){
  const index=i*2+j,buf=generated[index],next=anchors[(i+1)%8];
  const anchor=j?[(anchors[i][0]+next[0])/2,(anchors[i][1]+next[1])/2]:anchors[i];
  const f=`Run_${String(index).padStart(2,'0')}.png`;fs.writeFileSync(path.join(out,'Frames',f),buf);
  frames.push({index,path:'Assets/04.Sprites/NewPlayer/Run_v01/Frames/'+f,time:index*(0.5/16),pivotX:anchor[0]/512,pivotY:1-anchor[1]/512,kind:'new complete-cycle drawing',sha256:hash(buf)});
 }
 const canvas=createCanvas(2048,2048),ctx=canvas.getContext('2d');
 for(const f of frames)ctx.drawImage(await loadImage(fs.readFileSync(path.join(project,f.path))),(f.index%4)*512,Math.floor(f.index/4)*512);
 fs.writeFileSync(path.join(out,'Sheets/Run_Grid512_v01.png'),canvas.toBuffer('image/png'));
 const manifest={version:1,frameSize:[512,512],ppu:200,frameCount:16,cycleSeconds:0.5208333134651184,originalDrawingCount:0,newDrawingCount:16,generationInputSizes:sourceMetadata.map(m=>[m.width,m.height]),frames,calibration:qa,scope:'Run only. Original Player folder preserved. Current torso pivots preserved/interpolated. Mixed old/new drawings rejected for style flicker; complete set redrawn in 4-frame groups to avoid low-resolution source cells.'};
 fs.writeFileSync(path.join(__dirname,'Manifest.json'),JSON.stringify(manifest,null,2));
 console.log(JSON.stringify({frameCount:frames.length,sources:manifest.generationInputSizes,out}));
}
main().catch(e=>{console.error(e);process.exit(1)});

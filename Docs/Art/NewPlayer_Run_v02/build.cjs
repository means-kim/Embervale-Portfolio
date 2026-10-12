const fs=require('fs'),path=require('path'),crypto=require('crypto');
const deps='C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules';
const sharp=require(path.join(deps,'sharp')),{createCanvas,loadImage}=require(path.join(deps,'@napi-rs/canvas'));
const {face,extractBody}=require('./sprite-metrics.cjs');
const root=path.resolve(__dirname,'../../..'),out=path.join(root,'Assets/04.Sprites/NewPlayer/Run_v02');
const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
async function main(){
 for(const dir of ['Frames','Sheets','Source'])fs.mkdirSync(path.join(out,dir),{recursive:true});
 const original=path.join(root,'Assets/04.Sprites/Player/GA1_Player_Motions_v4/Frames/Run/Run_00.png');
 const originalFace=face(await sharp(original).ensureAlpha().raw().toBuffer(),512,512);
 const groups=[];
 for(let g=1;g<=4;g++){const src=path.join(__dirname,`Group_${g}_Generated.png`),m=await sharp(src).metadata();groups.push({src,...m});fs.copyFileSync(src,path.join(out,'Source',`Run_Group_${g}_Generated_v02.png`));}
 for(const i of [3,8])fs.copyFileSync(path.join(__dirname,`Repair_${i}_After.png`),path.join(out,'Source',`Run_${String(i).padStart(2,'0')}_Repaired_v02.png`));
 const frames=[],calibration=[];
 const sheet=createCanvas(2048,2048),sc=sheet.getContext('2d');
 const qa=createCanvas(1600,1600),qc=qa.getContext('2d');qc.fillStyle='#15263b';qc.fillRect(0,0,1600,1600);
 for(let i=0;i<16;i++){
  let cell;
  if(i===3||i===8)cell=await sharp(path.join(__dirname,`Repair_${i}_After.png`)).ensureAlpha().raw().toBuffer({resolveWithObject:true});
  else{const m=groups[Math.floor(i/4)],n=i%4,l=Math.round(n%2*m.width/2),r=Math.round((n%2+1)*m.width/2),t=Math.round(Math.floor(n/2)*m.height/2),b=Math.round((Math.floor(n/2)+1)*m.height/2);cell=await sharp(m.src).extract({left:l,top:t,width:r-l,height:b-t}).ensureAlpha().raw().toBuffer({resolveWithObject:true});}
  const w=cell.info.width,h=cell.info.height,raw=extractBody(cell.data,w,h),f=face(raw,w,h);
  if(!f)throw Error('No face in frame '+i);
  const scale=Math.sqrt(originalFace.area/f.area),bob=3*Math.sin(i/16*4*Math.PI),tx=originalFace.x,ty=originalFace.y+bob;
  const buf=await sharp(raw,{raw:{width:w,height:h,channels:4}}).png().toBuffer();
  const c=createCanvas(512,512),ctx=c.getContext('2d');ctx.translate(tx,ty);ctx.scale(scale,scale);ctx.drawImage(await loadImage(buf),-f.x,-f.y);
  const png=c.toBuffer('image/png'),name=`Run_${String(i).padStart(2,'0')}.png`;fs.writeFileSync(path.join(out,'Frames',name),png);
  const finalRaw=await sharp(png).ensureAlpha().raw().toBuffer();let minX=512,minY=512,maxX=0,maxY=0;
  for(let p=0;p<512*512;p++)if(finalRaw[p*4+3]>16){minX=Math.min(minX,p%512);maxX=Math.max(maxX,p%512);minY=Math.min(minY,Math.floor(p/512));maxY=Math.max(maxY,Math.floor(p/512));}
  if(minX<2||minY<2||maxX>509||maxY>509)throw Error('Clipped frame '+i);
  const image=await loadImage(png);sc.drawImage(image,i%4*512,Math.floor(i/4)*512);
  const x=i%4*400,y=Math.floor(i/4)*400;qc.drawImage(image,x,y,400,400);qc.fillStyle='#d9e5f5';qc.font='18px sans-serif';qc.fillText(String(i).padStart(2,'0'),x+14,y+26);
  frames.push({index:i,path:'Assets/04.Sprites/NewPlayer/Run_v02/Frames/'+name,time:i*.5/16,pivotX:270/512,pivotY:1-286/512,kind:'pose-guided drawing',sha256:hash(png)});
  calibration.push({index:i,sourceSize:[w,h],face:f,scale,targetFace:[tx,ty],alphaBounds:[minX,minY,maxX,maxY]});
 }
 fs.writeFileSync(path.join(out,'Sheets/Run_Grid512_v02.png'),sheet.toBuffer('image/png'));
 fs.writeFileSync(path.join(__dirname,'Run_ContactSheet.png'),qa.toBuffer('image/png'));
 const manifest={version:2,frameCount:16,frameSize:[512,512],ppu:200,cycleSeconds:.5208333134651184,frames,calibration,originalFace,generationInputSizes:groups.map(m=>[m.width,m.height]),scope:'Run only; 16 pose-guided drawings; reciprocal arm swing; original Run_00 face size and torso origin; controlled 3px bob. Original Player and other clips preserved.'};
 fs.writeFileSync(path.join(__dirname,'Manifest.json'),JSON.stringify(manifest,null,2));
 console.log(JSON.stringify({out,frames:frames.length,sourceSizes:manifest.generationInputSizes,bounds:calibration.map(f=>f.alphaBounds)}));
}main().catch(e=>{console.error(e);process.exit(1)});

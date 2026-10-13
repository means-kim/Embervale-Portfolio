const fs=require('fs'),path=require('path'),crypto=require('crypto');
const deps='C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules';
const sharp=require(path.join(deps,'sharp')),{createCanvas,loadImage}=require(path.join(deps,'@napi-rs/canvas'));
const {face,extractBody}=require('../NewPlayer_Run_v02/sprite-metrics.cjs');
const root=path.resolve(__dirname,'../../..'),out=path.join(root,'Assets/04.Sprites/NewPlayer/Idle_v01');
const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
async function main(){
 for(const name of ['Frames','Sheets','Source'])fs.mkdirSync(path.join(out,name),{recursive:true});
 const masterFile=path.join(root,'Assets/04.Sprites/NewPlayer/Design_v01/Player_Idle_Master_v01.png'),master=await sharp(masterFile).ensureAlpha().raw().toBuffer(),mf=face(master,512,512);
 const priorFile=path.join(__dirname,'Manifest.json');
 if(fs.existsSync(priorFile)){const prior=JSON.parse(fs.readFileSync(priorFile));for(const f of prior.frames.filter(f=>f.index>=12)){const file=path.join(root,f.path);if(fs.existsSync(file)&&hash(fs.readFileSync(file))!==f.sha256)throw Error('Unrecognized change in generated draft '+file);if(fs.existsSync(file))fs.unlinkSync(file);if(fs.existsSync(file+'.meta'))fs.unlinkSync(file+'.meta');}}
 const unusedSource=path.join(out,'Source','Idle_Group_1_Generated.png');if(fs.existsSync(unusedSource))fs.unlinkSync(unusedSource);if(fs.existsSync(unusedSource+'.meta'))fs.unlinkSync(unusedSource+'.meta');
 const groups=[];for(let i=2;i<=4;i++){const src=path.join(__dirname,`Idle_Group_${i}_Generated.png`),m=await sharp(src).metadata();groups.push({src,...m});fs.copyFileSync(src,path.join(out,'Source',`Idle_Group_${i}_Generated.png`));}
 const frames=[],stats=[],sheet=createCanvas(2048,1536),sctx=sheet.getContext('2d'),contact=createCanvas(1600,1200),cx=contact.getContext('2d');cx.fillStyle='#18283b';cx.fillRect(0,0,1600,1200);
 for(let i=0;i<12;i++){
  const g=groups[Math.floor(i/4)],n=i%4,l=Math.round(n%2*g.width/2),r=Math.round((n%2+1)*g.width/2),t=Math.round(Math.floor(n/2)*g.height/2),b=Math.round((Math.floor(n/2)+1)*g.height/2),w=r-l,h=b-t;
  const raw=extractBody(await sharp(g.src).extract({left:l,top:t,width:w,height:h}).ensureAlpha().raw().toBuffer(),w,h),f=face(raw,w,h),scale=Math.sqrt(mf.area/f.area);
  const c=createCanvas(512,512),ctx=c.getContext('2d');ctx.translate(mf.x,mf.y);ctx.scale(scale,scale);ctx.drawImage(await loadImage(await sharp(raw,{raw:{width:w,height:h,channels:4}}).png().toBuffer()),-f.x,-f.y);
  const png=c.toBuffer('image/png'),file=`Idle_${String(i).padStart(2,'0')}.png`;fs.writeFileSync(path.join(out,'Frames',file),png);
  const data=await sharp(png).ensureAlpha().raw().toBuffer();let minX=512,maxX=0,minY=512,maxY=0,armX=0,armY=0,armN=0;
  for(let p=0;p<512*512;p++){const x=p%512,y=Math.floor(p/512);if(data[p*4+3]>128){minX=Math.min(minX,x);maxX=Math.max(maxX,x);minY=Math.min(minY,y);maxY=Math.max(maxY,y);if(y>245&&y<350&&x>210&&x<265&&data[p*4]<65&&data[p*4+1]<55&&data[p*4+2]<50){armX+=x;armY+=y;armN++;}}}
  if(minX<2||minY<2||maxX>509||maxY>509)throw Error('Clipping '+i);
  const image=await loadImage(png);sctx.drawImage(image,i%4*512,Math.floor(i/4)*512);cx.drawImage(image,i%4*400,Math.floor(i/4)*400,400,400);cx.fillStyle='#dae7f5';cx.font='17px sans-serif';cx.fillText(String(i).padStart(2,'0'),i%4*400+14,Math.floor(i/4)*400+25);
  frames.push({index:i,sourceIndex:i+4,path:'Assets/04.Sprites/NewPlayer/Idle_v01/Frames/'+file,time:i*1.6/12,pivotX:270/512,pivotY:1-286/512,sha256:hash(png)});
  stats.push({index:i,sourceSize:[w,h],scale,face:f,targetFace:[mf.x,mf.y],alphaBounds:[minX,minY,maxX,maxY],nearArmCentroid:[armX/armN,armY/armN]});
 }
 fs.writeFileSync(path.join(out,'Sheets/Idle_Grid512_v01.png'),sheet.toBuffer('image/png'));fs.writeFileSync(path.join(__dirname,'Idle_ContactSheet.png'),contact.toBuffer('image/png'));
 const manifest={version:1,frameCount:12,frameSize:[512,512],ppu:200,sampleRate:20,loopClosingTime:1.6,cycleSeconds:1.65,master:'Assets/04.Sprites/NewPlayer/Design_v01/Player_Idle_Master_v01.png',run:'Assets/04.Sprites/NewPlayer/Run_v02/Frames',selection:'Groups 2-4 retained. Group1 and attempted correction rejected for inconsistent leg length. Master remains a design reference, not an extra animation frame.',frames,calibration:stats};
 fs.writeFileSync(path.join(__dirname,'Manifest.json'),JSON.stringify(manifest,null,2));console.log(JSON.stringify(stats.map(({index,alphaBounds,nearArmCentroid})=>({index,alphaBounds,nearArmCentroid}))));
}main().catch(e=>{console.error(e);process.exit(1)});

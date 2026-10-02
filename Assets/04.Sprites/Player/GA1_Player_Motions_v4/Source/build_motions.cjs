const fs=require('fs'),path=require('path'),sharp=require('sharp');
const {createCanvas,loadImage}=require('@napi-rs/canvas');
const ROOT=path.resolve(__dirname,'..'),WORK=path.join(ROOT,'BuildPreview');
const W=512,H=512,GROUND=410,FACE_AREA=4780;
const clamp=(v,a,b)=>Math.max(a,Math.min(b,v));
const ease=(a,b,v)=>{v=clamp((v-a)/(b-a),0,1);return v*v*(3-2*v)};
const specs={RunA:[4,1,4],RunB:[2,2,4],Aerial:[4,2,8],Attack:[4,2,8],Wall:[4,2,8],DashHurt:[4,2,8],Curl:[2,1,2]};
function components(d,w,h,pred){
 const mark=new Uint8Array(w*h),q=new Int32Array(w*h),out=[];
 for(let p=0;p<w*h;p++)if(!mark[p]&&pred(d,p*4)){
  let a=0,b=1;q[0]=p;mark[p]=1;let x0=w,y0=h,x1=0,y1=0,sx=0,sy=0;
  while(a<b){const v=q[a++],x=v%w,y=(v/w)|0;x0=Math.min(x0,x);y0=Math.min(y0,y);x1=Math.max(x1,x);y1=Math.max(y1,y);sx+=x;sy+=y;
   for(let yy=Math.max(0,y-1);yy<=Math.min(h-1,y+1);yy++)for(let xx=Math.max(0,x-1);xx<=Math.min(w-1,x+1);xx++){const n=yy*w+xx;if(!mark[n]&&pred(d,n*4)){mark[n]=1;q[b++]=n}}
  }
  if(b>500)out.push({area:b,bbox:[x0,y0,x1,y1],center:[sx/b,sy/b]});
 }
 return out.sort((a,b)=>b.area-a.area);
}
function bbox(d){let x0=W,y0=H,x1=-1,y1=-1;for(let y=0;y<H;y++)for(let x=0;x<W;x++)if(d[(y*W+x)*4+3]>16){x0=Math.min(x0,x);y0=Math.min(y0,y);x1=Math.max(x1,x);y1=Math.max(y1,y)}return[x0,y0,x1,y1]}
async function png(d){return sharp(d,{raw:{width:W,height:H,channels:4}}).png().toBuffer()}
function warp(src,fn){
 const d=Buffer.alloc(W*H*4);
 for(let y=0;y<H;y++)for(let x=0;x<W;x++){
  let sx=x,sy=y;for(let k=0;k<3;k++){const [dx,dy]=fn(sx,sy);sx=x-dx;sy=y-dy}
  const x0=Math.floor(sx),y0=Math.floor(sy),fx=sx-x0,fy=sy-y0;
  let a=0,r=0,g=0,b=0;
  for(let yy=0;yy<2;yy++)for(let xx=0;xx<2;xx++){const X=x0+xx,Y=y0+yy;if(X<0||X>=W||Y<0||Y>=H)continue;const p=(Y*W+X)*4,aa=src[p+3]/255*(xx?fx:1-fx)*(yy?fy:1-fy);a+=aa;r+=src[p]*aa;g+=src[p+1]*aa;b+=src[p+2]*aa}
  const p=(y*W+x)*4;if(a>0){d[p]=Math.round(r/a);d[p+1]=Math.round(g/a);d[p+2]=Math.round(b/a);d[p+3]=Math.round(a*255)}
 }
 return d;
}
function secondary(src,phase,kind='normal'){
 return warp(src,(x,y)=>{
  if(kind==='dash')return [2.2*Math.sin(phase+(x-250)*.024)*(1-ease(240,320,x)),1.1*Math.cos(phase+x*.02)*(1-ease(260,330,x))];
  // Face and contact points remain rigid. Only crest and rear mantle receive flame drift.
  const crest=1-ease(97,119,y),back=(1-ease(199,221,x))*ease(175,205,y)*(1-ease(340,377,y));
  return [crest*1.8*Math.sin(phase+(140-y)*.027)+back*1.1*Math.sin(phase-y*.024),back*.6*Math.cos(phase-y*.02)];
 });
}
async function rotate(src,radians,cx=256,cy=286){
 const c=createCanvas(W,H),ctx=c.getContext('2d');ctx.translate(cx,cy);ctx.rotate(radians);ctx.drawImage(await loadImage(await png(src)),-cx,-cy);
 return sharp(c.toBuffer('image/png')).ensureAlpha().raw().toBuffer();
}
async function main(){
 for(const d of ['Frames','Sheets','Previews','Keyposes','BuildPreview'])fs.mkdirSync(path.join(ROOT,d),{recursive:true});
 const sources={},calibration=[];
 for(const [name,[cols,rows,count]] of Object.entries(specs)){
  const {data,info}=await sharp(path.join(__dirname,name+'.png')).ensureAlpha().raw().toBuffer({resolveWithObject:true});
  const comps=components(data,info.width,info.height,(d,p)=>d[p+3]>16).filter(c=>c.area>3000);
  const faces=components(data,info.width,info.height,(d,p)=>d[p+3]>128&&d[p]>220&&d[p+1]>175&&d[p+2]>105).filter(c=>c.area>4000);
  sources[name]=[];
  for(let i=0;i<count;i++){
   const col=i%cols,row=Math.floor(i/cols);
   const match=c=>Math.floor(c.center[0]/(info.width/cols))===col&&Math.floor(c.center[1]/(info.height/rows))===row;
   const c=comps.filter(match)[0],f=faces.filter(match)[0];if(!c||!f)throw Error('Missing pose/face '+name+i);
   const s=Math.sqrt(FACE_AREA/f.area); // Calibrate artwork by head anatomy, never by whole-pose height.
   let fx=291,fy=GROUND-(c.bbox[3]-f.center[1])*s;
   if(name==='RunA'||name==='RunB'){fx=300;if(i===3)fy=200;}
   if(name==='Aerial'){if(i>=2&&i<=5)fy=[0,0,177,181,188,190][i];}
   if(name==='Attack')fx=[294,296,300,310,304,297,292,291][i];
   if(name==='DashHurt'){
    fx=[302,390,390,292,272,252,282,291][i];
    if(i===1||i===2)fy=263;
   }
   if(name==='Wall'){
    fx=350-(c.bbox[2]-f.center[0])*s;
    fy=[185,177,185,177,185,185,224,185][i];
   }
   if(name==='Curl'){
    fx=256+(f.center[0]-(c.bbox[0]+c.bbox[2])/2)*s;
    fy=286+(f.center[1]-(c.bbox[1]+c.bbox[3])/2)*s;
   }
   const bx=Math.max(0,c.bbox[0]-3),by=Math.max(0,c.bbox[1]-3),bw=Math.min(info.width-1,c.bbox[2]+3)-bx+1,bh=Math.min(info.height-1,c.bbox[3]+3)-by+1;
   const cut=await sharp(data,{raw:{width:info.width,height:info.height,channels:4}}).extract({left:bx,top:by,width:bw,height:bh}).png().toBuffer();
   const canvas=createCanvas(W,H),ctx=canvas.getContext('2d');ctx.translate(fx,fy);ctx.scale(s,s);ctx.drawImage(await loadImage(cut),bx-f.center[0],by-f.center[1]);
   const frame=await sharp(canvas.toBuffer('image/png')).ensureAlpha().raw().toBuffer();
   // Remove only nearly transparent isolated acquisition noise; preserve real edge alpha.
   for(let p=0;p<frame.length;p+=4)if(frame[p+3]<3)frame.fill(0,p,p+4);
   sources[name].push(frame);
   fs.writeFileSync(path.join(ROOT,'Keyposes',`${name}_${i}.png`),await png(frame));
   calibration.push({source:name,index:i,sourceFace:f,scale:s,targetFaceCenter:[fx,fy],bounds:bbox(frame),method:'One-time head-area calibration; preserve crouch/airborne pose height; no per-render auto-fit'});
  }
 }
 const master=await sharp(path.join(__dirname,'Idle_Master512.png')).ensureAlpha().raw().toBuffer();
 const bodyDip=amount=>warp(master,(x,y)=>[0,amount*(1-ease(262,407,y))]);
 const clips=[];
 async function add(name,frames,fps,loop=false,extra={}){
  const dir=path.join(ROOT,'Frames',name);fs.mkdirSync(dir,{recursive:true});
  const duration=extra.duration??frames.length/fps;
  const times=extra.times??frames.map((_,i)=>i/fps);
  const clip={name,frameCount:frames.length,fps,loop,duration,times,...extra,frames:frames.map((_,i)=>`Frames/${name}/${name}_${String(i).padStart(2,'0')}.png`)};
  for(let i=0;i<frames.length;i++)fs.writeFileSync(path.join(ROOT,clip.frames[i]),await png(frames[i]));
  const cols=Math.min(8,frames.length),rows=Math.ceil(frames.length/cols),aw=cols*W,ah=rows*H,atlas=Buffer.alloc(aw*ah*4);
  for(let i=0;i<frames.length;i++)for(let y=0;y<H;y++)frames[i].copy(atlas,((Math.floor(i/cols)*H+y)*aw+(i%cols)*W)*4,y*W*4,(y+1)*W*4);
  fs.writeFileSync(path.join(ROOT,'Sheets',name+'_Grid512.png'),await sharp(atlas,{raw:{width:aw,height:ah,channels:4}}).png().toBuffer());
  clip.sheet={file:`Sheets/${name}_Grid512.png`,columns:cols,rows,width:aw,height:ah,unusedCells:cols*rows-frames.length};
  clips.push(clip);return clip;
 }
 const runLoop=await require('./build_run_boundary.cjs').buildRun(ROOT);
 clips.push(runLoop.clip);
 await add('JumpStart',[bodyDip(12),sources.Aerial[0],sources.Aerial[1]],30,false,{keyDrawings:3});
 await add('JumpRise',Array.from({length:8},(_,i)=>secondary(sources.Aerial[2],i*Math.PI/4)),24,true,{keyDrawings:1});
 await add('JumpApex',Array.from({length:4},(_,i)=>secondary(sources.Aerial[4],i*Math.PI/2)),24,false,{keyDrawings:1});
 await add('Fall',Array.from({length:8},(_,i)=>secondary(sources.Aerial[5],i*Math.PI/4)),24,true,{keyDrawings:1});
 await add('Land',[sources.Aerial[6],bodyDip(34),bodyDip(12),master],30,false,{keyDrawings:2});
 const spin=[sources.Curl[0]];
 for(let i=0;i<12;i++)spin.push(await rotate(sources.Curl[1],i/12*2*Math.PI));
 spin.push(sources.Curl[1],sources.Curl[0],sources.Aerial[3]);
 await add('DoubleJump',spin,36,false,{keyDrawings:3,rotation:'Clockwise 360 degrees from one locked tuck pose; no face redraw while spinning'});
 await add('DashStart',[sources.DashHurt[0],sources.DashHurt[1]],40,false,{keyDrawings:2});
 await add('DashLoop',Array.from({length:8},(_,i)=>secondary(sources.DashHurt[2],i*Math.PI/4,'dash')),40,true,{keyDrawings:1});
 await add('DashEnd',[sources.DashHurt[3],bodyDip(9),master],30,false,{keyDrawings:2});
 await add('Attack',[...sources.Attack.slice(0,7),master],60,false,{keyDrawings:8,times:[0,.04,.075,.105,.135,.175,.235,.30],duration:.36,activeWindow:[.105,.175],note:'Visual sword appears in windup. Damage is controlled separately by gameplay.'});
 await add('WallCling',Array.from({length:8},(_,i)=>secondary(sources.Wall[0],i*Math.PI/4)),20,true,{keyDrawings:1});
 await add('WallClimb',sources.Wall.slice(0,4),8,true,{keyDrawings:4});
 await add('WallSlide',Array.from({length:8},(_,i)=>secondary(sources.Wall[4],i*Math.PI/4)),20,true,{keyDrawings:1});
 await add('WallJump',[sources.Wall[6],sources.Wall[7],secondary(sources.Wall[7],1.1)],30,false,{keyDrawings:2,note:'Art faces the wall on the right while physics pushes left; do not flip to velocity in mid-clip.'});
 await add('Hurt',[...sources.DashHurt.slice(4,7),master],24,false,{keyDrawings:4});
 const manifest={version:4,revision:'4.2-original-run-boundary',character:'Locked approved fire elemental; Idle v3 companion pack',frameSize:[W,H],pivot:[.5,(H-GROUND)/H],pixelsPerUnit:200,defaultFacing:'right',spriteBindingPath:'',order:'row-major top-left first',clipCount:clips.length,totalFrames:clips.reduce((a,c)=>a+c.frameCount,0),clips};
 fs.writeFileSync(path.join(ROOT,'Motions.json'),JSON.stringify(manifest,null,2));
 fs.writeFileSync(path.join(ROOT,'Source','Calibration.json'),JSON.stringify(calibration,null,2));
 const c=createCanvas(2048,1536),ctx=c.getContext('2d');ctx.fillStyle='#132135';ctx.fillRect(0,0,c.width,c.height);
 const picks=[['Idle v3',master],['Original Run / frame 1',runLoop.raws[0]],['Run / final bridge',runLoop.raws[9]],['Takeoff',sources.Aerial[1]],['Rise',sources.Aerial[2]],['Land',sources.Aerial[6]],['Tuck',sources.Curl[1]],['Dash',sources.DashHurt[2]],['Attack',sources.Attack[3]],['Wall',sources.Wall[0]],['Wall jump',sources.Wall[7]],['Hurt',sources.DashHurt[5]]];
 for(let i=0;i<picks.length;i++){let x=i%4*512,y=Math.floor(i/4)*512;ctx.strokeStyle='#42536d';ctx.beginPath();ctx.moveTo(x+20,y+410.5);ctx.lineTo(x+492,y+410.5);ctx.stroke();ctx.drawImage(await loadImage(await png(picks[i][1])),x,y);ctx.fillStyle='#ccd9ed';ctx.font='18px sans-serif';ctx.fillText(picks[i][0],x+25,y+468)}
 fs.writeFileSync(path.join(ROOT,'Previews','Scale_Comparison.png'),c.toBuffer('image/png'));
 console.log(JSON.stringify({clipCount:clips.length,totalFrames:manifest.totalFrames,out:ROOT}));
}
main().catch(e=>{console.error(e);process.exit(1)});

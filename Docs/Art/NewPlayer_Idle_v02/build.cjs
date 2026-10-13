// User-requested deterministic animation of an existing painting. No new AI frames.
const fs=require('fs'),path=require('path'),crypto=require('crypto');
const deps='C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules';
const sharp=require(path.join(deps,'sharp')),{createCanvas,loadImage}=require(path.join(deps,'@napi-rs/canvas'));
const root=path.resolve(__dirname,'../../..'),asset='Assets/04.Sprites/NewPlayer/Idle_v02',out=path.join(root,asset);
const master='Assets/04.Sprites/NewPlayer/Design_v01/Player_Idle_Master_v01.png';
const W=512,H=512,N=32,FPS=20,T=N/FPS;
const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
const walk=d=>fs.existsSync(d)?fs.readdirSync(d,{withFileTypes:true}).flatMap(e=>e.isDirectory()?walk(path.join(d,e.name)):[path.join(d,e.name)]):[];
const clamp=(x,a,b)=>Math.max(a,Math.min(b,x));
function smooth(a,b,x){const v=clamp((x-a)/(b-a),0,1);return v*v*(3-2*v);}
function bell(x,y,cx,cy,rx,ry){const r=Math.hypot((x-cx)/rx,(y-cy)/ry);return 1-smooth(.25,1,r);}
function segDist(x,y,a,b){const dx=b[0]-a[0],dy=b[1]-a[1],t=clamp(((x-a[0])*dx+(y-a[1])*dy)/(dx*dx+dy*dy),0,1);return Math.hypot(x-a[0]-t*dx,y-a[1]-t*dy);}
const arms=[[[252,253],[238,281]],[[238,281],[232,307]],[[232,307],[240,327]],[[240,327],[246,331]],[[240,321],[230,327]],[[301,285],[309,312]],[[309,312],[320,330]]];
function protection(x,y){
 // Solid face bounding area, softened only outside it; includes dark eyes and mask contour.
 const face=smooth(234,245,x)*smooth(116,128,y)*(1-smooth(235,247,y));
 const hands=Math.max(...arms.map(([a,b])=>1-smooth(12,19,segDist(x,y,a,b))));
 const feet=smooth(365,389,y);
 return Math.max(face,hands,feet);
}
function field(x,y,t){
 const p=2*Math.PI*(t/T),free=1-protection(x,y);
 if(free<1e-10)return [0,0];
 const crest=(1-smooth(113,153,y))*smooth(177,199,x)*(1-smooth(272,298,x));
 const neck=bell(x,y,206,188,39,40)*(1-smooth(222,240,x));
 const back=bell(x,y,185,249,43,47)*(1-smooth(212,238,x));
 const hip=bell(x,y,193,313,37,45)*(1-smooth(214,232,x));
 const c=2*p+(143-y)*.025,b=2*p-1.1-(y-235)*.018;
 const dx=crest*(4.4*Math.sin(c)+.6*Math.sin(p+.2))+neck*2.4*Math.sin(2*p-.7)+back*(3.6*Math.sin(b)+.45*Math.sin(p-.6))+hip*(2.4*Math.sin(2*p-1.9)+.3*Math.sin(p-1.7));
 const dy=crest*.85*Math.sin(c+.8)+neck*.8*Math.cos(2*p-.7)+back*1.15*Math.cos(b)+hip*.8*Math.cos(2*p-1.9);
 return [dx*free,dy*free];
}
// Precompute the static influence masks; retain the exact same field definition for QA.
const weights=new Float64Array(W*H*6);
for(let y=0;y<H;y++)for(let x=0;x<W;x++){
 const i=(y*W+x)*6,f=1-protection(x,y);
 weights[i]=f*(1-smooth(113,153,y))*smooth(177,199,x)*(1-smooth(272,298,x));
 weights[i+1]=f*bell(x,y,206,188,39,40)*(1-smooth(222,240,x));
 weights[i+2]=f*bell(x,y,185,249,43,47)*(1-smooth(212,238,x));
 weights[i+3]=f*bell(x,y,193,313,37,45)*(1-smooth(214,232,x));
 weights[i+4]=f;
}
function render(src,t){
 const p=2*Math.PI*t/T,offsets=new Float32Array(W*H*2);
 for(let y=0;y<H;y++)for(let x=0;x<W;x++){
  const i=(y*W+x)*6,q=(y*W+x)*2,c=2*p+(143-y)*.025,b=2*p-1.1-(y-235)*.018;
  offsets[q]=weights[i]*(4.4*Math.sin(c)+.6*Math.sin(p+.2))+weights[i+1]*2.4*Math.sin(2*p-.7)+weights[i+2]*(3.6*Math.sin(b)+.45*Math.sin(p-.6))+weights[i+3]*(2.4*Math.sin(2*p-1.9)+.3*Math.sin(p-1.7));
  offsets[q+1]=weights[i]*.85*Math.sin(c+.8)+weights[i+1]*.8*Math.cos(2*p-.7)+weights[i+2]*1.15*Math.cos(b)+weights[i+3]*.8*Math.cos(2*p-1.9);
 }
 function offset(x,y){const x0=Math.floor(x),y0=Math.floor(y),fx=x-x0,fy=y-y0;let dx=0,dy=0;
  for(const [xx,yy,w]of [[x0,y0,(1-fx)*(1-fy)],[x0+1,y0,fx*(1-fy)],[x0,y0+1,(1-fx)*fy],[x0+1,y0+1,fx*fy]]){
   if(xx>=0&&yy>=0&&xx<W&&yy<H){const q=(yy*W+xx)*2;dx+=offsets[q]*w;dy+=offsets[q+1]*w;}
  }return [dx,dy];
 }
 const dst=Buffer.from(src);
 for(let y=0;y<H;y++)for(let x=0;x<W;x++){
  const p=(y*W+x)*2;if(Math.abs(offsets[p])+Math.abs(offsets[p+1])<1e-9)continue;
  let sx=x,sy=y;
  for(let k=0;k<3;k++){const [dx,dy]=offset(sx,sy);sx=x-dx;sy=y-dy;}
  const x0=Math.floor(sx),y0=Math.floor(sy),fx=sx-x0,fy=sy-y0;let a=0,r=0,g=0,b=0;
  for(const [xx,yy,w]of [[x0,y0,(1-fx)*(1-fy)],[x0+1,y0,fx*(1-fy)],[x0,y0+1,(1-fx)*fy],[x0+1,y0+1,fx*fy]]){
   if(xx<0||yy<0||xx>=W||yy>=H)continue;const j=(yy*W+xx)*4,v=src[j+3]/255*w;a+=v;r+=src[j]*v;g+=src[j+1]*v;b+=src[j+2]*v;
  }
  const j=(y*W+x)*4;dst[j]=a?Math.round(r/a):0;dst[j+1]=a?Math.round(g/a):0;dst[j+2]=a?Math.round(b/a):0;dst[j+3]=Math.round(a*255);
 }
 return dst;
}
async function main(){
 const baseline=path.join(__dirname,'PreservationBaseline.json');
 if(!fs.existsSync(baseline)){
  const dirs=['Assets/04.Sprites/Player','Assets/04.Sprites/NewPlayer','Assets/05.Animation/player','Assets/05.Animation/PlayerAttack','Assets/02.Scripts/Player','Assets/03.Prefabs/Player','Assets/01.Scenes'];
  fs.writeFileSync(baseline,JSON.stringify(dirs.flatMap(d=>walk(path.join(root,d))).map(p=>({path:path.relative(root,p).replaceAll('\\','/'),sha256:hash(fs.readFileSync(p))})),null,2));
  fs.copyFileSync(path.join(root,'Assets/05.Animation/player/Idle.anim'),path.join(__dirname,'Idle_Before.anim.txt'));
 }
 for(const d of ['Frames','Sheets','Source'])fs.mkdirSync(path.join(out,d),{recursive:true});
 fs.copyFileSync(path.join(root,master),path.join(out,'Source/Idle_Master512.png'));
 const src=await sharp(path.join(root,master)).ensureAlpha().raw().toBuffer();
 const raws=[],pngs=[],frames=[];let protectedChanged=0,fixedFootChanged=0;
 for(let i=0;i<N;i++){
  const raw=render(src,i/FPS);raws.push(raw);const png=await sharp(raw,{raw:{width:W,height:H,channels:4}}).png().toBuffer();pngs.push(png);
  const file=asset+'/Frames/Idle_'+String(i).padStart(2,'0')+'.png';fs.writeFileSync(path.join(root,file),png);frames.push({path:file,time:i/FPS,pivotX:270/512,pivotY:226/512,sha256:hash(png)});
  for(let y=0;y<H;y++)for(let x=0;x<W;x++){const j=(y*W+x)*4;for(let c=0;c<4;c++)if(raw[j+c]!==src[j+c]){if(weights[(y*W+x)*6+4]===0)protectedChanged++;if(y>=389)fixedFootChanged++;}}
 }
 const closure=render(src,T);let closureChanged=0;for(let i=0;i<closure.length;i++)if(closure[i]!==raws[0][i])closureChanged++;
 function maxStep(n){let max=0;for(let i=0;i<n;i++)for(const [x,y]of [[219,62],[208,180],[172,250],[187,309]]){const a=field(x,y,i*T/n),b=field(x,y,(i+1)*T/n);max=Math.max(max,Math.hypot(a[0]-b[0],a[1]-b[1]));}return max;}
 const atlas=await sharp({create:{width:W*8,height:H*4,channels:4,background:{r:0,g:0,b:0,alpha:0}}}).composite(pngs.map((input,i)=>({input,left:i%8*W,top:Math.floor(i/8)*H}))).png().toBuffer();
 fs.writeFileSync(path.join(out,'Sheets/Idle_Grid512_v02.png'),atlas);
 const c=createCanvas(1536,1024),ctx=c.getContext('2d');ctx.fillStyle='#14253a';ctx.fillRect(0,0,1536,1024);
 for(let j=0;j<6;j++){const i=[0,5,11,16,21,27][j];ctx.drawImage(await loadImage(pngs[i]),j%3*512,Math.floor(j/3)*512);ctx.fillStyle='#dceafa';ctx.font='20px sans-serif';ctx.fillText('FRAME '+String(i).padStart(2,'0'),j%3*512+25,Math.floor(j/3)*512+42);}
 fs.writeFileSync(path.join(__dirname,'Keyframes.png'),c.toBuffer('image/png'));
 const mask=Buffer.alloc(W*H*4);for(let i=0;i<W*H;i++){const k=i*6,f=weights[k+4],strength=Math.min(1,(weights[k]+weights[k+1]+weights[k+2]+weights[k+3]));const j=i*4;mask[j]=f===0?25:255;mask[j+1]=f===0?220:135;mask[j+2]=f===0?195:30;mask[j+3]=f===0?90:Math.round(strength*140);}
 const overlay=await sharp(src,{raw:{width:W,height:H,channels:4}}).composite([{input:mask,raw:{width:W,height:H,channels:4}}]).png().toBuffer();fs.writeFileSync(path.join(__dirname,'MotionMask.png'),overlay);
 const manifest={method:'One existing painting + local periodic deformation; exact protected pixels, no generated per-frame drawings',frameCount:N,frameSize:[W,H],ppu:200,sampleRate:FPS,cycleSeconds:T,master,sourceHash:hash(fs.readFileSync(path.join(root,master))),frames};
 const verification={uniqueFrames:new Set(frames.map(f=>f.sha256)).size,protectedRegionChangedBytes:protectedChanged,fixedFootBandChangedBytes:fixedFootChanged,loopClosureChangedBytes:closureChanged,maximumTipStep16:maxStep(16),maximumTipStep32:maxStep(32),frameSize:[W,H],frameRate:FPS,durationSeconds:T,noDuplicateClosingKey:true};
 fs.writeFileSync(path.join(__dirname,'Manifest.json'),JSON.stringify(manifest,null,2));fs.writeFileSync(path.join(__dirname,'PixelVerification.json'),JSON.stringify(verification,null,2));
 if(protectedChanged||fixedFootChanged||closureChanged||verification.uniqueFrames!==N)throw Error('Pixel invariants failed');
 console.log(JSON.stringify(verification));
}
main().catch(e=>{console.error(e);process.exit(1)});

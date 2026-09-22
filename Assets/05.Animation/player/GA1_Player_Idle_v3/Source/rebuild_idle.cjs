const fs = require('fs');
const path = require('path');
const sharp = require('sharp');
const { createCanvas, loadImage } = require('@napi-rs/canvas');

// One approved painting, one periodic deformation field. Never normalize frames separately.
const ROOT = path.resolve(__dirname, '..');
const OUT = ROOT;
const FRAMES = path.join(OUT, 'Frames');
const TMP = path.join(ROOT, 'RebuildPreviewFrames');
for (const p of [OUT, FRAMES, TMP, path.join(OUT,'Source'), path.join(OUT,'Editor')]) fs.mkdirSync(p,{recursive:true});
const SOURCE = path.join(__dirname, 'Player_Idle_Cutout.png');
const W=512, H=512, N=32, FPS=20, DURATION=N/FPS;
const ground=410, rootX=256;
const clamp=(v,a,b)=>Math.max(a,Math.min(b,v));
const smooth=(a,b,x)=>{const v=clamp((x-a)/(b-a),0,1);return v*v*(3-2*v);};
const ellipse=(x,y,cx,cy,rx,ry)=>Math.exp(-2*((x-cx)**2/rx**2+(y-cy)**2/ry**2));

function field(x,y,t) {
  const p=2*Math.PI*t/DURATION;
  const breath=(1-Math.cos(p))*.5;
  const pin=1-smooth(393,405,y); // feet and their support line stay fixed
  const upper=1-smooth(250,400,y);
  // Face moves rigidly by at most 1.8 px. No scale/rotation/shape animation.
  let dx=0;
  let dy=-1.8*breath*upper;
  // Crest: waves travel from its base towards the tip. Entire facial region excluded.
  const crest=1-smooth(111,147,y);
  dx+=crest*4.4*Math.sin(2*p+(147-y)*.024);
  dy+=crest*.9*Math.sin(2*p+(147-y)*.024+.8);
  // Exterior mantle tongues: bounded local secondary motion, away from the face/hands.
  const neck=ellipse(x,y,204,195,30,31)*(1-smooth(212,234,x))*smooth(164,188,y);
  const back=ellipse(x,y,174,255,43,40)*(1-smooth(213,239,x));
  const hip=ellipse(x,y,190,335,34,35)*(1-smooth(215,239,x));
  dx+=neck*2.6*Math.sin(2*p-.6)+back*3.6*Math.sin(2*p-1.2)+hip*2.4*Math.sin(2*p-1.9);
  dy+=neck*1.0*Math.cos(2*p-.6)+back*1.3*Math.cos(2*p-1.2)+hip*.9*Math.cos(2*p-1.9);
  // Chest expands subtly, with no influence on the head.
  const torso=ellipse(x,y,282,279,48,59)*smooth(243,265,y);
  dx+=(x-277)*.006*breath*torso;
  // Small arm follow-through, keeping fingers/hand size consistent.
  const armNear=ellipse(x,y,235,316,17,27)*smooth(293,308,y);
  const armFar=ellipse(x,y,324,324,18,25)*smooth(295,308,y);
  dx+=.5*Math.sin(p-.4)*(armNear-armFar);
  dy+=.35*Math.sin(p-.9)*(armNear+armFar);
  const faceLock=smooth(221,225,x)*smooth(135,140,y)*(1-smooth(240,250,y));
  dx*=1-faceLock;
  dy=dy*(1-faceLock)+(-1.8*breath)*faceLock;
  return [dx*pin,dy*pin];
}

function warp(src,t) {
  const dst=Buffer.alloc(W*H*4);
  for(let y=0;y<H;y++) for(let x=0;x<W;x++) {
    let sx=x,sy=y;
    for(let k=0;k<3;k++){const [dx,dy]=field(sx,sy,t);sx=x-dx;sy=y-dy;}
    const x0=Math.floor(sx),y0=Math.floor(sy),fx=sx-x0,fy=sy-y0;
    const ws=[(1-fx)*(1-fy),fx*(1-fy),(1-fx)*fy,fx*fy];
    const ps=[[x0,y0],[x0+1,y0],[x0,y0+1],[x0+1,y0+1]];
    let aa=0,rr=0,gg=0,bb=0;
    for(let k=0;k<4;k++) {const [xx,yy]=ps[k];if(xx<0||yy<0||xx>=W||yy>=H)continue;
      const q=(yy*W+xx)*4, a=src[q+3]/255*ws[k];
      aa+=a;rr+=src[q]*a;gg+=src[q+1]*a;bb+=src[q+2]*a;
    }
    const q=(y*W+x)*4;
    if(aa>0){dst[q]=Math.round(rr/aa);dst[q+1]=Math.round(gg/aa);dst[q+2]=Math.round(bb/aa);dst[q+3]=Math.round(aa*255);}
  }
  return dst;
}

async function main(){
  // Shared resampling performed once, with the old project's pivot convention retained.
  const side=373, s=side/1254;
  const left=Math.round(rootX-640*s), top=Math.round(ground-1199*s);
  const scaled=await sharp(SOURCE).resize(side,side,{kernel:'lanczos3'}).toBuffer();
  const master=await sharp({create:{width:W,height:H,channels:4,background:{r:0,g:0,b:0,alpha:0}}})
    .composite([{input:scaled,left,top}]).png().toBuffer();
  const masterPath=path.join(OUT,'Source','Player_Idle_Master512.png');fs.writeFileSync(masterPath,master);

  const src=await sharp(master).ensureAlpha().raw().toBuffer();
  const pngs=[],raws=[];
  for(let i=0;i<N;i++) {
    const raw=warp(src,i/FPS);raws.push(raw);
    const png=await sharp(raw,{raw:{width:W,height:H,channels:4}}).png().toBuffer();
    pngs.push(png);fs.writeFileSync(path.join(FRAMES,`Idle_${String(i).padStart(2,'0')}.png`),png);
  }
  // Contact sheet / atlas is packed only after animation frames have been rendered.
  const atlasRaw=Buffer.alloc(W*8*H*4*4);
  for(let i=0;i<N;i++)for(let y=0;y<H;y++){
    const target=((Math.floor(i/8)*H+y)*(W*8)+(i%8)*W)*4;
    raws[i].copy(atlasRaw,target,y*W*4,(y+1)*W*4);
  }
  const atlas=await sharp(atlasRaw,{raw:{width:W*8,height:H*4,channels:4}}).png().toBuffer();
  fs.writeFileSync(path.join(OUT,'Player_Idle_v3_Grid512.png'),atlas);
  const masterImage=await loadImage(master);
  const previewW=800,previewH=512;
  for(let i=0;i<N;i++){
    const c=createCanvas(previewW,previewH),ctx=c.getContext('2d');
    const bg=ctx.createLinearGradient(0,0,0,512);bg.addColorStop(0,'#091120');bg.addColorStop(1,'#192c45');ctx.fillStyle=bg;ctx.fillRect(0,0,previewW,previewH);
    ctx.strokeStyle='#435267';ctx.lineWidth=1;ctx.beginPath();ctx.moveTo(36,410.5);ctx.lineTo(476,410.5);ctx.moveTo(560,350.5);ctx.lineTo(760,350.5);ctx.stroke();
    const img=await loadImage(pngs[i]);ctx.drawImage(img,0,0);
    const k=128/352;ctx.drawImage(img,660-rootX*k,350-ground*k,W*k,H*k);
    ctx.fillStyle='#c5d3e6';ctx.font='16px sans-serif';ctx.fillText('IDLE / 32 FRAMES / 20 FPS',36,462);
    ctx.font='13px sans-serif';ctx.fillStyle='#879bb5';ctx.fillText('1.6 s LOOP',36,485);ctx.fillText('GAME-SIZE PREVIEW',580,391);
    ctx.fillStyle='#627890';ctx.fillRect(560,428,200,2);ctx.fillStyle='#eda962';ctx.fillRect(560,428,200*(i+1)/N,2);
    fs.writeFileSync(path.join(TMP,`preview_${String(i).padStart(2,'0')}.png`),c.toBuffer('image/png'));
  }
  const c=createCanvas(2048,1024),ctx=c.getContext('2d');ctx.fillStyle='#122037';ctx.fillRect(0,0,c.width,c.height);
  for(let j=0;j<8;j++){const i=j*4;ctx.drawImage(await loadImage(pngs[i]),(j%4)*512,Math.floor(j/4)*512);ctx.fillStyle='#b6c9e0';ctx.font='18px sans-serif';ctx.fillText(`${i} / ${(i/FPS).toFixed(2)}s`,(j%4)*512+30,Math.floor(j/4)*512+470);}
  fs.writeFileSync(path.join(TMP,'keyposes.png'),c.toBuffer('image/png'));
  const metadata={version:3,method:'Single approved painting + periodic 2D deformation; no independently generated animation frames',uniqueFrames:N,frameRate:FPS,durationSeconds:DURATION,frameSize:[W,H],sheetSize:[W*8,H*4],columns:8,rows:4,order:'row-major, top-left first',pivotNormalized:[.5,(512-ground)/512],pivotPixelFromBottom:[rootX,512-ground],suggestedPPU:200,frameDurationSeconds:1/FPS,frames:Array.from({length:N},(_,i)=>({name:`Idle_${String(i).padStart(2,'0')}`,time:i/FPS,seconds:1/FPS})),loopEndpoint:'Do not add an extra held duplicate frame; animation clip returns to frame 00 at 1.6s.'};
  fs.writeFileSync(path.join(OUT,'Idle_Timing.json'),JSON.stringify(metadata,null,2));
  const closure=warp(src,DURATION);
  let seamMismatch=0,footMismatch=0,faceDifferential=0;
  for(let i=0;i<closure.length;i++)if(closure[i]!==raws[0][i])seamMismatch++;
  for(const raw of raws)for(let y=405;y<H;y++)for(let x=0;x<W;x++)for(let q=0;q<4;q++)if(raw[(y*W+x)*4+q]!==src[(y*W+x)*4+q])footMismatch++;
  // Field within the face must be one rigid translation throughout the loop.
  for(let i=0;i<N;i++){
    const a=field(289,200,i/FPS);
    for(let y=150;y<=235;y+=5)for(let x=230;x<=338;x+=5){const b=field(x,y,i/FPS);faceDifferential=Math.max(faceDifferential,Math.hypot(a[0]-b[0],a[1]-b[1]));}
  }
  function largestTipStep(n){let max=0;for(let i=0;i<n;i++)for(const [x,y] of [[226,58],[185,252],[202,190]]){const a=field(x,y,i*DURATION/n),b=field(x,y,(i+1)*DURATION/n);max=Math.max(max,Math.hypot(a[0]-b[0],a[1]-b[1]));}return max;}
  const qa={frameCount:N,loopEndpointChangedBytes:seamMismatch,fixedFootBandChangedBytes:footMismatch,faceDifferentialMotionPx:faceDifferential,maximumTipStepWith8FramesPx:largestTipStep(8),maximumTipStepWith32FramesPx:largestTipStep(32),framesHaveCommonDimensions:true,usesPerFrameScaleNormalization:false,unityEditorExecuted:false};
  fs.writeFileSync(path.join(OUT,'Verification.json'),JSON.stringify(qa,null,2));
  console.log(JSON.stringify({out:OUT,masterPlacement:{side,left,top},...qa},null,2));
}
main().catch(e=>{console.error(e);process.exit(1);});

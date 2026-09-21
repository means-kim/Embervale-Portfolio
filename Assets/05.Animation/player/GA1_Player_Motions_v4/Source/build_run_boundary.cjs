const fs=require('fs'),path=require('path'),crypto=require('crypto'),sharp=require('sharp');
const {createCanvas,loadImage}=require('@napi-rs/canvas');
const W=512;
const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
function faces(d,w,h){
 const seen=new Uint8Array(w*h),queue=new Int32Array(w*h),out=[];
 const yes=p=>d[p*4+3]>128&&d[p*4]>220&&d[p*4+1]>175&&d[p*4+2]>105;
 for(let p=0;p<w*h;p++)if(!seen[p]&&yes(p)){
  let a=0,b=1,sx=0,sy=0;seen[p]=1;queue[0]=p;
  while(a<b){let v=queue[a++],x=v%w,y=(v/w)|0;sx+=x;sy+=y;
   for(let yy=Math.max(0,y-1);yy<=Math.min(h-1,y+1);yy++)for(let xx=Math.max(0,x-1);xx<=Math.min(w-1,x+1);xx++){const q=yy*w+xx;if(!seen[q]&&yes(q)){seen[q]=1;queue[b++]=q}}
  }
  if(b>500)out.push({area:b,x:sx/b,y:sy/b});
 }
 return out.sort((a,b)=>b.area-a.area);
}
async function toPNG(raw){return sharp(raw,{raw:{width:W,height:W,channels:4}}).png().toBuffer()}
async function buildRun(root){
 const src=path.join(root,'Source','RunBoundary'),frameDir=path.join(root,'Frames','Run');
 const raws=[],originalHashes=[],pngs=[];
 for(let i=0;i<8;i++){
  const file=`Run_${String(i).padStart(2,'0')}.png`,input=fs.readFileSync(path.join(src,'Original',file));
  // Original eight files are copied, not rendered, resized or retouched.
  fs.writeFileSync(path.join(frameDir,file),input);pngs.push(input);
  raws.push(await sharp(input).ensureAlpha().raw().toBuffer());originalHashes.push({frame:i,sha256:hash(input)});
 }
 const from=faces(raws[7],W,W)[0],to=faces(raws[0],W,W)[0];
 const bridgePath=path.join(src,'BridgeSource.png');
 const {data,info}=await sharp(bridgePath).ensureAlpha().raw().toBuffer({resolveWithObject:true});
 const landmarks=faces(data,info.width,info.height),calibration=[];
 for(let k=0;k<2;k++){
  const f=landmarks.find(x=>Math.floor(x.x/(info.width/2))===k),t=(k+1)/3;
  if(!f)throw Error('Bridge face not found '+k);
  const tx=from.x*(1-t)+to.x*t,ty=from.y*(1-t)+to.y*t,targetArea=from.area*(1-t)+to.area*t,scale=Math.sqrt(targetArea/f.area);
  const left=Math.floor(k*info.width/2),right=Math.floor((k+1)*info.width/2);
  const cell=await sharp(bridgePath).extract({left,top:0,width:right-left,height:info.height}).png().toBuffer();
  const canvas=createCanvas(W,W),ctx=canvas.getContext('2d');ctx.translate(tx,ty);ctx.scale(scale,scale);ctx.drawImage(await loadImage(cell),left-f.x,-f.y);
  const png=canvas.toBuffer('image/png'),raw=await sharp(png).ensureAlpha().raw().toBuffer();
  pngs.push(png);raws.push(raw);fs.writeFileSync(path.join(frameDir,`Run_0${k+8}.png`),png);
  calibration.push({index:k+8,sourceFace:f,targetFace:[tx,ty],scale});
 }
 const columns=5,rows=2,aw=W*columns,ah=W*rows,atlas=Buffer.alloc(aw*ah*4);
 for(let i=0;i<10;i++)for(let y=0;y<W;y++)raws[i].copy(atlas,((Math.floor(i/columns)*W+y)*aw+(i%columns)*W)*4,y*W*4,(y+1)*W*4);
 fs.writeFileSync(path.join(root,'Sheets','Run_Grid512.png'),await sharp(atlas,{raw:{width:aw,height:ah,channels:4}}).png().toBuffer());
 const times=[0,3,6,9,12,15,18,21,22,23].map(n=>n/48);
 const clip={name:'Run',frameCount:10,fps:48,loop:true,duration:.5,times,keyDrawings:10,originalDrawingsPreserved:8,bridgeDrawings:2,method:'Eight original PNGs unchanged; two generated drawings bridge the original last frame to the original first. Original 0.5-second cycle retained.',frames:Array.from({length:10},(_,i)=>`Frames/Run/Run_${String(i).padStart(2,'0')}.png`),sheet:{file:'Sheets/Run_Grid512.png',columns,rows,width:aw,height:ah,unusedCells:0}};
 const qa={originalFramesUnchanged:originalHashes.every(x=>hash(fs.readFileSync(path.join(frameDir,`Run_${String(x.frame).padStart(2,'0')}.png`)))===x.sha256),originalCount:8,newBridgeCount:2,originalHashes,calibration,tailSequence:['original 07','bridge 08','bridge 09','original 00'],durationSeconds:.5,frameStartTimes:times,pivot:[.5,.19921875],ppu:200,usesDisjointLimbRig:false,unityEditorExecuted:false};
 fs.writeFileSync(path.join(src,'Boundary_Verification.json'),JSON.stringify(qa,null,2));
 const c=createCanvas(2560,512),ctx=c.getContext('2d');ctx.fillStyle='#132338';ctx.fillRect(0,0,2560,512);
 for(const [j,i] of [6,7,8,9,0].entries()){const x=j*512;ctx.strokeStyle='#465d76';ctx.beginPath();ctx.moveTo(x+20,410.5);ctx.lineTo(x+492,410.5);ctx.stroke();ctx.drawImage(await loadImage(pngs[i]),x,0);ctx.fillStyle='#d8e5f4';ctx.font='19px sans-serif';ctx.fillText(i<8?`ORIGINAL ${i+1}`:`BRIDGE ${i-7}`,x+26,462)}
 fs.writeFileSync(path.join(root,'Previews','Run_Boundary_Sequence.png'),c.toBuffer('image/png'));
 return {raws,clip,qa};
}
module.exports={buildRun};
if(require.main===module)buildRun(path.resolve(__dirname,'..')).then(r=>console.log(JSON.stringify(r.qa))).catch(e=>{console.error(e);process.exit(1)});

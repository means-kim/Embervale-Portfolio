const fs=require('fs'),path=require('path');
const deps='C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules';
const sharp=require(path.join(deps,'sharp')),{createCanvas,loadImage}=require(path.join(deps,'@napi-rs/canvas'));
const {extractBody}=require('../NewPlayer_Run_v02/sprite-metrics.cjs');
function bounds(d,w,h,alpha=16){let x0=w,y0=h,x1=-1,y1=-1;for(let y=0;y<h;y++)for(let x=0;x<w;x++)if(d[(y*w+x)*4+3]>alpha){x0=Math.min(x0,x);y0=Math.min(y0,y);x1=Math.max(x1,x);y1=Math.max(y1,y);}return [x0,y0,x1,y1];}
function face(d,w,h,hint=[.58,.38]){
 const seen=new Uint8Array(w*h),queue=new Int32Array(w*h),parts=[];
 const yes=p=>d[p*4+3]>128&&d[p*4]>220&&d[p*4+1]>175&&d[p*4+2]>105;
 for(let p=0;p<w*h;p++)if(!seen[p]&&yes(p)){let a=0,b=1,sx=0,sy=0,x0=w,y0=h,x1=0,y1=0;seen[p]=1;queue[0]=p;
  while(a<b){const v=queue[a++],x=v%w,y=Math.floor(v/w);sx+=x;sy+=y;x0=Math.min(x0,x);x1=Math.max(x1,x);y0=Math.min(y0,y);y1=Math.max(y1,y);
   for(let yy=Math.max(0,y-1);yy<=Math.min(h-1,y+1);yy++)for(let xx=Math.max(0,x-1);xx<=Math.min(w-1,x+1);xx++){const q=yy*w+xx;if(!seen[q]&&yes(q)){seen[q]=1;queue[b++]=q;}}
  }if(b>150)parts.push({area:b,x:sx/b,y:sy/b,bounds:[x0,y0,x1,y1]});
 }
 const ranked=parts.filter(f=>f.area>w*h*.006).sort((a,b)=>{const score=f=>Math.hypot(f.x/w-hint[0],f.y/h-hint[1])-.02*Math.log(f.area);return score(a)-score(b);});
 if(!ranked.length)throw Error('No face detected');return ranked[0];
}
async function normalize(file,{hint=[.58,.38],target=[297,190],targetArea=4708,sourceFaceArea=null,ground=null,wall=null}={}){
 const {data,info}=await sharp(file).ensureAlpha().raw().toBuffer({resolveWithObject:true}),raw=extractBody(data,info.width,info.height),f=face(raw,info.width,info.height,hint),b=bounds(raw,info.width,info.height),s=Math.sqrt(targetArea/(sourceFaceArea||f.area));
 let tx=target[0],ty=target[1];if(ground!==null){let foot=0;for(let y=Math.ceil(f.y);y<info.height;y++)for(let x=0;x<f.x-10;x++)if(raw[(y*info.width+x)*4+3]>64)foot=Math.max(foot,y);ty=ground-(foot-f.y)*s;}if(wall!==null)tx=wall-(b[2]-f.x)*s;
 const predicted=[tx+(b[0]-f.x)*s,ty+(b[1]-f.y)*s,tx+(b[2]-f.x)*s,ty+(b[3]-f.y)*s];
 if(predicted[2]-predicted[0]>496||predicted[3]-predicted[1]>496)throw Error('Anatomical scale cannot fit frame '+file);
 const shift=[predicted[0]<8?8-predicted[0]:predicted[2]>504?504-predicted[2]:0,predicted[1]<8?8-predicted[1]:predicted[3]>504?504-predicted[3]:0];tx+=shift[0];ty+=shift[1];
 const image=await loadImage(await sharp(raw,{raw:{width:info.width,height:info.height,channels:4}}).png().toBuffer()),c=createCanvas(512,512),ctx=c.getContext('2d');ctx.translate(tx,ty);ctx.scale(s,s);ctx.drawImage(image,-f.x,-f.y);
 const png=c.toBuffer('image/png'),output=await sharp(png).ensureAlpha().raw().toBuffer(),outBounds=bounds(output,512,512);
 if(outBounds[0]<3||outBounds[1]<3||outBounds[2]>508||outBounds[3]>508)throw Error('Frame clipping '+file+' '+outBounds);
 return {png,raw:output,calibration:{sourceSize:[info.width,info.height],sourceFace:f,scale:s,targetFace:[tx,ty],registrationShift:shift,pivotTopLeft:[270+shift[0],286+shift[1]],bounds:outBounds}};
}
module.exports={sharp,createCanvas,loadImage,extractBody,bounds,face,normalize};

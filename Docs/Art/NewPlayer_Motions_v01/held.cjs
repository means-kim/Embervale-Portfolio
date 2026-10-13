const fs=require('fs'),path=require('path'),crypto=require('crypto'),{sharp,face,bounds,createCanvas,loadImage}=require('./art-utils.cjs');
const root=path.resolve(__dirname,'../../..'),asset='Assets/04.Sprites/NewPlayer/Motions_v01',W=512,H=512;
const masters=require('./MasterCalibration.json'),before=require('./BeforeInventory.json');const hash=b=>crypto.createHash('sha256').update(b).digest('hex'),clamp=(x,a,b)=>Math.max(a,Math.min(b,x));function smooth(a,b,x){let t=clamp((x-a)/(b-a),0,1);return t*t*(3-2*t);}
function masks(src,name){const f=face(src,W,H,name==='Dash'?[.77,.52]:name==='Glide'?[.58,.44]:[.58,.38]),dark=new Uint8Array(W*H),protect=new Uint8Array(W*H),weights=new Float32Array(W*H);
for(let y=0;y<H;y++)for(let x=0;x<W;x++){let j=(y*W+x)*4;if(src[j+3]>64&&src[j]<105&&src[j+1]<90&&src[j+2]<80)dark[y*W+x]=1;}
for(let y=0;y<H;y++)for(let x=0;x<W;x++)if(dark[y*W+x])for(let yy=Math.max(0,y-12);yy<=Math.min(H-1,y+12);yy++)for(let xx=Math.max(0,x-12);xx<=Math.min(W-1,x+12);xx++)if((xx-x)**2+(yy-y)**2<145)protect[yy*W+xx]=1;
for(let y=0;y<H;y++)for(let x=0;x<W;x++){const j=y*W+x;const fb=f.bounds;if(x>=fb[0]-5&&x<=fb[2]+5&&y>=fb[1]-5&&y<=fb[3]+5)protect[j]=1;
const bodyLock=(1-smooth(30,65,Math.abs(x-270)))*(1-smooth(50,105,Math.abs(y-295)));
let shape=name==='Dash'?1-smooth(280,370,x):Math.max(1-smooth(fb[1]-15,fb[1]+10,y),1-smooth(205,255,x),smooth(337,394,y)*.75);
if(name==='Glide')shape=Math.max(shape,1-smooth(122,176,y));
weights[j]=protect[j]?0:shape*(1-bodyLock)*smooth(2,10,Math.min(x,y,511-x,511-y));}
return {protect,weights,face:f};}
function render(src,t,weights,name){const phase=2*Math.PI*(t%1),dst=Buffer.from(src),off=new Float32Array(W*H*2);
for(let y=0;y<H;y++)for(let x=0;x<W;x++){const i=y*W+x,w=weights[i],p=phase-y*.02+x*.013;off[i*2]=w*(name==='Dash'?1.1:2.8)*Math.sin(p);off[i*2+1]=w*(name==='Dash'?2.3:.85)*Math.sin(p+.9);}
function offset(x,y){let xx=clamp(Math.round(x),0,511),yy=clamp(Math.round(y),0,511),i=(yy*W+xx)*2;return [off[i],off[i+1]];}
for(let y=0;y<H;y++)for(let x=0;x<W;x++){const i=y*W+x;if(weights[i]===0)continue;let sx=x,sy=y;for(let k=0;k<3;k++){const [dx,dy]=offset(sx,sy);sx=x-dx;sy=y-dy;}const x0=Math.floor(sx),y0=Math.floor(sy),fx=sx-x0,fy=sy-y0;let a=0,r=0,g=0,b=0;for(const [xx,yy,w]of [[x0,y0,(1-fx)*(1-fy)],[x0+1,y0,fx*(1-fy)],[x0,y0+1,(1-fx)*fy],[x0+1,y0+1,fx*fy]]){if(xx<0||yy<0||xx>=W||yy>=H)continue;const j=(yy*W+xx)*4,v=src[j+3]/255*w;a+=v;r+=src[j]*v;g+=src[j+1]*v;b+=src[j+2]*v;}const j=i*4;dst[j]=a?Math.round(r/a):0;dst[j+1]=a?Math.round(g/a):0;dst[j+2]=a?Math.round(b/a):0;dst[j+3]=Math.round(a*255);}
return dst;}
async function main(){let manifest={},qa={};for(const [name,m]of Object.entries(masters)){const src=await sharp(path.join(root,m.path)).ensureAlpha().raw().toBuffer(),mask=masks(src,name),n=name==='Dash'?16:32,duration=name==='Dash'?.2:1.6,old=before.states.find(s=>s.state.endsWith('/'+name));let pngs=[],frames=[],changed=0;const out=path.join(root,asset,name);fs.mkdirSync(path.join(out,'Frames'),{recursive:true});fs.mkdirSync(path.join(out,'Sheets'),{recursive:true});
let first;for(let i=0;i<n;i++){const raw=render(src,i/n,mask.weights,name);if(i===0)first=raw;for(let p=0;p<W*H;p++)if(mask.protect[p])for(let c=0;c<4;c++)if(raw[p*4+c]!==src[p*4+c])changed++;const b=bounds(raw,W,H);if(b[0]<2||b[1]<2||b[2]>509||b[3]>509)throw Error(name+' clipping');const png=await sharp(raw,{raw:{width:W,height:H,channels:4}}).png().toBuffer(),p=asset+'/'+name+'/Frames/'+name+'_'+String(i).padStart(2,'0')+'.png';fs.writeFileSync(path.join(root,p),png);pngs.push(png);frames.push({path:p,time:i*duration/n,pivotX:m.pivotTopLeft[0]/512,pivotY:1-m.pivotTopLeft[1]/512,sha256:hash(png),bounds:b});}
const closure=render(src,1,mask.weights,name);let closureChanged=0;for(let j=0;j<closure.length;j++)if(closure[j]!==first[j])closureChanged++;
// Use a fundamental-frequency term so the 32/16 exported frames are all unique.
qa[name]={frames:n,unique:new Set(frames.map(f=>f.sha256)).size,protectedChanged:changed,closureChanged,face:mask.face};if(changed||closureChanged)throw Error(name+' protected/closure mismatch');
const sheet=await sharp({create:{width:4096,height:512*Math.ceil(n/8),channels:4,background:{r:0,g:0,b:0,alpha:0}}}).composite(pngs.map((input,i)=>({input,left:i%8*512,top:Math.floor(i/8)*512}))).png().toBuffer();fs.writeFileSync(path.join(out,'Sheets',name+'_Grid512.png'),sheet);
manifest[name]={name,oldState:name,clipPath:old.path,duration,sampleRate:n/duration,loop:true,stateSpeed:old.stateSpeed,method:'One approved-style pose with local periodic flame deformation; mask and charcoal limbs pixel-locked',frames};}
fs.writeFileSync(path.join(__dirname,'HeldManifest.json'),JSON.stringify(manifest,null,2));fs.writeFileSync(path.join(__dirname,'HeldPixelVerification.json'),JSON.stringify(qa,null,2));console.log(qa);}
main().catch(e=>{console.error(e);process.exit(1)});


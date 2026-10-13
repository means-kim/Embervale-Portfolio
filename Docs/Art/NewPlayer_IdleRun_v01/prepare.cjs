const fs=require('fs'),path=require('path'),crypto=require('crypto');
const deps='C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules';
const sharp=require(path.join(deps,'sharp')),{createCanvas,loadImage}=require(path.join(deps,'@napi-rs/canvas'));
const {face,extractBody}=require('../NewPlayer_Run_v02/sprite-metrics.cjs');
const root=path.resolve(__dirname,'../../..'),hash=b=>crypto.createHash('sha256').update(b).digest('hex');
const walk=d=>fs.readdirSync(d,{withFileTypes:true}).flatMap(e=>e.isDirectory()?walk(path.join(d,e.name)):[path.join(d,e.name)]);
async function main(){
 const baseline=path.join(__dirname,'PreservationBaseline.json');
 if(!fs.existsSync(baseline)){
 const dirs=['Assets/04.Sprites/Player','Assets/04.Sprites/NewPlayer','Assets/05.Animation/player','Assets/05.Animation/PlayerAttack','Assets/02.Scripts/Player','Assets/03.Prefabs/Player','Assets/01.Scenes'];
 fs.writeFileSync(baseline,JSON.stringify(dirs.flatMap(d=>walk(path.join(root,d))).map(p=>({path:path.relative(root,p).replaceAll('\\','/'),sha256:hash(fs.readFileSync(p))})),null,2));
 for(const name of ['Idle','Run'])fs.copyFileSync(path.join(root,'Assets/05.Animation/player',name+'.anim'),path.join(__dirname,name+'_Before.anim.txt'));
 }
 const original=path.join(root,'Assets/04.Sprites/Player/GA1_Player_Idle_v3/Frames/Idle_00.png'),run=path.join(root,'Assets/04.Sprites/NewPlayer/Run_v02/Frames/Run_00.png');
 const idleFace=face(await sharp(original).ensureAlpha().raw().toBuffer(),512,512),runFace=face(await sharp(run).ensureAlpha().raw().toBuffer(),512,512);
 const {data,info}=await sharp(path.join(__dirname,'Idle_Master_Generated.png')).ensureAlpha().raw().toBuffer({resolveWithObject:true}),raw=extractBody(data,info.width,info.height),mf=face(raw,info.width,info.height),scale=Math.sqrt(runFace.area/mf.area);
 let bottom=0;for(let p=0;p<info.width*info.height;p++)if(raw[p*4+3]>128)bottom=Math.max(bottom,Math.floor(p/info.width));
 // Fixed floor at original 410; match run mask area without stretching art.
 const tx=idleFace.x,ty=410-(bottom-mf.y)*scale;
 const img=await loadImage(await sharp(raw,{raw:{width:info.width,height:info.height,channels:4}}).png().toBuffer()),c=createCanvas(512,512),ctx=c.getContext('2d');ctx.translate(tx,ty);ctx.scale(scale,scale);ctx.drawImage(img,-mf.x,-mf.y);
 const out=path.join(root,'Assets/04.Sprites/NewPlayer/Design_v01');fs.mkdirSync(out,{recursive:true});fs.writeFileSync(path.join(out,'Player_Idle_Master_v01.png'),c.toBuffer('image/png'));
 const record={sourceSize:[info.width,info.height],originalIdleFace:idleFace,runFace,masterSourceFace:mf,scale,targetFace:[tx,ty],targetFeetY:410,pivot:[270,286],frame:[512,512],ppu:200};fs.writeFileSync(path.join(__dirname,'MasterCalibration.json'),JSON.stringify(record,null,2));
 const comparison=createCanvas(1536,512),cx=comparison.getContext('2d');cx.fillStyle='#17263b';cx.fillRect(0,0,1536,512);for(const [i,file]of [original,path.join(out,'Player_Idle_Master_v01.png'),run].entries())cx.drawImage(await loadImage(file),i*512,0);fs.writeFileSync(path.join(__dirname,'Master_Comparison.png'),comparison.toBuffer('image/png'));
 console.log(JSON.stringify(record));
}main().catch(e=>{console.error(e);process.exit(1)});

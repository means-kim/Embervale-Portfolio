const fs=require('fs'),path=require('path'),crypto=require('crypto');
const {createCanvas}=require('C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/@napi-rs/canvas');
const root=path.resolve(__dirname,'../../..');
const walk=d=>fs.existsSync(d)?fs.readdirSync(d,{withFileTypes:true}).flatMap(e=>e.isDirectory()?walk(path.join(d,e.name)):[path.join(d,e.name)]):[];
const hash=b=>crypto.createHash('sha256').update(b).digest('hex');
const baseline=path.join(__dirname,'PreservationBaseline.json');
if(!fs.existsSync(baseline)){
 const dirs=['Assets/04.Sprites/Player','Assets/04.Sprites/NewPlayer','Assets/04.Sprites/Boss','Assets/05.Animation/player','Assets/05.Animation/PlayerAttack','Assets/02.Scripts/Player','Assets/03.Prefabs/Player','Assets/07.Datas/Player','Assets/01.Scenes'];
 const files=dirs.flatMap(d=>walk(path.join(root,d)));
 fs.writeFileSync(baseline,JSON.stringify(files.map(p=>({path:path.relative(root,p).replaceAll('\\','/'),sha256:hash(fs.readFileSync(p))})),null,2));
 for(const file of files.filter(f=>/\.(cs|anim|controller|asset)(\.meta)?$/.test(f))){const target=path.join(__dirname,'Before',path.relative(root,file)+'.txt');fs.mkdirSync(path.dirname(target),{recursive:true});fs.copyFileSync(file,target);}
}
function lerp(a,b,t){return a.map((v,i)=>v+(b[i]-v)*t);}
function fireball(i){
 const recoil=[0,.25,.7,1,.95,.8,.65,.5,.3,.15,.05,0][i];
 const recovery=Math.max(0,(i-3)/8),hx=275-10*recoil,hy=307+3*recoil;
 return {head:[300-8*recoil,201+2*recoil],hip:[hx,hy+28],torso:[hx,hy-31],nearShoulder:[258-8*recoil,256],farShoulder:[284-8*recoil,256],nearElbow:lerp([307-28*recoil,276-9*recoil],[230,290],recovery),nearHand:lerp([362-63*recoil,252-12*recoil],[237,328],recovery),farElbow:lerp([275-12*recoil,288],[308,297],recovery),farHand:lerp([308-18*recoil,283],[316,327],recovery),nearKnee:lerp([306,373],[284,370],recovery),nearFoot:lerp([343,410],[312,410],recovery),farKnee:lerp([230,370],[236,370],recovery),farFoot:lerp([194,410],[220,410],recovery),label:['RELEASE','FLASH','RECOIL','RECOIL','RECOVER','RECOVER','RECOVER','RECOVER','RELAX','RELAX','IDLE','IDLE'][i]};
}
function drawPose(ctx,p,index){
 const line=(pts,color,width)=>{ctx.strokeStyle=color;ctx.lineWidth=width;ctx.lineJoin='round';ctx.lineCap='round';ctx.beginPath();pts.forEach(([x,y],i)=>i?ctx.lineTo(x,y):ctx.moveTo(x,y));ctx.stroke();};
 ctx.strokeStyle='#cbd5e1';ctx.lineWidth=1;ctx.beginPath();ctx.moveTo(65,412);ctx.lineTo(455,412);ctx.stroke();
 line([p.hip,p.farKnee,p.farFoot],'#18a9cb',19);line([p.farShoulder,p.farElbow,p.farHand],'#18a9cb',13);
 ctx.fillStyle='#9baab9';ctx.beginPath();ctx.ellipse(...p.torso,30,60,.15,0,Math.PI*2);ctx.fill();
 ctx.fillStyle='#e6ebf1';ctx.beginPath();ctx.ellipse(...p.head,42,52,-.25,0,Math.PI*2);ctx.fill();ctx.fillStyle='#29333e';ctx.beginPath();ctx.ellipse(p.head[0]+20,p.head[1]+8,6,14,.1,0,Math.PI*2);ctx.fill();
 line([p.hip,p.nearKnee,p.nearFoot],'#ce287e',21);line([p.nearShoulder,p.nearElbow,p.nearHand],'#ce287e',14);
 ctx.fillStyle='#ce287e';ctx.beginPath();ctx.arc(...p.nearHand,10,0,Math.PI*2);ctx.fill();
 if(index<3){ctx.strokeStyle='#f59b2d';ctx.lineWidth=3;ctx.beginPath();ctx.moveTo(p.nearHand[0]+12,p.nearHand[1]);ctx.lineTo(p.nearHand[0]+40,p.nearHand[1]);ctx.stroke();}
 ctx.fillStyle='#273849';ctx.font='20px sans-serif';ctx.fillText(String(index).padStart(2,'0')+' '+p.label,22,32);
}
const poses=Array.from({length:12},(_,i)=>fireball(i)),c=createCanvas(2048,1536),ctx=c.getContext('2d');ctx.fillStyle='#f6f8fb';ctx.fillRect(0,0,c.width,c.height);
poses.forEach((p,i)=>{ctx.save();ctx.translate(i%4*512,Math.floor(i/4)*512);drawPose(ctx,p,i);ctx.restore();});
fs.writeFileSync(path.join(__dirname,'FireBall_PoseGuide.png'),c.toBuffer('image/png'));fs.writeFileSync(path.join(__dirname,'FireBall_Poses.json'),JSON.stringify(poses,null,2));
console.log('Baseline/backups and 12-frame FireBall pose guide saved.');

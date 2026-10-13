const fs=require('fs'),path=require('path'),{createCanvas}=require('./art-utils.cjs');
const lerp=(a,b,t)=>a.map((x,i)=>x+(b[i]-x)*t), mix=(a,b,t)=>Object.fromEntries(Object.keys(a).map(k=>[k,Array.isArray(a[k])?lerp(a[k],b[k],t):a[k]]));
const base={head:[297,194],hip:[265,325],torso:[267,276],nearShoulder:[247,247],farShoulder:[283,250],nearElbow:[233,285],nearHand:[239,327],farElbow:[303,291],farHand:[317,325],nearKnee:[285,366],nearFoot:[310,410],farKnee:[239,366],farFoot:[215,410]};
const rise={...base,head:[297,186],nearElbow:[226,280],nearHand:[183,259],farElbow:[331,274],farHand:[365,249],nearKnee:[315,323],nearFoot:[324,353],farKnee:[252,382],farFoot:[270,426]};
const fall={...base,head:[297,195],nearElbow:[223,236],nearHand:[191,206],farElbow:[339,236],farHand:[361,207],nearKnee:[284,370],nearFoot:[307,410],farKnee:[242,368],farFoot:[249,410]};
function armAngle(p,angle,length=100){const r=angle*Math.PI/180,hand=[270+Math.cos(r)*length,260+Math.sin(r)*length];p.nearHand=hand;p.nearElbow=lerp(p.nearShoulder,hand,.55);p.blade=[hand,[hand[0]+Math.cos(r)*120,hand[1]+Math.sin(r)*120]];return p;}
const specs={JumpApex:8,Hit:8,DoubleJump:8,FirstAttack:12,SecondAttack:16,ThirdAttack:16,HorizontalAttack:16};
const all={};
for(const [name,n]of Object.entries(specs)){
const poses=[];
for(let i=0;i<n;i++){let t=i/(n-1),p=JSON.parse(JSON.stringify(base));
if(name==='JumpApex')p=mix(rise,fall,t);
else if(name==='Hit'){const q=Math.sin(Math.PI*t)*(1-t*.45);for(const k of Object.keys(p)){p[k][0]-=(410-p[k][1])*.15*q;p[k][1]+=4*q;}p.nearHand=lerp(p.nearHand,[255,269],q);p.farHand=lerp(p.farHand,[301,263],q);}
else if(name==='DoubleJump'){const q=Math.sin(Math.PI*t);p=mix(rise,{...base,head:[292,240],torso:[272,281],hip:[267,316],nearHand:[320,294],nearElbow:[284,297],farHand:[299,296],farElbow:[308,271],nearKnee:[313,286],nearFoot:[292,330],farKnee:[286,300],farFoot:[246,328]},q);p.rotation=0;}
else{
const active=Math.min(1,t/.65),recover=Math.max(0,(t-.65)/.35);
const angles=name==='FirstAttack'?[-65,90]:name==='SecondAttack'?[50,-125]:name==='ThirdAttack'?[-120,85]:[-35,35];
armAngle(p,angles[0]+(angles[1]-angles[0])*active,name==='HorizontalAttack'?112:88);
p.head[0]+=Math.sin(t*Math.PI)*7;p.torso[0]+=Math.sin(t*Math.PI)*7;
p.farHand=[247,292];p.farElbow=[262,282];
p.nearFoot=[336,410];p.nearKnee=[312,374];p.farFoot=[196,410];
if(recover>0){const blade=p.blade;p=mix({...p,blade:undefined},{...base,blade:undefined},recover);if(recover<.45)p.blade=blade;else delete p.blade;}
}
poses.push(p);}
all[name]=poses;const rows=Math.ceil(n/4),c=createCanvas(2048,512*rows),ctx=c.getContext('2d');ctx.fillStyle='#f3f6fa';ctx.fillRect(0,0,c.width,c.height);
poses.forEach((p,i)=>{ctx.save();ctx.translate(i%4*512,Math.floor(i/4)*512);function line(v,col,w){ctx.strokeStyle=col;ctx.lineWidth=w;ctx.lineCap='round';ctx.lineJoin='round';ctx.beginPath();v.forEach(([x,y],j)=>j?ctx.lineTo(x,y):ctx.moveTo(x,y));ctx.stroke();}
line([p.hip,p.farKnee,p.farFoot],'#1bacc7',18);line([p.farShoulder,p.farElbow,p.farHand],'#1bacc7',13);
ctx.fillStyle='#99aabb';ctx.beginPath();ctx.ellipse(...p.torso,30,58,0,0,Math.PI*2);ctx.fill();ctx.fillStyle='#e4e9ef';ctx.beginPath();ctx.ellipse(...p.head,43,50,-.22,0,Math.PI*2);ctx.fill();ctx.fillStyle='#243241';ctx.fillRect(p.head[0]+19,p.head[1],7,18);
line([p.hip,p.nearKnee,p.nearFoot],'#cb287e',20);line([p.nearShoulder,p.nearElbow,p.nearHand],'#cb287e',14);
if(p.blade)line(p.blade,'#ee8d25',12);
ctx.fillStyle='#354358';ctx.font='20px sans-serif';ctx.fillText(name+' '+String(i).padStart(2,'0'),20,30);ctx.restore();});
fs.writeFileSync(path.join(__dirname,name+'_PoseGuide.png'),c.toBuffer('image/png'));}
fs.writeFileSync(path.join(__dirname,'MotionPoses.json'),JSON.stringify(all,null,2));


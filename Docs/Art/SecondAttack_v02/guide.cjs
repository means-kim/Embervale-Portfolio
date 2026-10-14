const fs=require('fs'),path=require('path'),{createCanvas}=require('../NewPlayer_Motions_v01/art-utils.cjs');
// A circular upswing passes in front of the body and continues above/behind the head.
// The hand path is intentionally different from a straight rising punch.
const hands=[[239,327],[225,345],[224,369],[284,378],[362,343],[410,276],[417,197],[389,126],[338,85],[288,80],[243,103],[207,163],[202,240],[222,292],[237,320],[239,327]];
const elbows=[[230,288],[216,294],[210,318],[233,319],[290,303],[335,269],[338,229],[332,190],[302,161],[271,161],[236,168],[220,202],[212,239],[220,269],[230,285],[230,288]];
const headY=[194,200,207,206,202,195,188,185,185,188,190,192,194,194,194,194];
const phases=['READY','COIL','LOW WINDUP','SWEEP START','LOW FORWARD','UPSWING','UPSWING','ABOVE FACE','OVERHEAD','FOLLOW THROUGH','FINISH ARC','RECOVER','RECOVER','SETTLE','READY','READY'];
const c=createCanvas(2048,2048),x=c.getContext('2d');x.fillStyle='#edf2f7';x.fillRect(0,0,2048,2048);let poses=[];
for(let i=0;i<16;i++){x.save();x.translate(i%4*512,Math.floor(i/4)*512);const lean=[0,-2,-6,-3,3,8,10,8,5,2,0,-2,-1,0,0,0][i],head=[297+lean,headY[i]],hip=[265,325],shoulder=[247+lean,247+(headY[i]-194)*.5],hand=hands[i],elbow=elbows[i];
function line(points,col,w){x.strokeStyle=col;x.lineWidth=w;x.lineCap='round';x.lineJoin='round';x.beginPath();points.forEach((p,j)=>j?x.lineTo(...p):x.moveTo(...p));x.stroke();}
line([hip,[239,366],[206,410]],'#4c819e',18);line([[283+lean,250],[304,286],[290,305]],'#4c819e',13);
x.fillStyle='#99a9ba';x.beginPath();x.ellipse(267+lean,280,30,55,lean*.012,0,Math.PI*2);x.fill();
x.fillStyle='#fff6df';x.beginPath();x.ellipse(...head,43,50,-.22,0,Math.PI*2);x.fill();x.fillStyle='#243241';x.fillRect(head[0]+19,head[1],7,18);
line([hip,[285,366],[320,410]],'#9b4478',20);line([shoulder,elbow,hand],'#9b4478',15);x.fillStyle='#4c2040';x.beginPath();x.arc(...hand,12,0,Math.PI*2);x.fill();
if(i>=2&&i<=11){x.strokeStyle='#ef821b';x.lineWidth=6;x.beginPath();x.ellipse(...hand,18,19,0,0,Math.PI*2);x.stroke();}
x.fillStyle='#344357';x.font='bold 20px sans-serif';x.fillText(String(i).padStart(2,'0')+' '+phases[i],20,30);x.strokeStyle='#d1dbe7';x.lineWidth=1;x.strokeRect(1,1,510,510);x.restore();poses.push({hand,elbow,head,shoulder});}
fs.writeFileSync(path.join(__dirname,'PoseGuide.png'),c.toBuffer('image/png'));fs.writeFileSync(path.join(__dirname,'Poses.json'),JSON.stringify(poses,null,2));

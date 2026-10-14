const fs=require('fs'),path=require('path'),{createCanvas}=require('../NewPlayer_Motions_v01/art-utils.cjs');
const hands=[[239,327],[233,303],[210,286],[199,278],[237,264],[326,261],[393,263],[416,266],[421,272],[406,281],[375,287],[330,293],[282,304],[250,316],[241,324],[239,327]];
const elbows=[[230,288],[218,275],[208,261],[208,251],[220,247],[281,259],[335,263],[350,263],[354,268],[341,276],[313,279],[274,281],[247,279],[235,283],[231,288],[230,288]];
const phases=['READY','CLENCH','WINDUP','WINDUP','START','SWING','STRIKE','STRIKE','FOLLOW','FOLLOW','RETRACT','RETRACT','RECOVER','RECOVER','READY','READY'];
const c=createCanvas(2048,2048),x=c.getContext('2d');x.fillStyle='#ecf1f7';x.fillRect(0,0,2048,2048);
let poses=[];for(let i=0;i<16;i++){x.save();x.translate(i%4*512,Math.floor(i/4)*512);let q=Math.sin(Math.PI*i/15),lean=q*9,head=[297+lean,194+q*3],hip=[265,325],shoulder=[247+lean,247],hand=hands[i],elbow=elbows[i];
function line(points,col,w){x.strokeStyle=col;x.lineWidth=w;x.lineCap='round';x.lineJoin='round';x.beginPath();points.forEach((p,j)=>j?x.lineTo(...p):x.moveTo(...p));x.stroke();}
line([hip,[239,366],[206,410]],'#5482a3',18);line([[283+lean,250],[303,281],[291,300]],'#5482a3',13);
x.fillStyle='#9dabbc';x.beginPath();x.ellipse(267+lean,280,30,55,-q*.10,0,Math.PI*2);x.fill();
x.fillStyle='#fff6df';x.beginPath();x.ellipse(...head,43,50,-.22,0,Math.PI*2);x.fill();x.fillStyle='#243241';x.fillRect(head[0]+19,head[1],7,18);
line([hip,[285,366],[320,410]],'#9b4478',20);line([shoulder,elbow,hand],'#9b4478',15);x.fillStyle='#4c2040';x.beginPath();x.arc(...hand,12,0,Math.PI*2);x.fill();
if(i>1&&i<13){x.strokeStyle='#ef821b';x.lineWidth=6;x.beginPath();x.ellipse(...hand,18,19,0,0,Math.PI*2);x.stroke();}
x.fillStyle='#344357';x.font='bold 21px sans-serif';x.fillText(String(i).padStart(2,'0')+' '+phases[i],20,32);x.strokeStyle='#d1dbe7';x.lineWidth=1;x.strokeRect(1,1,510,510);x.restore();poses.push({hand,elbow,head});}
fs.writeFileSync(path.join(__dirname,'PoseGuide.png'),c.toBuffer('image/png'));fs.writeFileSync(path.join(__dirname,'Poses.json'),JSON.stringify(poses,null,2));

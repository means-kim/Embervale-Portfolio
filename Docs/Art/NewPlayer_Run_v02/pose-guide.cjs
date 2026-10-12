const fs=require('fs'),path=require('path');
const {createCanvas}=require('C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/@napi-rs/canvas');
// Motion planning diagram, not final art. Camera-near limb is magenta,
// camera-far limb is cyan; opposite limbs remain distinguishable across a cycle.
const legs=[[[295,361],[326,414]],[[283,371],[306,414]],[[264,370],[275,414]],[[241,360],[239,410]],[[221,350],[203,391]],[[235,328],[208,340]],[[278,320],[253,355]],[[308,337],[310,387]]];
function interp(points,phase){const n=points.length,x=(phase%1)*n,i=Math.floor(x),t=x-i;return points[i].map((p,k)=>p.map((v,d)=>v*(1-t)+points[(i+1)%n][k][d]*t));}
const poses=[];
for(let i=0;i<16;i++){
 const phase=i/16,bob=4*Math.sin(4*Math.PI*phase),hip=[252,325+bob];
 const nearLeg=interp(legs,phase),farLeg=interp(legs,(phase+.5)%1);
 // Elbows and hands follow a repeatable swing, exactly half a cycle apart.
 const arm=(p,far)=>{const x=Math.cos(p*2*Math.PI);return {shoulder:[far?278:256,255+bob],elbow:[257-46*x,287+bob],hand:[275-53*x,258+bob-5*x]};};
 poses.push({index:i,bob,hip,nearLeg,farLeg,nearArm:arm(phase,false),farArm:arm((phase+.5)%1,true)});
}
function drawPose(ctx,p){
 ctx.strokeStyle='#c8d0d7';ctx.lineWidth=1;ctx.beginPath();ctx.moveTo(75,420);ctx.lineTo(435,420);ctx.stroke();
 const limb=(ps,color,width)=>{ctx.strokeStyle=color;ctx.lineWidth=width;ctx.lineJoin='round';ctx.lineCap='round';ctx.beginPath();ps.forEach(([x,y],i)=>i?ctx.lineTo(x,y):ctx.moveTo(x,y));ctx.stroke();};
 const arm=(a,color)=>{limb([a.shoulder,a.elbow,a.hand],color,13);ctx.fillStyle=color;ctx.beginPath();ctx.arc(...a.hand,10,0,Math.PI*2);ctx.fill();};
 limb([p.hip,...p.farLeg],'#16a3c9',18);arm(p.farArm,'#16a3c9');
 ctx.fillStyle='#b5bec7';ctx.beginPath();ctx.ellipse(269,284+p.bob,32,55,.35,0,Math.PI*2);ctx.fill();
 ctx.fillStyle='#e1e5e8';ctx.beginPath();ctx.ellipse(300,205+p.bob,43,53,-.38,0,Math.PI*2);ctx.fill();
 ctx.fillStyle='#414c57';ctx.beginPath();ctx.ellipse(321,212+p.bob,7,16,.3,0,Math.PI*2);ctx.fill();
 limb([p.hip,...p.nearLeg],'#cc297e',20);arm(p.nearArm,'#cc297e');
 ctx.fillStyle='#293441';ctx.font='20px sans-serif';ctx.fillText(String(p.index).padStart(2,'0'),30,37);
}
function sheet(indices,file,cols){const rows=Math.ceil(indices.length/cols),c=createCanvas(cols*512,rows*512),ctx=c.getContext('2d');ctx.fillStyle='#f8fafc';ctx.fillRect(0,0,c.width,c.height);indices.forEach((i,k)=>{ctx.save();ctx.translate(k%cols*512,Math.floor(k/cols)*512);drawPose(ctx,poses[i]);ctx.restore();});fs.writeFileSync(path.join(__dirname,file),c.toBuffer('image/png'));}
sheet(poses.map(p=>p.index),'PoseGuide_16.png',4);
for(let g=0;g<4;g++)sheet([g*4,g*4+1,g*4+2,g*4+3],`PoseGuide_Group_${g+1}.png`,2);
fs.writeFileSync(path.join(__dirname,'PosePlan.json'),JSON.stringify(poses,null,2));

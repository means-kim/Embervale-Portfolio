const fs=require('fs'),path=require('path'),sharp=require('sharp'),{spawnSync}=require('child_process');
const {createCanvas,loadImage}=require('@napi-rs/canvas');
const ROOT=path.resolve(__dirname,'..'),TMP=path.join(ROOT,'BuildPreview');
const data=JSON.parse(fs.readFileSync(path.join(ROOT,'Motions.json'))),clips={};
function encode(input,output,fps=20){const r=spawnSync('ffmpeg',['-hide_banner','-loglevel','error','-y','-framerate',String(fps),'-i',input,'-filter_complex','[0:v]split[a][b];[a]palettegen=stats_mode=full[p];[b][p]paletteuse=dither=sierra2_4a','-loop','0',...(fps===48?['-final_delay','2']:[]),output],{encoding:'utf8'});if(r.status!==0)throw Error(r.stderr)}
function frame(name,t){const c=clips[name];t=c.loop?t%c.duration:Math.min(t,c.duration);let n=0;while(n+1<c.times.length&&c.times[n+1]<=t+1e-6)n++;return c.images[n]}
function dark(ctx,w,h){const g=ctx.createLinearGradient(0,0,0,h);g.addColorStop(0,'#0b1628');g.addColorStop(1,'#162b43');ctx.fillStyle=g;ctx.fillRect(0,0,w,h)}
async function main(){
 fs.mkdirSync(TMP,{recursive:true});
 for(const spec of data.clips){clips[spec.name]={...spec,images:await Promise.all(spec.frames.map(f=>loadImage(path.join(ROOT,f))))};}
 const idle=await loadImage(path.join(ROOT,'Source','Idle_Master512.png'));
 // Per-state GIFs are inspection replays, not new timing data. Unity uses Motions.json.
 for(const c of Object.values(clips)){
  const dir=path.join(TMP,c.name);fs.mkdirSync(dir,{recursive:true});
  const duration=c.loop?Math.max(c.duration*2,1):c.duration+.4;
  const previewFPS=c.name==='Run'?48:20;
  const n=Math.ceil(duration*previewFPS);
  for(let i=0;i<n;i++){
   const canvas=createCanvas(512,512),ctx=canvas.getContext('2d');dark(ctx,512,512);
   ctx.strokeStyle='#43536b';ctx.beginPath();ctx.moveTo(24,410.5);ctx.lineTo(488,410.5);ctx.stroke();
   if(c.name.startsWith('Wall')){ctx.strokeStyle='#70859f';ctx.beginPath();ctx.moveTo(350.5,45);ctx.lineTo(350.5,455);ctx.stroke()}
   ctx.drawImage(frame(c.name,i/previewFPS),0,0);
   ctx.fillStyle='#d8e4f3';ctx.font='bold 18px sans-serif';ctx.fillText(c.name,24,468);
   ctx.fillStyle='#91a8c4';ctx.font='13px sans-serif';ctx.fillText(`${c.frameCount} frames | ${c.duration.toFixed(3)} s | ${c.loop?'loop':'one shot'}`,24,491);
   fs.writeFileSync(path.join(dir,`f_${String(i).padStart(3,'0')}.png`),canvas.toBuffer('image/png'));
  }
  encode(path.join(dir,'f_%03d.png'),path.join(ROOT,'Previews',c.name+'.gif'),previewFPS);
 }
 const overview=path.join(TMP,'Overview');fs.mkdirSync(overview,{recursive:true});
 const names=['RUN','JUMP + LAND','DOUBLE JUMP','DASH','ATTACK','HURT','WALL CLIMB','WALL CLING / SLIDE','WALL JUMP'];
 const subtitles=['8 original + 2 boundary frames','start / rise / apex / fall / land','tuck / 360-degree roll / unfold','start / hold / end','windup / active / recovery','recoil / recovery','4 poses / 8 fps','separate looping clips','prepare / push away'];
 const scene=(i,t)=>{
  const p=t%2;
  if(i===0)return[frame('Run',t),0,0];
  if(i===1){let name=null,k=0,dy=0;
   if(p>=.25&&p<.35){name='JumpStart';k=p-.25}
   else if(p>=.35&&p<.70){name='JumpRise';k=p-.35}
   else if(p>=.70&&p<.87){name='JumpApex';k=p-.70}
   else if(p>=.87&&p<1.25){name='Fall';k=p-.87}
   else if(p>=1.25&&p<1.384){name='Land';k=p-1.25}
   if(p>=.35&&p<1.25)dy=-125*Math.sin((p-.35)/.9*Math.PI);
   return[name?frame(name,k):idle,0,dy];
  }
  if(i===2){let name=null,k=0,dy=0;
   if(p>=.25&&p<.45){name='JumpRise';k=p-.25;dy=-50*(p-.25)/.2}
   else if(p>=.45&&p<.895){name='DoubleJump';k=p-.45;dy=-50-45*Math.sin((p-.45)/.445*Math.PI)}
   else if(p>=.895&&p<1.25){name='Fall';k=p-.895;dy=-50*(1-(p-.895)/.355)}
   else if(p>=1.25&&p<1.384){name='Land';k=p-1.25}
   return[name?frame(name,k):idle,0,dy];
  }
  if(i===3){let name=null,k=0,dx=0;if(p>=.45&&p<.5){name='DashStart';k=p-.45;dx=-55}else if(p>=.5&&p<.7){name='DashLoop';k=p-.5;dx=-55+110*(p-.5)/.2}else if(p>=.7&&p<.8){name='DashEnd';k=p-.7;dx=55}else if(p>=.8&&p<1.5)dx=55;return[name?frame(name,k):idle,dx,0]}
  if(i===4){let q=t%1;return[q>=.2&&q<.56?frame('Attack',q-.2):idle,0,0]}
  if(i===5){let q=t%1.3;return[q>=.35&&q<.517?frame('Hurt',q-.35):idle,0,0]}
  if(i===6)return[frame('WallClimb',t),0,0];
  if(i===7)return[frame(p<1?'WallCling':'WallSlide',p),0,0];
  if(i===8){if(p<.6)return[frame('WallCling',p),0,0];if(p<.7)return[frame('WallJump',p-.6),-(p-.6)*400,-(p-.6)*350];if(p<1.2)return[frame('Fall',p-.7),-40-80*(p-.7),-35+35*(p-.7)/.5];return[idle,-80,0]}
 };
 for(let n=0;n<80;n++){
  const canvas=createCanvas(1200,1056),ctx=canvas.getContext('2d');dark(ctx,1200,1056);
  ctx.fillStyle='#edf3fb';ctx.font='bold 26px sans-serif';ctx.fillText('GA1 / PLAYER MOTIONS v4',30,39);
  ctx.fillStyle='#9bb2ce';ctx.font='14px sans-serif';ctx.fillText('Fixed scale + pivot  /  Idle v3 companion  /  Gameplay movement below is illustrative',30,63);
  for(let i=0;i<9;i++){
   const x=(i%3)*400,y=88+Math.floor(i/3)*316;
   ctx.strokeStyle='#263b55';ctx.strokeRect(x+10.5,y+4.5,379,300);
   ctx.fillStyle='#d8e6f7';ctx.font='bold 16px sans-serif';ctx.fillText(names[i],x+24,y+30);
   ctx.fillStyle='#8da5c2';ctx.font='12px sans-serif';ctx.fillText(subtitles[i],x+24,y+51);
   const scale=.48,ground=y+271,ox=x+200-256*scale,oy=ground-410*scale;
   ctx.strokeStyle='#40556c';ctx.beginPath();ctx.moveTo(x+24,ground+.5);ctx.lineTo(x+374,ground+.5);ctx.stroke();
   if(i>=6){ctx.strokeStyle='#8194a9';ctx.lineWidth=3;ctx.beginPath();ctx.moveTo(ox+350*scale,y+72);ctx.lineTo(ox+350*scale,ground+13);ctx.stroke();ctx.lineWidth=1;}
   const [img,dx,dy]=scene(i,n/20);ctx.drawImage(img,ox+dx*scale,oy+dy*scale,512*scale,512*scale);
  }
  fs.writeFileSync(path.join(overview,`f_${String(n).padStart(3,'0')}.png`),canvas.toBuffer('image/png'));
 }
 encode(path.join(overview,'f_%03d.png'),path.join(ROOT,'Previews','Motions_Overview.gif'));
 fs.copyFileSync(path.join(overview,'f_013.png'),path.join(ROOT,'Previews','Motions_Overview.png'));
 console.log(JSON.stringify({previews:17,overviewSeconds:4}));
}
main().catch(e=>{console.error(e);process.exit(1)});

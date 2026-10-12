const fs=require('fs'),path=require('path'),{spawnSync}=require('child_process');
const deps='C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules';
const {createCanvas,loadImage}=require(path.join(deps,'@napi-rs/canvas'));
const root=path.resolve(__dirname,'../../..'),manifest=JSON.parse(fs.readFileSync(path.join(__dirname,'Manifest.json')));
async function main(){
 const p=JSON.parse(fs.readFileSync(path.join(root,'Docs/PlayerPivot_v01/Plan.json')));
 const old=Array.from({length:10},(_,i)=>{const name=`Run_${String(i).padStart(2,'0')}.png`,file='Assets/04.Sprites/Player/GA1_Player_Motions_v4/Frames/Run/'+name,anchor=p.find(x=>x.path===file).changes[0].anchorTopLeft;return {file,anchor,time:i<8?i/16:(i===8?22/48:23/48)};});
 const fresh=manifest.frames.map(f=>({file:f.path,time:f.time,anchor:[f.pivotX*512,(1-f.pivotY)*512]}));
 const load=async list=>Promise.all(list.map(async f=>({...f,image:await loadImage(path.join(root,f.file)),src:'data:image/png;base64,'+fs.readFileSync(path.join(root,f.file)).toString('base64')})));
 const oldFrames=await load(old),newFrames=await load(fresh);
 const idlePath='Assets/04.Sprites/Player/GA1_Player_Idle_v3/Frames/Idle_00.png';
 const idleAnchor=p.find(x=>x.path===idlePath).changes[0].anchorTopLeft;
 const idle={src:'data:image/png;base64,'+fs.readFileSync(path.join(root,idlePath)).toString('base64'),anchor:idleAnchor,time:0};
 function pick(list,t){if(t>=.5)return list[0];let i=0;while(i+1<list.length&&list[i+1].time<=t)i++;return list[i];}
 const dir=path.join(__dirname,'PreviewCache');fs.mkdirSync(dir,{recursive:true});
 for(let n=0;n<50;n++){
  const c=createCanvas(960,480),ctx=c.getContext('2d');ctx.fillStyle='#102035';ctx.fillRect(0,0,960,480);
  for(const [j,list]of [oldFrames,newFrames].entries()){
   const f=pick(list,(n/48)%manifest.cycleSeconds),s=.86,ox=j*480+240,oy=252;
   ctx.drawImage(f.image,ox-f.anchor[0]*s,oy-f.anchor[1]*s,512*s,512*s);
   ctx.strokeStyle='#547084';ctx.beginPath();ctx.moveTo(j*480+20,oy);ctx.lineTo(j*480+460,oy);ctx.stroke();
   ctx.fillStyle='#e9f1fa';ctx.font='bold 21px sans-serif';ctx.fillText(j?'NEW / 16 drawings':'ORIGINAL / 10 drawings',j*480+24,36);
   ctx.font='14px sans-serif';ctx.fillText('Same torso origin / 0.520833 s cycle',j*480+24,456);
  }fs.writeFileSync(path.join(dir,`f_${String(n).padStart(3,'0')}.png`),c.toBuffer('image/png'));
 }
 const result=spawnSync('ffmpeg',['-hide_banner','-loglevel','error','-y','-framerate','48','-i',path.join(dir,'f_%03d.png'),'-filter_complex','[0:v]split[a][b];[a]palettegen=stats_mode=full[p];[b][p]paletteuse=dither=sierra2_4a','-loop','0',path.join(__dirname,'Run_Comparison.gif')],{encoding:'utf8'});
 if(result.status!==0)throw Error(result.stderr);
 fs.copyFileSync(path.join(dir,'f_012.png'),path.join(__dirname,'Run_Comparison.png'));
 const serial=list=>list.map(({src,time,anchor})=>({src,time,anchor}));
 const html=`<!doctype html><html lang="ko"><meta charset="utf-8"><title>달리기 개선 v01 비교</title><style>body{font:16px system-ui;background:#0a1423;color:#edf4fb;margin:30px;max-width:1120px}h1{font-size:28px}p{color:#a9bfd5;line-height:1.6}canvas{width:100%;border:1px solid #34485f;border-radius:12px;background:#102035}button,select,input{font:inherit;margin:10px 12px 10px 0}button,select{padding:9px;background:#253b54;color:white;border:1px solid #52708d;border-radius:6px}label{white-space:nowrap}.note{font-size:14px}</style><h1>달리기 개선 v01</h1><p>왼쪽 기존 10장 · 오른쪽 신규 16장. 현재 몸통 피벗과 동일한 반복 길이로 비교합니다.<br>512×512 / PPU 200. 다른 동작은 기존 버전을 사용합니다.</p><canvas id="c" width="960" height="480"></canvas><button id="play">일시정지</button><label>속도 <select id="speed"><option value="1">1×</option><option value=".5">0.5×</option><option value=".25">0.25×</option></select></label><label><input id="mirror" type="checkbox">왼쪽 방향</label><input id="scrub" type="range" min="0" max="1000" value="0"><span id="frame"></span><p class="note">그림과 반복 연결 검토용입니다. 이동 중 접지와 Idle↔Run 전환은 Unity Play에서 확인해주세요. 새 버전 채택은 사용자 승인 대기 상태입니다.</p><script>const sets=${JSON.stringify([serial(oldFrames),serial(newFrames)])};const duration=${manifest.cycleSeconds};const c=document.getElementById('c'),ctx=c.getContext('2d');let running=true,time=0,last=performance.now();let ready=Promise.all(sets.flat().map(f=>new Promise(r=>{f.image=new Image;f.image.onload=r;f.image.src=f.src})));function draw(){ctx.fillStyle='#102035';ctx.fillRect(0,0,960,480);let nums=[];sets.forEach((list,j)=>{let n=0;if(time<.5)while(n+1<list.length&&list[n+1].time<=time)n++;let f=list[n],s=.86,ox=j*480+240,oy=252;ctx.save();ctx.translate(ox,oy);ctx.scale(document.getElementById('mirror').checked?-s:s,s);ctx.drawImage(f.image,-f.anchor[0],-f.anchor[1]);ctx.restore();ctx.strokeStyle='#547084';ctx.beginPath();ctx.moveTo(j*480+20,oy);ctx.lineTo(j*480+460,oy);ctx.stroke();ctx.fillStyle='#edf4fb';ctx.font='bold 20px system-ui';ctx.fillText(j?'신규 16장':'기존 10장',j*480+24,38);nums.push(n+1)});document.getElementById('frame').textContent='기존 '+nums[0]+' / 신규 '+nums[1]}function tick(now){if(running)time=(time+(now-last)/1000*Number(document.getElementById('speed').value))%duration;last=now;draw();document.getElementById('scrub').value=time/duration*1000;requestAnimationFrame(tick)}document.getElementById('play').onclick=()=>{running=!running;document.getElementById('play').textContent=running?'일시정지':'재생'};document.getElementById('scrub').oninput=e=>{running=false;document.getElementById('play').textContent='재생';time=Number(e.target.value)/1000*duration;draw()};ready.then(()=>requestAnimationFrame(tick));</script></html>`;
 const withIdle=html.replace('<input id="scrub"','<label><input id="idle" type="checkbox">기존 대기 자세와 비교</label><input id="scrub"')
  .replace('const duration=',`const idle=${JSON.stringify(idle)};const duration=`)
  .replace('sets.flat().map(f=>','sets.flat().concat([idle]).map(f=>')
  .replace('let f=list[n],s=.86','let f=document.getElementById("idle").checked?idle:list[n],s=.86');
 fs.writeFileSync(path.join(__dirname,'Run_Comparison_v01.html'),withIdle);
 if(path.dirname(dir)!==__dirname)throw Error('Preview cache outside expected directory');fs.rmSync(dir,{recursive:true});
 console.log('Comparison HTML, PNG and GIF saved. Preview cache removed.');
}main().catch(e=>{console.error(e);process.exit(1)});

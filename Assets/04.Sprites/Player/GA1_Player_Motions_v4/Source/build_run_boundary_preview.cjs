const fs=require('fs'),path=require('path'),{spawnSync}=require('child_process');
const {createCanvas,loadImage}=require('@napi-rs/canvas');
const ROOT=path.resolve(__dirname,'..'),TMP=path.join(ROOT,'BuildPreview','RunBoundary');
async function main(){
 const run=JSON.parse(fs.readFileSync(path.join(ROOT,'Motions.json'))).clips.find(c=>c.name==='Run');
 const images=await Promise.all(run.frames.map(f=>loadImage(path.join(ROOT,f))));
 fs.mkdirSync(TMP,{recursive:true});
 for(let i=0;i<72;i++){
  const t=(i%24)/48;let k=0;
  while(k+1<run.times.length&&run.times[k+1]<=t+1e-6)k++;
  const canvas=createCanvas(800,512),ctx=canvas.getContext('2d');
  const g=ctx.createLinearGradient(0,0,0,512);g.addColorStop(0,'#0a1424');g.addColorStop(1,'#192c44');
  ctx.fillStyle=g;ctx.fillRect(0,0,800,512);
  ctx.strokeStyle='#4c617a';ctx.beginPath();ctx.moveTo(36,410.5);ctx.lineTo(478,410.5);ctx.moveTo(552,350.5);ctx.lineTo(762,350.5);ctx.stroke();
  ctx.drawImage(images[k],0,0);
  ctx.drawImage(images[k],650-256*.4,350-410*.4,204.8,204.8);
  ctx.fillStyle='#dfebf8';ctx.font='bold 16px sans-serif';ctx.fillText('ORIGINAL RUN / END-TO-START FIX',36,459);
  ctx.fillStyle='#98b1cf';ctx.font='13px sans-serif';ctx.fillText('8 original frames + 2 transition frames / 0.5 s',36,484);
  ctx.font='12px sans-serif';ctx.fillText('GAME-SIZE PREVIEW',578,391);
  fs.writeFileSync(path.join(TMP,`f_${String(i).padStart(3,'0')}.png`),canvas.toBuffer('image/png'));
 }
 const out=path.join(ROOT,'Previews','Run_Boundary_Preview.gif');
 const result=spawnSync('ffmpeg',['-hide_banner','-loglevel','error','-y','-framerate','48','-i',path.join(TMP,'f_%03d.png'),'-filter_complex','[0:v]split[a][b];[a]palettegen=stats_mode=full[p];[b][p]paletteuse=dither=sierra2_4a','-loop','0','-final_delay','2',out],{encoding:'utf8'});
 if(result.status!==0)throw Error(result.stderr);
 console.log(JSON.stringify({output:out,seconds:1.5,cycles:3}));
}
main().catch(e=>{console.error(e);process.exit(1)});

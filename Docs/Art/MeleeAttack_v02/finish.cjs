const fs=require('fs'),path=require('path'),crypto=require('crypto'),cp=require('child_process');
const {createCanvas,loadImage,sharp}=require('../NewPlayer_Motions_v01/art-utils.cjs');
const root=path.resolve(__dirname,'../../..');
const hash=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
const configs=[
 {name:'FirstAttack',label:'First / Horizontal',dir:path.join(__dirname,'FirstAttack')},
 {name:'SecondAttack',label:'Second - upward',dir:path.resolve(__dirname,'../SecondAttack_v02')},
 {name:'ThirdAttack',label:'Third - downward',dir:path.join(__dirname,'ThirdAttack')}
];
async function main(){
 const baseline=JSON.parse(fs.readFileSync(path.join(__dirname,'Baseline.json')));
 const changed=[],missing=[];
 for(const [file,sha] of Object.entries(baseline.protectedFiles)){
  if(!fs.existsSync(path.join(root,file)))missing.push(file);
  else if(hash(path.join(root,file))!==sha)changed.push(file);
 }
 const dirty=cp.execFileSync('git',['status','--porcelain','--',...changed],{cwd:root,encoding:'utf8'}).trim();
 const checked=[];
 for(const c of configs){
  c.manifest=JSON.parse(fs.readFileSync(path.join(c.dir,'Manifest.json')));
  if(c.manifest.frames.length!==16)throw Error('Frame count '+c.name);
  for(const f of c.manifest.frames){
   const p=path.join(root,f.path),meta=await sharp(p).metadata();
   if(hash(p)!==f.sha256||meta.width!==512||meta.height!==512||!meta.hasAlpha)throw Error('Frame changed '+f.path);
  }
  checked.push({name:c.name,frames:16,hashesMatch:true,width:512,height:512,alpha:true});
 }
 const frameDir=path.join(__dirname,'ComparisonFrames');fs.mkdirSync(frameDir,{recursive:true});
 for(let i=0;i<16;i++){
  const canvas=createCanvas(1200,500),g=canvas.getContext('2d');
  g.fillStyle='#122133';g.fillRect(0,0,1200,500);
  g.fillStyle='#f5e9d6';g.font='bold 22px Arial';g.textAlign='center';
  g.fillText('MELEE v02  |  SLOW MOTION COMPARISON',600,34);
  g.font='15px Arial';g.fillStyle='#aebdcf';g.fillText('16 frames per motion / 1 second preview / game playback is faster',600,60);
  for(let j=0;j<configs.length;j++){
   const c=configs[j],x=j*400;
   g.fillStyle='#f5e9d6';g.font='bold 22px Arial';g.fillText(c.label,x+200,100);
   g.strokeStyle='#314254';g.beginPath();g.moveTo(x+35,429);g.lineTo(x+365,429);g.stroke();
   g.drawImage(await loadImage(path.join(root,c.manifest.frames[i].path)),x+8,122,384,384);
   g.fillStyle='#aebdcf';g.font='16px Arial';g.fillText(String(i+1).padStart(2,'0')+' / 16',x+200,480);
  }
  fs.writeFileSync(path.join(frameDir,String(i).padStart(2,'0')+'.png'),canvas.toBuffer('image/png'));
 }
 const ffmpeg='C:/Users/hwang/AppData/Local/BluePixel/tools/ffmpeg-9.0.1/ffmpeg-9.0.1-full_build/bin/ffmpeg.exe';
 for(const [input,output] of [
  [path.join(__dirname,'FirstAttack/PreviewFrames/%02d.png'),path.join(__dirname,'FirstAttack/Preview.gif')],
  [path.join(__dirname,'ThirdAttack/PreviewFrames/%02d.png'),path.join(__dirname,'ThirdAttack/Preview.gif')],
  [path.join(frameDir,'%02d.png'),path.join(__dirname,'Comparison.gif')]
 ]) cp.execFileSync(ffmpeg,['-hide_banner','-loglevel','error','-y','-framerate','16','-i',input,'-filter_complex','[0:v]tpad=stop_mode=clone:stop_duration=0.5,split[a][b];[a]palettegen=stats_mode=diff[p];[b][p]paletteuse=dither=sierra2_4a',output]);
 const report={checkedAt:new Date().toISOString(),sourceFrames:checked,protectedCount:Object.keys(baseline.protectedFiles).length,unchangedCount:Object.keys(baseline.protectedFiles).length-changed.length-missing.length,changedSinceOctober13Baseline:changed,missing,changedFilesMatchCurrentGitHEAD:dirty==='',note:'Four baseline differences were already committed before this continuation. No changes or reversions were made to them.'};
 fs.writeFileSync(path.join(__dirname,'FinalAssetVerification.json'),JSON.stringify(report,null,2)+'\n');
 for(const c of configs){c.manifest.status='applied';c.manifest.integrationReport=path.relative(c.dir,path.join(__dirname,'ApplyVerification.json')).replaceAll('\\','/');fs.writeFileSync(path.join(c.dir,'Manifest.json'),JSON.stringify(c.manifest,null,2)+'\n');}
 console.log(JSON.stringify(report,null,2));
}
main().catch(e=>{console.error(e);process.exitCode=1;});

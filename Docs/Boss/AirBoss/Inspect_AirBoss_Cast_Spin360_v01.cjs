// Static QA only. Never changes Unity scenes, clips or original images.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),sharp=require('sharp');
const root=path.resolve(__dirname,'../../..'),name='AirBoss_Cast_Spin360';
const digest=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
async function main(){
  const frames=JSON.parse(fs.readFileSync(path.join(__dirname,name+'_v01.metrics.json')));
  const before=JSON.parse(fs.readFileSync(path.join(__dirname,name+'_v01.protected_before.json'))),changed=[];
  for(const [p,h]of Object.entries(before)){if(!fs.existsSync(path.join(root,p))||digest(path.join(root,p))!==h)changed.push(p);}
  const report={protectedFiles:Object.keys(before).length,changed,checkedAt:new Date().toISOString(),frameCount:frames.length,uniqueFiles:new Set(frames.map(f=>f.sha256)).size,allMarginsPass:frames.every(f=>f.outside64===0),smallestMargin:Math.min(...frames.flatMap(f=>f.margins)),master00Identical:fs.readFileSync(path.join(root,frames.find(f=>f.frame===0).path)).equals(fs.readFileSync(path.join(root,'ArtSource/AirBoss/idle/AirBoss_Idle_Master_image2.png')))};
  fs.writeFileSync(path.join(__dirname,name+'_v01.protected_after.json'),JSON.stringify(report,null,2));
  const tiles=[['Idle - reference','idle',null],['05 - Right-facing profile','cast_spin360','05'],['08 - Rear view','cast_spin360','08'],['11 - Left-facing profile','cast_spin360','11'],['13 - Front three-quarter','cast_spin360','13'],['15 - Idle recovery','cast_spin360','15']];const layers=[];
  for(let i=0;i<tiles.length;i++){const [label,dir,id]=tiles[i],p=id?path.join(root,'ArtSource/AirBoss',dir,'frames/v01',name+'_v01_'+id+'.png'):path.join(root,'ArtSource/AirBoss/idle/AirBoss_Idle_Master_image2.png');if(!fs.existsSync(p))continue;layers.push({input:await sharp(p).resize(384,384).toBuffer(),left:i*384,top:36});layers.push({input:Buffer.from(`<svg width="384" height="36"><text x="12" y="24" font-family="sans-serif" font-size="16" fill="#eef4ff">${label}</text></svg>`),left:i*384,top:0});}
  await sharp({create:{width:2304,height:420,channels:4,background:'#202936'}}).composite(layers).png().toFile(path.join(__dirname,name+'_v01_keyposes.png'));
  console.log(JSON.stringify(report));if(changed.length||!report.allMarginsPass||!report.master00Identical)process.exitCode=1;
}
main().catch(e=>{console.error(e);process.exitCode=1});

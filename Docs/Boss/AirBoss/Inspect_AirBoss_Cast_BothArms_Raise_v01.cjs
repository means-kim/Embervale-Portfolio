// Static review only; does not create animation clips or change source art.
const fs=require('fs'),path=require('path'),crypto=require('crypto'),sharp=require('sharp');
const root=path.resolve(__dirname,'../../..'),name='AirBoss_Cast_BothArms_Raise';
async function main(){
  const tiles=[['LiftDrop - gathered hold','cast_botharms_liftdrop','LiftDrop','05'],['LiftDrop - downstroke','cast_botharms_liftdrop','LiftDrop','09'],['Raise - open sustained hold','cast_botharms_raise','Raise','05'],['Raise - gentle lowering','cast_botharms_raise','Raise','12']];
  const layers=[];
  for(let i=0;i<tiles.length;i++){const [label,dir,action,id]=tiles[i];layers.push({input:await sharp(path.join(root,'ArtSource/AirBoss',dir,'frames/v01',`AirBoss_Cast_BothArms_${action}_v01_${id}.png`)).resize(384,384).toBuffer(),left:i*384,top:36});layers.push({input:Buffer.from(`<svg width="384" height="36"><text x="12" y="24" font-family="sans-serif" font-size="17" fill="#eef4ff">${label}</text></svg>`),left:i*384,top:0});}
  await sharp({create:{width:1536,height:420,channels:4,background:'#202936'}}).composite(layers).png().toFile(path.join(__dirname,name+'_v01_vs_LiftDrop.png'));
  const before=JSON.parse(fs.readFileSync(path.join(__dirname,name+'_v01.protected_before.json'))),changed=[];
  for(const [p,h]of Object.entries(before)){if(!fs.existsSync(path.join(root,p))||crypto.createHash('sha256').update(fs.readFileSync(path.join(root,p))).digest('hex')!==h)changed.push(p);}
  const report={protectedFiles:Object.keys(before).length,changed,checkedAt:new Date().toISOString()};fs.writeFileSync(path.join(__dirname,name+'_v01.protected_after.json'),JSON.stringify(report,null,2));
  console.log(JSON.stringify(report));if(changed.length)process.exitCode=1;
}
main().catch(e=>{console.error(e);process.exitCode=1});

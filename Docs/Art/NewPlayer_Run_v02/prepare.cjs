const fs=require('fs'),path=require('path');
const sharp=require('C:/Users/hwang/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/sharp');
async function main(){
const sources=process.argv.slice(2);
for(let g=0;g<4;g++)fs.copyFileSync(sources[g],path.join(__dirname,`Group_${g+1}_Generated.png`));
for(const [index,g,cell] of [[3,0,3],[8,2,0]]){
const m=await sharp(sources[g]).metadata(),l=Math.round(cell%2*m.width/2),r=Math.round((cell%2+1)*m.width/2),t=Math.round(Math.floor(cell/2)*m.height/2),b=Math.round((Math.floor(cell/2)+1)*m.height/2);
await sharp(sources[g]).extract({left:l,top:t,width:r-l,height:b-t}).png().toFile(path.join(__dirname,`Repair_${index}_Before.png`));
}
}main().catch(e=>{console.error(e);process.exit(1)});

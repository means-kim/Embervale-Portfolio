// Static review artifacts only; no animation or game assets are edited.
const fs=require('fs'),path=require('path'),sharp=require('sharp');
const root=path.resolve(__dirname,'../../..'),name='AirBoss_Encounter_Start';
const dir=path.join(root,'ArtSource/AirBoss/encounter_start/frames/v01');
const frame=i=>path.join(dir,name+'_v01_'+String(i).padStart(2,'0')+'.png');
const label=(text,w)=>Buffer.from('<svg width="'+w+'" height="30"><text x="12" y="22" fill="#e8f0fb" font-family="sans-serif" font-size="16">'+text+'</text></svg>');
(async()=>{
 const layers=[], keys=[0,3,7,15],names=['00 / Idle master','03 / Gather and bow','07 / Wide awakening','15 / Return toward Idle'];
 for(let j=0;j<4;j++){layers.push({input:await sharp(frame(keys[j])).resize(384,384).toBuffer(),left:j*384,top:38});layers.push({input:label(names[j],384),left:j*384,top:0});}
 await sharp({create:{width:1536,height:430,channels:4,background:'#202936'}}).composite(layers).png().toFile(path.join(__dirname,name+'_v01_keyposes.png'));
 const comparison=[];
 for(let j=0;j<3;j++){
  const id=[3,7,15][j];
  const master=await sharp(frame(0)).resize(400,400).ensureAlpha().raw().toBuffer();
  const sample=await sharp(frame(id)).resize(400,400).ensureAlpha().raw().toBuffer();
  for(let k=3;k<master.length;k+=4){master[k]=Math.round(master[k]*0.5);sample[k]=Math.round(sample[k]*0.5);}
  comparison.push({input:await sharp(master,{raw:{width:400,height:400,channels:4}}).png().toBuffer(),left:400*j,top:35});
  comparison.push({input:await sharp(sample,{raw:{width:400,height:400,channels:4}}).png().toBuffer(),left:400*j,top:35});
  comparison.push({input:label('00 + '+String(id).padStart(2,'0')+' / same coordinates',400),left:400*j,top:0});
 }
 await sharp({create:{width:1200,height:440,channels:4,background:'#202936'}}).composite(comparison).png().toFile(path.join(__dirname,name+'_v01_alignment.png'));
 const metadata=await sharp(path.join(root,'Assets/04.Sprites/AirBoss',name+'_4x4_1024_v01.png')).metadata();
 console.log(JSON.stringify({width:metadata.width,height:metadata.height,channels:metadata.channels,depth:metadata.depth,hasAlpha:metadata.hasAlpha}));
})().catch(e=>{console.error(e);process.exit(1)});


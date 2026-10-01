const fs=require('fs'),path=require('path'),sharp=require('sharp'),crypto=require('crypto');
const root=path.resolve(__dirname,'../../..'),name='AirBoss_Cast_Spin360',art=path.join(root,'ArtSource/AirBoss/cast_spin360'),history=path.join(art,'source/v02/history');
const rel=p=>path.relative(root,p).replaceAll('\\','/');
async function main(){
 const logs=fs.readdirSync(history).filter(f=>f.endsWith('.json')).map(f=>JSON.parse(fs.readFileSync(path.join(history,f),'utf8'))).sort((a,b)=>a.generation-b.generation);
 const hash=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
 const selected=new Map();for(let f=1;f<24;f++){const p=path.join(art,'source/v02','frame'+String(f).padStart(2,'0')+'_generated.png');if(fs.existsSync(p))selected.set(hash(p),f);}
 const originToProject=new Map(logs.map(l=>[l.origin,rel(path.join(history,'gen'+String(l.generation).padStart(2,'0')+'_frame'+String(l.frame).padStart(2,'0')+'.png'))]));
 const records=logs.map(l=>{const source=originToProject.get(l.origin),selectedFrame=selected.get(hash(path.join(root,source)));return {generation:l.generation,requestedFrame:l.frame,selectedFrame:selectedFrame??null,selected:selectedFrame!==undefined,tool:'built-in image_gen.imagegen',requestedSize:[1024,1024],prompt:l.prompt,referenceImages:l.refs.map(r=>originToProject.get(r)||r.replaceAll('\\','/').replace(root.replaceAll('\\','/')+'/','')),source};});
 for(const r of records)if(!fs.existsSync(path.join(root,r.source)))throw Error('Missing project source '+r.source);
 fs.writeFileSync(path.join(__dirname,name+'_v02.prompts.json'),JSON.stringify(records,null,2));
 const metrics=JSON.parse(fs.readFileSync(path.join(__dirname,name+'_v02.metrics.json'),'utf8'));
 for(const [label,ids,size] of [['keyposes',[0,4,7,12,17,23],256],['sheet_boundary',[14,15,16,17],384]]){
  const layers=[];for(let j=0;j<ids.length;j++){const f=ids[j],p=path.join(art,'frames/v02',name+'_v02_'+String(f).padStart(2,'0')+'.png');if(!fs.existsSync(p))throw Error('Missing frame '+f);layers.push({input:await sharp(p).resize(size,size).toBuffer(),left:j*size,top:28},{input:Buffer.from('<svg width="'+size+'" height="28"><text x="10" y="20" fill="#eef4ff" font-family="sans-serif" font-size="16">'+String(f).padStart(2,'0')+'</text></svg>'),left:j*size,top:0});}
  await sharp({create:{width:size*ids.length,height:size+28,channels:4,background:'#202936'}}).composite(layers).png().toFile(path.join(__dirname,name+'_v02_'+label+'.png'));
 }
 const summary={generationCount:logs.length,selectedGeneratedFrames:selected.size,frameCount:metrics.length,uniqueFrameHashes:new Set(metrics.map(f=>f.sha256)).size,outside64:metrics.reduce((s,f)=>s+f.outside64,0),minimumMargin:Math.min(...metrics.flatMap(f=>f.margins)),faintNoisePixelsRemoved:metrics.reduce((s,f)=>s+f.faintNoisePixelsRemoved,0),originalSourceSizes:[...new Set(metrics.filter(f=>f.frame).map(f=>f.sourceSize.join('x')))],animationCreatedOrModified:false};
 fs.writeFileSync(path.join(__dirname,name+'_v02.summary.json'),JSON.stringify(summary,null,2));console.log(summary);
}
main().catch(e=>{console.error(e);process.exit(1)});

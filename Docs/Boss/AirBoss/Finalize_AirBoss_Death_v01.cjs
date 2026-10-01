const fs=require('fs'),path=require('path'),sharp=require('sharp');
const root=path.resolve(__dirname,'../../..'),name='AirBoss_Death_v01',frame=n=>path.join(root,'ArtSource/AirBoss/death/frames/v01/'+name+'_'+String(n).padStart(2,'0')+'.png');
(async()=>{
 const log=JSON.parse(fs.readFileSync(path.join(__dirname,name+'.prompts.json')));
 for(const g of log.generations){const dst=path.join(root,g.projectSource);fs.mkdirSync(path.dirname(dst),{recursive:true});if(!fs.existsSync(dst))fs.copyFileSync(g.generatedOriginPath,dst);}
 const metrics=JSON.parse(fs.readFileSync(path.join(__dirname,name+'.metrics.json')));
 let time=0;const timing=[];for(let i=0;i<32;i++){const ms=i<=10?140:i<=19?60:i<=23?100:i<=27?140:240;timing.push({frame:i,startMs:time,durationMs:ms,stage:i<=10?'slow preparation':i<=19?'fast breakup':'lingering debris'});time+=ms;}
 fs.writeFileSync(path.join(__dirname,name+'.timing.json'),JSON.stringify({status:'Suggested timing only; no AnimationClip created or playback verified',durationMs:time,frames:timing},null,2));
 for(const [suffix,ids,w] of [['keyposes',[0,8,15,19,25,31],256],['sheet_boundary',[14,15,16,17],384]]){
 const layers=[];for(let j=0;j<ids.length;j++){layers.push({input:await sharp(frame(ids[j])).resize(w,w).toBuffer(),left:j*w,top:30},{input:Buffer.from('<svg width="'+w+'" height="30"><text x="12" y="22" fill="#edf5ff" font-family="sans-serif" font-size="18">'+String(ids[j]).padStart(2,'0')+'</text></svg>'),left:j*w,top:0});}
 await sharp({create:{width:ids.length*w,height:w+30,channels:4,background:'#202936'}}).composite(layers).png().toFile(path.join(__dirname,name+'_'+suffix+'.png'));
 }
 const report={frames:metrics.length,unique:new Set(metrics.map(f=>f.sha256)).size,minAlphaMargin:Math.min(...metrics.flatMap(f=>f.margins)),outside64:metrics.reduce((a,f)=>a+f.outside64,0),rawSizes:[...new Set(metrics.slice(1).map(f=>f.sourceSize.join('x')))],faintNoisePixelsRemoved:metrics.reduce((a,f)=>a+(f.faintNoisePixelsRemoved||0),0),generations:log.generations.length,selected:log.generations.filter(g=>g.selectedFrame!==null).length,proposedDurationMs:time};
 fs.writeFileSync(path.join(__dirname,name+'.summary.json'),JSON.stringify(report,null,2));console.log(report);
})();

const fs=require('fs'),path=require('path');
const prior=path.resolve(__dirname,'../NewPlayer_Run_v01');
const source=fs.readFileSync(path.join(prior,'build.cjs'),'utf8');
const start=source.slice(source.indexOf('function face('),source.indexOf('async function main'));
fs.writeFileSync(path.join(__dirname,'sprite-metrics.cjs'),start+'\nmodule.exports={face,extractBody};\n');
for(const f of ['PreservationBaseline.json','OriginalCurve.json','Run_Original.anim.txt','Restore.cs'])fs.copyFileSync(path.join(prior,f),path.join(__dirname,f));
for(const f of ['Apply.cs','VerifyPlay.cs'])fs.writeFileSync(path.join(__dirname,f),fs.readFileSync(path.join(prior,f),'utf8').replaceAll('Run_v01','Run_v02').replaceAll('Run v01','Run v02'));

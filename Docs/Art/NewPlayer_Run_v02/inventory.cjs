const fs=require('fs'),path=require('path'),crypto=require('crypto');
const root=path.resolve(__dirname,'../../..');
const walk=d=>fs.readdirSync(d,{withFileTypes:true}).flatMap(e=>e.isDirectory()?walk(path.join(d,e.name)):[path.join(d,e.name)]);
const files=['Assets/05.Animation','Assets/04.Sprites/Player','Assets/04.Sprites/NewPlayer'].flatMap(p=>walk(path.join(root,p)));
const map=new Map();for(const f of files.filter(f=>f.endsWith('.meta'))){const g=fs.readFileSync(f,'utf8').match(/^guid: (\w+)/m)?.[1];if(g)map.set(g,path.relative(root,f.slice(0,-5)).replaceAll('\\','/'));}
const controller=fs.readFileSync(path.join(root,'Assets/05.Animation/player/Player.controller'),'utf8');
const rows=controller.split(/--- !u!/).filter(x=>x.startsWith('1102 ')).map(block=>{
 const state=block.match(/m_Name: (.*)/)?.[1].trim(),speed=Number(block.match(/m_Speed: (.*)/)?.[1]),guid=block.match(/m_Motion:.*guid: (\w+)/)?.[1];
 if(!guid)return {state,uniqueFrames:0,note:'No motion clip assigned'};
 const file=map.get(guid);if(!file)throw Error('Unresolved clip '+guid);
 const text=fs.readFileSync(path.join(root,file),'utf8'),curves=text.split('  m_PPtrCurves:')[1]?.split('  m_SampleRate:')[0]||'';
 const refs=Array.from(curves.matchAll(/value: \{fileID: (-?\d+), guid: (\w+)/g)),sources=[...new Set(refs.map(r=>map.get(r[2])||r[2]))];
 const length=Number(text.match(/m_StopTime: (.*)/)?.[1]);return {state,clip:file,uniqueFrames:new Set(refs.map(r=>r[2]+':'+r[1])).size,keys:refs.length,sampleRate:Number(text.match(/m_SampleRate: (.*)/)?.[1]),clipSeconds:length,stateSpeed:speed,effectiveSeconds:length/speed,loop:text.includes('m_LoopTime: 1'),sources};
});
fs.writeFileSync(path.join(__dirname,'CurrentAnimationInventory.json'),JSON.stringify({source:'Read-only serialized clip/controller inspection; Editor disconnected',states:rows},null,2));
for(const r of rows)console.log(JSON.stringify({...r,sources:r.sources?.length>2?[r.sources[0],`...${r.sources.length} files`]:r.sources}));
const baseline=JSON.parse(fs.readFileSync(path.join(__dirname,'PreservationBaseline.json'))),hash=f=>crypto.createHash('sha256').update(fs.readFileSync(f)).digest('hex');
const changed=baseline.filter(x=>!fs.existsSync(path.join(root,x.path))||hash(path.join(root,x.path))!==x.sha256).map(x=>x.path);
const record={originalPlayerFiles:baseline.filter(x=>x.path.startsWith('Assets/04.Sprites/Player/')).length,changedFromOriginalBaseline:changed,allowedTrackedChange:'Assets/05.Animation/player/Run.anim',allOriginalPlayerPreserved:changed.every(p=>!p.startsWith('Assets/04.Sprites/Player/')),otherAnimationFilesPreserved:changed.every(p=>p==='Assets/05.Animation/player/Run.anim')};
fs.writeFileSync(path.join(__dirname,'FileVerification.json'),JSON.stringify(record,null,2));console.log(JSON.stringify(record));

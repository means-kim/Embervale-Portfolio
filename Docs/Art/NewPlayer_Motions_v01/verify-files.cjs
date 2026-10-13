const fs=require('fs'),path=require('path'),crypto=require('crypto');
const root=path.resolve(__dirname,'../../..'),hash=p=>crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex');
const baseline=require('./ResumePreservation.json'),applied=require('./UnityApplyVerification.json'),allowed=new Set([...applied.clips.map(c=>c.path),'Assets/05.Animation/Player_Motion/Player.controller']);
const changed=[],missing=[];for(const r of baseline){const p=path.join(root,r.path);if(!fs.existsSync(p))missing.push(r.path);else if(hash(p)!==r.sha256)changed.push(r.path);}
const protectedPrefixes=['Assets/04.Sprites/Player/','Assets/04.Sprites/Boss/','Assets/04.Sprites/NewPlayer/Idle_v02/','Assets/04.Sprites/NewPlayer/Run_v02/','Assets/04.Sprites/NewPlayer/Design_v01/'];
const protectedChanges=[...changed,...missing].filter(p=>protectedPrefixes.some(a=>p.startsWith(a)));
const unexpected=changed.filter(p=>!allowed.has(p));
const manifest={...require('./MovingManifest.json'),...require('./HeldManifest.json')};let frames=0;
for(const [name,m]of Object.entries(manifest)){const appliedClip=applied.clips.find(c=>c.name===name);m.clipPath=appliedClip.path;m.clipGuid=appliedClip.guid;for(const f of m.frames){if(hash(path.join(root,f.path))!==f.sha256)throw Error('PNG differs '+f.path);if(!fs.existsSync(path.join(root,f.path+'.meta')))throw Error('Missing meta '+f.path);frames++;}}
fs.writeFileSync(path.join(__dirname,'FinalManifest.json'),JSON.stringify(manifest,null,2));
const r={framesVerified:frames,protectedFiles:baseline.filter(r=>protectedPrefixes.some(p=>r.path.startsWith(p))).length,protectedChanges,changed,missing,externalChangesSinceResume:unexpected,allClipGuidsPreserved:applied.success};
fs.writeFileSync(path.join(__dirname,'FileVerification.json'),JSON.stringify(r,null,2));console.log(JSON.stringify(r,null,2));if(protectedChanges.length||missing.length)process.exit(1);


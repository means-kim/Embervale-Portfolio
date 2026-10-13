const fs=require('fs'),path=require('path'),crypto=require('crypto');
const root=path.resolve(__dirname,'../../..'),baseline=JSON.parse(fs.readFileSync(path.join(__dirname,'PreservationBaseline.json'))),hash=f=>crypto.createHash('sha256').update(fs.readFileSync(f)).digest('hex');
const changed=baseline.filter(f=>!fs.existsSync(path.join(root,f.path))||hash(path.join(root,f.path))!==f.sha256).map(f=>f.path);
const allowed='Assets/05.Animation/player/Idle.anim';
if(changed.some(f=>f!==allowed))throw Error('Protected files changed: '+changed.join(','));
const manifest=JSON.parse(fs.readFileSync(path.join(__dirname,'Manifest.json')));
for(const f of manifest.frames){if(hash(path.join(root,f.path))!==f.sha256)throw Error('Output hash mismatch');if(!fs.existsSync(path.join(root,f.path+'.meta')))throw Error('Unity metadata missing');}
const record={originalPlayerFiles:baseline.filter(f=>f.path.startsWith('Assets/04.Sprites/Player/')).length,protectedFiles:baseline.length,changed,originalPlayerPreserved:true,previousRunVersionsPreserved:true,otherAnimationsCodeScenesPrefabsPreserved:true,newIdleFrames:manifest.frames.length};
fs.writeFileSync(path.join(__dirname,'FileVerification.json'),JSON.stringify(record,null,2));console.log(JSON.stringify(record));

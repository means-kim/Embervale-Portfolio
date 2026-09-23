import fs from 'node:fs/promises';
import crypto from 'node:crypto';
export async function build(root, { overwrite = false } = {}) {
 const dir=root+'/Assets/MapReview', S=1.68/50.625, Y0=2678.3;
 if (!overwrite) { try { await fs.access(dir+'/MapReview_v01.unity'); throw new Error('Review scene exists; refuse to overwrite manual edits.'); } catch(e) { if(e.code!=='ENOENT')throw e; } }
 const uid=()=>crypto.randomBytes(16).toString('hex');
 async function meta(path,kind='file') {
  try { return (await fs.readFile(path+'.meta','utf8')).match(/guid: (\w+)/)[1]; } catch {}
  const g=uid();let tail=kind==='folder'?'folderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n':kind==='cs'?'MonoImporter:\n  externalObjects: {}\n  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n  userData:\n  assetBundleName:\n  assetBundleVariant:\n':kind==='shader'?'ShaderImporter:\n  externalObjects: {}\n  defaultTextures: []\n  nonModifiableTextures: []\n  userData:\n  assetBundleName:\n  assetBundleVariant:\n':'DefaultImporter:\n  externalObjects: {}\n';
  await fs.writeFile(path+'.meta','fileFormatVersion: 2\nguid: '+g+'\n'+tail);return g;
 }
 for(const d of [dir,dir+'/Runtime',dir+'/Editor'])await meta(d,'folder');
 const artGuid=await meta(dir+'/Runtime/ReviewArt.cs','cs'), terrainGuid=await meta(dir+'/Runtime/ReviewTerrain.cs','cs'), playerGuid=await meta(dir+'/Runtime/ReviewPlayer.cs','cs'), shaderGuid=await meta(dir+'/Runtime/ReviewArt.shader','shader');
 const source=JSON.parse(await fs.readFile(root+'/Docs/Unity/MapReview_RenderedArt_v01.json','utf8'));
 const split=s=>{let out=[],level=0,start=0;for(let i=0;i<s.length;i++){if(s[i]==='(')level++;if(s[i]===')')level--;if(s[i]===','&&level===0){out.push(s.slice(start,i).trim());start=i+1;}}out.push(s.slice(start).trim());return out;};
 function gradients(q) {return q.masks.flatMap(m=>split(m.value).map(g=>{let terms=split(g.slice(g.indexOf('(')+1,-1));let direction='bottom';if(terms[0].startsWith('to '))direction=terms.shift().slice(3);const horizontal=['left','right'].includes(direction),extent=horizontal?m.rect.w:m.rect.h;let stops=terms.map((t,i)=>{let c=t.match(/rgba?\(([^)]+)\)/),parts=c[1].split(',').map(Number),p=t.slice(c[0].length).trim();return {a:parts.length===4?parts[3]:1,p:p?(p.endsWith('%')?parseFloat(p)*extent/100:parseFloat(p)):i===0?0:extent};});return {r:m.rect,horizontal,reverse:['left','top'].includes(direction),extent,stops};}));}
 function alpha(g,x,y){let t=g.horizontal?x-g.r.x:y-g.r.y;if(g.reverse)t=g.extent-t;let a=g.stops[0];if(t<=a.p)return a.a;for(let i=1;i<g.stops.length;i++){let b=g.stops[i];if(t<=b.p)return a.a+(b.a-a.a)*(t-a.p)/Math.max(.001,b.p-a.p);a=b;}return a.a;}
 function room(q){if(q.path.includes('/StartMap/'))return '01_Start';if(q.rect.x>4900&&q.rect.y>1750)return '05_Drop';if(q.rect.x>3200)return '04_Boss';if(q.rect.x>2150)return '03_Corridor';return '02_Climb';}
 const batches=[];
 function append(q,state=''){
  if(q.path.includes('Run_00.png'))return;
  const r=q.rect,c=q.clip,region=room(q),key=q.path+'|'+region+'|'+state;
  let b=batches.at(-1);
  if(!b||b.key!==key){b={key,name:q.path.split('/').at(-1).replace('.png',''),path:q.path,region,state,origin:[r.x*S,(Y0-r.y)*S],positions:[],uv:[],colors:[],triangles:[]};batches.push(b);}
  let nx=Math.max(1,Math.ceil(c.w/40)),ny=Math.max(1,Math.ceil(c.h/70));if(c.w<10)ny=Math.min(ny,8);
  const gs=gradients(q),base=b.positions.length/2;
  let renderedW=r.w,renderedH=r.h;
  if(q.cover){let sc=Math.max(r.w/q.naturalW,r.h/q.naturalH);renderedW=q.naturalW*sc;renderedH=q.naturalH*sc;}
  for(let j=0;j<=ny;j++)for(let i=0;i<=nx;i++){
   let x=c.x+c.w*i/nx,y=c.y+c.h*j/ny,u=(x-r.x+(renderedW-r.w)/2)/renderedW,v=1-(y-r.y+(renderedH-r.h)/2)/renderedH;
   b.positions.push((x*S-b.origin[0]),((Y0-y)*S-b.origin[1]));b.uv.push(q.flip?1-u:u,v);
   b.colors.push(q.brightness,gs.reduce((a,g)=>a*alpha(g,x,y),q.opacity));
  }
  for(let j=0;j<ny;j++)for(let i=0;i<nx;i++){let a=base+j*(nx+1)+i;b.triangles.push(a,a+nx+1,a+1,a+1,a+nx+1,a+nx+2);}
 }
 for(const q of source){let state=q.path.includes('BreakEdge')&&q.rect.w<400||q.path.includes('BreakFloorDebris')?'broken':'';append(q,state);}
 function quad(path,x,y,w,h,clip){return {path,rect:{x,y,w,h},clip:clip||{x,y,w,h},masks:[],opacity:1,brightness:.85,flip:false};}
 for(const state of ['intact','cracked']){
  append(quad('Assets/04.Sprites/Backgrounds/BossMap1/BG_Boss1_'+(state==='intact'?'BreakFloor':'BreakFloorCracked')+'_v02.png',5540,1507.9660087719,440,440),state);
  let q=quad('Assets/04.Sprites/Backgrounds/BossMap1/BG_Boss1_GroundSnow_v01.png',5230,1495.8995165746,1620,540,{x:5590,y:1495.8995165746,w:340,h:540});q.brightness=.8;append(q,state);
 }
 for(const b of batches)for(const k of ['positions','uv','colors'])b[k]=b[k].map(x=>Number(x.toFixed(6)));
 const geomPath=dir+'/ReviewGeometry.json';await fs.writeFile(geomPath,JSON.stringify({batches}));const geomGuid=await meta(geomPath);
 // Native scene serialization, no existing scene is opened, replaced or saved.
 let id=1000,objects=[],chunks=[];
 function go(name,parent=null,pos=[0,0,0],active=1){const o={go:id++,tr:id++,comps:[],name,parent,children:[],pos,active};if(parent)parent.children.push(o);objects.push(o);return o;}
 function comp(o,type,body){let c=id++;o.comps.push(c);chunks.push(`--- !u!${type} &${c}\n${body.replaceAll('$GO',String(o.go))}`);return c;}
 function mono(o,guid,fields){return comp(o,114,`MonoBehaviour:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: $GO}\n  m_Enabled: 1\n  m_EditorHideFlags: 0\n  m_Script: {fileID: 11500000, guid: ${guid}, type: 3}\n  m_Name:\n  m_EditorClassIdentifier:\n${fields}`);}
 const rootGo=go('MapReview_v01 — isolated prototype'),regions={};
 for(const r of ['01_Start','02_Climb','03_Corridor','04_Boss','05_Drop']){let rg=go(r,rootGo);regions[r]={root:rg,art:go('Art',rg),gameplay:go('Gameplay',rg)};}
 const states={intact:go('Intact',regions['04_Boss'].art),cracked:go('Cracked',regions['04_Boss'].art,[0,0,0],0),broken:go('Broken',regions['04_Boss'].art,[0,0,0],0)};
 for(let i=0;i<batches.length;i++){let b=batches[i],origin=[...b.origin,0];if(b.name==='BG_Boss1_Ruins_v01')origin[1]-=45.5625*S;let o=go(b.name,b.state?states[b.state]:regions[b.region].art,origin);let texturePath=dir+'/Textures/'+b.path.split('/').at(-1);try{await fs.access(texturePath);}catch{texturePath=root+'/'+b.path;}const tg=await meta(texturePath);mono(o,artGuid,`  geometry: {fileID: 4900000, guid: ${geomGuid}, type: 3}\n  texture: {fileID: 2800000, guid: ${tg}, type: 3}\n  shader: {fileID: 4800000, guid: ${shaderGuid}, type: 3}\n  batchIndex: ${i}\n  order: ${i+1}\n`);}
 const world=(x,y)=>[x*S,(Y0-y)*S];
 function edge(name,region,points){let o=go(name,regions[region].gameplay);mono(o,terrainGuid,'  points:\n'+points.map(p=>{let v=world(...p);return `  - {x: ${v[0]}, y: ${v[1]}}`;}).join('\n')+'\n');return o;}
 edge('Start floor — provisional','01_Start',[[0,2678.3],[1250,2678.3]]);
 let joined=JSON.parse(await fs.readFile(root+'/Docs/Backgrounds/StartToClimb_Joined_v05.json','utf8'));
 for(const p of joined.climb.platforms){let f=p.id==='entry'?joined.climb.entryPreviewSurfaceFactor:joined.climb.platformTypes[p.type].surface;let top=1200+p.y+(p.id==='entry'?15:0)+p.w*f;edge('Ledge_'+p.id,'02_Climb',[[1250+p.x+p.w*.08,top],[1250+p.x+p.w*.92,top]]);}
 edge('Exit to corridor','03_Corridor',[[2210,1572],[2600,1586],[3350,1594],[4070,1594]]);
 let arena=JSON.parse(await fs.readFile(root+'/Docs/Backgrounds/BossArena_Connection_v09.json','utf8')),shape=arena.terrainProposal.shape;
 function relief(x){for(let i=1;i<shape.length;i++){if(x<=shape[i][0]){let a=shape[i-1],b=shape[i],t=Math.max(0,Math.min(1,(x-a[0])/(b[0]-a[0])));t=.65*t+.35*t*t*(3-2*t);return(a[1]+(b[1]-a[1])*t)*151.875;}}return -53.15625;}
 for(const [a,b] of [[2820,4340],[4680,6200]]){let pts=[];for(let x=a;x<b;x+=35)pts.push([1250+x,1594+relief(x)]);pts.push([1250+b,1594+relief(b)]);edge('Boss slope '+a,'04_Boss',pts);}
 for(const p of arena.platformProposal)edge('Boss ledge '+p.id,'04_Boss',[[1250+p.x+p.w*.08,1200+p.surfaceY],[1250+p.x+p.w*.92,1200+p.surfaceY]]);
 const drop=JSON.parse(await fs.readFile(root+'/Docs/Backgrounds/DropPassage_Prototype_v02.json','utf8'));
 edge('Landing — five second reference','05_Drop',[[4950,1200+drop.landing.surfaceYApprox],[6850,1200+drop.landing.surfaceYApprox]]);
 edge('Left shaft bound — provisional','05_Drop',[[5580,1700],[5580,4850]]);edge('Right shaft bound — provisional','05_Drop',[[5940,1700],[5940,4850]]);
 const ptop=1639.5625,breakPos=world(5760,ptop);let bgo=go('Central break floor collision',regions['04_Boss'].gameplay,[breakPos[0],breakPos[1]-.15,0]);
 const boxid=comp(bgo,61,`BoxCollider2D:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: $GO}\n  m_Enabled: 1\n  serializedVersion: 3\n  m_Density: 1\n  m_Material: {fileID: 0}\n  m_IsTrigger: 0\n  m_UsedByEffector: 0\n  m_CompositeOperation: 0\n  m_Offset: {x: 0, y: 0}\n  m_Size: {x: ${340*S}, y: 0.3}\n  m_EdgeRadius: 0\n`);
 const checkRoot=go('Review checkpoints',rootGo),checkpoints=[];
 for(const [name,x,y] of [['1 Start',337,2678.3],['2 Climb',1500,2500],['3 Corridor',2950,1586],['4 Boss',5300,1594+relief(4050)],['5 Fall start',5760,ptop],['6 Landing',5760,1200+drop.landing.surfaceYApprox]]){let w=world(x,y);checkpoints.push(go(name,checkRoot,[w[0],w[1]+.84,0]));}
 let camera=go('Review Camera — 16 by 9',rootGo,[16.5926,3.74,-10]);
 let camid=comp(camera,20,`Camera:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: $GO}\n  m_Enabled: 1\n  serializedVersion: 2\n  m_ClearFlags: 2\n  m_BackGroundColor: {r: 0.002, g: 0.004, b: 0.008, a: 1}\n  m_projectionMatrixMode: 1\n  m_GateFitMode: 2\n  m_FOVAxisMode: 0\n  m_SensorSize: {x: 36, y: 24}\n  m_LensShift: {x: 0, y: 0}\n  m_FocalLength: 50\n  m_NormalizedViewPortRect:\n    serializedVersion: 2\n    x: 0\n    y: 0\n    width: 1\n    height: 1\n  near clip plane: 0.3\n  far clip plane: 1000\n  field of view: 60\n  orthographic: 1\n  orthographic size: ${1.68/.09/2}\n  m_Depth: 100\n  m_CullingMask:\n    serializedVersion: 2\n    m_Bits: 2147483648\n  m_RenderingPath: -1\n  m_TargetTexture: {fileID: 0}\n  m_TargetDisplay: 0\n  m_TargetEye: 3\n  m_HDR: 0\n  m_AllowMSAA: 0\n  m_AllowDynamicResolution: 0\n  m_ForceIntoRT: 0\n  m_OcclusionCulling: 0\n  m_StereoConvergence: 10\n  m_StereoSeparation: 0.022\n`);
 let player=go('Review Player — temporary controller',rootGo,checkpoints[0].pos);
 comp(player,50,`Rigidbody2D:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: $GO}\n  serializedVersion: 5\n  m_BodyType: 0\n  m_Simulated: 1\n  m_UseFullKinematicContacts: 0\n  m_UseAutoMass: 0\n  m_Mass: 1\n  m_LinearDamping: 0\n  m_AngularDamping: 0\n  m_GravityScale: 0\n  m_Material: {fileID: 0}\n  m_Interpolate: 1\n  m_SleepingMode: 1\n  m_CollisionDetection: 1\n  m_Constraints: 4\n`);
 comp(player,70,`CapsuleCollider2D:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: $GO}\n  m_Enabled: 1\n  m_Density: 1\n  m_Material: {fileID: 0}\n  m_IsTrigger: 0\n  m_UsedByEffector: 0\n  m_CompositeOperation: 0\n  m_Offset: {x: 0, y: 0}\n  m_Size: {x: 0.7, y: 1.68}\n  m_Direction: 0\n`);
 mono(player,playerGuid,`  reviewCamera: {fileID: ${camid}}\n  intactFloor: {fileID: ${states.intact.go}}\n  crackedFloor: {fileID: ${states.cracked.go}}\n  brokenEdges: {fileID: ${states.broken.go}}\n  breakCollider: {fileID: ${boxid}}\n  checkpoints:\n${checkpoints.map(c=>`  - {fileID: ${c.tr}}`).join('\n')}\n  visibleHeight: 1.68\n  speed: 8\n  gravity: 9.81\n  measuredFallSeconds: -1\n  status: Temporary review controller\n`);
 let visual=go('Player size reference',player,[0,-.84,0]);
 const spguid=await meta(root+'/Assets/05.Animation/player/GA1_Player_Motions_v4/Frames/Run/Run_00.png');
 comp(visual,212,`SpriteRenderer:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: $GO}\n  m_Enabled: 1\n  m_CastShadows: 0\n  m_ReceiveShadows: 0\n  m_Materials:\n  - {fileID: 10754, guid: 0000000000000000f000000000000000, type: 0}\n  m_SortingLayerID: 0\n  m_SortingOrder: 32000\n  m_Sprite: {fileID: 21300000, guid: ${spguid}, type: 3}\n  m_Color: {r: 1, g: 1, b: 1, a: 1}\n  m_FlipX: 0\n  m_FlipY: 0\n  m_DrawMode: 0\n  m_Size: {x: 2.56, y: 2.56}\n  m_WasSpriteAssigned: 1\n  m_MaskInteraction: 0\n  m_SpriteSortPoint: 0\n`);
 const sample=await fs.readFile(root+'/Assets/01.Scenes/SampleScene.unity','utf8');
 let header=sample.slice(0,sample.indexOf('--- !u!1 &'));
 let yaml=header;
 for(const o of objects){yaml+=`--- !u!1 &${o.go}\nGameObject:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  serializedVersion: 6\n  m_Component:\n  - component: {fileID: ${o.tr}}\n${o.comps.map(c=>`  - component: {fileID: ${c}}\n`).join('')}  m_Layer: 31\n  m_Name: ${JSON.stringify(o.name)}\n  m_TagString: Untagged\n  m_Icon: {fileID: 0}\n  m_NavMeshLayer: 0\n  m_StaticEditorFlags: 0\n  m_IsActive: ${o.active}\n--- !u!4 &${o.tr}\nTransform:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: ${o.go}}\n  serializedVersion: 2\n  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}\n  m_LocalPosition: {x: ${o.pos[0]}, y: ${o.pos[1]}, z: ${o.pos[2]}}\n  m_LocalScale: {x: 1, y: 1, z: 1}\n  m_ConstrainProportionsScale: 0\n  m_Children:${o.children.length?'\n'+o.children.map(c=>`  - {fileID: ${c.tr}}`).join('\n'):' []'}\n  m_Father: {fileID: ${o.parent?.tr||0}}\n  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}\n`;}
 yaml+=chunks.join('');yaml+=`--- !u!1660057539 &9223372036854775807\nSceneRoots:\n  m_ObjectHideFlags: 0\n  m_Roots:\n  - {fileID: ${rootGo.tr}}\n`;
 const scenePath=dir+'/MapReview_v01.unity';await fs.writeFile(scenePath,yaml);await meta(scenePath);
 await fs.writeFile(root+'/Docs/Unity/MapReview_BuildReport.json',JSON.stringify({scene:'Assets/MapReview/MapReview_v01.unity',scale:S,originY:Y0,objects:objects.length,artBatches:batches.length,vertices:batches.reduce((n,b)=>n+b.positions.length/2,0),sourceQuads:source.length,orthographicSize:1.68/.09/2,dropDistance:122.625,validation:'Generated only; Unity import, rendering and physics pending'},null,2));
 return {objects:objects.length,batches:batches.length,scenePath};
}

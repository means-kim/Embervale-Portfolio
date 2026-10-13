if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play before applying.");
var doc="C:/Unity/2d-team08/Docs/Art/NewPlayer_Motions_v01/";
var manifest=Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(doc+"MovingManifest.json"));
manifest.Merge(Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(doc+"HeldManifest.json")));
var scene=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();bool dirty=scene.isDirty;
var controller=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(UnityEditor.AssetDatabase.GUIDToAssetPath("e143031bb42e5794d894e2c0d42150d1"));
var machine=controller.layers[0].stateMachine;
var dataBefore=new System.Collections.Generic.Dictionary<string,string>();
foreach(var guid in UnityEditor.AssetDatabase.FindAssets("t:PlayerAttackDataSO",new[]{"Assets/07.Datas/Player"})){
 var path=UnityEditor.AssetDatabase.GUIDToAssetPath(guid);var data=UnityEditor.AssetDatabase.LoadAssetAtPath<PlayerAttackDataSO>(path);
 dataBefore[path]=UnityEditor.EditorJsonUtility.ToJson(data);
}
if(!System.IO.File.Exists(doc+"AttackDataBefore.json"))System.IO.File.WriteAllText(doc+"AttackDataBefore.json",Newtonsoft.Json.JsonConvert.SerializeObject(dataBefore,Newtonsoft.Json.Formatting.Indented));
UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
int imported=0;
UnityEditor.AssetDatabase.StartAssetEditing();
try{
 foreach(var prop in manifest.Properties())foreach(var f in prop.Value["frames"]){
 var p=(string)f["path"];var importer=(UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(p);if(importer==null)throw new System.Exception("Missing importer "+p);
 importer.textureType=UnityEditor.TextureImporterType.Sprite;importer.spriteImportMode=UnityEditor.SpriteImportMode.Single;importer.spritePixelsPerUnit=200;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.filterMode=UnityEngine.FilterMode.Bilinear;importer.wrapMode=UnityEngine.TextureWrapMode.Clamp;importer.textureCompression=UnityEditor.TextureImporterCompression.Uncompressed;importer.maxTextureSize=512;
 var settings=new UnityEditor.TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteAlignment=(int)UnityEngine.SpriteAlignment.Custom;settings.spritePivot=new UnityEngine.Vector2((float)f["pivotX"],(float)f["pivotY"]);settings.spriteMeshType=UnityEngine.SpriteMeshType.FullRect;importer.SetTextureSettings(settings);importer.SaveAndReimport();imported++;
 }
}finally{UnityEditor.AssetDatabase.StopAssetEditing();}
var checks=new System.Collections.Generic.List<object>();
foreach(var prop in manifest.Properties()){
 var m=prop.Value;string oldName=(string)m["oldState"],newName=prop.Name,oldPath=(string)m["clipPath"];
 var state=System.Linq.Enumerable.Single(machine.states,c=>c.state.name==oldName||c.state.name==newName).state;
 var clip=state.motion as UnityEngine.AnimationClip;if(clip==null)throw new System.Exception("Missing clip "+newName);
 string newPath=UnityEditor.AssetDatabase.GetAssetPath(clip);
 string guidBefore=UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(clip));
 var eventsBefore=Newtonsoft.Json.JsonConvert.SerializeObject(UnityEditor.AnimationUtility.GetAnimationEvents(clip));
 var binding=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip),b=>b.propertyName=="m_Sprite");
 var frames=(Newtonsoft.Json.Linq.JArray)m["frames"];var keys=new UnityEditor.ObjectReferenceKeyframe[frames.Count];
 for(int i=0;i<frames.Count;i++){
  var f=frames[i];var sprite=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>((string)f["path"]);
  if(sprite==null||sprite.rect.width!=512||sprite.rect.height!=512||sprite.pixelsPerUnit!=200)throw new System.Exception("Sprite import "+f["path"]);
  if(UnityEngine.Vector2.Distance(sprite.pivot,new UnityEngine.Vector2((float)f["pivotX"]*512,(float)f["pivotY"]*512))>.02f)throw new System.Exception("Pivot mismatch "+f["path"]);
  keys[i]=new UnityEditor.ObjectReferenceKeyframe{time=(float)f["time"],value=sprite};
 }
 UnityEditor.Undo.RecordObject(clip,"Apply NewPlayer motions");
 clip.frameRate=(float)m["sampleRate"];UnityEditor.AnimationUtility.SetObjectReferenceCurve(clip,binding,keys);
 var settings=UnityEditor.AnimationUtility.GetAnimationClipSettings(clip);settings.loopTime=(bool)m["loop"];settings.startTime=0;settings.stopTime=(float)m["duration"];UnityEditor.AnimationUtility.SetAnimationClipSettings(clip,settings);
 UnityEditor.EditorUtility.SetDirty(clip);UnityEditor.AssetDatabase.SaveAssetIfDirty(clip);
 state.name=newName;
 if(state.motion!=clip||System.Math.Abs(state.speed-(float)m["stateSpeed"])>.0001f)throw new System.Exception("Controller motion/speed changed");
 if(UnityEditor.AssetDatabase.AssetPathToGUID(newPath)!=guidBefore)throw new System.Exception("GUID changed");
 if(System.Math.Abs(clip.length-(float)m["duration"])>.0002f||eventsBefore!=Newtonsoft.Json.JsonConvert.SerializeObject(UnityEditor.AnimationUtility.GetAnimationEvents(clip)))throw new System.Exception("Duration/events changed "+newName+" "+clip.length);
 checks.Add(new {name=newName,path=newPath,guid=guidBefore,keys=keys.Length,length=clip.length,stateSpeed=state.speed,effectiveSeconds=clip.length/state.speed,loop=clip.isLooping});
 foreach(var pair in dataBefore){var data=UnityEditor.AssetDatabase.LoadAssetAtPath<PlayerAttackDataSO>(pair.Key);if(data.AnimClipName==oldName&&oldName!=newName){UnityEditor.Undo.RecordObject(data,"Rename player attack animation");data.AnimClipName=newName;UnityEditor.EditorUtility.SetDirty(data);UnityEditor.AssetDatabase.SaveAssetIfDirty(data);}}
}
foreach(var name in new[]{"JumpStart","Land"}){var state=System.Linq.Enumerable.FirstOrDefault(machine.states,c=>c.state.name==name).state;if(state!=null)machine.RemoveState(state);}
UnityEditor.EditorUtility.SetDirty(controller);UnityEditor.AssetDatabase.SaveAssetIfDirty(controller);
var dataChecks=new System.Collections.Generic.List<object>();
foreach(var pair in dataBefore){
 var data=UnityEditor.AssetDatabase.LoadAssetAtPath<PlayerAttackDataSO>(pair.Key);
 var a=Newtonsoft.Json.Linq.JObject.Parse(pair.Value);var b=Newtonsoft.Json.Linq.JObject.Parse(UnityEditor.EditorJsonUtility.ToJson(data));a.Remove("AnimClipName");b.Remove("AnimClipName");
 if(!Newtonsoft.Json.Linq.JToken.DeepEquals(a,b))throw new System.Exception("Attack gameplay data changed "+pair.Key);
 if(!System.Linq.Enumerable.Any(machine.states,s=>s.state.name==data.AnimClipName))throw new System.Exception("Missing attack state");
 dataChecks.Add(new {path=pair.Key,animation=data.AnimClipName,data.Damage,data.HealthCost,data.WindupDuration,data.RecoveryDuration,prefab=UnityEditor.AssetDatabase.GetAssetPath(data.AttackPrefab)});
}
if(scene.isDirty!=dirty)throw new System.Exception("Scene dirty changed");
var report=new{success=true,imported,clips=checks,attackData=dataChecks,scene=scene.path,sceneDirty=scene.isDirty};
System.IO.File.WriteAllText(doc+"UnityApplyVerification.json",Newtonsoft.Json.JsonConvert.SerializeObject(report,Newtonsoft.Json.Formatting.Indented));
return "Applied "+imported+" sprites and "+checks.Count+" clips, preserved clip GUIDs/gameplay attack values; scene unchanged.";


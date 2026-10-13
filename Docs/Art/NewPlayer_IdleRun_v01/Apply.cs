if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Exit Play mode before changing Idle.");
var root=System.IO.Directory.GetParent(UnityEngine.Application.dataPath).FullName;
var doc=root+"/Docs/Art/NewPlayer_IdleRun_v01/";
var manifest=Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(doc+"Manifest.json"));
var frames=(Newtonsoft.Json.Linq.JArray)manifest["frames"];
UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
var paths=new System.Collections.Generic.List<string>{(string)manifest["master"]};
foreach(var f in frames)paths.Add((string)f["path"]);
foreach(var path in paths){
 var importer=(UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(path);
 if(importer==null)throw new System.Exception("Missing importer: "+path);
 importer.textureType=UnityEditor.TextureImporterType.Sprite;importer.spriteImportMode=UnityEditor.SpriteImportMode.Single;
 importer.spritePixelsPerUnit=200;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;
 importer.filterMode=UnityEngine.FilterMode.Bilinear;importer.wrapMode=UnityEngine.TextureWrapMode.Clamp;
 importer.textureCompression=UnityEditor.TextureImporterCompression.Uncompressed;importer.maxTextureSize=512;
 var settings=new UnityEditor.TextureImporterSettings();importer.ReadTextureSettings(settings);
 settings.spriteAlignment=(int)UnityEngine.SpriteAlignment.Custom;settings.spritePivot=new UnityEngine.Vector2(270f/512f,1f-286f/512f);settings.spriteMeshType=UnityEngine.SpriteMeshType.FullRect;
 importer.SetTextureSettings(settings);importer.SaveAndReimport();
}
var clip=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>("Assets/05.Animation/player/Idle.anim");
var binding=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip),b=>b.propertyName=="m_Sprite");
var original=UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,binding);
var before=new {name=clip.name,frameRate=clip.frameRate,length=clip.length,frames=System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(original,k=>new {time=k.time,path=UnityEditor.AssetDatabase.GetAssetPath(k.value)}))};
if(!System.IO.File.Exists(doc+"Idle_BeforeCurve.json"))System.IO.File.WriteAllText(doc+"Idle_BeforeCurve.json",Newtonsoft.Json.JsonConvert.SerializeObject(before,Newtonsoft.Json.Formatting.Indented));
var originalEvents=UnityEditor.AnimationUtility.GetAnimationEvents(clip);
var keys=new UnityEditor.ObjectReferenceKeyframe[frames.Count+1];
for(int i=0;i<frames.Count;i++){
 var sprite=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>((string)frames[i]["path"]);
 if(sprite==null||sprite.rect.width!=512||sprite.rect.height!=512||sprite.pixelsPerUnit!=200)throw new System.Exception("Invalid sprite: "+frames[i]["path"]);
 if(UnityEngine.Vector2.Distance(sprite.pivot,new UnityEngine.Vector2(270,226))>0.01f)throw new System.Exception("Pivot mismatch");
 keys[i]=new UnityEditor.ObjectReferenceKeyframe{time=(float)frames[i]["time"],value=sprite};
}
keys[frames.Count]=new UnityEditor.ObjectReferenceKeyframe{time=(float)manifest["loopClosingTime"],value=keys[0].value};
UnityEditor.Undo.RecordObject(clip,"Apply unified NewPlayer Idle v01");
UnityEditor.AnimationUtility.SetObjectReferenceCurve(clip,binding,keys);
clip.frameRate=(float)manifest["sampleRate"];
var clipSettings=UnityEditor.AnimationUtility.GetAnimationClipSettings(clip);clipSettings.loopTime=true;clipSettings.startTime=0;clipSettings.stopTime=(float)manifest["cycleSeconds"];
UnityEditor.AnimationUtility.SetAnimationClipSettings(clip,clipSettings);UnityEditor.EditorUtility.SetDirty(clip);UnityEditor.AssetDatabase.SaveAssetIfDirty(clip);
var check=UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,binding);
if(check.Length!=13||System.Linq.Enumerable.Count(System.Linq.Enumerable.Distinct(System.Linq.Enumerable.Select(check,k=>k.value)))!=12)throw new System.Exception("Idle frame count mismatch");
if(System.Math.Abs(clip.length-1.65f)>.0001f||UnityEditor.AnimationUtility.GetAnimationEvents(clip).Length!=originalEvents.Length)throw new System.Exception("Idle timing/events changed");
var run=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>("Assets/05.Animation/player/Run.anim");
var runBinding=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(run),b=>b.propertyName=="m_Sprite");
var runKeys=UnityEditor.AnimationUtility.GetObjectReferenceCurve(run,runBinding);
if(System.Linq.Enumerable.Count(System.Linq.Enumerable.Distinct(System.Linq.Enumerable.Select(runKeys,k=>k.value)))!=16||System.Linq.Enumerable.Any(runKeys,k=>!UnityEditor.AssetDatabase.GetAssetPath(k.value).StartsWith("Assets/04.Sprites/NewPlayer/Run_v02/Frames/")))throw new System.Exception("Run changed unexpectedly");
var record=new{idleFrames=12,idleKeys=check.Length,idleLength=clip.length,idleLoop=clipSettings.loopTime,runFrames=16,runLength=run.length,ppu=200,pivotX=270,pivotY=226,scene=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,sceneDirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty,scope="Only Idle clip changed. Run_v02 drawings and timing retained."};
var json=Newtonsoft.Json.JsonConvert.SerializeObject(record,Newtonsoft.Json.Formatting.Indented);System.IO.File.WriteAllText(doc+"UnityVerification.json",json);return json;

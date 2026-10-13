if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Exit Play mode before replacing Idle.");
var root=System.IO.Directory.GetParent(UnityEngine.Application.dataPath).FullName;
var doc=root+"/Docs/Art/NewPlayer_Idle_v02/";
var manifest=Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(doc+"Manifest.json"));
var frames=(Newtonsoft.Json.Linq.JArray)manifest["frames"];
var scene=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();bool wasDirty=scene.isDirty;
UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
var paths=new System.Collections.Generic.List<string>{"Assets/04.Sprites/NewPlayer/Idle_v02/Source/Idle_Master512.png"};
foreach(var f in frames)paths.Add((string)f["path"]);
foreach(var path in paths){
 var importer=(UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(path);
 if(importer==null)throw new System.Exception("Missing importer: "+path);
 importer.textureType=UnityEditor.TextureImporterType.Sprite;importer.spriteImportMode=UnityEditor.SpriteImportMode.Single;
 importer.spritePixelsPerUnit=200;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;
 importer.filterMode=UnityEngine.FilterMode.Bilinear;importer.wrapMode=UnityEngine.TextureWrapMode.Clamp;
 importer.textureCompression=UnityEditor.TextureImporterCompression.Uncompressed;importer.maxTextureSize=512;
 var settings=new UnityEditor.TextureImporterSettings();importer.ReadTextureSettings(settings);
 settings.spriteAlignment=(int)UnityEngine.SpriteAlignment.Custom;settings.spritePivot=new UnityEngine.Vector2(270f/512f,226f/512f);settings.spriteMeshType=UnityEngine.SpriteMeshType.FullRect;
 importer.SetTextureSettings(settings);importer.SaveAndReimport();
}
var sheet=(UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath("Assets/04.Sprites/NewPlayer/Idle_v02/Sheets/Idle_Grid512_v02.png");
sheet.maxTextureSize=4096;sheet.textureCompression=UnityEditor.TextureImporterCompression.Uncompressed;sheet.mipmapEnabled=false;sheet.alphaIsTransparency=true;sheet.SaveAndReimport();
var clip=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>("Assets/05.Animation/player/Idle.anim");
var binding=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip),b=>b.propertyName=="m_Sprite");
var original=UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,binding);
var before=new {name=clip.name,frameRate=clip.frameRate,length=clip.length,frames=System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(original,k=>new {time=k.time,path=UnityEditor.AssetDatabase.GetAssetPath(k.value)}))};
if(!System.IO.File.Exists(doc+"Idle_BeforeCurve.json"))System.IO.File.WriteAllText(doc+"Idle_BeforeCurve.json",Newtonsoft.Json.JsonConvert.SerializeObject(before,Newtonsoft.Json.Formatting.Indented));
var eventsBefore=Newtonsoft.Json.JsonConvert.SerializeObject(UnityEditor.AnimationUtility.GetAnimationEvents(clip));
var keys=new UnityEditor.ObjectReferenceKeyframe[frames.Count];
for(int i=0;i<frames.Count;i++){
 var sprite=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>((string)frames[i]["path"]);
 if(sprite==null||sprite.rect.width!=512||sprite.rect.height!=512||sprite.pixelsPerUnit!=200)throw new System.Exception("Invalid sprite: "+frames[i]["path"]);
 if(UnityEngine.Vector2.Distance(sprite.pivot,new UnityEngine.Vector2(270,226))>.01f)throw new System.Exception("Pivot mismatch");
 keys[i]=new UnityEditor.ObjectReferenceKeyframe{time=(float)frames[i]["time"],value=sprite};
}
UnityEditor.Undo.RecordObject(clip,"Apply deterministic flame Idle v02");
clip.frameRate=(float)manifest["sampleRate"];
UnityEditor.AnimationUtility.SetObjectReferenceCurve(clip,binding,keys);
var settingsClip=UnityEditor.AnimationUtility.GetAnimationClipSettings(clip);settingsClip.loopTime=true;settingsClip.startTime=0;settingsClip.stopTime=(float)manifest["cycleSeconds"];
UnityEditor.AnimationUtility.SetAnimationClipSettings(clip,settingsClip);UnityEditor.EditorUtility.SetDirty(clip);UnityEditor.AssetDatabase.SaveAssetIfDirty(clip);
var check=UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,binding);
if(check.Length!=32||System.Linq.Enumerable.Count(System.Linq.Enumerable.Distinct(System.Linq.Enumerable.Select(check,k=>k.value)))!=32)throw new System.Exception("Idle count mismatch");
if(System.Math.Abs(clip.length-1.6f)>.0001f||Newtonsoft.Json.JsonConvert.SerializeObject(UnityEditor.AnimationUtility.GetAnimationEvents(clip))!=eventsBefore)throw new System.Exception("Idle duration/events mismatch");
var run=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>("Assets/05.Animation/player/Run.anim");
var runBinding=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(run),b=>b.propertyName=="m_Sprite");var runKeys=UnityEditor.AnimationUtility.GetObjectReferenceCurve(run,runBinding);
if(System.Linq.Enumerable.Count(System.Linq.Enumerable.Distinct(System.Linq.Enumerable.Select(runKeys,k=>k.value)))!=16||System.Linq.Enumerable.Any(runKeys,k=>!UnityEditor.AssetDatabase.GetAssetPath(k.value).StartsWith("Assets/04.Sprites/NewPlayer/Run_v02/Frames/")))throw new System.Exception("Unexpected Run state");
if(scene.isDirty!=wasDirty)throw new System.Exception("Scene dirtiness changed");
var record=new{idleFrames=32,idleKeys=check.Length,idleLength=clip.length,idleFrameRate=clip.frameRate,idleLoop=settingsClip.loopTime,runFrames=16,runLength=run.length,ppu=200,pivotX=270,pivotY=226,scene=scene.path,sceneDirty=scene.isDirty,scope="Only Idle clip replaced. New frames use one source painting. No duplicate closing frame."};
var json=Newtonsoft.Json.JsonConvert.SerializeObject(record,Newtonsoft.Json.Formatting.Indented);System.IO.File.WriteAllText(doc+"UnityVerification.json",json);return json;

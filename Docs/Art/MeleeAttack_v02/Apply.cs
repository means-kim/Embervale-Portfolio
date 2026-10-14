if(UnityEditor.EditorApplication.isPlaying||UnityEditor.EditorApplication.isCompiling)throw new System.Exception("Editor must be stopped and compiled");
var doc="C:/Unity/2d-team08/Docs/Art/MeleeAttack_v02/";
var manifests=new System.Collections.Generic.Dictionary<string,Newtonsoft.Json.Linq.JObject>();
foreach(var n in new[]{"FirstAttack","SecondAttack","ThirdAttack"}){
 var file=n=="SecondAttack"?"C:/Unity/2d-team08/Docs/Art/SecondAttack_v02/Manifest.json":doc+n+"/Manifest.json";
 manifests[n]=Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(file));
}
var mapping=new System.Collections.Generic.Dictionary<string,string>();
foreach(var n in new[]{"FirstAttack","SecondAttack","ThirdAttack","HorizontalAttack"})mapping["Assets/05.Animation/Player_AttackMotion/Player_"+n+".anim"]=n=="HorizontalAttack"?"FirstAttack":n;
mapping["Assets/02.Scripts/Z_HyunseoTest/Animation/Player/LeftHorizontalAttack.anim"]="FirstAttack";
mapping["Assets/02.Scripts/Z_HyunseoTest/Animation/Player/LeftLookUp.anim"]="SecondAttack";
mapping["Assets/02.Scripts/Z_HyunseoTest/Animation/Player/DownAttack.anim"]="ThirdAttack";
mapping["Assets/02.Scripts/Z_HyunseoTest/Animation/Player/RightHorizontalAttack.anim"]="FirstAttack";
foreach(var path in mapping.Keys){var clip=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>(path);if(clip==null)throw new System.Exception("Missing clip "+path);var b=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip),x=>x.propertyName=="m_Sprite");if(b.path!="")throw new System.Exception("Unexpected binding path "+path);}
var scene=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();bool wasDirty=scene.isDirty;
UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);int imported=0;
foreach(var pair in manifests)foreach(var f in pair.Value["frames"]){
 var path=(string)f["path"];
 if(pair.Key!="SecondAttack"){
  var imp=(UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(path);if(imp==null)throw new System.Exception("Missing sprite "+path);
  imp.textureType=UnityEditor.TextureImporterType.Sprite;imp.spriteImportMode=UnityEditor.SpriteImportMode.Single;imp.spritePixelsPerUnit=200;imp.alphaIsTransparency=true;imp.mipmapEnabled=false;imp.filterMode=UnityEngine.FilterMode.Bilinear;imp.wrapMode=UnityEngine.TextureWrapMode.Clamp;imp.textureCompression=UnityEditor.TextureImporterCompression.Uncompressed;imp.maxTextureSize=512;
  var settings=new UnityEditor.TextureImporterSettings();imp.ReadTextureSettings(settings);settings.spriteAlignment=(int)UnityEngine.SpriteAlignment.Custom;settings.spritePivot=new UnityEngine.Vector2((float)f["pivotX"],(float)f["pivotY"]);settings.spriteMeshType=UnityEngine.SpriteMeshType.FullRect;imp.SetTextureSettings(settings);imp.SaveAndReimport();imported++;
 }
 var s=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>(path);if(s==null||s.rect.width!=512||s.rect.height!=512||s.pixelsPerUnit!=200||UnityEngine.Vector2.Distance(s.pivot,new UnityEngine.Vector2(270,226))>.01f)throw new System.Exception("Sprite settings mismatch "+path);
}
var checks=new System.Collections.Generic.List<object>();int sampled=0;
var preview=UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();
try{
 foreach(var pair in mapping){
  var clip=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>(pair.Key);float length=clip.length;
  var guid=UnityEditor.AssetDatabase.AssetPathToGUID(pair.Key);var settings=UnityEditor.AnimationUtility.GetAnimationClipSettings(clip);
  var settingsBefore=Newtonsoft.Json.JsonConvert.SerializeObject(settings);var eventsBefore=Newtonsoft.Json.JsonConvert.SerializeObject(UnityEditor.AnimationUtility.GetAnimationEvents(clip));
  var binding=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip),x=>x.propertyName=="m_Sprite");var frames=(Newtonsoft.Json.Linq.JArray)manifests[pair.Value]["frames"];int count=frames.Count;
  var keys=new UnityEditor.ObjectReferenceKeyframe[count];int i=0;foreach(var frame in frames){keys[i]=new UnityEditor.ObjectReferenceKeyframe{time=i*length/count,value=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>((string)frame["path"])};i++;}
  UnityEditor.Undo.RecordObject(clip,"Apply flame-fist melee animations");clip.frameRate=count/length;UnityEditor.AnimationUtility.SetObjectReferenceCurve(clip,binding,keys);UnityEditor.AnimationUtility.SetAnimationClipSettings(clip,settings);UnityEditor.EditorUtility.SetDirty(clip);UnityEditor.AssetDatabase.SaveAssetIfDirty(clip);
  if(System.Math.Abs(clip.length-length)>.0002f||guid!=UnityEditor.AssetDatabase.AssetPathToGUID(pair.Key)||settingsBefore!=Newtonsoft.Json.JsonConvert.SerializeObject(UnityEditor.AnimationUtility.GetAnimationClipSettings(clip))||eventsBefore!=Newtonsoft.Json.JsonConvert.SerializeObject(UnityEditor.AnimationUtility.GetAnimationEvents(clip)))throw new System.Exception("Clip contract changed "+pair.Key);
  var go=new UnityEngine.GameObject("MeleeFrameCheck");UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go,preview);go.AddComponent<UnityEngine.Animator>();var renderer=go.AddComponent<UnityEngine.SpriteRenderer>();
  foreach(var key in keys){clip.SampleAnimation(go,key.time+.000001f);if(renderer.sprite!=key.value)throw new System.Exception("Frame sample failed "+clip.name);sampled++;}UnityEngine.Object.DestroyImmediate(go);
  checks.Add(new{path=pair.Key,source=pair.Value,keys=count,length=clip.length,frameRate=clip.frameRate,guid,eventsPreserved=true,settingsPreserved=true});
 }
}finally{UnityEditor.SceneManagement.EditorSceneManager.ClosePreviewScene(preview);}
if(scene.isDirty!=wasDirty)throw new System.Exception("Scene dirty state changed");
var result=new{success=true,importedSprites=imported,checkedSourceSprites=48,clips=checks,sampledKeys=sampled,scene=scene.path,sceneDirty=scene.isDirty};System.IO.File.WriteAllText(doc+"ApplyVerification.json",Newtonsoft.Json.JsonConvert.SerializeObject(result,Newtonsoft.Json.Formatting.Indented));return "Applied 8 clips and sampled "+sampled+" keys; First and Horizontal share sprites.";

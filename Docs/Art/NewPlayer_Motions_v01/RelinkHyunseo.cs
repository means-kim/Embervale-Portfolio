if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Editor must be stopped");
var dir="C:/Unity/2d-team08/Docs/Art/NewPlayer_Motions_v01/HyunseoRelink/";
System.IO.Directory.CreateDirectory(dir);
var mappings=new System.Collections.Generic.Dictionary<string,string>{
 {"Idle","Idle"},{"Run","Run"},{"DashStart","Dash"},{"DashLoop","Dash"},{"DashEnd","Dash"},
 {"JumpStart","JumpRise"},{"JumpRise","JumpRise"},{"JumpApex","JumpApex"},{"Fall","Fall"},{"Land","Idle"},
 {"DoubleJump","DoubleJump"},{"Hit","Hit"},{"WallClimb","WallCling"},{"WallCling","WallCling"},
 {"LeftHorizontalAttack","FirstAttack"},{"LeftLookUp","SecondAttack"},{"DownAttack","ThirdAttack"},
 {"RightHorizontalAttack","HorizontalAttack"},{"FireballAttack","FireballAttack"}};
var plans=new System.Collections.Generic.List<System.Tuple<UnityEngine.AnimationClip,UnityEditor.EditorCurveBinding,UnityEditor.ObjectReferenceKeyframe[],float,string,string>>();
foreach(var pair in mappings){
 string dst="Assets/02.Scripts/Z_HyunseoTest/Animation/Player/"+pair.Key+".anim";
 bool attack=pair.Value.EndsWith("Attack");
 string src="Assets/05.Animation/"+(attack?"Player_AttackMotion":"Player_Motion")+"/Player_"+pair.Value+".anim";
 var target=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>(dst);
 var source=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>(src);
 if(target==null||source==null)throw new System.Exception("Missing clip "+dst+" / "+src);
 var binding=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(target),b=>b.propertyName=="m_Sprite");
 var sb=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(source),b=>b.propertyName=="m_Sprite");
 var keys=UnityEditor.AnimationUtility.GetObjectReferenceCurve(source,sb);
 if(!System.IO.File.Exists(dir+pair.Key+".anim.before"))System.IO.File.Copy(dst,dir+pair.Key+".anim.before",false);
 var original=System.IO.File.ReadAllText(dir+pair.Key+".anim.before");
 float length=float.Parse(System.Text.RegularExpressions.Regex.Match(original,@"m_StopTime: ([\d.]+)").Groups[1].Value,System.Globalization.CultureInfo.InvariantCulture);
 float end=length-1f/target.frameRate;
 float sourceEnd=keys[keys.Length-1].time;
 for(int i=0;i<keys.Length;i++){
  if(keys[i].value==null||!UnityEditor.AssetDatabase.GetAssetPath(keys[i].value).StartsWith("Assets/04.Sprites/NewPlayer/"))throw new System.Exception("Not new sprite "+src);
  keys[i].time=keys[i].time/sourceEnd*end;
 }
 plans.Add(System.Tuple.Create(target,binding,keys,length,UnityEditor.EditorJsonUtility.ToJson(target),UnityEditor.AssetDatabase.AssetPathToGUID(dst)));
}
var prefabPath="Assets/02.Scripts/Z_HyunseoTest/Prefabs/Player_Hyunseo.prefab";
if(!System.IO.File.Exists(dir+"Player_Hyunseo.prefab.before"))System.IO.File.Copy(prefabPath,dir+"Player_Hyunseo.prefab.before",false);
var checks=new System.Collections.Generic.List<object>();
var preview=UnityEditor.SceneManagement.EditorSceneManager.NewPreviewScene();
try{
 foreach(var plan in plans){
  var clip=plan.Item1;var settings=UnityEditor.AnimationUtility.GetAnimationClipSettings(clip);
  var settingsBefore=Newtonsoft.Json.JsonConvert.SerializeObject(settings);
  var eventsBefore=Newtonsoft.Json.JsonConvert.SerializeObject(UnityEditor.AnimationUtility.GetAnimationEvents(clip));
  UnityEditor.Undo.RecordObject(clip,"Relink Hyunseo to NewPlayer");
  UnityEditor.AnimationUtility.SetObjectReferenceCurve(clip,plan.Item2,plan.Item3);
  UnityEditor.AnimationUtility.SetAnimationClipSettings(clip,settings);
  UnityEditor.EditorUtility.SetDirty(clip);UnityEditor.AssetDatabase.SaveAssetIfDirty(clip);
  if(System.Math.Abs(clip.length-plan.Item4)>.001f||eventsBefore!=Newtonsoft.Json.JsonConvert.SerializeObject(UnityEditor.AnimationUtility.GetAnimationEvents(clip))||settingsBefore!=Newtonsoft.Json.JsonConvert.SerializeObject(UnityEditor.AnimationUtility.GetAnimationClipSettings(clip)))throw new System.Exception("Timing/events/settings changed "+clip.name);
  if(UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(clip))!=plan.Item6)throw new System.Exception("GUID changed");
  var go=new UnityEngine.GameObject("SpriteBindingCheck");UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go,preview);
  go.AddComponent<UnityEngine.Animator>();
  var renderer=go.AddComponent<UnityEngine.SpriteRenderer>();
  foreach(var key in plan.Item3){clip.SampleAnimation(go,key.time+.000001f);if(renderer.sprite!=key.value)throw new System.Exception("Sample mismatch "+clip.name+" at "+key.time);}
  UnityEngine.Object.DestroyImmediate(go);
  checks.Add(new{clip=clip.name,length=clip.length,keys=plan.Item3.Length,source=mappings[clip.name],sampled=true});
 }
}finally{UnityEditor.SceneManagement.EditorSceneManager.ClosePreviewScene(preview);}
var root=UnityEditor.PrefabUtility.LoadPrefabContents(prefabPath);int replaced=0;
try{
 var idle=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>("Assets/05.Animation/Player_Motion/Player_Idle.anim");
 var ib=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(idle),b=>b.propertyName=="m_Sprite");
 var sprite=(UnityEngine.Sprite)UnityEditor.AnimationUtility.GetObjectReferenceCurve(idle,ib)[0].value;
 var body=root.GetComponent<UnityEngine.SpriteRenderer>();
 if(body==null)throw new System.Exception("Missing root SpriteRenderer");
 body.sprite=sprite;replaced++;
 if(replaced!=1)throw new System.Exception("Unexpected default renderer count "+replaced);
 UnityEditor.PrefabUtility.SaveAsPrefabAsset(root,prefabPath);
}finally{UnityEditor.PrefabUtility.UnloadPrefabContents(root);}
System.IO.File.WriteAllText(dir+"Verification.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{success=true,clips=checks,prefabSpriteReplacements=replaced,scene=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path,sceneDirty=UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isDirty},Newtonsoft.Json.Formatting.Indented));
return "Relinked and sampled "+checks.Count+" clips; prefab default sprite updated.";

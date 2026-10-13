var controller=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/05.Animation/player/Player.controller");
var rows=new System.Collections.Generic.List<object>();
var bound=new System.Collections.Generic.HashSet<string>();
System.Action<UnityEditor.Animations.AnimatorStateMachine,string> visit=null;
visit=(machine,prefix)=>{
 foreach(var child in machine.states){
  var state=child.state;var clip=state.motion as UnityEngine.AnimationClip;
  if(clip==null){rows.Add(new {state=prefix+state.name,motion=state.motion!=null?state.motion.name:null});continue;}
  var path=UnityEditor.AssetDatabase.GetAssetPath(clip);bound.Add(path);
  var sprites=new System.Collections.Generic.List<UnityEngine.Object>();int keys=0;
  foreach(var binding in UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip))if(binding.propertyName=="m_Sprite"){
   var curve=UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,binding);keys+=curve.Length;foreach(var k in curve)if(k.value!=null)sprites.Add(k.value);
  }
  var unique=System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Distinct(sprites));
  rows.Add(new {state=prefix+state.name,path,uniqueFrames=unique.Length,keys,length=clip.length,sampleRate=clip.frameRate,stateSpeed=state.speed,loop=clip.isLooping,sources=System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Distinct(System.Linq.Enumerable.Select(unique,s=>UnityEditor.AssetDatabase.GetAssetPath(s)))),events=System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(UnityEditor.AnimationUtility.GetAnimationEvents(clip),e=>new {e.time,e.functionName}))});
 }
 foreach(var child in machine.stateMachines)visit(child.stateMachine,prefix+child.stateMachine.name+"/");
};
foreach(var layer in controller.layers)visit(layer.stateMachine,layer.name+"/");
var extra=new System.Collections.Generic.List<string>();
foreach(var guid in UnityEditor.AssetDatabase.FindAssets("t:AnimationClip",new[]{"Assets/05.Animation/player","Assets/05.Animation/PlayerAttack"})){var p=UnityEditor.AssetDatabase.GUIDToAssetPath(guid);if(!bound.Contains(p))extra.Add(p);}
var result=new {controller="Assets/05.Animation/player/Player.controller",states=rows,unboundClips=extra,playing=UnityEditor.EditorApplication.isPlaying};
var json=Newtonsoft.Json.JsonConvert.SerializeObject(result,Newtonsoft.Json.Formatting.Indented);
System.IO.File.WriteAllText("C:/Unity/2d-team08/Docs/Art/NewPlayer_Run_v02/CurrentAnimationInventory.json",json);
return json;

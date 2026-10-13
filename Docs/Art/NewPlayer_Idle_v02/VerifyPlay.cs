if(!UnityEditor.EditorApplication.isPlaying||UnityEditor.EditorApplication.isPaused)throw new System.Exception("Running, unpaused Play mode required");
var doc="C:/Unity/2d-team08/Docs/Art/NewPlayer_Idle_v02/";
var player=UnityEngine.Object.FindFirstObjectByType<PlayerController>();
if(player==null)throw new System.Exception("Player missing");
var animator=player.GetComponent<UnityEngine.Animator>();var renderer=player.GetComponent<UnityEngine.SpriteRenderer>();
var input=player.GetComponent<PlayerInput>();var jump=player.GetComponent<PlayerJump>();
var bindings=new System.Collections.Generic.List<object>();
foreach(var state in new[]{"Idle","Run"}){
 var clip=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>("Assets/05.Animation/player/"+state+".anim");
 var curve=UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip),b=>b.propertyName=="m_Sprite"));
 var seen=new System.Collections.Generic.HashSet<string>();
 foreach(var key in curve){animator.Play(state,0,(key.time+.001f)/clip.length);animator.Update(0);var path=UnityEditor.AssetDatabase.GetAssetPath(renderer.sprite);if(path!=UnityEditor.AssetDatabase.GetAssetPath(key.value))throw new System.Exception(state+" sprite sampling mismatch");seen.Add(path);}
 var expected=state=="Idle"?32:16;if(seen.Count!=expected)throw new System.Exception(state+" frame count mismatch");
 bindings.Add(new {state,uniqueDisplayedFrames=seen.Count,keyCount=curve.Length,length=clip.length});
}
animator.Play("Idle",0,0);animator.Update(0);
var field=typeof(PlayerInput).GetField("OnMoveInput",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);
var send=(System.Action<UnityEngine.Vector2>)field.GetValue(input);
if(send==null)throw new System.Exception("Move input subscribers missing");
bool priorInput=input.enabled;input.enabled=false;
var samples=new System.Collections.Generic.List<object>();var states=new System.Collections.Generic.HashSet<string>();var displayed=new System.Collections.Generic.HashSet<string>();
var origin=player.transform.position;double started=UnityEditor.EditorApplication.timeSinceStartup,last=-1;int captured=0;bool completed=false;
UnityEditor.EditorApplication.CallbackFunction tick=null;
System.Action<bool,string> finish=(success,error)=>{
 if(completed)return;completed=true;UnityEditor.EditorApplication.update-=tick;
 if(input!=null){send(UnityEngine.Vector2.zero);input.enabled=priorInput;}
 var report=new {success,error,scope="Play mode with temporary movement-event injection; production player state/controller/physics run normally. Not keyboard input. Runtime input component restored.",bindings,states=System.Linq.Enumerable.ToArray(states),displayed=System.Linq.Enumerable.ToArray(displayed),samples,captured,initialPosition=new[]{origin.x,origin.y},finalPosition=player!=null?new[]{player.transform.position.x,player.transform.position.y}:null};
 System.IO.File.WriteAllText(doc+"PlayVerification.json",Newtonsoft.Json.JsonConvert.SerializeObject(report,Newtonsoft.Json.Formatting.Indented));
};
tick=()=>{
 try{
  if(!UnityEditor.EditorApplication.isPlaying||player==null){finish(false,"Play ended during probe");return;}
  double t=UnityEditor.EditorApplication.timeSinceStartup-started;
  if(t>5.1){finish(true,null);return;}
  float direction=t<1.8?0:t<2.5?1:t<3.2?-1:0;send(new UnityEngine.Vector2(direction,0));
  if(t-last<.012)return;last=t;
  string state=player.CurrentState.ToString(),sprite=UnityEditor.AssetDatabase.GetAssetPath(renderer.sprite);states.Add(state);displayed.Add(sprite);
  samples.Add(new {t,inputX=direction,state,x=player.transform.position.x,y=player.transform.position.y,grounded=jump.IsGrounded,sprite,flip=player.transform.localScale.x<0});
  string shot=t>.8&&captured==0?"Idle_Before":t>2.05&&captured==1?"Run_Right":t>2.75&&captured==2?"Run_Left":t>4.5&&captured==3?"Idle_After":null;
  if(shot!=null&&UnityEngine.Camera.main!=null){
   var camera=UnityEngine.Camera.main;var previousTarget=camera.targetTexture;var previousActive=UnityEngine.RenderTexture.active;
   var rt=new UnityEngine.RenderTexture(1280,720,24);var tex=new UnityEngine.Texture2D(1280,720,UnityEngine.TextureFormat.RGB24,false);
   try{camera.targetTexture=rt;camera.Render();UnityEngine.RenderTexture.active=rt;tex.ReadPixels(new UnityEngine.Rect(0,0,1280,720),0,0);tex.Apply();System.IO.File.WriteAllBytes(doc+"Unity_"+shot+".png",tex.EncodeToPNG());captured++;}
   finally{camera.targetTexture=previousTarget;UnityEngine.RenderTexture.active=previousActive;rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(tex);}
  }
 }catch(System.Exception e){finish(false,e.ToString());}
};
UnityEditor.EditorApplication.update+=tick;
return "Started 5-second Idle / right Run / left Run / Idle probe; movement component restores itself and writes PlayVerification.json.";

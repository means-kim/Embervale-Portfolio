if(!UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Play mode required");
var player=UnityEngine.Object.FindFirstObjectByType<PlayerAnimator>();
if(player==null)throw new System.Exception("PlayerAnimator missing in active scene");
var animator=player.GetComponent<UnityEngine.Animator>();
var renderer=player.GetComponent<UnityEngine.SpriteRenderer>();
var clip=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>("Assets/05.Animation/player/Run.anim");
var keys=UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip)[0]);
var paths=new System.Collections.Generic.List<string>();
foreach(var key in keys){animator.Play("Run",0,(key.time+0.001f)/clip.length);animator.Update(0f);paths.Add(UnityEditor.AssetDatabase.GetAssetPath(renderer.sprite));}
var camera=UnityEngine.Camera.main;
bool capture=false;
if(camera!=null){
 var previousTarget=camera.targetTexture;
 var previousActive=UnityEngine.RenderTexture.active;
 var rt=new UnityEngine.RenderTexture(1280,720,24);
 var tex=new UnityEngine.Texture2D(1280,720,UnityEngine.TextureFormat.RGB24,false);
 try{
  animator.Play("Run",0,0.24f);animator.Update(0f);
  camera.targetTexture=rt;camera.Render();UnityEngine.RenderTexture.active=rt;
  tex.ReadPixels(new UnityEngine.Rect(0,0,1280,720),0,0);tex.Apply();
  System.IO.File.WriteAllBytes("C:/Unity/2d-team08/Docs/Art/NewPlayer_Run_v01/Unity_Run_Play.png",tex.EncodeToPNG());capture=true;
 }finally{camera.targetTexture=previousTarget;UnityEngine.RenderTexture.active=previousActive;rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(tex);}
}
animator.Play("Idle",0,0);animator.Update(0f);
var record=new {playMode=true,sampledKeys=paths.Count,uniqueDisplayedSprites=System.Linq.Enumerable.Count(System.Linq.Enumerable.Distinct(paths)),newRunOnly=System.Linq.Enumerable.All(paths,p=>p.StartsWith("Assets/04.Sprites/NewPlayer/Run_v01/Frames/")),rendererEnabled=renderer.enabled,cameraCaptured=capture,scope="Run state explicitly sampled in Play. Keyboard running/ground contact/Idle transitions not tested."};
if(!record.newRunOnly||record.uniqueDisplayedSprites!=16)throw new System.Exception("Play Run reference verification failed");
var json=Newtonsoft.Json.JsonConvert.SerializeObject(record,Newtonsoft.Json.Formatting.Indented);
System.IO.File.WriteAllText("C:/Unity/2d-team08/Docs/Art/NewPlayer_Run_v01/PlayVerification.json",json);
return json;

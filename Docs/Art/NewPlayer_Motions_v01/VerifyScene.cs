if(!UnityEditor.EditorApplication.isPlaying||UnityEditor.EditorApplication.isPaused)throw new System.Exception("Unpaused Play required");
var doc="C:/Unity/2d-team08/Docs/Art/NewPlayer_Motions_v01/";var p=UnityEngine.Object.FindFirstObjectByType<PlayerController>();
var input=p.GetComponent<PlayerInput>();var jump=p.GetComponent<PlayerJump>();var attack=p.GetComponent<PlayerAttack>();var glide=p.GetComponent<PlayerGlide>();var animator=p.GetComponent<UnityEngine.Animator>();var renderer=p.GetComponent<UnityEngine.SpriteRenderer>();var body=p.GetComponent<UnityEngine.Rigidbody2D>();
var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
var jumpInput=(System.Action)typeof(PlayerInput).GetField("OnJumpInput",flags).GetValue(input);
var attackInput=(System.Action<PlayerAttackDataSO>)typeof(PlayerInput).GetField("OnAttackInput",flags).GetValue(input);
var dashInput=(System.Action)typeof(PlayerInput).GetField("OnDashInput",flags).GetValue(input);
var samples=new System.Collections.Generic.List<object>();var actions=new System.Collections.Generic.List<object>();var seen=new System.Collections.Generic.HashSet<string>();var spawned=new System.Collections.Generic.HashSet<string>();var screenshots=new System.Collections.Generic.HashSet<string>();var errors=new System.Collections.Generic.List<string>();
string[] names={"FirstAttack","SecondAttack","ThirdAttack","HorizontalAttack","FireBallAttack"};
var done=new System.Collections.Generic.HashSet<int>();double start=UnityEditor.EditorApplication.timeSinceStartup,last=-1;bool finished=false;
UnityEngine.Application.LogCallback log=(message,trace,type)=>{if(type==UnityEngine.LogType.Exception||type==UnityEngine.LogType.Error)errors.Add(message);};UnityEngine.Application.logMessageReceived+=log;
System.Action<string> capture=(name)=>{var camera=UnityEngine.Camera.main;if(camera==null)return;var previousTarget=camera.targetTexture;var previousActive=UnityEngine.RenderTexture.active;var rt=new UnityEngine.RenderTexture(1280,720,24);var tex=new UnityEngine.Texture2D(1280,720,UnityEngine.TextureFormat.RGB24,false);try{camera.targetTexture=rt;camera.Render();UnityEngine.RenderTexture.active=rt;tex.ReadPixels(new UnityEngine.Rect(0,0,1280,720),0,0);tex.Apply();System.IO.File.WriteAllBytes(doc+"Unity_"+name+".png",tex.EncodeToPNG());}finally{camera.targetTexture=previousTarget;UnityEngine.RenderTexture.active=previousActive;rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(tex);}};
UnityEditor.EditorApplication.CallbackFunction tick=null;System.Action<string> finish=(error)=>{if(finished)return;finished=true;UnityEditor.EditorApplication.update-=tick;UnityEngine.Application.logMessageReceived-=log;
var report=new{success=error==null&&errors.Count==0,error,scope="YuchanScene live physics and production input-event dispatch; not hardware key injection. Wall contact and full-level traversal not tested.",seen=System.Linq.Enumerable.ToArray(seen),spawned=System.Linq.Enumerable.ToArray(spawned),actions,errors,samples,screenshots=System.Linq.Enumerable.ToArray(screenshots),finalState=p!=null?p.CurrentState.ToString():null,finalGrounded=p!=null&&jump.IsGrounded};
System.IO.File.WriteAllText(doc+"ScenePlayVerification.json",Newtonsoft.Json.JsonConvert.SerializeObject(report,Newtonsoft.Json.Formatting.Indented));};
tick=()=>{try{if(!UnityEditor.EditorApplication.isPlaying||p==null){finish("Play ended");return;}double t=UnityEditor.EditorApplication.timeSinceStartup-start;
if(t>11.5){if(!seen.Contains("JumpRise")||!seen.Contains("JumpApex")||!seen.Contains("Fall")||!seen.Contains("DoubleJump")){finish("Missing jump phases");return;}if(spawned.Count!=5){finish("Missing attack spawns "+spawned.Count);return;}finish(null);return;}
foreach(var item in new[]{new{time=1.0,id=0},new{time=3.0,id=1},new{time=3.17,id=2}})if(t>=item.time&&done.Add(item.id)){jumpInput();actions.Add(new{t,action="Jump",count=jump.CurrentJumpCount});}
if(t>=3.5&&done.Add(3)){bool result=glide.TryHandleJumpInput();actions.Add(new{t,action="Glide on",result});}
if(t>=4.05&&done.Add(4)){bool result=glide.TryHandleJumpInput();actions.Add(new{t,action="Glide off",result});}
for(int i=0;i<names.Length;i++)if(t>=5+i*.85&&done.Add(10+i)){var data=UnityEditor.AssetDatabase.LoadAssetAtPath<PlayerAttackDataSO>("Assets/07.Datas/Player/"+names[i]+".asset");attackInput(data);actions.Add(new{t,action=names[i],started=attack.IsAttacking,clip=attack.CurrentAttackClip});}
if(t>=10&&done.Add(30)){dashInput();actions.Add(new{t,action="Dash"});}
if(t-last<.007)return;last=t;var sprite=UnityEditor.AssetDatabase.GetAssetPath(renderer.sprite);string clipName=System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(sprite)));seen.Add(clipName);
var instance=(PlayerAttackInstance)typeof(PlayerAttack).GetField("_instance",flags).GetValue(attack);if(attack.IsAttacking&&(bool)typeof(PlayerAttack).GetField("_spawned",flags).GetValue(attack)&&instance!=null)spawned.Add(attack.CurrentAttackClip);
samples.Add(new{t,state=p.CurrentState.ToString(),clip=clipName,grounded=jump.IsGrounded,vy=body.linearVelocity.y,sprite,animatorSpeed=animator.speed,gliding=glide.IsGliding});
if(System.Array.IndexOf(new[]{"JumpRise","JumpApex","Fall","DoubleJump","Glide","Dash","FirstAttack","FireBallAttack"},clipName)>=0&&screenshots.Add(clipName))capture(clipName);
}catch(System.Exception e){finish(e.ToString());}};
UnityEditor.EditorApplication.update+=tick;return "Started 11.5-second production scene probe.";

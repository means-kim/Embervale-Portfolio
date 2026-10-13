if(!UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Play required");
var doc="C:/Unity/2d-team08/Docs/Art/NewPlayer_Motions_v01/";
var player=UnityEngine.Object.FindFirstObjectByType<PlayerController>();if(player==null)throw new System.Exception("Player missing");
var animator=player.GetComponent<UnityEngine.Animator>();var renderer=player.GetComponent<UnityEngine.SpriteRenderer>();var pa=player.GetComponent<PlayerAnimator>();
var controller=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(UnityEditor.AssetDatabase.GUIDToAssetPath("e143031bb42e5794d894e2c0d42150d1"));
var bindings=new System.Collections.Generic.List<object>();int total=0;
foreach(var child in controller.layers[0].stateMachine.states){
 var state=child.state;var clip=state.motion as UnityEngine.AnimationClip;if(clip==null)continue;
 if(state.name=="JumpStart"||state.name=="Land")throw new System.Exception("Retired state still active");
 var binding=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip),b=>b.propertyName=="m_Sprite");var curve=UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,binding);
 var unique=new System.Collections.Generic.HashSet<string>();animator.speed=1;
 foreach(var key in curve){animator.Play(state.name,0,(key.time+.00001f)/clip.length);animator.Update(0);var path=UnityEditor.AssetDatabase.GetAssetPath(renderer.sprite);if(path!=UnityEditor.AssetDatabase.GetAssetPath(key.value))throw new System.Exception(state.name+" sample mismatch "+key.time+" "+path);unique.Add(path);}
 bindings.Add(new{state=state.name,unique=unique.Count,keys=curve.Length});total+=unique.Count;
}
var flags=System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance;
pa.ResetJumpPhase();pa.BeginJump(false);animator.Update(0);
if(!animator.GetCurrentAnimatorStateInfo(0).IsName("JumpRise"))throw new System.Exception("Normal jump did not start Rise");
typeof(PlayerAnimator).GetField("_jumpEnterTime",flags).SetValue(pa,UnityEngine.Time.time-1f);
var phases=new System.Collections.Generic.List<string>();
foreach(float vy in new[]{10f,4f,2f,0f,-2f,-4f,-10f}){pa.Tick(PlayerState.Jump,false,vy);animator.Update(0);var st=animator.GetCurrentAnimatorStateInfo(0);string name=st.IsName("JumpRise")?"JumpRise":st.IsName("JumpApex")?"JumpApex":st.IsName("Fall")?"Fall":"unexpected";phases.Add(name);if(name=="unexpected")throw new System.Exception("Jump phase mismatch");}
pa.ResetJumpPhase();pa.BeginJump(true);animator.Update(0);if(!animator.GetCurrentAnimatorStateInfo(0).IsName("DoubleJump"))throw new System.Exception("DoubleJump not entered");
var doubleSpeed=animator.speed;pa.ResetJumpPhase();pa.PlayState(PlayerState.Idle,false);animator.Update(0);
pa.PlayChargeJumpLaunch(new UnityEngine.Vector2(1,1).normalized);animator.Update(0);var chargeRenderer=pa.GetChargeJumpVisualRenderer(renderer);bool chargeOk=chargeRenderer!=renderer&&chargeRenderer.sprite!=null&&UnityEditor.AssetDatabase.GetAssetPath(chargeRenderer.sprite).Contains("/Motions_v01/Dash/");pa.EndChargeJumpLaunch();if(!chargeOk)throw new System.Exception("Charge launch sprite failed");
pa.PlayState(player.CurrentState,false);animator.Update(0);
var json=Newtonsoft.Json.JsonConvert.SerializeObject(new{success=true,scope="Live Play Animator sampling and visual phase controls; no keyboard input",displayedUniqueFrames=total,bindings,phases,doubleSpeed,chargeOk,scene=player.gameObject.scene.path},Newtonsoft.Json.Formatting.Indented);
System.IO.File.WriteAllText(doc+"PlayBindingVerification.json",json);return json;

var controller=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(UnityEditor.AssetDatabase.GUIDToAssetPath("e143031bb42e5794d894e2c0d42150d1"));
var states=new System.Collections.Generic.List<object>();
foreach(var child in controller.layers[0].stateMachine.states){var state=child.state;var clip=state.motion as UnityEngine.AnimationClip;if(clip==null)continue;var bind=System.Linq.Enumerable.FirstOrDefault(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip),b=>b.propertyName=="m_Sprite");var curve=UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,bind);
states.Add(new{state=state.name,path=UnityEditor.AssetDatabase.GetAssetPath(clip),guid=UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(clip)),frames=curve.Length,length=clip.length,speed=state.speed,first=UnityEditor.AssetDatabase.GetAssetPath(curve[0].value)});}
var json=Newtonsoft.Json.JsonConvert.SerializeObject(new{controller=UnityEditor.AssetDatabase.GetAssetPath(controller),states},Newtonsoft.Json.Formatting.Indented);
System.IO.File.WriteAllText("C:/Unity/2d-team08/Docs/Art/NewPlayer_Motions_v01/ResumeInventory.json",json);return json;

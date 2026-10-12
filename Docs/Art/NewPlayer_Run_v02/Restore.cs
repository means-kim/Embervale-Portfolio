if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Exit Play mode first");
var json=Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText("C:/Unity/2d-team08/Docs/Art/NewPlayer_Run_v01/OriginalCurve.json"));
var clip=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>("Assets/05.Animation/player/Run.anim");
var binding=System.Linq.Enumerable.Single(UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip),b=>b.propertyName=="m_Sprite");
var frames=(Newtonsoft.Json.Linq.JArray)json["frames"];
var keys=new UnityEditor.ObjectReferenceKeyframe[frames.Count];
for(int i=0;i<keys.Length;i++){
 var sprite=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>(UnityEditor.AssetDatabase.GUIDToAssetPath((string)frames[i]["guid"]));
 if(sprite==null)throw new System.Exception("Original sprite missing");
 keys[i]=new UnityEditor.ObjectReferenceKeyframe{time=(float)frames[i]["time"],value=sprite};
}
UnityEditor.Undo.RecordObject(clip,"Restore original Run animation");
UnityEditor.AnimationUtility.SetObjectReferenceCurve(clip,binding,keys);
clip.frameRate=(float)json["frameRate"];
var settings=UnityEditor.AnimationUtility.GetAnimationClipSettings(clip);settings.loopTime=true;settings.stopTime=(float)json["length"];
UnityEditor.AnimationUtility.SetAnimationClipSettings(clip,settings);
UnityEditor.EditorUtility.SetDirty(clip);UnityEditor.AssetDatabase.SaveAssetIfDirty(clip);
return "Original Run restored; all NewPlayer files retained.";

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public static class JumpFirePlayCheck {
 public static async Task<string> Main(){
 if(!EditorApplication.isPlaying)throw new Exception("Play mode required");
 var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/03.Prefabs/VFX/PlayerJump.prefab");
 var go=UnityEngine.Object.Instantiate(prefab,new Vector3(10000,10000,0),Quaternion.identity);
 go.name="Codex_JumpFireVerification";
 var audio=go.GetComponent<AudioSource>();if(audio!=null)audio.mute=true;
 var sr=go.GetComponent<SpriteRenderer>();
 var seen=new HashSet<string>();bool empty=false,hadState=false;float begin=Time.time;float lastTime=begin;
 try{
 for(int i=0;i<120;i++){
 await Task.Delay(15);
 if(go==null) return "observed="+string.Join(",",seen)+"; empty="+empty+"; animatorState="+hadState+"; destroyed=true; gameSeconds="+(Time.time-begin);
 if(sr.sprite!=null)seen.Add(sr.sprite.name);else empty=true;
 var a=go.GetComponent<Animator>();if(a.GetCurrentAnimatorClipInfo(0).Length>0)hadState=true;
 lastTime=Time.time;
 }
 throw new Exception("Prefab did not destroy in observation window; gameTime="+(lastTime-begin));
 }finally{if(go!=null)UnityEngine.Object.Destroy(go);}
 }
}

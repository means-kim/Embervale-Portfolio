using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;
public static class JumpFireProbe {
 public static string Main() {
 var path="Assets/04.Sprites/VFX/NewPlayerJump.png";
 var f=new SpriteDataProviderFactories(); f.Init();
 var d=f.GetSpriteEditorDataProviderFromObject(AssetImporter.GetAtPath(path)); d.InitSpriteEditorDataProvider();
 var cap=d.GetDataProvider<ISpriteFrameEditCapability>();
 if(cap==null) throw new Exception("No edit capability");
 var c=cap.GetEditCapability();
 foreach(var flag in new[]{EEditCapability.EditSpriteRect,EEditCapability.EditPivot,EEditCapability.CreateAndDeleteSprite})
 if(!c.HasCapability(flag)) throw new Exception("Unsupported "+flag);
 var tex=new Texture2D(2,2); ImageConversion.LoadImage(tex,File.ReadAllBytes(path));
 var px=tex.GetPixels32(); var report="";
 for(int i=0;i<6;i++) {
 int col=i%4, row=i/4, x0=(int)Math.Round(col*tex.width/4.0),x1=(int)Math.Round((col+1)*tex.width/4.0);
 int top=(int)Math.Round(row*tex.height/2.0), bottom=(int)Math.Round((row+1)*tex.height/2.0);
 int ay=-1; double ax=0;
 for(int y=top;y<bottom;y++){
 double sx=0; int count=0;
 for(int x=(x0+x1)/2-42;x<=(x0+x1)/2+42;x++) {
 var p=px[(tex.height-1-y)*tex.width+x];
 if(p.a>150 && p.r>230 && p.g>205 && p.b>100){sx+=x;count++;}
 }
 if(count>=4){ay=y; ax=sx/count;}
 }
 report+=i+": anchor="+ax.ToString("F1")+","+ay+"\n";
 }
 UnityEngine.Object.DestroyImmediate(tex);
 return report+"capabilities OK";
 }
}

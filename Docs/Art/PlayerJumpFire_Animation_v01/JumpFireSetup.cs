using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEditor.Animations;
public static class JumpFireSetup {
 public static string Main() {
 if(EditorApplication.isPlaying) throw new Exception("Stop play before editing");
 const string path="Assets/04.Sprites/VFX/NewPlayerJump.png";
 const string prefabPath="Assets/03.Prefabs/VFX/PlayerJump.prefab";
 const string animPath="Assets/05.Animation/VFX/PlayerJumpFire.anim";
 const string ctrlPath="Assets/05.Animation/VFX/PlayerJumpFire.controller";
 var importer=(TextureImporter)AssetImporter.GetAtPath(path);
 var f=new SpriteDataProviderFactories(); f.Init();
 var d=f.GetSpriteEditorDataProviderFromObject(importer); d.InitSpriteEditorDataProvider();
 var cap=d.GetDataProvider<ISpriteFrameEditCapability>();
 if(cap==null) throw new Exception("Missing capability");
 var capability=cap.GetEditCapability();
 foreach(var flag in new[]{EEditCapability.EditSpriteRect,EEditCapability.EditPivot,EEditCapability.CreateAndDeleteSprite})
 if(!capability.HasCapability(flag)) throw new Exception("Unsupported "+flag);
 var old=d.GetSpriteRects();
 var backup="Docs/Art/PlayerJumpFire_Animation_v01/BeforeAlignment";
 Directory.CreateDirectory(backup);
 foreach(var file in new[]{path,path+".meta",prefabPath,prefabPath+".meta"})
 {var dst=Path.Combine(backup,Path.GetFileName(file)); if(!File.Exists(dst)) File.Copy(file,dst);}
 var src=new Texture2D(2,2,TextureFormat.RGBA32,false);
 ImageConversion.LoadImage(src,File.ReadAllBytes(path));
 if(src.width!=1774 || src.height!=887) throw new Exception("Unexpected source dimensions");
 var input=src.GetPixels32(); var pixels=new Color32[2048*1024];
 int[] anchorX={228,662,1095,1536,231,675,1118,1562};
 int[] anchorY={429,429,429,429,789,789,789,789};
 int dropped=0;
 for(int i=0;i<8;i++) {
 int col=i%4,row=i/4;
 int x0=(int)Math.Round(col*src.width/4.0),x1=(int)Math.Round((col+1)*src.width/4.0);
 int y0=(int)Math.Round(row*src.height/2.0),y1=(int)Math.Round((row+1)*src.height/2.0);
 for(int y=y0;y<y1;y++) for(int x=x0;x<x1;x++) {
 var p=input[(src.height-1-y)*src.width+x];
 int dx=256+x-anchorX[i],dy=430+y-anchorY[i];
 if(dx<0||dx>=512||dy<0||dy>=512) {if(p.a>0)dropped++;continue;}
 pixels[(1023-(row*512+dy))*2048+col*512+dx]=p;
 }
 }
 if(dropped>0) throw new Exception("Visible pixel crop: "+dropped);
 var output=new Texture2D(2048,1024,TextureFormat.RGBA32,false);
 output.SetPixels32(pixels); output.Apply();
 File.WriteAllBytes(path,ImageConversion.EncodeToPNG(output));
 UnityEngine.Object.DestroyImmediate(src); UnityEngine.Object.DestroyImmediate(output);
 AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceUpdate);
 importer=(TextureImporter)AssetImporter.GetAtPath(path);
 importer.spritePixelsPerUnit=100;
 importer.textureCompression=TextureImporterCompression.Uncompressed;
 importer.mipmapEnabled=false;
 importer.spriteImportMode=SpriteImportMode.Multiple;
 importer.maxTextureSize=2048;
 importer.SaveAndReimport();
 d=f.GetSpriteEditorDataProviderFromObject(importer); d.InitSpriteEditorDataProvider();
 var rects=new SpriteRect[8];
 for(int i=0;i<8;i++) {
 var previous=old.FirstOrDefault(r=>r.name=="NewPlayerJump_"+i);
 rects[i]=new SpriteRect {name="NewPlayerJump_"+i,
 spriteID=previous!=null?previous.spriteID:GUID.Generate(),
 rect=new Rect(i%4*512,(1-i/4)*512,512,512),
 alignment=SpriteAlignment.Custom,pivot=new Vector2(0.5f,82f/512f),border=Vector4.zero};
 }
 var ids=d.GetDataProvider<ISpriteNameFileIdDataProvider>();
 if(ids==null) throw new Exception("Missing name file ID provider");
 d.SetSpriteRects(rects);
 ids.SetNameFileIdPairs(rects.Select(r=>new SpriteNameFileIdPair(r.name,r.spriteID)));
 d.Apply(); importer.SaveAndReimport();
 if(!AssetDatabase.IsValidFolder("Assets/05.Animation/VFX")) AssetDatabase.CreateFolder("Assets/05.Animation","VFX");
 if(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(animPath)!=null || AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ctrlPath)!=null)
 throw new Exception("Animation destination already exists");
 var sprites=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s=>int.Parse(s.name.Split('_').Last())).ToArray();
 if(sprites.Length!=8) throw new Exception("Expected eight sprites");
 var clip=new AnimationClip{name="PlayerJumpFire",frameRate=24};
 var keys=new ObjectReferenceKeyframe[9];
 for(int i=0;i<8;i++)keys[i]=new ObjectReferenceKeyframe{time=i/24f,value=sprites[i]};
 keys[8]=new ObjectReferenceKeyframe{time=8/24f,value=null};
 AnimationUtility.SetObjectReferenceCurve(clip,EditorCurveBinding.PPtrCurve("",typeof(SpriteRenderer),"m_Sprite"),keys);
 var settings=AnimationUtility.GetAnimationClipSettings(clip);settings.loopTime=false;AnimationUtility.SetAnimationClipSettings(clip,settings);
 AssetDatabase.CreateAsset(clip,animPath);
 var controller=AnimatorController.CreateAnimatorControllerAtPath(ctrlPath);
 var state=controller.layers[0].stateMachine.AddState("PlayerJumpFire");state.motion=clip;state.writeDefaultValues=false;
 controller.layers[0].stateMachine.defaultState=state;
 var root=PrefabUtility.LoadPrefabContents(prefabPath);
 try {
 var animator=root.GetComponent<Animator>();if(animator==null)animator=root.AddComponent<Animator>();
 animator.runtimeAnimatorController=controller;animator.applyRootMotion=false;animator.cullingMode=AnimatorCullingMode.AlwaysAnimate;
 root.GetComponent<SpriteRenderer>().sprite=sprites[0];
 PrefabUtility.SaveAsPrefabAsset(root,prefabPath);
 } finally {PrefabUtility.UnloadPrefabContents(root);}
 AssetDatabase.SaveAssets();
 return "Aligned PNG 2048x1024; 8 x 512x512; pivot=(256,82), PPU=100; no visible pixel crop; clip=24fps/"+clip.length+"s/nonloop; prefab Animator connected.";
 }
}

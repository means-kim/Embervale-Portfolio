using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class GlideCapture
{
    public static async System.Threading.Tasks.Task<string> Main()
    {
        var root = new GameObject("GlideCaptureTransient");
        var visual = new GameObject("Visual"); visual.transform.SetParent(root.transform, false);
        root.transform.position = new Vector3(2000,2000);
        var sprite = visual.AddComponent<SpriteRenderer>();
        var material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
        sprite.sharedMaterial = material;
        var cameraObject = new GameObject("CaptureCamera"); cameraObject.transform.SetParent(root.transform);
        var camera = cameraObject.AddComponent<Camera>(); camera.enabled=false;
        camera.orthographic=true; camera.orthographicSize=2f; camera.aspect=1;
        camera.transform.localPosition=new Vector3(0,.4f,-10);
        camera.clearFlags=CameraClearFlags.SolidColor; camera.backgroundColor=new Color(.055f,.07f,.10f);
        cameraObject.AddComponent<UniversalAdditionalCameraData>();
        var target=new RenderTexture(640,640,24);
        var previous=RenderTexture.active;
        var image=new Texture2D(640,640,TextureFormat.RGBA32,false);
        try {
            foreach(var item in new[]{"Fall","Glide_Right","Glide_Left","Glide_Frame4"}) {
                var clip=AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/05.Animation/player/"+(item=="Fall"?"Fall":"Glide")+".anim");
                visual.transform.localScale = new Vector3(item=="Glide_Left"?-1:1,1,1);
                var keys = AnimationUtility.GetObjectReferenceCurve(clip,
                    EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite"));
                sprite.sprite = (Sprite)keys[item=="Glide_Frame4"?4:0].value;
                EditorApplication.QueuePlayerLoopUpdate();
                await System.Threading.Tasks.Task.Delay(150);
                RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=target});
                RenderTexture.active=target;
                image.ReadPixels(new Rect(0,0,640,640),0,0); image.Apply();
                File.WriteAllBytes("Docs/Art/PlayerGlide_v01/"+item+".png",image.EncodeToPNG());
            }
            return "Four Unity sprite captures saved; identical camera and PPU, unlit art comparison";
        } finally {
            RenderTexture.active=previous; target.Release();
            UnityEngine.Object.DestroyImmediate(image); UnityEngine.Object.DestroyImmediate(target);
            UnityEngine.Object.DestroyImmediate(root); UnityEngine.Object.DestroyImmediate(material);
        }
    }
}

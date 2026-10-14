if(UnityEditor.EditorApplication.isPlaying)throw new System.Exception("Stop Play before import settings");
var manifest=Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText("C:/Unity/2d-team08/Docs/Art/HorizontalAttack_v02/Manifest.json"));
UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceSynchronousImport);
int count=0;
foreach(var f in manifest["frames"]){
 var path=(string)f["path"];var imp=(UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(path);
 imp.textureType=UnityEditor.TextureImporterType.Sprite;imp.spriteImportMode=UnityEditor.SpriteImportMode.Single;imp.spritePixelsPerUnit=200;imp.alphaIsTransparency=true;imp.mipmapEnabled=false;imp.filterMode=UnityEngine.FilterMode.Bilinear;imp.wrapMode=UnityEngine.TextureWrapMode.Clamp;imp.textureCompression=UnityEditor.TextureImporterCompression.Uncompressed;imp.maxTextureSize=512;
 var settings=new UnityEditor.TextureImporterSettings();imp.ReadTextureSettings(settings);settings.spriteAlignment=(int)UnityEngine.SpriteAlignment.Custom;settings.spritePivot=new UnityEngine.Vector2((float)f["pivotX"],(float)f["pivotY"]);settings.spriteMeshType=UnityEngine.SpriteMeshType.FullRect;imp.SetTextureSettings(settings);imp.SaveAndReimport();
 var sprite=UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Sprite>(path);
 if(sprite==null||sprite.rect.width!=512||sprite.rect.height!=512||sprite.pixelsPerUnit!=200)throw new System.Exception("Invalid sprite "+path);
 count++;
}
System.IO.File.WriteAllText("C:/Unity/2d-team08/Docs/Art/HorizontalAttack_v02/ImportVerification.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{success=true,sprites=count,ppu=200,frameSize=512,controllerChanged=false,clipChanged=false,previewOnly=true},Newtonsoft.Json.Formatting.Indented));
return "Imported 16 preview sprites; no clip/controller changes.";

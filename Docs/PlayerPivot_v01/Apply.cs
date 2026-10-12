if (UnityEditor.EditorApplication.isPlaying) throw new System.Exception("Stop: Play Mode active");
var plan=Newtonsoft.Json.Linq.JArray.Parse(System.IO.File.ReadAllText("Docs/PlayerPivot_v01/Plan.json"));
var batch=Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText("Docs/PlayerPivot_v01/Batch.json"));
int start=(int)batch["start"], count=(int)batch["count"];
var factory=new UnityEditor.U2D.Sprites.SpriteDataProviderFactories(); factory.Init();
var providers=new System.Collections.Generic.Dictionary<string,UnityEditor.U2D.Sprites.ISpriteEditorDataProvider>();
var originals=new Newtonsoft.Json.Linq.JArray();
// Preflight the entire immutable plan before applying any importer change.
foreach(var item in plan) {
    string path=(string)item["path"];
    if(UnityEditor.AssetDatabase.AssetPathToGUID(path)!=(string)item["guid"]) throw new System.Exception("GUID changed: "+path);
    var imp=UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
    if(imp==null || imp.spritePixelsPerUnit!=(float)item["ppu"]) throw new System.Exception("Importer changed: "+path);
    var dp=factory.GetSpriteEditorDataProviderFromObject(imp);
    if(dp==null) throw new System.Exception("No provider: "+path);
    dp.InitSpriteEditorDataProvider();
    var cap=dp.GetDataProvider<UnityEditor.U2D.Sprites.ISpriteFrameEditCapability>();
    if(cap==null || !cap.GetEditCapability().HasCapability(UnityEditor.U2D.Sprites.EEditCapability.EditPivot))
        throw new System.Exception("Pivot capability unavailable: "+path);
    var rects=dp.GetSpriteRects();
    if(rects.Length!=item["changes"].Count()) throw new System.Exception("Sprite count changed: "+path);
    foreach(var c in item["changes"]) if(!rects.Any(r=>r.spriteID.ToString()==(string)c["id"])) throw new System.Exception("Sprite ID missing: "+path);
    providers[path]=dp;
    var identities=new Newtonsoft.Json.Linq.JArray();
    foreach(var s in UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path).OfType<UnityEngine.Sprite>()) {
        string guid;long id;UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(s,out guid,out id);
        identities.Add(new Newtonsoft.Json.Linq.JObject{{"name",s.name},{"guid",guid},{"localID",id}});
    }
    originals.Add(new Newtonsoft.Json.Linq.JObject{{"path",path},{"identities",identities},{"sprites",Newtonsoft.Json.Linq.JArray.FromObject(rects.Select(r=>new {id=r.spriteID.ToString(),r.name,alignment=(int)r.alignment,px=r.pivot.x,py=r.pivot.y,x=r.rect.x,y=r.rect.y,width=r.rect.width,height=r.rect.height,border=new {x=r.border.x,y=r.border.y,z=r.border.z,w=r.border.w}}))}});
}
if(start==0) {
    if(System.IO.File.Exists("Docs/PlayerPivot_v01/OriginalImporter.json")) throw new System.Exception("Original snapshot already exists");
    System.IO.File.WriteAllText("Docs/PlayerPivot_v01/OriginalImporter.json",originals.ToString());
}
int updated=0;
foreach(var item in plan.Skip(start).Take(count)) {
    string path=(string)item["path"];
    var imp=UnityEditor.AssetImporter.GetAtPath(path);
    var dp=providers[path];var rects=dp.GetSpriteRects();
    foreach(var c in item["changes"]) {
        var rect=rects.First(r=>r.spriteID.ToString()==(string)c["id"]);
        rect.alignment=UnityEngine.SpriteAlignment.Custom;
        rect.pivot=new UnityEngine.Vector2((float)c["newX"],(float)c["newY"]);
    }
    dp.SetSpriteRects(rects);dp.Apply();imp.SaveAndReimport(); updated++;
}
return "Updated textures "+updated+" in range "+start+".."+(start+updated-1);

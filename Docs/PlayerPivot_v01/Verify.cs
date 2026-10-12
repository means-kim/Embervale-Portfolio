var plan=Newtonsoft.Json.Linq.JArray.Parse(System.IO.File.ReadAllText("Docs/PlayerPivot_v01/Plan.json"));
var old=Newtonsoft.Json.Linq.JArray.Parse(System.IO.File.ReadAllText("Docs/PlayerPivot_v01/OriginalImporter.json"));
var factory=new UnityEditor.U2D.Sprites.SpriteDataProviderFactories();factory.Init();
int sprites=0;var failures=new System.Collections.Generic.List<string>();
foreach(var item in plan) {
    string path=(string)item["path"];
    var baseline=old.First(x=>(string)x["path"]==path);
    var imp=UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
    var dp=factory.GetSpriteEditorDataProviderFromObject(imp);dp.InitSpriteEditorDataProvider();
    var rects=dp.GetSpriteRects();
    if(UnityEditor.AssetDatabase.AssetPathToGUID(path)!=(string)item["guid"] || imp.spritePixelsPerUnit!=(float)item["ppu"]) failures.Add("Importer contract: "+path);
    foreach(var c in item["changes"]) {
        var r=rects.First(x=>x.spriteID.ToString()==(string)c["id"]);
        var b=baseline["sprites"].First(x=>(string)x["id"]==(string)c["id"]);
        if(UnityEngine.Vector2.Distance(r.pivot,new UnityEngine.Vector2((float)c["newX"],(float)c["newY"]))>0.00001f) failures.Add("Pivot: "+path+" "+r.name);
        if(r.name!=(string)b["name"] || r.rect!=new UnityEngine.Rect((float)b["x"],(float)b["y"],(float)b["width"],(float)b["height"])) failures.Add("Rect/name: "+path+" "+r.name);
        sprites++;
    }
    foreach(var s in UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path).OfType<UnityEngine.Sprite>()) {
        string guid;long id;UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(s,out guid,out id);
        if(!baseline["identities"].Any(x=>(string)x["name"]==s.name && (string)x["guid"]==guid && (long)x["localID"]==id)) failures.Add("Reference identity: "+path+" "+s.name);
        var c=item["changes"].First(x=>(string)x["name"]==s.name);
        var r=rects.First(x=>x.name==s.name);
        // Unity scales both sprite rect and PPU when source sheets exceed importer Max Size.
        if(UnityEngine.Vector2.Distance(s.pivot,new UnityEngine.Vector2((float)c["newX"]*s.rect.width,(float)c["newY"]*s.rect.height))>0.01f) failures.Add("Loaded Sprite pivot: "+path+" "+s.name);
    }
}
var report=new {textures=plan.Count,sprites,failures,importerChecksPassed=failures.Count==0,playModeTested=false};
System.IO.File.WriteAllText("Docs/PlayerPivot_v01/UnityVerification.json",Newtonsoft.Json.JsonConvert.SerializeObject(report,Newtonsoft.Json.Formatting.Indented));
if(failures.Count>0) throw new System.Exception(string.Join("; ",failures));
return "Verified "+plan.Count+" textures, "+sprites+" sprite pivots and reference IDs";

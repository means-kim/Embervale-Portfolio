if(UnityEditor.EditorApplication.isPlaying) throw new System.Exception("Stop: Play Mode active");
var old=Newtonsoft.Json.Linq.JArray.Parse(System.IO.File.ReadAllText("Docs/PlayerPivot_v01/OriginalImporter.json"));
var factory=new UnityEditor.U2D.Sprites.SpriteDataProviderFactories();factory.Init();
var providers=new System.Collections.Generic.Dictionary<string,UnityEditor.U2D.Sprites.ISpriteEditorDataProvider>();
foreach(var item in old) {
    string path=(string)item["path"];
    var dp=factory.GetSpriteEditorDataProviderFromObject(UnityEditor.AssetImporter.GetAtPath(path));dp.InitSpriteEditorDataProvider();
    var cap=dp.GetDataProvider<UnityEditor.U2D.Sprites.ISpriteFrameEditCapability>();
    if(cap==null || !cap.GetEditCapability().HasCapability(UnityEditor.U2D.Sprites.EEditCapability.EditPivot)) throw new System.Exception("Pivot capability unavailable: "+path);
    foreach(var b in item["sprites"]) if(!dp.GetSpriteRects().Any(r=>r.spriteID.ToString()==(string)b["id"])) throw new System.Exception("Sprite ID changed: "+path);
    providers[path]=dp;
}
foreach(var item in old) {
    string path=(string)item["path"];var dp=providers[path];var rects=dp.GetSpriteRects();
    foreach(var b in item["sprites"]) {
        var r=rects.First(x=>x.spriteID.ToString()==(string)b["id"]);
        r.pivot=new UnityEngine.Vector2((float)b["px"],(float)b["py"]);r.alignment=(UnityEngine.SpriteAlignment)(int)b["alignment"];
    }
    dp.SetSpriteRects(rects);dp.Apply();UnityEditor.AssetImporter.GetAtPath(path).SaveAndReimport();
}
return "Original pivots restored";

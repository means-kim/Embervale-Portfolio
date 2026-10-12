if (UnityEditor.EditorApplication.isPlaying) throw new System.Exception("Stop: Play Mode active");
var uses = new System.Collections.Generic.Dictionary<string,System.Collections.Generic.HashSet<string>>();
foreach (var g in UnityEditor.AssetDatabase.FindAssets("t:AnimationClip", new [] {"Assets"})) {
    var p = UnityEditor.AssetDatabase.GUIDToAssetPath(g);
    var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.AnimationClip>(p);
    foreach (var b in UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip))
    foreach (var k in UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip,b)) {
        var s=k.value as UnityEngine.Sprite; if(s==null) continue;
        var sp=UnityEditor.AssetDatabase.GetAssetPath(s);
        if(!sp.StartsWith("Assets/04.Sprites/Player/")) continue;
        if(!uses.ContainsKey(sp)) uses[sp]=new System.Collections.Generic.HashSet<string>();
        uses[sp].Add(p);
    }
}
var factory=new UnityEditor.U2D.Sprites.SpriteDataProviderFactories(); factory.Init();
var items=new System.Collections.Generic.List<object>();
foreach(var kv in uses.OrderBy(k=>k.Key)) {
    var imp=UnityEditor.AssetImporter.GetAtPath(kv.Key) as UnityEditor.TextureImporter;
    var dp=factory.GetSpriteEditorDataProviderFromObject(imp); dp.InitSpriteEditorDataProvider();
    var cap=dp.GetDataProvider<UnityEditor.U2D.Sprites.ISpriteFrameEditCapability>();
    items.Add(new {path=kv.Key,guid=UnityEditor.AssetDatabase.AssetPathToGUID(kv.Key),ppu=imp.spritePixelsPerUnit,mode=imp.spriteImportMode.ToString(),clips=kv.Value.ToArray(),canEdit=cap!=null && cap.GetEditCapability().HasCapability(UnityEditor.U2D.Sprites.EEditCapability.EditPivot),sprites=dp.GetSpriteRects().Select(r=>new {r.name,id=r.spriteID.ToString(),x=r.rect.x,y=r.rect.y,width=r.rect.width,height=r.rect.height,px=r.pivot.x,py=r.pivot.y}).ToArray()});
}
System.IO.File.WriteAllText("Docs/PlayerPivot_v01/Inventory.json",Newtonsoft.Json.JsonConvert.SerializeObject(items,Newtonsoft.Json.Formatting.Indented));
return "Inventoried "+items.Count+" textures";

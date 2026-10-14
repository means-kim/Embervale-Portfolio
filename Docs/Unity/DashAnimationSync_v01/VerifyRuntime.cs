if (!UnityEditor.EditorApplication.isPlaying) throw new System.Exception("Play required");
var p = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
var input = p.GetComponent<PlayerInput>();
var dash = p.GetComponent<PlayerDash>();
var charge = p.GetComponent<PlayerChargeJump>();
var visual = p.GetComponent<PlayerChargeJumpVisual>();
var body = p.GetComponent<UnityEngine.Rigidbody2D>();
var sr = p.GetComponent<UnityEngine.SpriteRenderer>();
var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
System.Action<string> dispatch = name => ((System.Action)typeof(PlayerInput).GetField(name, flags).GetValue(input))();
var rows = new System.Collections.Generic.List<object>();
var errors = new System.Collections.Generic.List<string>();
var origin = body.position + UnityEngine.Vector2.up * 8f;
double start = UnityEditor.EditorApplication.timeSinceStartup;
int step = 0;
bool seenActive = false, finished = false;
UnityEngine.Application.LogCallback log = (message, trace, type) => { if (type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception) errors.Add(message); };
UnityEngine.Application.logMessageReceived += log;
UnityEditor.EditorApplication.CallbackFunction tick = null;
System.Action<string> finish = error =>
{
    if (finished) return;
    finished = true;
    UnityEditor.EditorApplication.update -= tick;
    UnityEngine.Application.logMessageReceived -= log;
    var result = new { success = error == null && errors.Count == 0 && step == 4, error, rows, errors, scope = "Production input events and real Update timers on the scene player. Runtime position lifted for clearance; physical keys/mouse not injected." };
    System.IO.File.WriteAllText("C:/Unity/2d-team08/Docs/Unity/DashAnimationSync_v01/Runtime.json", Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
};
tick = () =>
{
    try
    {
        if (!UnityEditor.EditorApplication.isPlaying || p == null) { finish("Play ended"); return; }
        double t = UnityEditor.EditorApplication.timeSinceStartup - start;
        if (t > 4f) { finish("Timeout"); return; }
        if ((step == 0 && t > 0.2f) || (step == 2 && t > 1.0f))
        {
            body.position = origin;
            body.linearVelocity = UnityEngine.Vector2.zero;
            p.GetComponent<PlayerJump>().BeginExternalLaunch();
            if (step == 0) dispatch("OnDashInput");
            else { dispatch("OnChargeJumpPressed"); dispatch("OnChargeJumpReleased"); }
            bool active = step == 0 ? dash.IsDashing : charge.IsLaunching;
            var shown = visual.Resolve(sr);
            rows.Add(new { motion = step == 0 ? "dash" : "charge", point = "start", t, frame = UnityEngine.Time.frameCount, active, visualActive = visual.IsActive, sprite = shown.sprite.name });
            if (!active || !visual.IsActive || !shown.sprite.name.StartsWith("Dash_")) { finish("Launch rejected or stale start sprite"); return; }
            seenActive = true;
            step++;
        }
        else if ((step == 1 || step == 3) && seenActive && !(step == 1 ? dash.IsDashing : charge.IsLaunching))
        {
            rows.Add(new { motion = step == 1 ? "dash" : "charge", point = "first observed end", t, frame = UnityEngine.Time.frameCount, visualActive = visual.IsActive, sourceHidden = sr.forceRenderingOff, sprite = sr.sprite.name, state = p.CurrentState.ToString() });
            if (visual.IsActive || sr.forceRenderingOff || sr.sprite.name.StartsWith("Dash_")) { finish("Stale dash sprite after exit"); return; }
            step++;
            seenActive = false;
            if (step == 4) finish(null);
        }
    }
    catch (System.Exception e) { finish(e.ToString()); }
};
UnityEditor.EditorApplication.update += tick;
return "Natural exit probe started";

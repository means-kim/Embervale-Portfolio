if (!UnityEditor.EditorApplication.isPlaying) throw new System.Exception("Play required");
var p = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
var pa = p.GetComponent<PlayerAnimator>();
var a = p.GetComponent<UnityEngine.Animator>();
var b = p.GetComponent<UnityEngine.Rigidbody2D>();
var sr = p.GetComponent<UnityEngine.SpriteRenderer>();
var visual = p.GetComponent<PlayerChargeJumpVisual>();
var errors = new System.Collections.Generic.List<string>();
var cases = new System.Collections.Generic.List<object>();
UnityEngine.Application.LogCallback log = (message, trace, type) => { if (type == UnityEngine.LogType.Error || type == UnityEngine.LogType.Exception || type == UnityEngine.LogType.Warning) errors.Add(message); };
UnityEngine.Application.logMessageReceived += log;
var originalRotation = p.transform.rotation;
try
{
    p.GetComponent<PlayerDash>().CancelDash();
    p.GetComponent<PlayerChargeJump>().CancelChargeJump();
    foreach (bool charge in new bool[] { false, true })
    foreach (float x in new float[] { 1f, -1f })
    foreach (float y in new float[] { 8f, -8f, 0f })
    {
        p.GetComponent<PlayerMove>().SetFacingDirection(x);
        b.linearVelocity = new UnityEngine.Vector2(0f, y);
        p.ChangeState(PlayerState.Jump);
        pa.PlayState(PlayerState.Jump, false);
        a.Update(0f);
        string before = sr.sprite.name;
        var direction = new UnityEngine.Vector2(x, 1f).normalized;
        if (charge) pa.PlayChargeJumpLaunch(direction); else pa.PlayDirectionalDash(direction);
        var shown = visual.Resolve(sr);
        bool startPass = visual.IsActive && sr.forceRenderingOff && shown.enabled && shown.sprite.name.StartsWith("Dash_") && shown != sr;
        string start = shown.sprite.name;
        if (charge) pa.EndChargeJumpLaunch(); else pa.EndDirectionalDash();
        string expected = y > 0 ? "JumpRise" : y < 0 ? "Fall" : "JumpApex";
        // No Animator.Update or player-loop advance between End and this assertion.
        bool endPass = !visual.IsActive && !sr.forceRenderingOff && !shown.enabled && sr.sprite.name.StartsWith(expected + "_") && a.GetCurrentAnimatorStateInfo(0).IsName(expected);
        bool rootPreserved = UnityEngine.Quaternion.Angle(originalRotation, p.transform.rotation) < 0.001f;
        cases.Add(new { kind = charge ? "charge" : "dash", x, y, before, start, end = sr.sprite.name, expected, startPass, endPass, rootPreserved });
        if (!startPass || !endPass || !rootPreserved) throw new System.Exception("Pose mismatch " + charge + " " + x + " " + y);
    }
    b.linearVelocity = UnityEngine.Vector2.zero;
    pa.PlayState(PlayerState.Jump, false);
    a.Update(0f);
    pa.Tick(PlayerState.Jump, false, 0f);
    a.Update(0f);
    bool gracePreserved = sr.sprite.name.StartsWith("JumpRise_");
    cases.Add(new { kind = "ordinary jump apex grace", pass = gracePreserved });
    if (!gracePreserved) throw new System.Exception("Ordinary jump grace changed");
    pa.BeginJump(true);
    a.Update(0f);
    b.linearVelocity = UnityEngine.Vector2.up * 8f;
    pa.PlayDirectionalDash(UnityEngine.Vector2.up);
    pa.EndDirectionalDash();
    bool doubleJumpPass = sr.sprite.name.StartsWith("JumpRise_") && a.speed == 1f;
    cases.Add(new { kind = "double jump interruption", pass = doubleJumpPass });
    if (!doubleJumpPass) throw new System.Exception("Double jump pose retained");
    p.ChangeState(PlayerState.Idle);
    foreach (var state in new PlayerState[] { PlayerState.Idle, PlayerState.Hit, PlayerState.Death })
    {
        pa.PlayDirectionalDash(UnityEngine.Vector2.up);
        if (state == PlayerState.Idle) pa.EndDirectionalDash(); else pa.PlayState(state, false);
        bool pass = !visual.IsActive && !sr.forceRenderingOff && a.GetCurrentAnimatorStateInfo(0).IsName(state.ToString());
        cases.Add(new { kind = "exit to " + state, pass });
        if (!pass) throw new System.Exception("Interrupt mismatch " + state);
    }
    pa.PlayDirectionalDash(UnityEngine.Vector2.up);
    pa.enabled = false;
    bool disablePass = !visual.IsActive && !sr.forceRenderingOff;
    pa.enabled = true;
    cases.Add(new { kind = "component disable", pass = disablePass });
    if (!disablePass) throw new System.Exception("Hidden renderer after disable");
}
finally
{
    UnityEngine.Application.logMessageReceived -= log;
    pa.EndDirectionalDash();
    pa.PlayState(p.CurrentState, false);
    a.Update(0f);
}
var report = new { success = errors.Count == 0, cases, errors, scene = p.gameObject.scene.path, scope = "Immediate displayed sprite at directional motion boundaries; no hardware input or full-scene traversal claim" };
System.IO.File.WriteAllText("C:/Unity/2d-team08/Docs/Unity/DashAnimationSync_v01/After.json", Newtonsoft.Json.JsonConvert.SerializeObject(report, Newtonsoft.Json.Formatting.Indented));
return report;

if (!UnityEditor.EditorApplication.isPlaying) throw new System.Exception("Play required");
var p = UnityEngine.Object.FindFirstObjectByType<PlayerController>();
var pa = p.GetComponent<PlayerAnimator>();
var a = p.GetComponent<UnityEngine.Animator>();
var b = p.GetComponent<UnityEngine.Rigidbody2D>();
var sr = p.GetComponent<UnityEngine.SpriteRenderer>();
var visual = p.GetComponent<PlayerChargeJumpVisual>();
var cases = new System.Collections.Generic.List<object>();
foreach (float y in new float[] { 8f, -8f, 0f })
{
    p.GetComponent<PlayerDash>().CancelDash();
    p.GetComponent<PlayerChargeJump>().CancelChargeJump();
    pa.EndDirectionalDash();
    b.linearVelocity = new UnityEngine.Vector2(0f, y);
    p.ChangeState(PlayerState.Jump);
    pa.PlayState(PlayerState.Jump, false);
    a.Update(0f);
    string before = sr.sprite.name;
    pa.PlayDirectionalDash(new UnityEngine.Vector2(1f, 1f).normalized);
    string immediateStart = visual.Resolve(sr).sprite.name;
    a.Update(0f);
    string during = visual.Resolve(sr).sprite.name;
    pa.EndDirectionalDash();
    string immediateEnd = sr.sprite.name;
    a.Update(0f);
    cases.Add(new { y, before, immediateStart, during, immediateEnd, evaluatedEnd = sr.sprite.name, visualActive = visual.IsActive, sourceHidden = sr.forceRenderingOff });
}
var output = "C:/Unity/2d-team08/Docs/Unity/DashAnimationSync_v01/Before.json";
System.IO.File.WriteAllText(output, Newtonsoft.Json.JsonConvert.SerializeObject(cases, Newtonsoft.Json.Formatting.Indented));
return cases;

var written = new System.Collections.Generic.List<string>();
{
var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/05.Animation/Boss/Charge/Boss_Charge_Loop.anim");
UnityEditor.Undo.RegisterCompleteObjectUndo(clip, "Add boss collider curves");
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,-0.3f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,2.5f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(Transform), "localEulerAnglesRaw.z");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(BoxCollider2D), "m_Size.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,5.7f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(BoxCollider2D), "m_Size.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,4.7f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(BoxCollider2D), "m_Enabled");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,1f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,-3.5f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0.7f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(Transform), "localEulerAnglesRaw.z");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(BoxCollider2D), "m_Size.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,1.9f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(BoxCollider2D), "m_Size.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,1.3f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(BoxCollider2D), "m_Enabled");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,-1.15f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,3.45f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(Transform), "localEulerAnglesRaw.z");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,13f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(BoxCollider2D), "m_Size.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,5.45f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(BoxCollider2D), "m_Size.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,1.55f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(BoxCollider2D), "m_Enabled");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,-1.5f),new Keyframe(0.100000000f,-1.4f),new Keyframe(0.200000000f,-1.5f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,2.6f),new Keyframe(0.100000000f,2.5f),new Keyframe(0.200000000f,2.6f),new Keyframe(0.300000000f,2.8f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(Transform), "localEulerAnglesRaw.z");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(BoxCollider2D), "m_Size.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,3.7f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(BoxCollider2D), "m_Size.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,4.6f),new Keyframe(0.100000000f,4.5f),new Keyframe(0.200000000f,4.6f),new Keyframe(0.300000000f,4.4f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(BoxCollider2D), "m_Enabled");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,1f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
UnityEditor.EditorUtility.SetDirty(clip);
UnityEditor.AssetDatabase.SaveAssetIfDirty(clip);
written.Add(clip.name);
}
{
var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/05.Animation/Boss/Charge/Boss_Charge_Start.anim");
UnityEditor.Undo.RegisterCompleteObjectUndo(clip, "Add boss collider curves");
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f),new Keyframe(0.150000000f,-0.25f),new Keyframe(0.700000000f,-0.3f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,2.95f),new Keyframe(0.150000000f,2.25f),new Keyframe(0.700000000f,2.5f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(Transform), "localEulerAnglesRaw.z");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(BoxCollider2D), "m_Size.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,5.6f),new Keyframe(0.150000000f,5.4f),new Keyframe(0.700000000f,5.7f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(BoxCollider2D), "m_Size.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,5.6f),new Keyframe(0.150000000f,4.3f),new Keyframe(0.700000000f,4.7f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(BoxCollider2D), "m_Enabled");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,1f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,-3.5f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0.7f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(Transform), "localEulerAnglesRaw.z");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(BoxCollider2D), "m_Size.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,1.9f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(BoxCollider2D), "m_Size.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,1.3f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SlamHitbox", typeof(BoxCollider2D), "m_Enabled");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,-1.15f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,3.45f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(Transform), "localEulerAnglesRaw.z");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,13f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(BoxCollider2D), "m_Size.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,5.45f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(BoxCollider2D), "m_Size.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,1.55f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(BoxCollider2D), "m_Enabled");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,-1.45f),new Keyframe(0.700000000f,-1.5f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,2.7f),new Keyframe(0.700000000f,2.6f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(Transform), "localEulerAnglesRaw.z");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(BoxCollider2D), "m_Size.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,3.7f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(BoxCollider2D), "m_Size.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,4.8f),new Keyframe(0.700000000f,4.6f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(BoxCollider2D), "m_Enabled");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f),new Keyframe(0.700000000f,1f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
UnityEditor.EditorUtility.SetDirty(clip);
UnityEditor.AssetDatabase.SaveAssetIfDirty(clip);
written.Add(clip.name);
}

return written;

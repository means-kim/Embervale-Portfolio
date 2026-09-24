var written = new System.Collections.Generic.List<string>();
{
var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/05.Animation/Boss/Boss_Swing.anim");
UnityEditor.Undo.RegisterCompleteObjectUndo(clip, "Add boss collider curves");
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f),new Keyframe(0.250000000f,-0.34f),new Keyframe(0.400000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,2.95f) });
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
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,5.6f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(BoxCollider2D), "m_Size.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,5.6f) });
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
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,-1.15f),new Keyframe(0.700000000f,-1.6f),new Keyframe(0.800000000f,-1.15f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,3.45f),new Keyframe(0.700000000f,3.55f),new Keyframe(0.800000000f,3.45f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(Transform), "localEulerAnglesRaw.z");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,13f),new Keyframe(0.700000000f,8f),new Keyframe(0.800000000f,13f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/SwingHitbox", typeof(BoxCollider2D), "m_Size.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,5.45f),new Keyframe(0.700000000f,5.5f),new Keyframe(0.800000000f,5.45f) });
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
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f),new Keyframe(0.600000000f,1f),new Keyframe(0.800000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,-1.45f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,2.7f) });
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
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,4.8f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(BoxCollider2D), "m_Enabled");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
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
var clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/05.Animation/Boss/Boss_Walk.anim");
UnityEditor.Undo.RegisterCompleteObjectUndo(clip, "Add boss collider curves");
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(Transform), "m_LocalPosition.x");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,2.95f) });
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
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,5.6f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/Hurtbox", typeof(BoxCollider2D), "m_Size.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,5.6f) });
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
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,-1.45f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(Transform), "m_LocalPosition.y");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,2.7f) });
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
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,4.8f) });
for (int k = 0; k < curve.length; k++) {
    UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
    UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, k, UnityEditor.AnimationUtility.TangentMode.Constant);
}
UnityEditor.AnimationUtility.SetEditorCurve(clip, binding, curve);
}
{
var binding = UnityEditor.EditorCurveBinding.FloatCurve("Colliders/ChargeHitbox", typeof(BoxCollider2D), "m_Enabled");
var curve = new AnimationCurve(new[] { new Keyframe(0.000000000f,0f) });
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

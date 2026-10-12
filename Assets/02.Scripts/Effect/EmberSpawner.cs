using UnityEngine;

/// <summary>EmberPoint에서 불씨를 독립 생성한다. 프리팹의 파티클은 Play On Awake로 재생한다.</summary>
[DisallowMultipleComponent]
public class EmberSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("소환할 불씨 프리팹. Particle System의 Play On Awake를 켜고 Looping을 끄세요.")]
    private GameObject _emberPrefab;
    [SerializeField, Tooltip("공격 VFX가 생성되면 Start에서 한 번 소환합니다.")]
    private bool _spawnOnStart = true;
    [SerializeField, Tooltip("EmberPoint의 월드 크기와 좌우 반전을 불씨에 적용합니다.")]
    private bool _inheritScale = true;

    // 공격 프리팹의 Initialize에서 방향/크기가 설정된 다음 소환한다.
    private void Start()
    {
        if (_spawnOnStart) Spawn();
    }

    [ContextMenu("Spawn Embers (Play Mode)")]
    public void Spawn()
    {
        if (!Application.isPlaying || !isActiveAndEnabled || _emberPrefab == null) return;

        // 부모를 지정하지 않아 공격 VFX가 삭제되어도 불씨는 남는다.
        var ember = Instantiate(_emberPrefab, transform.position, transform.rotation);
        if (_inheritScale)
            ember.transform.localScale = Vector3.Scale(ember.transform.localScale, transform.lossyScale);

        // 기존 정리 컴포넌트가 없으면 파티클 수명 기반 정리를 추가한다.
        if (ember.GetComponent<OneShotVFX>() == null)
            ember.AddComponent<OneShotVFX>();
    }
}

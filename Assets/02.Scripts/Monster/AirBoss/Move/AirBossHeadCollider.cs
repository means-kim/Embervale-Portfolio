using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class AirBossHeadCollider : MonoBehaviour
{
    [Serializable]
    private struct HeadPosition
    {
        public Sprite sprite;
        public Vector2 offset;
    }

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private CircleCollider2D _headCollider;

    [Header("루트 기준 기본 머리 위치")]
    [SerializeField] private Vector2 _defaultOffset = new Vector2(0.5f, 10f);

    [Header("좌우 이동 스프라이트별 머리 위치")]
    [SerializeField] private HeadPosition[] _headPositions;

    private void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (_headCollider == null)
        {
            _headCollider = GetComponent<CircleCollider2D>();
        }
    }

    private void LateUpdate()
    {
        if (_spriteRenderer == null || _headCollider == null) return;

        // Animator가 실제로 표시한 프레임을 기준으로 맞춘다.
        // 등록되지 않은 Idle/상하 이동/공격 프레임은 기본 위치로 복귀한다.
        Sprite currentSprite = _spriteRenderer.sprite;
        Vector2 offset = _defaultOffset;

        if (currentSprite != null && _headPositions != null)
        {
            foreach (HeadPosition position in _headPositions)
            {
                if (position.sprite != currentSprite) continue;

                offset = position.offset;
                break;
            }
        }

        _headCollider.offset = offset;
    }

    private void OnDisable()
    {
        if (_headCollider != null)
            _headCollider.offset = _defaultOffset;
    }
}
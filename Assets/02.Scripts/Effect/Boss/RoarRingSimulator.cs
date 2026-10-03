using UnityEngine;

namespace Game.Boss.Roar
{
    public sealed class RoarRingSimulator
    {
        public const int MaxRings = 8;

        private struct Ring
        {
            public float Radius;
            public float Intensity;
            public float Seed;
        }

        private readonly Ring[] _rings = new Ring[MaxRings];
        private int _count;
        private BossRoarSettings _roarSetting;

        private float _elapsed;
        private float _duration;
        private float _spawnTimer;
        private bool _playing;
        private bool _releasing;
        private float _releaseTimer;
        private int _spawned;
        private uint _seedCounter;

        // 현재 화면 틴트/링 전체에 곱해지는 엔벨로프 (0~1)
        public float Envelope { get; private set; }
        // 현재 플래시 세기
        public float Flash { get; private set; }
        // 현재 흔들림 진폭 (화면 높이 단위)
        public float ShakeAmplitude { get; private set; }
        public bool IsActive => _playing || _count > 0 || Envelope > 0.001f;
        public int RingCount => _count;

        public void Play(BossRoarSettings settings, float duration)
        {
            _roarSetting = settings;
            _duration = Mathf.Max(0.1f, duration);
            _elapsed = 0f;
            _spawnTimer = 0f;          // 즉시 첫 링
            _spawned = 0;
            _playing = true;
            _releasing = false;
            Flash = Mathf.Max(Flash, _roarSetting.startFlash);
        }

        // 포효 끝냄 (페이드아웃은 유지)
        public void Stop()
        {
            if (_playing)
            {
                _playing = false;
                _releasing = true;
                _releaseTimer = 0f;
            }
        }

        // 타이틀 등장 등 임의 시점 플래시
        public void AddFlash(float amount) => Flash = Mathf.Max(Flash, amount);

        // <param name="maxRadius">이 반지름을 넘으면 링 제거 (화면 밖 = 중심에서 가장 먼 모서리까지 거리)
        public void Tick(float dt, float maxRadius)
        {
            if (_roarSetting == null)
            {
                return;
            }

            // ---- 재생 / 링 생성
            if (_playing)
            {
                _elapsed += dt;
                _spawnTimer -= dt;
                while (_spawnTimer <= 0f && _elapsed < _duration)
                {
                    Spawn(-_spawnTimer);   // 프레임 사이 생성 시점 보정 (프레임레이트 무관하게 간격 일정)
                    _spawnTimer += _roarSetting.ringInterval;
                }
                if (_elapsed >= _duration)
                {
                    Stop();
                }
            }

            // ---- 엔벨로프
            if (_playing)
            {
                Envelope = _roarSetting.fadeIn > 0f ? Mathf.Clamp01(_elapsed / _roarSetting.fadeIn) : 1f;
            }
            else if (_releasing)
            {
                _releaseTimer += dt;
                Envelope = _roarSetting.fadeOut > 0f ? 1f - Mathf.Clamp01(_releaseTimer / _roarSetting.fadeOut) : 0f;
                if (Envelope <= 0f)
                {
                    _releasing = false;
                }
            }
            else
            {
                Envelope = 0f;
            }

            // ---- 링 확장 / 소멸 (바깥 40% 구간에서 페이드)
            int write = 0;
            for (int i = 0; i < _count; i++)
            {
                Ring r = _rings[i];
                r.Radius += _roarSetting.ringSpeed * dt;
                if (r.Radius < maxRadius)
                {
                    _rings[write++] = r;
                }
            }
            _count = write;

            // ---- 플래시 / 흔들림
            Flash *= Mathf.Exp(-_roarSetting.flashDecay * dt);
            float startKick = _roarSetting.shakeOnStart * Mathf.Exp(-_elapsed * 6f) * (_playing ? 1f : 0f);
            ShakeAmplitude = startKick + _roarSetting.shakeSustain * Envelope;
        }

        private void Spawn(float ageOffset)
        {
            if (_count >= MaxRings)
            {
                // 가장 오래된(가장 큰) 링 제거
                for (int i = 1; i < _count; i++)
                {
                    _rings[i - 1] = _rings[i];
                }
                _count--;
            }
            float jitter = 1f + Random.Range(-_roarSetting.intensityJitter, _roarSetting.intensityJitter);
            float boost = _spawned == 0 ? _roarSetting.firstRingBoost : 1f;
            _rings[_count++] = new Ring
            {
                Radius = _roarSetting.startRadius + _roarSetting.ringSpeed * ageOffset,
                Intensity = _roarSetting.ringIntensity * jitter * boost,
                Seed = (_seedCounter++ % 997) * 1.618f,
            };
            _spawned++;
            Flash = Mathf.Max(Flash, _roarSetting.pulsePerRing * boost);
        }

        // 셰이더 전달용 배열 채우기. 반지름에 따른 페이드와 엔벨로프를 반영한다
        public int Fill(Vector2 viewportCenter, float maxRadius, Vector4[] rings, float[] seeds)
        {
            float fadeStart = maxRadius * 0.6f;
            for (int i = 0; i < _count; i++)
            {
                Ring r = _rings[i];
                float fade = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(fadeStart, maxRadius, r.Radius));
                float grow = Mathf.SmoothStep(0f, 1f, r.Radius / Mathf.Max(_roarSetting.thickness, 1e-3f));   // 생성 직후 팝 방지
                rings[i] = new Vector4(viewportCenter.x, viewportCenter.y, r.Radius, r.Intensity * fade * grow * Mathf.Max(Envelope, 0.35f));
                seeds[i] = r.Seed;
            }
            return _count;
        }
    }
}

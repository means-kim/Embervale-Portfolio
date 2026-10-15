using UnityEngine;

public class ResponeZone : MonoBehaviour
{
    [SerializeField] private GameObject _checkPoint;
    [SerializeField, Tooltip("보스 2·3페이즈 연출 중에는 낙하 피해와 강제 복귀를 실행하지 않습니다.")]
    private bool _ignoreDuringBossCinematics = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInParent<Player>();

        if (player == null || _checkPoint == null)
        {
            return;
        }

        if (Respone.Instance == null)
        {
            return;
        }

        if (_ignoreDuringBossCinematics && IsBossPhaseCinematicPlaying(player))
        {
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsRecentRetry)
        {
            Debug.LogWarning("ResponeZone: 재시작 직후 위험 구역 진입. zone=" + name +
                             ", position=" + player.transform.position + ", destination=" +
                             _checkPoint.transform.position + ", dead=" + player.GetComponent<Health>().IsDead, this);
        }

        Respone.Instance.ResponePlayer(player, _checkPoint.transform);
    }

    private static bool IsBossPhaseCinematicPlaying(Player player)
    {
        // Phase3 includes the platform transition after its Timeline has ended.
        foreach (CinematicSequence sequence in FindObjectsByType<CinematicSequence>(FindObjectsSortMode.None))
        {
            if (sequence.gameObject.scene != player.gameObject.scene) continue;
            if (sequence is BossPhase2Cinematic phase2 && phase2.IsPlaying) return true;
            if (sequence is BossPhase3Cinematic phase3 && phase3.IsPlaying) return true;
        }
        return false;
    }
}

using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] private CinematicBars _cinematicBars;
    [SerializeField] private BossEncounterCinematic _bossEncounter;

    public void PlayCinematic()
    {
        if (_cinematicBars != null)
        {
            _cinematicBars.FadeIn();
        }
    }

    public void EndCinematic()
    {
        if (_bossEncounter != null)
        {
            _bossEncounter.Finish();
        }

        if (_cinematicBars != null)
        {
            _cinematicBars.FadeOut();
        }
    }

    public void PlayBossEncounter()
    {
        if (_bossEncounter == null)
        {
            return;
        }

        if (!_bossEncounter.TryPlay())
        {
            return;
        }

        PlayCinematic();
    }
}
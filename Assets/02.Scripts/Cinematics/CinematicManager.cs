using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] private CinematicBars _cinematicBars;

    public void PlayCinematic()
    {
        if (_cinematicBars != null)
        {
            _cinematicBars.FadeIn();
        }
    }

    public void EndCinematic()
    {
        if (_cinematicBars != null)
        {
            _cinematicBars.FadeOut();
        }
    }
}

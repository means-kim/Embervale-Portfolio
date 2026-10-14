using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

// Run with Unity CLI run_script outside Assets; no scene or asset is saved.
public static class CinematicSkipVerification
{
    const string Report = "Docs/Unity/CinematicSkip_v01/Verification.txt";
    const string Prefs = "Temp/CinematicSkipPrefs.json";
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly List<string> Results = new List<string>();
    static T Find<T>() where T : UnityEngine.Object => UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
    static T Get<T>(object obj, string name) => (T)obj.GetType().GetField(name, Private).GetValue(obj);
    static void Set(object obj, string name, object value) => obj.GetType().GetField(name, Private).SetValue(obj, value);
    static void Call(object obj, string name, params object[] args) => obj.GetType().GetMethod(name, Private).Invoke(obj, args);
    static void Check(string name, bool condition)
    {
        Results.Add((condition ? "PASS " : "FAIL ") + name);
        File.WriteAllLines(Report, Results);
        if (!condition) throw new Exception(name);
    }
    static async Task Wait(Func<bool> condition, string name, float seconds = 15)
    {
        double end = EditorApplication.timeSinceStartup + seconds;
        while (!condition() && EditorApplication.timeSinceStartup < end) await Task.Delay(20);
        Check(name, condition());
    }
    static void Key(bool down)
    {
        using (StateEvent.From(Keyboard.current, out InputEventPtr ev))
        {
            Keyboard.current.escapeKey.WriteValueIntoEvent(down ? 1f : 0f, ev);
            InputSystem.QueueEvent(ev);
        }
        InputSystem.Update();
    }
    static void FocusGame()
    {
        var focus = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("Unity.Pipeline.Editor.Commands.FocusEditorCommand"))
            .First(t => t != null);
        focus.GetMethod("FocusEditor", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
        EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView")).Focus();
    }
    static void AppendHeader(string value)
    {
        Results.Clear();
        if (File.Exists(Report)) Results.AddRange(File.ReadAllLines(Report));
        Results.Add(value);
    }
    public static string SnapshotPrefs()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Report));
        var keys = new List<string> { "Game.Intro.LivingTree.Completed", "Game.Difficulty" };
        keys.AddRange(Enum.GetNames(typeof(EncounterId)).Select(x => "Encounter.IntroSeen." + x));
        keys.AddRange(Enum.GetNames(typeof(EliminateId)).Select(x => "EliminateId." + x));
        keys.AddRange(Enum.GetNames(typeof(UpgradeType)).Select(x => "Upgrade.Level." + x));
        var data = keys.ToDictionary(x => x, x => PlayerPrefs.HasKey(x) ? (int?)PlayerPrefs.GetInt(x) : null);
        File.WriteAllText(Prefs, JsonConvert.SerializeObject(data));
        File.WriteAllText(Report, "Unity 6000.3.22f1 cinematic skip verification\n");
        return "Saved " + data.Count + " existing preference states for restoration.";
    }
    public static string RestorePrefs()
    {
        var data = JsonConvert.DeserializeObject<Dictionary<string, int?>>(File.ReadAllText(Prefs));
        foreach (var pair in data)
        {
            if (pair.Value.HasValue) PlayerPrefs.SetInt(pair.Key, pair.Value.Value);
            else PlayerPrefs.DeleteKey(pair.Key);
        }
        PlayerPrefs.Save();
        foreach (var pair in data)
            if (PlayerPrefs.HasKey(pair.Key) != pair.Value.HasValue ||
                (pair.Value.HasValue && PlayerPrefs.GetInt(pair.Key) != pair.Value.Value))
                throw new Exception("Preference restoration failed: " + pair.Key);
        return "Restored and verified " + data.Count + " existing preference states.";
    }
    static void PrepareHistory()
    {
        foreach (var name in Enum.GetNames(typeof(EliminateId))) PlayerPrefs.DeleteKey("EliminateId." + name);
        foreach (var name in Enum.GetNames(typeof(EncounterId))) PlayerPrefs.DeleteKey("Encounter.IntroSeen." + name);
    }
    static async Task LoadMain()
    {
        PrepareHistory();
        Check("game scene load accepted", GameManager.LoadGameScene("MainTestScene"));
        await Wait(() => SceneManager.GetActiveScene().name == "MainTestScene" && !GameManager.IsLoading, "MainTestScene loaded");
        await Wait(() => GameManager.Instance != null && Time.timeScale == 1, "GameManager resumed gameplay");
        await Task.Delay(150);
    }
    public static async Task<string> MainFlows()
    {
        if (!Application.isPlaying) throw new Exception("Play mode required");
        AppendHeader("\nMain scene flows and interruption checks");
        FocusGame(); Key(false);
        await LoadMain();
        var manager = Find<CinematicManager>();
        var pause = Find<InGamePauseMenu>();
        var boss = Find<BossEncounterCinematic>();
        var phase2 = Find<BossPhase2Cinematic>();
        var phase3 = Find<BossPhase3Cinematic>();
        var elite = Find<EliteEncounterCinematic>();
        var controls = Get<Behaviour[]>(manager, "_playerControls");
        bool[] enabled = controls.Select(x => x.enabled).ToArray();
        Check("default hold duration is 1.5 seconds", Mathf.Approximately(manager.SkipHoldDuration, 1.5f));

        var timerOwner = new GameObject("TransientSkipTimerOwner").AddComponent<CinematicBars>();
        int skips = 0;
        CinematicSkipController.Begin(timerOwner, () => skips++, 1.5f);
        var skip = Find<CinematicSkipController>();
        var group = Get<CanvasGroup>(skip, "_group");
        var fill = Get<RectTransform>(skip, "_fill");
        Call(skip, "Tick", false, 0f); Call(skip, "Tick", true, .7f);
        Check("partial hold shows nonblocking progress", skips == 0 && group.alpha == 1 &&
            !group.blocksRaycasts && !group.interactable && fill.anchorMax.x > .45f);
        Call(skip, "Tick", false, 0f);
        Check("release resets and hides progress", group.alpha == 0 && fill.anchorMax.x == 0);
        Call(skip, "Tick", true, 1.49f);
        Check("short hold cannot skip", skips == 0);
        Call(skip, "Tick", true, .02f); Call(skip, "Tick", true, 3f);
        Check("threshold skips exactly once", skips == 1 && !CinematicSkipController.IsActive);
        CinematicSkipController.Begin(timerOwner, () => skips++, 1.5f);
        Call(skip, "Tick", false, 0f); Call(skip, "Tick", true, .8f);
        Call(skip, "OnApplicationFocus", false); Call(skip, "OnApplicationFocus", true);
        Call(skip, "Tick", true, 3f);
        Check("focus interruption requires a release", skips == 1 && group.alpha == 0);
        Call(skip, "Tick", false, 0f); Call(skip, "Tick", true, 1.6f);
        Check("fresh hold works after focus recovery", skips == 2);
        Key(true);
        CinematicSkipController.Begin(timerOwner, () => skips++, .1f);
        Call(skip, "Tick", true, 3f);
        Check("ESC held before sequence cannot skip it", skips == 2);
        Key(false); Call(skip, "Tick", false, 0f); Call(skip, "Tick", true, .11f);
        Check("release then fresh hold works on next sequence", skips == 3);
        CinematicSkipController.Begin(timerOwner, () => skips++, 1.5f);
        timerOwner.enabled = false; await Task.Delay(50);
        Check("disabled owner clears its prompt", !CinematicSkipController.IsActive && group.alpha == 0);
        UnityEngine.Object.Destroy(timerOwner.gameObject);

        int eliteDone = 0, eliteCancel = 0;
        elite.Completed += () => eliteDone++; elite.Cancelled += () => eliteCancel++;
        Check("elite starts with skip target", elite.TryPlay() && manager.IsPlaying && CinematicSkipController.IsActive);
        await Task.Delay(250);
        var camera = Get<Camera>(elite, "_camera");
        Vector3 originalCamera = Get<Vector3>(elite, "_originalPosition");
        elite.Skip();
        Check("elite skip completes and restores camera", eliteDone == 1 && eliteCancel == 0 && !manager.IsPlaying &&
            Vector3.Distance(camera.transform.position, originalCamera) < .01f && camera.GetComponent<Unity.Cinemachine.CinemachineBrain>().enabled);
        Check("player control states restored after skip", controls.Select((x,i) => x.enabled == enabled[i]).All(x => x));
        Check("elite can replay", elite.TryPlay());
        elite.Cancel();
        Check("cancel retains cancellation semantics", eliteDone == 1 && eliteCancel == 1 && !CinematicSkipController.IsActive);

        int bossDone = 0; boss.Completed += () => bossDone++;
        Check("boss introduction starts", boss.TryPlay());
        await Task.Delay(100);
        boss.Skip();
        var bossController = Get<AirBossController>(boss, "_boss");
        Check("boss skip finishes walk and enables battle actor", bossDone == 1 && bossController.enabled &&
            Get<SpriteRenderer>(boss, "_bossRenderer").enabled && Mathf.Abs(manager.PlayerBody.position.x - Get<Transform>(boss, "_walkTarget").position.x) < .01f);
        Check("boss BGM applied without its Timeline Signal", Get<bool>(boss, "_startedBossBgm"));
        Check("boss repeat route is skippable without manager", boss.TryPlayWithoutCinematic() && !manager.IsPlaying && CinematicSkipController.IsActive);
        Check("repeat route blocks ESC pause", !pause.TryOpen());
        boss.Skip();
        Check("boss repeat skip completes exactly once", bossDone == 2 && !CinematicSkipController.IsActive);

        var bossHealth = bossController.GetComponent<Health>();
        var phase = bossController.GetComponent<AirBossPhaseController>();
        bossHealth.CurrentHealth = Mathf.CeilToInt(bossHealth.MaxHealth * .6f);
        phase.CheckPhase(); await Task.Delay(80);
        Check("real phase 2 request starts Timeline", phase.PendingPhase == 2 && phase2.IsPlaying);
        phase2.Skip();
        Check("phase 2 skip advances boss phase", phase.CurrentPhase == 2 && !phase.IsTransition);
        Check("phase 2 skip leaves moon fully covered", Get<GameObject>(phase2, "_moonCover").activeSelf &&
            !Get<UnityEngine.Rendering.Universal.Light2D>(phase2, "_moonLight").enabled &&
            Get<SpriteRenderer>(phase2, "_coverRenderer").maskInteraction == SpriteMaskInteraction.None);

        Vector3 beforePhase3 = bossController.transform.position;
        var phase3Roar = Get<Game.Boss.Roar.BossRoarScreenEffect>(phase3, "_roarEffect");
        phase3Roar.gameObject.SetActive(true);
        bossHealth.CurrentHealth = Mathf.CeilToInt(bossHealth.MaxHealth * .2f);
        phase.CheckPhase(); await Task.Delay(80);
        Check("real phase 3 request starts Timeline", phase.PendingPhase == 3 && phase3.IsPlaying);
        await Wait(() => phase3Roar.IsPlaying, "phase 3 Timeline starts roar before skip", 5);
        phase3.Skip();
        Check("phase 3 skip immediately clears roar", !phase3Roar.IsPlaying && !phase3Roar.GetComponent<UnityEngine.UI.Image>().enabled);
        Check("phase 3 Timeline skip completes transition", phase.CurrentPhase == 3 && !phase.IsTransition && phase3.IsPhase3Active && !phase3.IsPlaying);
        CheckPlatforms(phase3);
        Check("phase 3 skip places boss at destination", Vector3.Distance(bossController.transform.position, phase3.BossDestination.position) < .01f);
        phase3.ResetPlatforms(); bossController.transform.position = beforePhase3;
        Set(phase, "_currentPhase", 2); phase.CheckPhase(); await Task.Delay(80);
        manager.Complete(); await Task.Delay(200);
        Check("post-Timeline platform stage remains skippable", !manager.IsPlaying && phase3.IsPlaying && CinematicSkipController.IsActive);
        phase3.Skip(); CheckPlatforms(phase3);
        Check("platform-stage skip advances phase exactly once", phase.CurrentPhase == 3 && phase3.IsPhase3Active && !phase3.IsPlaying);
        bossController.enabled = false;
        Check("normal pause remains usable after skip", pause.TryOpen() && pause.IsOpen);
        pause.Resume(); Check("pause resume restores gameplay", !pause.IsOpen && Time.timeScale == 1);

        Check("phase 2 replay accepted for cancellation test", phase2.TryPlay());
        await Task.Delay(80); phase2.Cancel();
        Check("cancel clears manager and skip target", !manager.IsPlaying && !CinematicSkipController.IsActive &&
            controls.Select((x,i) => x.enabled == enabled[i]).All(x => x));

        var ending = Find<GameEndingCinematic>();
        float fixedTime = Time.fixedDeltaTime;
        Check("ending starts", ending.TryPlay()); await Task.Delay(150);
        Check("ending enters slow motion", Time.timeScale < .1f);
        ending.Skip();
        Check("ending skip restores time and clears input", Time.timeScale == 1 && Mathf.Approximately(Time.fixedDeltaTime, fixedTime) &&
            !CinematicSkipController.IsActive && !Get<EndingFX.EndingHitMontage>(ending, "_hitMontage").IsAcceptingInput);
        await Wait(() => SceneManager.GetActiveScene().name == "OutGameScene", "ending skip returns to title");
        return string.Join("\n", Results);
    }
    static void CheckPlatforms(BossPhase3Cinematic phase3)
    {
        Check("old platforms removed after skip", Get<GameObject[]>(phase3, "_existingPlatforms").Where(x => x != null).All(x => !x.activeSelf));
        Check("new platforms visible and solid after skip", Get<GameObject[]>(phase3, "_phase3Platforms").Where(x => x != null).All(x =>
            x.activeSelf && x.GetComponentsInChildren<Renderer>(true).Where(r => r.gameObject.name != "Phase3_Flash").All(r => r.enabled) &&
            x.GetComponentsInChildren<Collider2D>(true).All(c => c.enabled)));
    }
    public static async Task<string> IntroInput()
    {
        AppendHeader("\nVideo intro and real Input System ESC hold");
        FocusGame(); Key(false); await Task.Delay(100);
        var title = Find<OutGameUI>();
        var clip = Get<UnityEngine.Video.VideoClip>(title, "_introClip");
        PrepareHistory();
        Check("intro load accepted", GameManager.LoadGameScene("MainTestScene", introClip: clip, replayIntro: true));
        await Wait(() => Find<GameIntroCinematic>() != null && Get<bool>(Find<GameIntroCinematic>(), "_firstFrameReady"), "intro video produces a frame", 20);
        FocusGame(); await Task.Delay(150);
        Check("intro uses paused game time", GameIntroCinematic.IsTransitioning && Time.timeScale == 0);
        Key(true); await Task.Delay(250); Key(false); await Task.Delay(100);
        Check("short real ESC hold preserves intro", GameIntroCinematic.IsTransitioning);
        Key(true); await Task.Delay(650);
        var skip = Find<CinematicSkipController>();
        Check("real input advances visible progress with timeScale zero (focused=" + Application.isFocused + ", key=" + Keyboard.current.escapeKey.isPressed + ")", Get<CanvasGroup>(skip, "_group").alpha == 1 &&
            Get<RectTransform>(skip, "_fill").anchorMax.x > .25f);
        ScreenCapture.CaptureScreenshot("Docs/Unity/CinematicSkip_v01/Intro_Hold.png");
        await Wait(() => !GameIntroCinematic.IsTransitioning, "long real ESC hold skips intro", 5);
        Key(false);
        await Wait(() => Time.timeScale == 1, "intro skip resumes game");
        Check("intro skip saves completion history", PlayerPrefs.GetInt("Game.Intro.LivingTree.Completed", 0) == 1);
        Check("intro skip frees video overlay and player input", Find<GameIntroCinematic>() == null && !CinematicSkipController.IsActive &&
            Find<CinematicManager>().PlayerBody.GetComponent<PlayerInput>().enabled);
        return string.Join("\n", Results.Skip(Math.Max(0, Results.Count - 10)));
    }
    public static async Task<string> AchievementEnding()
    {
        AppendHeader("\nEnding achievement video skip");
        FocusGame(); Key(false); await LoadMain();
        var boss = Find<BossEncounterCinematic>();
        Get<AirBossController>(boss, "_boss").enabled = false;
        var ending = Find<GameEndingCinematic>();
        var video = Find<EndingAchievementVideo>();
        Check("full ending starts", ending.TryPlay());
        await Wait(() => video.IsPlaying, "achievement video stage reached", 25);
        FocusGame(); await Task.Delay(150); Key(true);
        await Wait(() => SceneManager.GetActiveScene().name == "OutGameScene", "ESC skips ending achievement video to title", 6);
        Key(false);
        Check("achievement skip restores time and removes prompt", Time.timeScale == 1 && !CinematicSkipController.IsActive);
        return string.Join("\n", Results.Skip(Math.Max(0, Results.Count - 7)));
    }

    public static async Task<string> BattleEntryAndEarlyIntro()
    {
        AppendHeader("\nBattle entry, retry and early-loading intro skip");
        FocusGame(); Key(false); await LoadMain();
        var elite = Find<EliteEncounterCinematic>();
        var boss = Find<BossEncounterCinematic>();
        var areas = UnityEngine.Object.FindObjectsByType<AreaController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var eliteArea = areas.First(a => Get<CinematicSequence>(a, "_intro") == elite);
        var bossArea = areas.First(a => Get<CinematicSequence>(a, "_intro") == boss);
        eliteArea.TryEnter(); await Task.Delay(80); elite.Skip();
        Check("elite skip starts its actual AreaController battle", Get<object>(eliteArea, "_state").ToString() == "Fighting" &&
            PlayerPrefs.GetInt("Encounter.IntroSeen.Elite01", 0) == 1);
        bossArea.TryEnter(); await Task.Delay(80); boss.Skip();
        Check("boss skip starts its actual AreaController battle", Get<object>(bossArea, "_state").ToString() == "Fighting" &&
            PlayerPrefs.GetInt("Encounter.IntroSeen.Boss01", 0) == 1);
        Check("skipped battle retains configured closed arena barriers", new[] {
            Get<AreaBlock>(bossArea, "_leftBlock"), Get<AreaBlock>(bossArea, "_rightBlock")
        }.Where(x => x != null).All(x => x.IsClosed));
        bossArea.ResetForRetry(); bossArea.TryEnter();
        Check("actual retry uses skippable repeat route", Get<object>(bossArea, "_state").ToString() == "Intro" &&
            !Find<CinematicManager>().IsPlaying && CinematicSkipController.IsActive);
        boss.Skip();
        Check("retry skip resumes its battle", Get<object>(bossArea, "_state").ToString() == "Fighting");
        bossArea.ResetForRetry(); bossArea.TryEnter();
        await Wait(() => !CinematicSkipController.IsActive, "natural Timeline completion clears skip target", 8);
        Check("natural completion still starts battle", Get<object>(bossArea, "_state").ToString() == "Fighting");

        var load = SceneManager.LoadSceneAsync("OutGameScene");
        await Wait(() => load.isDone, "title reloaded for early intro test");
        await Task.Delay(100);
        var clip = Get<UnityEngine.Video.VideoClip>(Find<OutGameUI>(), "_introClip");
        Check("early intro load accepted", GameManager.LoadGameScene("MainTestScene", introClip: clip, replayIntro: true));
        var intro = Find<GameIntroCinematic>();
        intro.RequestSkip();
        Check("early skip keeps overlay during scene loading", GameIntroCinematic.IsTransitioning);
        await Wait(() => !GameIntroCinematic.IsTransitioning && SceneManager.GetActiveScene().name == "MainTestScene", "early skip reveals ready destination");
        await Wait(() => Time.timeScale == 1 && !GameManager.IsLoading, "early skip resumes gameplay after loading");
        Check("early skip retains completion history", PlayerPrefs.GetInt("Game.Intro.LivingTree.Completed", 0) == 1);
        return string.Join("\n", Results.Skip(Math.Max(0, Results.Count - 16)));
    }

    public static async Task<string> NaturalPlaybackAndRoar()
    {
        AppendHeader("\nNatural playback and inactive/active roar effects");
        FocusGame(); Key(false); await LoadMain();
        var boss = Find<BossEncounterCinematic>();
        var roar = Get<Game.Boss.Roar.BossRoarScreenEffect>(boss, "_roarEffect");
        Check("scene roar effect starts intentionally inactive", !roar.gameObject.activeInHierarchy);
        boss.PlayRoar(); Check("inactive roar call safely returns", !roar.IsPlaying);
        var manager = Find<CinematicManager>();
        var target = Get<Transform>(boss, "_walkTarget");
        manager.PlayerBody.position = new Vector2(target.position.x, manager.PlayerBody.position.y);
        int done = 0; boss.Completed += () => done++;
        Check("normal boss introduction starts", boss.TryPlay());
        await Wait(() => done == 1, "natural introduction finishes without skip", 8);
        Check("natural introduction restores controls and prompt", !manager.IsPlaying && !CinematicSkipController.IsActive &&
            manager.PlayerBody.GetComponent<PlayerInput>().enabled);

        roar.gameObject.SetActive(true);
        Check("active roar repeat starts", boss.TryPlayWithoutCinematic());
        await Wait(() => roar.IsPlaying, "Timeline starts active roar", 5);
        boss.Skip();
        Check("skip during active roar completes repeat", done == 2);
        await Task.Delay(100);
        Check("roar skip clears visual effect and skip target", !roar.IsPlaying &&
            !roar.GetComponent<UnityEngine.UI.Image>().enabled && !CinematicSkipController.IsActive);
        Check("roar skip leaves boss enabled", Get<AirBossController>(boss, "_boss").enabled &&
            Get<SpriteRenderer>(boss, "_bossRenderer").enabled);
        return string.Join("\n", Results.Skip(Math.Max(0, Results.Count - 12)));
    }

    public static async Task<string> FullSuite()
    {
        File.WriteAllText(Report, "Unity 6000.3.22f1 final cinematic skip verification\n");
        try
        {
            await MainFlows();
            await IntroInput();
            await AchievementEnding();
            await BattleEntryAndEarlyIntro();
            await NaturalPlaybackAndRoar();
            string result = "All " + Results.Count(x => x.StartsWith("PASS ")) + " checks passed. " + RestorePrefs();
            File.AppendAllText(Report, "\n" + result + "\n");
            return result;
        }
        finally { Key(false); RestorePrefs(); }
    }
}

using UnityEngine;

namespace Team08.UI
{
    [ExecuteAlways]
    public sealed class HealthFlameHUD : MonoBehaviour
    {
        [SerializeField] HealthFlameGraphic[] slots;
        [SerializeField, Min(0)] float currentHealth = 5;
        [SerializeField, Min(.05f)] float secondsPerSlot = 1.2f;
        [SerializeField] bool previewFillOnPlay = true;
        float displayedHealth;
        public int MaxHealth => slots == null ? 0 : slots.Length;
        public float CurrentHealth => currentHealth;
        public float DisplayedHealth => displayedHealth;

        public void Configure(HealthFlameGraphic[] value) { slots=value; SetHealth(currentHealth,true); }
        void OnEnable() { SetHealth(currentHealth,true); }
        void Start()
        {
            if(Application.isPlaying && previewFillOnPlay) PreviewFromEmpty();
        }
        void Update()
        {
            currentHealth=Mathf.Clamp(currentHealth,0,MaxHealth);
            if(!Application.isPlaying) displayedHealth=currentHealth;
            else Advance(Time.unscaledDeltaTime);
            Refresh();
        }
        public void SetHealth(float value, bool immediate=false)
        {
            currentHealth=Mathf.Clamp(value,0,MaxHealth);
            if(immediate || currentHealth<displayedHealth) displayedHealth=currentHealth;
            Refresh();
        }
        // One continuous value means a slot always finishes before the next begins.
        public void Advance(float deltaTime)
        {
            currentHealth=Mathf.Clamp(currentHealth,0,MaxHealth);
            displayedHealth=currentHealth<displayedHealth ? currentHealth : Mathf.MoveTowards(displayedHealth,currentHealth,Mathf.Max(0,deltaTime)/Mathf.Max(.05f,secondsPerSlot));
            Refresh();
        }
        public static float SlotFill(float health,int index) => Mathf.Clamp01(health-index);
        void Refresh()
        {
            if(slots==null)return;
            for(int i=0;i<slots.Length;i++) if(slots[i]) slots[i].Fill=SlotFill(displayedHealth,i);
        }
        [ContextMenu("Preview Fill From Empty")]
        public void PreviewFromEmpty() { displayedHealth=0; Refresh(); }
    }
}

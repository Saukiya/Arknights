using Manager;
using UnityEngine;
using XLua;

namespace UI {
    [CSharpCallLua]
    public abstract class UIBase : MonoBehaviour {
        public new GameObject gameObject { get; private set; }
        public new RectTransform transform { get; private set; }
        public CanvasGroup canvasGroup { get; private set; }
        internal string Name;

        internal UIState state = UIState.Hide;

        public bool debug;
        
        private void Awake() {
            gameObject = base.gameObject;
            transform = base.transform as RectTransform;
            canvasGroup = GetComponent<CanvasGroup>();
            if (debug) {
                gameObject.SetActive(true);
                Init();
                Show();
            }
        }

        public virtual void Init() { }
        
        public virtual void UpdateView() { }

        public virtual void Show() {
            gameObject.SetActive(true);
        }

        public virtual void Hide(bool destroy = false) {
            gameObject.SetActive(false);
            if (destroy) UIManager.Inst().UnLoad(Name);
        }

        public void Show(string value) => UIManager.Inst().Show(value);

        public void Hide(string value) => UIManager.Inst().Hide(value);

        public void HideAndDestroy(string value) => UIManager.Inst().Hide(value,true);

        public void PlayEffect(AudioClip clip) => SoundManager.Inst().PlayEffect(clip);
    }

    // [Serializable]
    // public class Injection {
    //     public string name;
    //     public Object value;
    // }

    public enum UIState {
        Show,
        Hide
    }
}
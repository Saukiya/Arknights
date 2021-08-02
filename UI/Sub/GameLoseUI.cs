using DG.Tweening;
using Manager;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Sub {
    public class GameLoseUI : UIBase {
        
        public Image blur;

        private Tweener tweener;

        public override void Init() {
            blur = GetComponent<Image>();
        }
        
        public override void Show() {
            base.Show();
            Material blurryShader = blur.material;
            blurryShader.SetFloat("_Size",0);
            float value = 0;
            DOTween.To(() => value,v => value = v,160,0.8f).OnUpdate(() => blurryShader.SetFloat("_Size",value)).OnComplete(() => {
                GetComponent<Button>().onClick.AddListener(Click);
            });
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1,0.6f);

            tweener = transform.GetComponent<Text>("Text").DOFade(0.4f, 1f).SetLoops(-1, LoopType.Yoyo);
        }

        public override void Hide(bool destroy = false) {
            GetComponent<Button>().onClick.RemoveListener(Click);
            Material blurryShader = blur.material;
            float value = blurryShader.GetFloat("_Size");
            DOTween.To(() => value,v => value = v,0,0.5f).OnUpdate(() => blurryShader.SetFloat("_Size",value));
            canvasGroup.DOFade(0,0.5f).OnComplete(() => { {base.Hide(destroy);} });
            tweener.Kill();
        }

        public void Click() {
            UIManager.Inst().Hide("GameLoseUI", true);
            UIManager.Inst().Hide("GameUI", true);
            Delay.add(() => UIManager.Inst().Show("HomeUI"), 2);
        }
    }
}
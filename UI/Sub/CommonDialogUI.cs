using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Sub {
    public class CommonDialogUI : UIBase {

        public Image blur;

        public GameObject blackGround;
        public GameObject whiteGround;

        public Text message;

        public GameObject mode_One;
        public GameObject mode_Two;
        
        public Button mode_One_back;
        
        public Button mode_Two_enter;
        public Button mode_Two_back;

        private GroundType type;
        private Mode mode;
        
        public override void Show() {
            base.Show();
            
            Material blurryShader = blur.material;
            blurryShader.SetFloat("_Size",0);
            float value = 0;
            DOTween.To(() => value,v => value = v,160,0.5f).OnUpdate(() => blurryShader.SetFloat("_Size", value));
            
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1,0.3f).OnComplete(() => {
                mode_One_back.onClick.AddListener(() => UIManager.Inst().Hide(Name));
                mode_Two_back.onClick.AddListener(() => UIManager.Inst().Hide(Name));
                mode_Two_enter.onClick.AddListener(() => UIManager.Inst().Hide(Name));
            });
        }

        public override void Hide(bool destroy = false) {
            mode_One_back.onClick.RemoveAllListeners();
            mode_Two_back.onClick.RemoveAllListeners();
            mode_Two_enter.onClick.RemoveAllListeners();
            Material blurryShader = blur.material;
            float value = blurryShader.GetFloat("_Size");
            DOTween.To(() => value,v => value = v,0,0.2f).OnUpdate(() => blurryShader.SetFloat("_Size", value));
            canvasGroup.DOFade(0,0.2f).OnComplete(() => {
                blackGround.SetActive(false);
                whiteGround.SetActive(false);
                mode_Two.SetActive(false);
                mode_One.SetActive(false);
                base.Hide(destroy);
            });
        }

        public static CommonDialogUI Message(GroundType type,string message) {
            CommonDialogUI ui = UIManager.Inst().Show("CommonDialogUI") as CommonDialogUI;
            ui.Initialization(type, Mode.ONE, message);
            return ui;
        }

        public static CommonDialogUI Message(GroundType type,string message, UnityAction enterMethod) {
            CommonDialogUI ui = UIManager.Inst().Show("CommonDialogUI") as CommonDialogUI;
            ui.Initialization(type, Mode.TWO, message);
            ui.mode_Two_enter.onClick.AddListener(enterMethod);
            return ui;
        }

        private void Initialization(GroundType type, Mode mode, string message) {
            this.type = type;
            this.mode = mode;
            this.message.color = GetText();
            this.message.text = message;
            GetGround().SetActive(true);
            GetMode().SetActive(true);
        }

        public void AddBackListener(UnityAction method) {
            GetBackButton().onClick.AddListener(method);
        }

        private GameObject GetGround() {
            return type == GroundType.WHITE ? whiteGround : blackGround;
        }

        private Color GetText() {
            return type == GroundType.WHITE ? Color.black : Color.white;
        }

        private GameObject GetMode() {
            return mode == Mode.ONE ? mode_One : mode_Two;
        }

        private Button GetBackButton() {
            return mode == Mode.ONE ? mode_One_back : mode_Two_back;
        }

        private Button GetEnterButton() {
            return mode == Mode.TWO ? mode_Two_enter : null;
        }

        public enum GroundType {
            WHITE,
            BLACK
        }

        public enum Mode {
            ONE,
            TWO
        }
    }
}
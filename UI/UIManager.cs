using System.Collections.Generic;
using Settings;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class UIManager : Single<UIManager> {
        private Dictionary<string, UIBase> uiDictionary = new Dictionary<string, UIBase>();

        private GameObject camera;
        private GameObject canvas;

        private Vector2 canvasSize;

        public GameObject GetCamera() => camera;
        public GameObject GetCanvas() => canvas;
        public Vector2 GetCanvasSize() => canvasSize;

        protected override void Initialization() {
            //克隆画布摄像机 + 画布
            camera = Object.Instantiate(Asset.Load<GameObject>(GameSettings.UI_PREFAB_PATH, "Camera"));
            camera.name = "UIManager";
            canvas = camera.transform.Find("Canvas").gameObject;
            // camera.AddComponent<UIComponent>();
            //画布不能随着场景的切换而销毁
            //该项目是以宽进行适配，所以宽不变, 注意：你用的时候，要确定你的项目画布是以什么方式适配的
            canvasSize = canvas.GetComponent<CanvasScaler>().referenceResolution;
            //高根据屏幕的分辨率的宽高比来计算
            // 分辨率.宽 / 分辨率.高 = 画布.宽 / 画布.高
            // 画布.高 = 画布.宽 / (分辨率.宽 / 分辨率.高) = 画布.宽 * 分辨率.高 / 分辨率.宽
            canvasSize.x = canvasSize.y * Screen.width / Screen.height;
            // canvasSize.y = canvasSize.x * Screen.height / Screen.width;
            Object.DontDestroyOnLoad(camera);
        }

        // 打开
        public UIBase Show(string type) {
            if (!uiDictionary.TryGetValue(type, out UIBase ui)) ui = Load(type);
            ui.transform.SetAsLastSibling();
            ui.Show();
            ui.UpdateView();
            if (ui.state != UIState.Hide) return ui;
            ui.state = UIState.Show;
            return ui;
        }

        public void Update(string type) {
            if (uiDictionary.TryGetValue(type, out UIBase ui)) ui.UpdateView();
        }
        
        // 关闭
        public void Hide(string type, bool destroy = false) {
            if (!uiDictionary.ContainsKey(type)) return;
            UIBase ui = uiDictionary[type];
            if (ui.state == UIState.Show) {
                ui.Hide(destroy);
                ui.state = UIState.Hide;
            } else if (destroy) {
                UnLoad(type);
            }
        }

        // 加载
        private UIBase Load(string type) {
            GameObject obj = Object.Instantiate(Asset.Load<GameObject>(GameSettings.UI_PREFAB_PATH, type), canvas.transform);
            obj.name = type;
            UIBase ui = obj.GetComponent<UIBase>();
            ui.Name = type;
            ui.Init();
            uiDictionary.Add(type, ui);
            return ui;
        }

        // 卸载
        public void UnLoad(string type) {
            if (!uiDictionary.ContainsKey(type)) return;
            UIBase ui = uiDictionary[type];
            uiDictionary.Remove(type);
            Object.Destroy(ui.gameObject);
        }
    }
}
using System;
using Data.Player;
using DG.Tweening;
using Manager;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Sub {
    public class LoginUI : UIBase {
        
        public RectTransform sphere;
        public Camera sphereCamera;
        public CanvasGroup backGround;

        public CanvasGroup homePanel;
        public CanvasGroup loginPanel;
        public CanvasGroup registerPanel;
        public CanvasGroup loadingPanel;
        public CanvasGroup lowerRightPanel;

        public Button home_login;
        public Button home_register;
        
        public InputField login_userName;
        public InputField login_password;
        public Button login_enter;
        public Button login_back;
        
        public InputField register_userName;
        public InputField register_password;
        public InputField register_rePassword;
        public Toggle register_toggle;
        public Button register_enter;
        public Button register_back;

        public RectTransform loading_bar1;
        public RectTransform loading_bar2;
        
        private Color black = new Color(0, 0, 0, 0);
        private Color red = new Color(0.74f, 0.36f, 0.36f, 0);

        private AudioClip clip;
        private AudioClip loop_clip;
        
        public override void Init() {
            clip = Asset.Load<AudioClip>("Audio/Music/Login", "m_sys_title_intro");
            loop_clip = Asset.Load<AudioClip>("Audio/Music/Login", "m_sys_title_loop");
            
            homePanel.gameObject.SetActive(true);
            loginPanel.gameObject.SetActive(false);
            registerPanel.gameObject.SetActive(false);
            loadingPanel.gameObject.SetActive(false);
            
            home_login.onClick.AddListener(() => {
                homePanel.DOFade(0,0.5f).OnComplete(() => {
                    homePanel.gameObject.SetActive(false);
                    loginPanel.gameObject.SetActive(true);
                    loginPanel.DOFade(1,0.5f);
                    backGround.DOFade(1,0.5f);
                    // sphere.DOLocalMove(new Vector3(0,65,-800),1f);
                    sphere.DOAnchorPosY(0,1f);
                    sphereCamera.DOColor(black,1f);
                });
            });
            
            login_back.onClick.AddListener(() => {
                loginPanel.DOFade(0,0.5f).OnComplete(() => {
                    login_password.text = "";
                    loginPanel.gameObject.SetActive(false);
                    homePanel.gameObject.SetActive(true);
                    homePanel.DOFade(1,0.5f);
                    backGround.DOFade(0,0.5f).SetDelay(0.5f);
                    // sphere.DOLocalMove(new Vector3(0,263,-830),1f).SetEase(Ease.Linear);
                    sphere.DOAnchorPosY(263,1f).SetEase(Ease.Linear);
                    sphereCamera.DOColor(red,1f);
                    // sphere.DOScale(1.5f,1f).SetEase(Ease.Linear);
                });
            });
            
            home_register.onClick.AddListener(() => {
                homePanel.DOFade(0,0.5f).OnComplete(() => {
                    homePanel.gameObject.SetActive(false);
                    registerPanel.gameObject.SetActive(true);
                    registerPanel.DOFade(1,0.5f);
                    backGround.DOFade(1,0.5f);
                    // sphere.DOLocalMove(new Vector3(0,80,-800),1f);
                    sphere.DOAnchorPosY(140,1f);
                    sphereCamera.DOColor(black,1f);
                });
            });
            
            register_back.onClick.AddListener(() => {
                registerPanel.DOFade(0,0.5f).OnComplete(() => {
                    register_password.text = "";
                    register_rePassword.text = "";
                    registerPanel.gameObject.SetActive(false);
                    homePanel.gameObject.SetActive(true);
                    homePanel.DOFade(1,0.5f);
                    backGround.DOFade(0,0.5f).SetDelay(0.5f);
                    // sphere.DOLocalMove(new Vector3(0,100,-830),1f).SetEase(Ease.Linear);
                    sphere.DOAnchorPosY(263,1f).SetEase(Ease.Linear);
                    sphereCamera.DOColor(red,1f);
                    // sphere.DOScale(1.5f,1f).SetEase(Ease.Linear);
                });
            });
            
            login_enter.onClick.AddListener(() => {
                string userName = login_userName.text;
                string password = login_password.text;
                PlayerData playerData = PlayerManager.Inst().Login(userName,password);
                if (playerData == null) {
                    CommonDialogUI.Message(CommonDialogUI.GroundType.BLACK, "用户名或密码错误 请重新登录");
                    return;
                }
                // 登陆界面消失
                lowerRightPanel.DOFade(0,0.5f);
                
                loginPanel.DOFade(0,0.5f).OnComplete(() => {
                    // LoadingPanel界面显示
                    lowerRightPanel.gameObject.SetActive(false);
                    loadingPanel.alpha = 0f;
                    loadingPanel.gameObject.SetActive(true);
                    loadingPanel.DOFade(1,0.5f);
                    Text bar1Text = loading_bar1.transform.GetComponentInChildren<Text>();
                    Text bar2Text = loading_bar2.transform.GetComponentInChildren<Text>();
                    float value = 0.02f;
                    bar1Text.text = $"{value * 200:0.##}%";
                    bar2Text.text = $"{value * 200:0.##}%";
                    // 球移动
                    
                    // sphere.DOLocalMove(new Vector3(0,10,-700),1f).SetEase(Ease.Linear).OnComplete(() => {
                    sphere.DOScale(0.8f,1f).SetEase(Ease.Linear);
                    sphere.DOAnchorPosY(-540, 1f).SetEase(Ease.Linear).OnComplete(() => {
                        // 进度条移动
                        DOTween.To(() => value,v => value = v,0.5f,3f).SetEase(Ease.InQuint)
                            .OnUpdate(() => {
                                Vector2 vector = loading_bar1.anchorMax;
                                vector.x = value;
                                loading_bar1.anchorMax = vector;
                                vector = loading_bar2.anchorMin;
                                vector.x = 1 - value;
                                loading_bar2.anchorMin = vector;

                                bar1Text.text = $"{value * 200:0.##}%";
                                bar2Text.text = $"{value * 200:0.##}%";
                            })
                            .OnComplete(() => {
                                DOTween.To(() => value,z => value = z,0f,0.5f).OnComplete(() => {
                                    Delay.add(() => UIManager.Inst().Show("HomeUI"), 2);
                                    UIManager.Inst().Hide(Name, true);
                                });
                            });
                    });
                });
            });
            
            register_enter.onClick.AddListener(() => {
                string userName = register_userName.text;
                string password = register_password.text;
                string rePassword = register_rePassword.text;

                if (userName.Length < 3) {
                    CommonDialogUI.Message(CommonDialogUI.GroundType.BLACK, "用户名不得小于三位");
                    return;
                }

                if (password != rePassword) {
                    CommonDialogUI.Message(CommonDialogUI.GroundType.BLACK, "密码不一致");
                    return;
                }

                if (password.Length < 6 || password.Length > 18) {
                    CommonDialogUI.Message(CommonDialogUI.GroundType.BLACK, "密码为<color=#00B0FF>6-18</color>位的数字，字母，字符(可包含!#$%&*,.:;^`~)");
                    return;
                }

                if (!register_toggle.isOn) {
                    CommonDialogUI.Message(CommonDialogUI.GroundType.BLACK, "需同意<color=#00B0FF>注册协议</color>和<color=#00B0FF>隐私协议</color>");
                    return;
                }

                try {
                    PlayerManager.Inst().Register(userName, password);
                }
                catch (Exception e) {
                    CommonDialogUI.Message(CommonDialogUI.GroundType.BLACK, e.Message);
                    return;
                }
                
                CommonDialogUI.Message(CommonDialogUI.GroundType.WHITE, "注册成功")
                    .AddBackListener(() => {
                    registerPanel.DOFade(0,0.5f).OnComplete(() => {
                        registerPanel.gameObject.SetActive(false);
                        homePanel.gameObject.SetActive(true);
                        homePanel.DOFade(1,0.5f);
                        backGround.DOFade(0,0.5f).SetDelay(0.5f);
                        sphere.DOAnchorPosY(263,1f).SetEase(Ease.Linear);
                    });
                });
            });
            
        }

        public override void Show() {
            base.Show();
            SoundManager.Inst().PlayMusic(clip, false, () => {
                SoundManager.Inst().PlayMusic(loop_clip, true);
            });
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1,0.5f); 
        }

        public override void Hide(bool destroy = false) {
            canvasGroup.DOFade(0,2f).OnComplete(() => {
                base.Hide(destroy);
            });
        }
    }
}
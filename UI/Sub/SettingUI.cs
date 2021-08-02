using Data.Player;
using DG.Tweening;
using Settings;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Sub {
    public class SettingUI : UIBase {
        public Image blur;

        public ToggleController game_keep_speed;
        public ToggleController game_performance;
        public Slider game_profileScreen;
        public Button game_exit;

        public ToggleController sound_soundEffect;
        public Slider sound_soundEffect_value;
        public ToggleController sound_music;
        public Slider sound_music_value;
        public ToggleController sound_voice;
        public Slider sound_voice_value;

        public override void Init() {
            game_keep_speed.Initialization(GameSettings.GAME_KEEP_SPEED);
            game_performance.Initialization(GameSettings.GAME_PERFORMANCE);
            game_profileScreen.value = GameSettings.GAME_PROFILED_SCREEN;
            sound_soundEffect.Initialization(GameSettings.SOUND_SOUND_EFFECT_ENABLE);
            sound_soundEffect_value.value = GameSettings.SOUND_SOUND_EFFECT_VALUE;
            sound_music.Initialization(GameSettings.SOUND_MUSIC_ENABLE);
            sound_music_value.value = GameSettings.SOUND_MUSIC_VALUE;
            sound_voice.Initialization(GameSettings.SOUND_VOICE_ENABLE);
            sound_voice_value.value = GameSettings.SOUND_VOICE_VALUE;
            
            game_keep_speed.onValueChanged.AddListener(value => GameSettings.GAME_KEEP_SPEED = value);
            game_performance.onValueChanged.AddListener(value => GameSettings.GAME_PERFORMANCE = value);
            game_profileScreen.onValueChanged.AddListener(value => {
                GameSettings.GAME_PROFILED_SCREEN = (int) value;
            });

            sound_soundEffect.onValueChanged.AddListener(value => {
                GameSettings.SOUND_SOUND_EFFECT_ENABLE = value;
                SoundManager.Inst().UpdateData();
            });
            sound_soundEffect_value.onValueChanged.AddListener(value => {
                GameSettings.SOUND_SOUND_EFFECT_VALUE = (int) value;
                SoundManager.Inst().UpdateData();
            });
            sound_music.onValueChanged.AddListener(value => {
                GameSettings.SOUND_MUSIC_ENABLE = value;
                SoundManager.Inst().UpdateData();
            });
            sound_music_value.onValueChanged.AddListener(value => {
                GameSettings.SOUND_MUSIC_VALUE = (int) value;
                SoundManager.Inst().UpdateData();
            });
            sound_voice.onValueChanged.AddListener(value => {
                GameSettings.SOUND_VOICE_ENABLE = value;
                SoundManager.Inst().UpdateData();
            });
            sound_voice_value.onValueChanged.AddListener(value => {
                GameSettings.SOUND_VOICE_VALUE = (int) value;
                SoundManager.Inst().UpdateData();
            });

            game_exit.onClick.AddListener(() => {
                CommonDialogUI.Message(CommonDialogUI.GroundType.WHITE,"即将返回登陆界面，是否继续？",() => {
                    PlayerManager.Inst().Exit();
                    UIManager.Inst().Hide("HomeUI",true);
                    UIManager.Inst().Hide("SettingUI",true);
                    UIManager.Inst().Hide("HouseUI",true);
                    UIManager.Inst().Hide("CharUI",true);
                    float v = 0;
                    DOTween.To(() => v,value => v = value,3,2f).OnComplete(() => UIManager.Inst().Show("LoginUI"));
                });
            });
        }

        public override void Show() {
            base.Show();
            Material blurryShader = blur.material;
            blurryShader.SetFloat("_Size",0);
            float value = 0;
            DOTween.To(() => value,v => value = v,160,0.5f).OnUpdate(() => blurryShader.SetFloat("_Size",value));
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1,0.3f);
        }

        public override void Hide(bool destroy = false) {
            Material blurryShader = blur.material;
            float value = blurryShader.GetFloat("_Size");
            DOTween.To(() => value,v => value = v,0,0.2f).OnUpdate(() => blurryShader.SetFloat("_Size",value));
            canvasGroup.DOFade(0,0.2f).OnComplete(() => { base.Hide(destroy); });
        }
    }
}
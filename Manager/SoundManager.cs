using System;
using System.Collections;
using DG.Tweening;
using Settings;
using Tools;
using UnityEngine;

namespace Manager {
    public class SoundManager : MonoSingle<SoundManager> {

        private AudioSource soundEffect;
        private AudioSource music;
        private AudioSource voice;
        private Coroutine cor;
        

        protected override void Initialization() {
            soundEffect = AddAudio("SoundEffect");
            music = AddAudio("Music");
            voice = AddAudio("Voice");
            UpdateData();
        }

        public AudioSource AddAudio(string name = "AudioSource") {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.parent = transform;
            return gameObject.AddComponent<AudioSource>();
        }

        // 播放音频 （参数：音频，是否循环，回调函数）
        public void PlayMusic(AudioClip clip, bool loop, Action action = null) {
            if (cor != null) {//停止之前的携程
                StopCoroutine(cor);
                cor = null;
            }
            if (!loop && GameSettings.SOUND_MUSIC_ENABLE) {// 如果不是循环并且音乐频道为开启，则是淡入淡出播放
                DOTween.To(() => music.volume, value => music.volume = value, 0, 1).OnComplete(() => {
                    DOTween.To(() => music.volume, value => music.volume = value, GameSettings.SOUND_MUSIC_VALUE / 100f, 1);
                    music.clip = clip;
                    music.loop = false;
                    music.Play();
                    if (action != null) {//如果存在回调函数，则创建协程 -- 因为延迟函数所以要多些一行
                        cor = StartCoroutine(startYingPingJiShi(clip, action));
                    }
                });
            } else {//循环模式则直接切歌
                music.volume = GameSettings.SOUND_MUSIC_VALUE / 100f;
                music.clip = clip;
                music.loop = loop;
                music.Play();
                if (action != null) {//如果存在回调函数，则创建协程
                    cor = StartCoroutine(startYingPingJiShi(clip, action));
                }
            }
        }

        // 播放完后音频回调函数
        IEnumerator startYingPingJiShi(AudioClip clip, Action action) {
            yield return  new WaitForSeconds(clip.length);
            cor = null;//清理携程变量
            action();
        }

        public void PlayEffect(AudioClip clip) {
            soundEffect.PlayOneShot(clip);
        }

        public void StopMusic() {
            DOTween.To(() => music.volume, value => music.volume = value, 0, 1).OnComplete(() => {
                music.Stop();
            });

        }

        public void UpdateData() {
            soundEffect.volume = GameSettings.SOUND_SOUND_EFFECT_ENABLE ? GameSettings.SOUND_SOUND_EFFECT_VALUE / 100f : 0;
            music.volume = GameSettings.SOUND_MUSIC_ENABLE ? GameSettings.SOUND_MUSIC_VALUE / 100f : 0;
            voice.volume = GameSettings.SOUND_VOICE_ENABLE ? GameSettings.SOUND_VOICE_VALUE / 100f : 0;
        }
    }
}
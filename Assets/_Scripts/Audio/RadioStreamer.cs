using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace _Scripts.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class RadioStreamer : MonoBehaviour
    {
        public string[] _radioUrls =
        {
            "https://streams.90s90s.de/dab-national/mp3-192/",
            "http://streams.90s90s.de/inthemix/mp3-192/",
            "http://streams.90s90s.de/grunge/mp3-192/",
            "http://streams.90s90s.de/eurodance/mp3-192/",
            "http://streams.90s90s.de/hiphop/mp3-192/",
            "http://stream.jam.fm/jamfm-nmr/mp3-192/",
        };

        private AudioSource audioSource;
        private int currentRadioIndex = 0;

        // Вызывается при запуске
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            // Начните воспроизведение первой радиостанции
            PlayRadioStation(currentRadioIndex);
        }

        // Метод для воспроизведения радиостанции
        private void PlayRadioStation(int index)
        {
            // Проверьте, что индекс в допустимом диапазоне
            if (index >= 0 && index < _radioUrls.Length)
            {
                audioSource.Stop(); // Остановите текущий поток
                audioSource.clip = null; // Сбросите аудиопоток
                audioSource.loop = true; // Включите режим циклического воспроизведения
                audioSource.Play(); // Начните воспроизведение

                StartCoroutine(OpenRadioStream(_radioUrls[index]));
            }
        }

        // Метод для асинхронной загрузки и воспроизведения потокового аудио
        private IEnumerator OpenRadioStream(string url)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogError("Error loading audio stream: " + www.error);
                }
                else
                {
                    audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.Play();
                }
            }
        }

        // Метод для переключения на следующую радиостанцию
        public void SwitchToNextRadio()
        {
            currentRadioIndex = (currentRadioIndex + 1) % _radioUrls.Length;
            PlayRadioStation(currentRadioIndex);
        }

        // Вызывается, когда вы хотите переключиться на следующую радиостанцию
        public void ChangeRadioStation()
        {
            SwitchToNextRadio();
        }
    }
}
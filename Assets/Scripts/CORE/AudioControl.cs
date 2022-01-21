using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EdgeworldBase
{
    [RequireComponent(typeof(ResourcesLoader))]
    public class AudioControl : MonoBehaviour
    {

        #region Essential
        public string m_sInstancePrefab;
        private ResourcesLoader m_res;
        private List<GameObject> m_listInstances = new List<GameObject>();
        private Dictionary<string, float> m_dicVolumeGroup = new Dictionary<string, float>();

        public static AudioControl Instance;

        [SerializeField]
        protected Transform m_tInstancesContainer;

        [SerializeField]
        protected AudioSource MusicSource;

        [SerializeField]
        protected AudioSource SoundscapeSource;

        void Awake()
        {
            m_res = GetComponent<ResourcesLoader>();
            Instance = this;

            m_dicVolumeGroup.Add("Untagged", PlayerPrefs.GetFloat("Untagged", 1f));
            m_dicVolumeGroup.Add("Music", PlayerPrefs.GetFloat("Music", 0.6f));
            m_dicVolumeGroup.Add("Soundscape", PlayerPrefs.GetFloat("Soundscape", 0.6f));

            SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));

            MusicSource.volume = m_dicVolumeGroup["Music"];
            if(MusicSource.volume <= 0f)
            {
                PlayerPrefs.SetFloat("Music",0.6f);
                m_dicVolumeGroup["Music"] = 0.6f;
                MusicSource.volume = 0.6f;
            }

            SoundscapeSource.volume = m_dicVolumeGroup["Soundscape"];

        }

        #endregion

        #region Methods

        public float GetVolumeByTag(string tag)
        {
            return m_dicVolumeGroup[tag];
        }




        public void PlayInPosition(string gClip, Vector3 pos, float MaxDistance = 200f, float pitch = 1f)
        {
            if (ResourcesLoader.Instance.m_bLoading)
            {
                return;
            }

            GameObject currentInstance = null;

            for (int i = 0; i < m_listInstances.Count; i++)
            {
                if (!m_listInstances[i].GetComponent<AudioSource>().isPlaying)
                {
                    currentInstance = m_listInstances[i];
                    break;
                }
            }

            if (currentInstance == null)
            {
                currentInstance = (GameObject)Instantiate(m_res.GetObject(m_sInstancePrefab));
                currentInstance.transform.parent = m_tInstancesContainer;
                m_listInstances.Add(currentInstance);
            }

            currentInstance.transform.position = pos;
            currentInstance.GetComponent<AudioSource>().spatialBlend = 1f;
            currentInstance.GetComponent<AudioSource>().maxDistance = MaxDistance;
            currentInstance.GetComponent<AudioSource>().pitch = pitch;
            currentInstance.GetComponent<AudioSource>().loop = false;
            currentInstance.GetComponent<AudioSource>().clip = m_res.GetClip(gClip);
            currentInstance.GetComponent<AudioSource>().Play();

            if (m_dicVolumeGroup.ContainsKey(currentInstance.tag))
            {
                currentInstance.GetComponent<AudioSource>().volume = m_dicVolumeGroup[currentInstance.tag];
            }
            else
            {
            }
        }

        public void SetInBackground()
        {
            // MusicSource.Pause();
            // SoundscapeSource.Pause();
            AudioListener.pause = true;
        }

        public void SetNoInBackground()
        {
            AudioListener.pause = false;

            if (MusicSource.time <= 1f)
            {
                MusicSource.Play();
            }

            if (SoundscapeSource.time <= 1f)
            {
                SoundscapeSource.Play();
            }
        }

        public void Play(string gClip, bool gLoop = false, string gTag = "")
        {
            if (ResourcesLoader.Instance.m_bLoading)
            {
                return;
            }

            GameObject currentInstance = null;

            int alreadyPlaying = 0;
            for (int i = 0; i < m_listInstances.Count; i++)
            {
                AudioSource currentAS = m_listInstances[i].GetComponent<AudioSource>();
                if (!currentAS.isPlaying)
                {
                    currentInstance = m_listInstances[i];
                }
                else
                {
                    if (currentAS.clip.name == gClip)
                    {
                        alreadyPlaying++;
                        if (alreadyPlaying > 3)
                        {
                            return;
                        }
                    }
                }
            }

            if (currentInstance == null)
            {
                currentInstance = (GameObject)Instantiate(m_res.GetObject(m_sInstancePrefab));
                currentInstance.transform.parent = m_tInstancesContainer;
                m_listInstances.Add(currentInstance);
            }

            AudioSource instanceSource = currentInstance.GetComponent<AudioSource>();
            instanceSource.spatialBlend = 0f;
            instanceSource.pitch = 1f;
            instanceSource.loop = gLoop;
            instanceSource.clip = m_res.GetClip(gClip);
            instanceSource.Play();

            if (!string.IsNullOrEmpty(gTag))
                currentInstance.tag = gTag;

            if (m_dicVolumeGroup.ContainsKey(currentInstance.tag))
            {
                instanceSource.volume = m_dicVolumeGroup[currentInstance.tag];
            }
        }

        public void ResetVolume(string gTag, bool save = true)
        {
            Debug.LogError("VOLUME RESET!?!?!??");
            SetVolume(gTag, PlayerPrefs.GetFloat(gTag), save);
        }

        public void SetVolume(string gTag, float gVolume, bool save = true)
        {
            if (save)
            {
                PlayerPrefs.SetFloat(gTag, gVolume);
                PlayerPrefs.Save();
            }

            if (gTag == "Music")
            {
                MusicSource.volume = gVolume;
            }

            if (gTag == "Soundscape")
            {
                SoundscapeSource.volume = gVolume;
            }


            if (!m_dicVolumeGroup.ContainsKey(gTag))
            {
                m_dicVolumeGroup.Add(gTag, gVolume);
            }
            else
            {
                m_dicVolumeGroup[gTag] = gVolume;
            }

            for (int i = 0; i < m_listInstances.Count; i++)
            {
                if (m_listInstances[i].tag == gTag)
                {
                    m_listInstances[i].GetComponent<AudioSource>().volume = gVolume;
                }
            }

            Debug.LogError(MusicSource.volume);
        }

        public void SetMasterVolume(float gVolume)
        {
            AudioListener.volume = gVolume;
            PlayerPrefs.SetFloat("MasterVolume", gVolume);
            PlayerPrefs.Save();
        }

        public void PlayWithPitch(string gClip, float fPitch)
        {
            if (ResourcesLoader.Instance.m_bLoading)
            {
                return;
            }

            GameObject currentInstance = null;

            for (int i = 0; i < m_listInstances.Count; i++)
            {
                if (!m_listInstances[i].GetComponent<AudioSource>().isPlaying)
                {
                    currentInstance = m_listInstances[i];
                    break;
                }
            }

            if (currentInstance == null)
            {
                currentInstance = (GameObject)Instantiate(m_res.GetObject(m_sInstancePrefab));
                currentInstance.transform.parent = m_tInstancesContainer;
                m_listInstances.Add(currentInstance);
            }

            currentInstance.GetComponent<AudioSource>().spatialBlend = 0f;
            currentInstance.GetComponent<AudioSource>().clip = m_res.GetClip(gClip);
            currentInstance.GetComponent<AudioSource>().pitch = fPitch;
            currentInstance.GetComponent<AudioSource>().Play();

            if (m_dicVolumeGroup.ContainsKey(currentInstance.tag))
            {
                currentInstance.GetComponent<AudioSource>().volume = m_dicVolumeGroup[currentInstance.tag];
            }
        }

        public void SetCurrentClassMusic()
        {
            SetClassMusic(CORE.PlayerActor.ClassJobReference);
        }

        public void SetClassMusic(ClassJob classj)
        {
            if (SwitchMusicFadeInstance != null)
            {
                StopCoroutine(SwitchMusicFadeInstance);
            }

            SwitchMusicFadeInstance = StartCoroutine(SwitchMusicFade(classj.ClassMusic.name, true));
        }

        Coroutine SwitchMusicFadeInstance;
        IEnumerator SwitchMusicFade(string musicKey, bool keepPlaybackTime = false)
        {
            float initVolume = PlayerPrefs.GetFloat("Music", 0.6f);
            if(initVolume < 0.1f)
            {
                initVolume = 0.6f;
            }

            float t = 0f;
            while (t < 1f)
            {
                SetVolume("Music", Mathf.Lerp(initVolume, 0f, t), false);
                t += Time.deltaTime;
                yield return 0;
            }

            float currentPlaybackTime = MusicSource.time;

            SetMusic(musicKey);

            if (keepPlaybackTime)
            {
                MusicSource.time = currentPlaybackTime;
            }
            else
            {
                MusicSource.time = 0f;
            }

            t = 0f;
            while (t < 1f)
            {
                SetVolume("Music", Mathf.Lerp(0f, initVolume, t), false);
                t += Time.deltaTime;
                yield return 0;
            }


            SwitchMusicFadeInstance = null;
        }

        public void SetMusic(string gClip, float fPitch = 1f)
        {
            if (ResourcesLoader.Instance.m_bLoading)
            {
                return;
            }

            MusicSource.volume = m_dicVolumeGroup["Music"];

            if (string.IsNullOrEmpty(gClip))
            {
                MusicSource.Stop();
                MusicSource.clip = null;
                return;
            }

            MusicSource.pitch = fPitch;

            if (MusicSource.clip == null || MusicSource.clip.name != gClip)
            {
                MusicSource.Stop();
                MusicSource.clip = ResourcesLoader.Instance.GetClip(gClip);

                if (!CORE.Instance.IsAppInBackground)
                {
                    MusicSource.time = 0f;
                    MusicSource.Play();
                }
            }
        }

        public void SetSoundscape(string gClip, float fPitch = 1f)
        {
            if (ResourcesLoader.Instance.m_bLoading)
            {
                return;
            }

            SoundscapeSource.volume = m_dicVolumeGroup["Soundscape"];

            if (string.IsNullOrEmpty(gClip))
            {
                SoundscapeSource.Stop();
                SoundscapeSource.clip = null;
                return;
            }

            SoundscapeSource.pitch = fPitch;

            if (SoundscapeSource.clip == null || SoundscapeSource.clip.name != gClip)
            {
                SoundscapeSource.Stop();
                SoundscapeSource.clip = ResourcesLoader.Instance.GetClip(gClip);

                if (!CORE.Instance.IsAppInBackground)
                {
                    SoundscapeSource.Play();
                }
            }
        }

        public void StopSound(string gClip)
        {
            foreach (GameObject obj in m_listInstances)
            {
                if (obj.GetComponent<AudioSource>().isPlaying)
                {
                    if (obj.GetComponent<AudioSource>().clip.name == gClip)
                    {
                        obj.GetComponent<AudioSource>().Stop();
                    }
                }
            }
        }


        #endregion
    }
}
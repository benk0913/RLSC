using EdgeworldBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceController : MonoBehaviour
{
    AudioSource Source;

    [SerializeField]
    string VolumeTag = "Untagged";

    public bool TakeOverMusic;

    

    private void OnEnable()
    {
        Source = GetComponent<AudioSource>();

       
    }

    void Update()
    {
        if(TakeOverMusic)
        {
            if(CORE.PlayerActor == null)
            {
                return;
            }

            if(CORE.PlayerActor.ActorEntity == null)
            {
                return;
            }

            float dist = Vector3.Distance(CORE.PlayerActor.ActorEntity.transform.position, Source.transform.position);

            // if(dist < Source.minDistance)
            // {
            //     return;
            // }
             if(AudioControl.Instance.GetVolumeByTag(VolumeTag) < 0.1f)
            {
                Source.volume = AudioControl.Instance.GetVolumeByTag(VolumeTag);
            }
            else
            {
                Source.volume = 1f;
            }
            
            AudioControl.Instance.GetComponent<AudioSource>().volume = Mathf.Lerp(0f, AudioControl.Instance.GetVolumeByTag("Music"), dist / Source.maxDistance);
            
        }
    }
}

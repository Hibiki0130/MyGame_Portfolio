using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    private AudioSource audioSourse;
    private bool ablePlay;
    // Start is called before the first frame update
    void Awake()
    {
        audioSourse = GetComponent<AudioSource>();

        if (audioSourse == null)
        {
            Debug.Log("audiosourseÇ»Ç¢ÇÊÅ[");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Stop()
    {
        audioSourse.Stop();
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        audioSourse.PlayOneShot(clip);
        //Debug.Log("Ç»Ç¡ÇΩÅI");
    }

    public void Play(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        audioSourse.clip = clip;
        audioSourse.Play();
    }

    public IEnumerator PlayIntarvalShot(AudioClip clip)
    {
        if(!ablePlay)
        {
            yield break;
        }

        audioSourse.PlayOneShot(clip);
        ablePlay = false;

        yield return new WaitForSeconds(clip.length);

        ablePlay = true;
    }
}

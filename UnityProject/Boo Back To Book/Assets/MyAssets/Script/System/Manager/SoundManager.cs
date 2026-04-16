using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Oculus.Interaction.Context;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI debugLog;

    [Header("それぞれのオーディオソース")]
    [SerializeField] private AudioSource BGMAudioSourse;
    [SerializeField] private AudioSource SEAudioSourse;

    [Header("BGM")]
    [SerializeField] private AudioClip attention;
    [SerializeField] private AudioClip wind;
    [SerializeField] private AudioClip InGame;
    [SerializeField] private AudioClip ED;
    //重複したら削除
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnStateChange += BGMManager;
        DifferenceManager.Instance.OnDiffrenceStateChange += InGameBGM;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void BGMManager(GameState newstate)
    {
        if (newstate == GameState.Attention)
        {
            PlayBGM(attention);
        }
        if (newstate == GameState.Standby)
        {
            PlayBGM(wind);
        }
        if (newstate == GameState.ED)
        {
            PlayBGM(ED);
        }
    }
    private void InGameBGM(DifferenceState newState)
    {
        if (newState == DifferenceState.Serch)
        {
            PlayBGM(InGame);
        }
    }

    public void BGMStop()
    {
        BGMAudioSourse.Stop();
    }
    public void SEStop()
    {
        SEAudioSourse.Stop();
    }
    public void PlaySE(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        SEAudioSourse.PlayOneShot(clip);
        //Debug.Log("なった！");
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }
        BGMStop();

        BGMAudioSourse.clip = clip;
        BGMAudioSourse.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static OVRPlugin;
public enum FadeState
{
    Dark,
    Bright,
}
public class Fade : MonoBehaviour
{
    public FadeState State { get; private set; }

    private bool ableFade;
    private float fadeInDuration = 3.0f;
    private float fadeOutDuration = 2.0f;
    private float time; 
    Renderer myRenderer;

    // Start is called before the first frame update
    void Start()
    {
        State= FadeState.Bright;
        //state= MyState.FadeIn;

        ableFade = true;

        myRenderer = GetComponent<Renderer>();
        time = 0f;
        //myRenderer.color = new Color(0, 0, 0, 255);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator FadeIn()
    {
        float alphaValue = 0f;
        while (true)
        {
            // 現在のAlpha値を計算
            if (myRenderer.material.color.a <= 0)
            {
                break;
            }
            alphaValue = 1 - (time / fadeInDuration);
            // マテリアルの色を更新
            myRenderer.material.color = new Color(myRenderer.material.color.r,
                                          myRenderer.material.color.g,
                                          myRenderer.material.color.b,
                                          alphaValue);

            // 時間を更新
            time += Time.deltaTime;

            yield return null;
            //Debug.Log(alphaValue);
        }

        time = 0f;
        State = FadeState.Bright;

        Debug.Log(State);
    }

    private IEnumerator FadeOut()
    {
        ableFade = false;

        float alphaValue = 0f;
        while (true)
        {
            // 現在のAlpha値を計算
            //FadeOut
            if (myRenderer.material.color.a >= 1)
            {
                break;
            }

            alphaValue = 0 + (time / fadeOutDuration);
            // マテリアルの色を更新
            myRenderer.material.color = new Color(myRenderer.material.color.r,
                                          myRenderer.material.color.g,
                                          myRenderer.material.color.b,
                                          alphaValue);

            // 時間を更新
            time += Time.deltaTime;

            yield return null;
            //Debug.Log(alphaValue);
        }
        time = 0f;
        State = FadeState.Dark;

        Debug.Log(State);
        ableFade = true;
    }
    private IEnumerator FadeInToOut(float time)
    {
        ableFade = false;

        Debug.Log("FadeOut");
        yield return StartCoroutine(FadeOut());
        yield return new WaitForSeconds(time);

        StartCoroutine(FadeIn());
    }
    public void StartFade(float waitTime)
    {
        if (!ableFade) return;

        StartCoroutine(FadeInToOut(waitTime));
    }
}

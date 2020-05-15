using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameUI : MonoBehaviour
{
    public static UIGameUI Instance;

    public Sprite[] Sprite_DashIcons;
    public Image DashIconImage;
    public Image[] DashBars;

    public Image[] DashUIEffects;
    Coroutine DashHitRoutine = null;

    void Start()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HitDash()
    {
        if (DashHitRoutine != null)
            StopCoroutine(DashHitRoutine);
        DashHitRoutine = StartCoroutine(ShowThenFade(DashUIEffects, Routine:DashHitRoutine));
    }

    public void ShowDashAvailable(int dashamount)
    {
        if (dashamount <= 0)
            DashIconImage.sprite = Sprite_DashIcons[1];
        else
            DashIconImage.sprite = Sprite_DashIcons[0];

        for ( int i = 0; i < DashBars.Length; i++)
            DashBars[i].transform.GetChild(0).gameObject.SetActive(i+1 <= dashamount);
    }

    IEnumerator ShowThenFade(Image[] images, float duration = 0.25f, Coroutine Routine = null)
    {
        foreach (Image i in images)
            i.color = new Color(i.color.r, i.color.g, i.color.b, 1f);
        yield return new WaitForEndOfFrame();
        float timer = duration;
        while (timer > 0)
        {
            foreach (Image i in images)
                i.color = new Color(i.color.r, i.color.g, i.color.b, Mathf.Lerp(0,1,timer / duration));
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
        Routine = null;
    }

    /*IEnumerator ShowThenFade(Image[] images, float FadeDuration = 0.25f)
    {
        float timer = FadeDuration;
        while (timer > 0)
        {
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
    }*/
}

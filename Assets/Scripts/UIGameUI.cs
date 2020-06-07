using arena.combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameUI : MonoBehaviour
{
    [Header("Groups")]
    public GameObject GroupDefault;
    public GameObject GroupGameOver;
    public GameObject GroupPrepare;
    [Header("Start Screen Tweens")]
    public List<GameObject> StartTweenObjects;
    public List<GameObject> StartHideObjects;
    public AudioClip Clip_Begin;

    public static UIGameUI Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<UIGameUI>();

            return _Instance;
        }
    }
    static UIGameUI _Instance;

    public Image PHealthBar;
    public Image HitEffect;
    public Text PHealthText;

    public Text KillCount;

    public Sprite[] Sprite_DashIcons;
    public Image DashIconImage;
    public Image[] DashBars;

    public Image[] DashUIEffects;
    public Transform AmmoCounterParent;
    public Text WeaponName;
    public Text AmmoCounter;
    Coroutine DashHitRoutine = null;
    int current_max_ammo_cnt = 4;
    int current_killcount = 0;
    AudioSource AS;

    void Start()
    {
        //ReactToDamage.
        AS = GetComponent<AudioSource>();
        GroupPrepare.SetActive(true);
        GroupDefault.SetActive(false);
        GroupGameOver.SetActive(false);
        ReactToDamage.ev_obj_hchange_const += PlayerHealth_ev_obj_hchange_const;
        ReactToDamage.ev_obj_death_const += ReactToDamage_ev_obj_death_const;
    }

    private void ReactToDamage_ev_obj_death_const(IHasHealth dead_object)
    {
        if (dead_object.GetType() != typeof(PlayerHealth))
            return;
        GroupDefault.SetActive(false);
        GroupGameOver.SetActive(true);
    }

    private void PlayerHealth_ev_obj_hchange_const(IHasHealth target_object)
    {
        if (target_object.GetType() != typeof(PlayerHealth))
            return;

        StartCoroutine(ShowThenFade(HitEffect));
        PHealthBar.fillAmount = target_object.GetRatio();
        PHealthText.text = target_object.GetHealth().ToString();
    }

    private void OnDestroy()
    {
        ReactToDamage.ev_obj_hchange_const -= PlayerHealth_ev_obj_hchange_const;
        ReactToDamage.ev_obj_death_const -= ReactToDamage_ev_obj_death_const;
    }

    void PlayMode()
    {
        //GroupPrepare.SetActive(false);
        GroupDefault.SetActive(true);
        FPControl.Instance.SwitchToPlayMode();
        AS.clip = Clip_Begin;
        AS.Play();
        StartCoroutine(StartGameTween());
    }

    IEnumerator StartGameTween()
    {
        StartHideObjects.ForEach(x => x.SetActive(false));
        float timer = 0;
        float duration = 2f;
        float linear = 0;

        Action<MaskableGraphic, float> change_alpha = (mg, a) =>
         {
             Color cc = mg.color;
             cc.a = a;
             mg.color = cc;
         };

        while (timer < duration)
        {
            linear = timer / duration;
            Vector3 newscale = new Vector3(1 + linear, 1, 1);
            float alpha = 1f - linear;
            StartTweenObjects.ForEach((element) =>
           {
               element.transform.localScale = newscale;
               MaskableGraphic current_image = element.GetComponent<MaskableGraphic>();
               change_alpha(current_image, alpha);
           });
            StartHideObjects.ForEach((element) =>
            {
                MaskableGraphic image = element.GetComponent<MaskableGraphic>();
                change_alpha(image, alpha);
            });
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        GroupPrepare.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (FPControl.Instance.CurrentStat == FPControl.GameStatus.Waiting)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                PlayMode();
        }
    }

    public void AddKill()
    {
        current_killcount++;
        KillCount.text = current_killcount.ToString();
    }

    public void HitDash()
    {
        if (DashHitRoutine != null)
            StopCoroutine(DashHitRoutine);
        DashHitRoutine = StartCoroutine(ShowThenFade(DashUIEffects, Routine:DashHitRoutine));
    }

    public void AmmoCounterInit(int max_count, string weapon_name)
    {
        WeaponName.text = weapon_name;
        for (int i = 0; i < AmmoCounterParent.transform.childCount - 1; i++)
            AmmoCounterParent.transform.GetChild(i).gameObject.SetActive((i + 1 <= max_count));
        current_max_ammo_cnt = max_count;
    }
    public void AmmoCounterUpdate(int current_ammo)
    {
        AmmoCounter.text = current_ammo.ToString();
        for (int i = 0; i < current_max_ammo_cnt; i++)
        {
            if (i >= AmmoCounterParent.transform.childCount)
                break;

            GameObject G = AmmoCounterParent.transform.GetChild(i).gameObject;
            G.transform.GetChild(0).gameObject.SetActive(current_ammo >= i + 1);
        }
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

    IEnumerator ShowThenFade(Image image, float duration = 0.25f, Coroutine Routine = null)
    {
        Image[] im = new Image[1];
        im[0] = image;
        yield return StartCoroutine(ShowThenFade(im, duration, Routine));
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

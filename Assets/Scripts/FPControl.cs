using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using arena.combat;

public class FPControl : MonoBehaviour
{
    public float MoveSpeed = 4f;
    public float DashMult = 20f;
    public float DashTime = 0.15f;
    public float JumpForce = 10f;
    public float Gravity = 20f;
    public WeaponMaster[] Weapons;
    public WeaponMaster MainWeapon;

    float CurrentJump = 0;
    bool Dashing = false;
    bool Dead = false;

    [HideInInspector]
    public CharacterController cCont;
    Camera mainCam;
    Vector3 movDelta;
    Vector3 movForward;
    Vector2 msDelta;

    #region Cam_Effect_Vars
    float cam_x_angle_kicked = 0;
    float cam_x_angle_recover = 0;
    float cam_fov_kicked = 60;
    float cam_fov_original = 60;
    #endregion

    float DashEnergy = 4;
    int JumpCount = 2;

    Coroutine WeaponChangeRoutine;
    PlayerHealth pHealthComp;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        cCont = GetComponent<CharacterController>();
        pHealthComp = GetComponent<PlayerHealth>();
        movDelta = new Vector3(0, 0, 0);
        movForward = transform.forward;
        MainWeapon = Weapons[0];

        pHealthComp.ev_obj_hchange += PHealthComp_ev_obj_hchange;
        pHealthComp.ev_obj_death += PHealthComp_ev_obj_death;

        CursorLock();
    }

    private void PHealthComp_ev_obj_death(IHasHealth dead_object)
    {
        if (Dead)
            return;

        Dead = true;
        mainCam.transform.GetChild(0).gameObject.SetActive(false);
        mainCam.gameObject.AddComponent<SphereCollider>();
        mainCam.gameObject.AddComponent<Rigidbody>();
        mainCam.transform.parent = null;
    }

    private void PHealthComp_ev_obj_hchange(IHasHealth target_object)
    {
        Debug.Log($"Health Changed : {target_object.GetHealth()}");
    }

    public void Shock(Vector3 forward, float amount)
    {
        FOVKick(amount);
        StartCoroutine(ShockRoutine(forward, amount));
    }

    IEnumerator ShockRoutine(Vector3 forward, float amount, float duration = 0.5f)
    {
        float timer = duration;
        float mult = 1f;
        while (timer > 0)
        {
            cCont.Move(forward * amount * mult * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            mult = timer / duration;
            timer -= Time.deltaTime;
        }

    }

    public void FOVKick(float amount)
    {
        cam_x_angle_recover = mainCam.transform.localEulerAngles.x;
        cam_x_angle_kicked = mainCam.transform.localEulerAngles.x - amount;
        cam_fov_kicked = mainCam.fieldOfView = 60 + amount;
        mainCam.transform.Rotate(Vector3.right, -amount);
        StartCoroutine(ResetKick(amount * 0.05f));
    }
    IEnumerator ResetKick(float time)
    {
        float mTimer = 0;
        float t = 0;
        while (mTimer < time)
        {
            t = mTimer / time;
            mainCam.transform.localRotation =
                Quaternion.Euler(Mathf.SmoothStep(cam_x_angle_kicked,
                cam_x_angle_recover, t),
                mainCam.transform.localEulerAngles.y,
                mainCam.transform.localEulerAngles.z);
            mainCam.fieldOfView = Mathf.SmoothStep(cam_fov_kicked,
                cam_fov_original, t);
            yield return new WaitForEndOfFrame();
            mTimer += Time.deltaTime;
        }
    }

    IEnumerator ChangeRoutine(int slot)
    {
        if (Weapons[slot] == MainWeapon)
            yield break;

        yield return StartCoroutine(MainWeapon.HideWeapon());
        Weapons[slot].gameObject.SetActive(true);
        MainWeapon = Weapons[slot];
        WeaponChangeRoutine = null;
    }

    void CursorLock(bool locked = true)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
    // Update is called once per frame
    void Update()
    {
        if (Dead)
            return;
        // 시점변경
        msDelta.x = Input.GetAxis("Mouse X");
        msDelta.y = Input.GetAxis("Mouse Y") * -1;
        transform.Rotate(Vector3.up, msDelta.x);
        mainCam.transform.Rotate(Vector3.right, msDelta.y);
        // 지금 가진 무기 발사
        if (Input.GetButtonDown("Fire1"))
        {
            if (MainWeapon.FullAuto)
                MainWeapon.StartFiring();
            else if (MainWeapon.ShootWeaponSingle())
                FOVKick(5);
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (MainWeapon.FullAuto)
                MainWeapon.StopFiring();

        }
        if (WeaponChangeRoutine == null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                WeaponChangeRoutine = StartCoroutine(ChangeRoutine(0));
            if (Input.GetKeyDown(KeyCode.Alpha2))
                WeaponChangeRoutine = StartCoroutine(ChangeRoutine(1));
            if (Input.GetKeyDown(KeyCode.Alpha3))
                WeaponChangeRoutine = StartCoroutine(ChangeRoutine(2));
        }

        if (!Dashing)
        {
            if (cCont.isGrounded)
                JumpCount = 2;

            // 캐릭터 이동
            GetForward(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            cCont.Move(movForward * Time.deltaTime);
            if (MainWeapon.WeaponAnimator != null)
                MainWeapon.WeaponAnimator.SetFloat("WalkBlend", movForward.magnitude);
            // 점프
            if (Input.GetKeyDown(KeyCode.Space) && JumpCount > 0)
            {
                CurrentJump = JumpForce;
                JumpCount--;
            }
            cCont.Move(CurrentJump * Vector3.up * Time.deltaTime);
            CurrentJump -= Gravity * Time.deltaTime;
            // 대쉬
            if (Input.GetKeyDown(KeyCode.LeftShift))
                StartCoroutine(Dash());
        }

        if (DashEnergy < 4)
        {
            DashEnergy += Time.deltaTime;
            UIGameUI.Instance.ShowDashAvailable((int)(DashEnergy / 2));
        }
    }

    void GetForward(float h, float v)
    {
        movForward = transform.forward * v +
               Quaternion.AngleAxis(90, Vector3.up) *
               transform.forward * h;
        movForward *= MoveSpeed;
    }

    IEnumerator Dash()
    {
        if (DashEnergy < 2)
            yield break;
        UIGameUI.Instance.HitDash();
        DashEnergy -= 2;
        UIGameUI.Instance.ShowDashAvailable((int)(DashEnergy / 2));
        float second = 0;
        Dashing = true;
        CurrentJump = 0;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0)
        {
            if (Mathf.Abs(h) > Mathf.Abs(v))
                v = 0;
            else
                h = 0;
        }
        else
            v = 1;
        GetForward(h, v);
        while (second < DashTime)
        {
            cCont.Move(movForward * Time.deltaTime * DashMult);
            yield return new WaitForEndOfFrame();
            second += Time.deltaTime;
        }
        Dashing = false;
    }
}

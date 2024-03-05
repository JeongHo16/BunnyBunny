using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MWType //Main Weapon Type
{
    Gun,
    Blade,
}

public class WeaponMgr : MonoBehaviour
{
    //메인 무기
    [Header("------ Main Weapon ------")]
    public GameObject[] MWPrefabs = null;
    public Transform MainWeapon = null; 
    public MWType MainType = MWType.Gun; //TODO : public 이어야 할지 잘 모르겠네

    GunCtrl gunCtrlSc = null;
    public GunCtrl GunCtrlSc
    {
        get
        {//TODO : 사용할때 null check 하면 이렇게 구현 안해도 되는데... 흠..
            if (gunCtrlSc != null) 
                return gunCtrlSc;
            else return null;
        }
    }
    //TODO : bladeCtrl.cs 추가하기
    //메인 무기

    //수호자 관련
    [Header("------ Guardians ------")]
    public GameObject Guardians = null;
    GuardiansCtrl guardiansCtrlSc = null;
    public GuardiansCtrl GuardiansCtrlSc
    {
        get
        {
            if (guardiansCtrlSc != null)
                return guardiansCtrlSc;
            else return null;
        }
    }
    //수호자 관련

    //로켓 관련
    [Header("------ Rockets ------")]
    public GameObject Rockets = null;
    RocketCtrl rocketCtrlSc = null;
    public RocketCtrl RocketCtrlSc
    {
        get
        {
            if (rocketCtrlSc != null)
                return rocketCtrlSc;
            else return null;
        }
    }
    //로켓 관련

    public static WeaponMgr Inst = null;

    void Awake()
    {
        Inst = this;

        SetMainWeapon(MainType);
    }

    void Start() { }

    //void Update() { }

    void SetMainWeapon(MWType mwType) //메인 무기 세팅 및 교체 함수
    {
        for (int i = 0; i < MainWeapon.childCount; i++) //교체를 위해 이전 오브젝트 삭제. 
            Destroy(MainWeapon.GetChild(i)); //교체가 필요할진 모르겠네.

        GameObject obj = Instantiate(MWPrefabs[(int)mwType], MainWeapon);
        if (mwType == MWType.Gun)
        {
            gunCtrlSc = obj.GetComponent<GunCtrl>();
        }
        else if (mwType == MWType.Blade)
        {
            //TODO : BladeCtrl.cs 만들고 추가해야함.
        }
    }

    //수호자 관련
    public void SetGuardians()
    {
        if (!Guardians.activeSelf)
        {
            Guardians.SetActive(true);
            guardiansCtrlSc = Guardians.GetComponent<GuardiansCtrl>();
        }
    }
    //수호자 관련

    //로켓 관련
    public void SetRockets()
    {
        if (!Rockets.activeSelf)
        {
            Rockets.SetActive(true);
            rocketCtrlSc = Rockets.GetComponent<RocketCtrl>();
        }
    }
    //로켓 관련

    //드릴 관련
    public void SetDrills()
    {

    }
    //드릴 관련
}
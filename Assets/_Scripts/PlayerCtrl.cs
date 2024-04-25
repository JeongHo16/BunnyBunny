using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerAnim { Idle, Run, Hurt, Dead }

public enum PlayerType { Fire, Water, Wind }

public class PlayerCtrl : MonoBehaviour
{
    public bool FullPowerTest = true;

    //이동 관련
    [HideInInspector] public float h = 0.0f;
    [HideInInspector] public float v = 0.0f;
    float moveSpeed = 3.0f;
    Vector3 moveDir = Vector3.zero;
    SpriteRenderer playerSpRenderer = null;
    Rigidbody2D rigid = null;
    const float OffsetX = 4.7f;
    const float OffsetY = 5.1f;
    //이동 관련

    //Flip 관련
    CapsuleCollider2D capColl = null;
    GameObject gun = null;
    Transform mainWeapon = null;
    const float capOffsetX = 0.04f;
    const float gunOffsetX = 0.15f;
    const float gunImgOffsetX = 0.05f;
    //Flip 관련

    //화살표 관련
    public GameObject DirArrow = null;
    Vector3 arrowDir = Vector3.up;
    float arrowAngle = 0.0f;
    float angleOffset = 90.0f;
    const float ArrowOffset = 0.7f;
    //화살표 관련

    //능력치 관련
    bool isDead = false;
    float curHp = 100.0f;
    public float maxHp = 100.0f;
    //float maxHp = float.MaxValue;
    float attack = 10.0f;
    float defense = 10.0f;
    //능력치 관련

    //UI 관련
    public Canvas SubCanvas = null;
    public Image HpBar_Img = null;
    Vector3 dmgTxtOffset = new Vector3(0, 0.5f, 0);
    //UI 관련

    //Timer 관련
    float mAtkTimer = 0.2f; //메인 총알
    float mAtkTime = 0.2f;
    float rktTimer = 2.0f; //로켓
    float rktTime = 2.0f;
    float drlTimer = 4.0f; //드릴
    float drlTime = 4.0f;
    //Timer 관련

    //Animation 관련
    [HideInInspector] public PlayerAnim AnimState = PlayerAnim.Idle;
    Animator animator = null;
    //Animation 관련

    //Player 속성(Type) 관련
    public PlayerType PlayerProperty = PlayerType.Fire;
    //Player 속성(Type) 관련

    WeaponMgr wpMgr = null; //이 Script에서는 너무 많이 써서 선언하고 쓰는 중.

    void Start()
    {
        playerSpRenderer = GameObject.Find("Player_Img").GetComponent<SpriteRenderer>();
        capColl = GetComponent<CapsuleCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        wpMgr = FindObjectOfType<WeaponMgr>();
        mainWeapon = GameObject.Find("MainWeapon").transform;

        curHp = maxHp;

        if (FullPowerTest)
        {
            //wpMgr.SetRockets(); //로켓 test 용
            //wpMgr.SetGuardians(); //가디언 test 용
            //wpMgr.SetDrills(); //드릴 test 용
        }
    }

    //MapRePosition하는 큰 Box Collider 때문에 웬만하면 여기서 이 함수 구현 안함
    //void OnTriggerEnter2D(Collider2D coll) { }

    void FixedUpdate()
    {
        Move(); //충돌(collision, not trigger) 때문에 여기서 호출
    }

    void Update()
    {
        DirectionArrow();
        CalcWeaponsTimer();
        PlayerStateUpdate();

        if (wpMgr.MainType == MWType.Gun)
            RotateGun();

        SubCanvas.transform.position = transform.position; //Move()에 있었는데 느려서 여기서 호출 

        //if (Input.GetKeyDown(KeyCode.Space) && FullPowerTest)
        //{
        //    wpMgr.GuardiansCtrlSc.LevelUpWeapon(); //가디언 test 용
        //    wpMgr.RocketCtrlSc.LevelUpWeapon(); //로켓 test 용
        //    wpMgr.DrillCtrlSc.LevelUpWeapon(); //드릴 test 용
        //    wpMgr.GunCtrlSc.LevelUpWeapon();
        //}
        if (Input.GetKeyDown(KeyCode.Alpha1) && wpMgr.RocketCtrlSc != null)
        {
            wpMgr.RocketCtrlSc.LevelUpWeapon(); //로켓 test 용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && wpMgr.GuardiansCtrlSc != null)
        {
            wpMgr.GuardiansCtrlSc.LevelUpWeapon(); //가디언 test 용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && wpMgr.DrillCtrlSc != null)
        {
            wpMgr.DrillCtrlSc.LevelUpWeapon(); //드릴 test 용
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && wpMgr.GunCtrlSc != null)
        {
            wpMgr.GunCtrlSc.LevelUpWeapon(); //Gun test 용
        }
    }

    void Move()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //좌우 방향 바뀔때마다 flip
        if (0.0f < h) Flip(true);
        else if (h < 0.0f) Flip(false);

        moveDir = (Vector2.up * v) + (Vector2.right * h);
        if (1.0f < moveDir.magnitude)
            moveDir.Normalize();

        Vector3 targetPos = transform.position + moveDir * moveSpeed * Time.deltaTime;
        rigid.MovePosition(targetPos);

        if (GameMgr.Inst.hasRing) LimitPos(targetPos);
    }

    void LimitPos(Vector2 pos)
    {
        pos.x = Mathf.Clamp(pos.x, GameMgr.Inst.BattleRing.transform.position.x - OffsetX,
            GameMgr.Inst.BattleRing.transform.position.x + OffsetX);
        pos.y = Mathf.Clamp(pos.y, GameMgr.Inst.BattleRing.transform.position.y - OffsetY,
            GameMgr.Inst.BattleRing.transform.position.y + OffsetY);

        rigid.MovePosition(pos);
    }

    void Flip(bool flip)
    {
        Vector2 tmpVec = Vector2.zero;

        //player flip
        playerSpRenderer.flipX = flip;

        //collider flip
        tmpVec = capColl.offset;
        tmpVec.x = flip ? capOffsetX : -capOffsetX;
        capColl.offset = tmpVec;

        if (wpMgr.MainType == MWType.Gun)
        {
            //gun flip
            if (gun == null)
                gun = mainWeapon.GetChild(0).gameObject;

            tmpVec = gun.transform.localPosition;
            tmpVec.x = flip ? gunOffsetX : -gunOffsetX;
            gun.transform.localPosition = tmpVec;

            SpriteRenderer spRend = gun.GetComponentInChildren<SpriteRenderer>();
            spRend.flipX = flip;

            tmpVec = spRend.transform.localPosition;
            tmpVec.x = flip ? gunImgOffsetX : -gunImgOffsetX;
            spRend.transform.localPosition = tmpVec;
        }
    }

    void DirectionArrow()
    {
        if (moveDir.normalized != Vector3.zero)
            arrowDir = moveDir.normalized;

        arrowAngle = Mathf.Atan2(arrowDir.normalized.y, arrowDir.normalized.x) * Mathf.Rad2Deg;
        DirArrow.transform.rotation = Quaternion.AngleAxis(arrowAngle - angleOffset, Vector3.forward);
        DirArrow.transform.position = transform.position + arrowDir.normalized * ArrowOffset;
    }

    void RotateGun()
    {
        if (gun == null)
            gun = mainWeapon.GetChild(0).gameObject;

        if (moveDir.normalized != Vector3.zero)
            arrowDir = moveDir.normalized;

        if (h < 0.0f)
            gun.transform.rotation = Quaternion.AngleAxis(arrowAngle - 180f, Vector3.forward);
        else if (0.0f < h)
            gun.transform.rotation = Quaternion.AngleAxis(arrowAngle, Vector3.forward);
    }

    public void TakeDamage(float damage)
    {
        //1. Hp변수 깎기
        float dmgTxt = curHp < damage ? curHp : damage;
        curHp -= damage;
        //2. Hp UI 수정
        HpBar_Img.fillAmount = curHp / maxHp;
        //3. Dmg Txt 띄우기 
        GameMgr.Inst.SpawnDmgTxt(transform.position + dmgTxtOffset, dmgTxt, Color.red);

        if (curHp <= 0.0f)
        {
            PlayerDie();
        }
    }

    public void GetHp(float healRate)
    {
        float heal = maxHp * healRate;

        curHp += heal;
        if (maxHp <= curHp)
            curHp = maxHp;

        HpBar_Img.fillAmount = curHp / maxHp;

        GameMgr.Inst.SpawnDmgTxt(transform.position + dmgTxtOffset, heal, Color.blue);
    }

    void CalcWeaponsTimer()
    {
        //메인 무기 타이머
        mAtkTimer -= Time.deltaTime;
        if (mAtkTimer <= 0.0f)
        {
            mAtkTimer = mAtkTime;
            if (wpMgr.GunCtrlSc != null)
            {
                //wpMgr.GunCtrlSc.FireBullet(arrowDir);
                //wpMgr.GunCtrlSc.FireBulletOneShot(arrowDir);
                wpMgr.GunCtrlSc.FanFire(arrowDir);
            }

        }
        //메인 무기 타이머

        //로켓 타이머
        rktTimer -= Time.deltaTime;
        if (rktTimer <= 0.0f)
        {
            rktTimer = rktTime;
            if (wpMgr.RocketCtrlSc != null)
            {
                if (!WeaponMgr.Inst.RocketCtrlSc.IsEvolve)
                    wpMgr.RocketCtrlSc.FireRocket();
                else
                    wpMgr.RocketCtrlSc.FireNuclear();
            }
        }
        //로켓 타이머

        //드릴 타이머
        drlTimer -= Time.deltaTime;
        if (drlTimer <= 0.0f)
        {
            drlTimer = drlTime;
            if (wpMgr.DrillCtrlSc != null)
            {
                if(!WeaponMgr.Inst.DrillCtrlSc.IsEvolve)
                    wpMgr.DrillCtrlSc.FireDrills();
                else
                    wpMgr.DrillCtrlSc.FireArrowHead();
            }
        }
        //드릴 타이머
    }

    //state, action update 하는 함수 따로 만들기?
    void PlayerStateUpdate()
    {
        if (moveDir.normalized == Vector3.zero)
            animator.SetBool("Moving", false);
        else
            animator.SetBool("Moving", true);
    }

    public void TrapBossRing(bool trap)
    {
        if (trap)
        {
            //boxColl.enabled = false;
            capColl.isTrigger = false;
            rigid.bodyType = RigidbodyType2D.Dynamic;
            rigid.gravityScale = 0.0f;
            rigid.mass = 0; // 플레이어가 보스 밀지 못하게 0
        }
        else
        {
            capColl.isTrigger = true;
        }
    }

    void PlayerDie()
    {
        GameMgr.Inst.GameOver();
        //isDead = true; //TODO : 어떤 상태 넣을건지?
        //AnimState = PlayerAnim.Dead;

        return;
    }

    /*
    void MoveLimit() 
    {
        limitPos = transform.position;

        if (ScreenMgr.ScMin.x > transform.position.x)
            limitPos.x = ScreenMgr.ScMin.x;
        if (ScreenMgr.ScMax.x < transform.position.x)
            limitPos.x = ScreenMgr.ScMax.x;
        if (ScreenMgr.ScMin.y > transform.position.y)
            limitPos.y = ScreenMgr.ScMin.y;
        if (ScreenMgr.ScMax.y < transform.position.y)
            limitPos.y = ScreenMgr.ScMax.y;

        transform.position = limitPos;
    }
    */
}

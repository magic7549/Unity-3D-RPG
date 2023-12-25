using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    // 캐릭터 Die 처리 델리게이트
    public delegate void OnPlayerDeath();
    public static event OnPlayerDeath playerDeathEvent;

    // 캐릭터 Respawn 처리 델리게이트
    public delegate void OnPlayerRespawn();
    public static event OnPlayerRespawn playerRespawnEvent;

    public Animator animator;
    public BoxCollider attackCollider;
    public BoxCollider skill2_Collider;
    [SerializeField] private ParticleSystem[] SkillEffect;
    public Transform skill_1Position;

    private Camera playerCamera;
    private CameraMovement cameraMovement;
    private CharacterController characterController;
    private PlayerStat stat;
    private GameObject scanObject;  // trigger 이벤트를 호출한 오브젝트
    [SerializeField] private ParticleSystem[] levelupParticle;
    [SerializeField] private AudioSource attackSource;
    [SerializeField] private AudioSource skill1Source;
    [SerializeField] private GameObject _damageText;    // 데미지 표시

    public float speed = 7f;
    public float smoothness = 50f;
    public float jumpForce = 3.0f;
    public float groundedOffset = -0.25f;
    public float groundedRadius = 0.4f;
    public LayerMask groundLayers;
    public float attackDelay = 0.2f;
    public Vector3 startPoint = new Vector3(244f, 36.43701f, 73f);

    private Vector3 moveDirection;
    private float _hAxis;
    private float _vAxis;
    private bool _jDown;
    private bool _m1Down;    // 마우스 좌클릭
    private bool _m2Down;    // 마우스 우클릭
    private bool _1Down;    // 1
    private bool _2Down;    // 2

    // 중력 관련 속도
    public float gravity = -30.0f;
    public float verticalVelocity;
    private float maxGravityVelocity = -20.0f;

    // 플레이어 상태 확인 변수
    private bool canMove = true;
    private bool isGround = true;
    private bool isAttack = false;
    private bool isRoll = false;    // 구르기를 사용 중인지
    private bool isSkill_1 = false;  // 1번 스킬을 사용 중인지
    private bool isSkill_1_movestop = false;  // 1번 스킬 사용 모션 움직임 멈춤
    private bool isSkill_2 = false;  // 2번 스킬을 사용 중인지
    private bool isDead = false;
    private bool isSavepoint = false;   // 세이브 포인트 범위 안에 있는지
    private bool isNearbyNpc = false;   // NPC와 가까운지
    private bool isNearbyShop = false;   // ShopNpc와 가까운지
    private bool isNearbyBoat = false;  // 보트와 가까운지
    private bool isNearbyCar = false;  // 차와 가까운지

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        stat = GetComponent<PlayerStat>();

        if (!photonView.IsMine) return;

        Cursor.visible = false;                     //마우스 커서가 보이지 않게 함
        Cursor.lockState = CursorLockMode.Locked;   //마우스 커서를 고정시킴

        playerCamera = Camera.main;
        cameraMovement = playerCamera.GetComponentInParent<CameraMovement>();
        cameraMovement.objectToFollow = transform.Find("FollowCam");
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (canMove)
        {
            GetInput();
            JumpAndGravity();
            groundedCheck();
            CheckHp();
            Move();
            Attack();
            Roll();
            Skill_1();
            Skill_2();
            OpenInven();
            OpenEquipment();
            OpenExitUI();
            Interact();
        }
    }

    private void GetInput()
    {
        _hAxis = Input.GetAxisRaw("Horizontal");
        _vAxis = Input.GetAxisRaw("Vertical");
        _jDown = Input.GetButtonDown("Jump");
        _m1Down = Input.GetButtonDown("Fire1");
        _m2Down = Input.GetButtonDown("Fire2");
        _1Down = Input.GetButtonDown("1");
        _2Down = Input.GetButtonDown("2");
    }

    private void groundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        isGround = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

        animator.SetBool("isGround", isGround);
    }

    void OnDrawGizmos()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, groundedRadius);
    }

    private void Move()
    {
        if (!isAttack && !isDead && !isRoll && !isSkill_1_movestop && !isSkill_2)
        {
            Vector3 forward = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
            Vector3 right = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;

            moveDirection = (forward * _vAxis + right * _hAxis).normalized;

            // 키 입력한 방향 바라보게
            if (moveDirection.magnitude != 0)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * smoothness);

            characterController.Move(moveDirection.normalized * (speed * IngameManager.instance.stat.speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);

            float percent = moveDirection.magnitude;
            animator.SetFloat("RunBlend", percent, 0.1f, Time.deltaTime);
        }
        else if (isDead)
        {
            characterController.Move(Physics.gravity * Time.deltaTime);
        }

        if (isRoll)
        {
            // 키 입력한 방향 즉시 바라보게
            if (moveDirection.magnitude != 0)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
                characterController.SimpleMove(moveDirection.normalized * IngameManager.instance.stat.speed * 12f);
            }
            else
            {
                characterController.SimpleMove(transform.forward * IngameManager.instance.stat.speed * 12f);
            }
        }
    }

    private void JumpAndGravity()
    {
        if (isGround && !isDead)
        {
            animator.SetBool("isJump", false);
            animator.SetBool("isAir", false);

            // 땅일 경우 중력이 적용되는 것을 멈춤
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            // Jump
            if (_jDown && !isAttack && !isRoll && !isSkill_1_movestop && !isSkill_2)
            {
                // 루트 (점프 힘 * -2f * 중력)
                verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);

                animator.SetBool("isJump", true);
            }
        }
        else
        {
            // 중력이 과하게 세지는 것을 방지
            if (verticalVelocity < maxGravityVelocity)
            {
                verticalVelocity = maxGravityVelocity;
            }

            animator.SetBool("isAir", true);
        }

        // 중력
        verticalVelocity += gravity * Time.deltaTime;
    }

    [PunRPC]
    private void AnimTriggerRpc(string animStr)
    {
        if (animator != null)
            animator.SetTrigger(animStr);
    }

    private void Attack()
    {
        if (!IngameManager.instance.uiManager.OverUI() && _m1Down && isGround && !isDead && !isRoll && !isSkill_1_movestop && !isSkill_2)
        {
            isAttack = true;

            // 공격 모션 동기화
            photonView.RPC("AnimTriggerRpc", RpcTarget.All, "isAttack");

            // real damage 동기화
            photonView.RPC("SetRealDmgRPC", RpcTarget.All, stat.damage);
        }
    }

    [PunRPC]
    private void SetRealDmgRPC(float damage)
    {
        stat.realDmg = damage;
    }

    private void Roll()
    {
        if(!IngameManager.instance.uiManager.OverUI() && _m2Down && isGround && !isDead && !isAttack && !isRoll && !isSkill_1_movestop && !isSkill_2)
        {
            isRoll = true;

            // 구르기 모션 동기화
            photonView.RPC("AnimTriggerRpc", RpcTarget.All, "doRoll");
        }
    }

    public void EndRoll()
    {
        Debug.Log("EndRoll");

        isRoll = false;
    }

    public void AttackColliderEnable()
    {
        attackCollider.enabled = true;
        // 공격하는 사운드
        attackSource.Play();

        StartCoroutine(AttackColliderDisable());
    }

    private IEnumerator AttackColliderDisable()
    {
        yield return new WaitForSeconds(0.1f);
        attackCollider.enabled = false;
    }

    public void AttackEnd()
    {
        isAttack = false;
        animator.ResetTrigger("isAttack");
        Debug.Log("AttackEnd");
    }

    // 첫번째 스킬
    private void Skill_1()
    {
        if (!IngameManager.instance.uiManager.OverUI() && _1Down && isGround && !isAttack && !isDead && !isRoll && !isSkill_1 && SkillUnlockParsing(stat.skill_unlock[0]))
        {
            if (stat.currMp >= 10)
            {
                isSkill_1 = true;
                isSkill_1_movestop = true;
                photonView.RPC("AnimTriggerRpc", RpcTarget.All, "doSkill_1");
                stat.currMp -= 10;

                skill1Source.Play();
                GameObject skill = PhotonNetwork.Instantiate("Skill_1", skill_1Position.position, skill_1Position.rotation);
                StartCoroutine(SkillDuration(skill, 5f));
            }
        }
    }

    private void Skill1EndMotion()
    {
        isSkill_1_movestop = false;
    }

    private IEnumerator SkillDuration(GameObject skill, float delay)
    {
        yield return new WaitForSeconds(delay);
        isSkill_1 = false;
    }

    // 두번째 스킬
    private void Skill_2()
    {
        if (!IngameManager.instance.uiManager.OverUI() && _2Down && isGround && !isAttack && !isDead && !isRoll && !isSkill_1_movestop && !isSkill_2 && SkillUnlockParsing(stat.skill_unlock[1]))
        {
            if (stat.currMp >= 10)
            {
                isSkill_2 = true;
                stat.currMp -= 10;
                photonView.RPC("AnimTriggerRpc", RpcTarget.All, "doSkill_2");
            }
        }
    }

    public void SkillColliderEnable()
    {
        // 공격하는 사운드
        attackSource.Play();

        stat.realDmg = stat.damage * 4f;
        SkillEffect[1].gameObject.SetActive(true);
        skill2_Collider.enabled = true;
        StartCoroutine(SkillColliderDisable());
    }

    private IEnumerator SkillColliderDisable()
    {
        yield return new WaitForSeconds(0.1f);
        skill2_Collider.enabled = false;
        stat.realDmg = stat.damage;
    }

    public void Skill_2End()
    {
        isSkill_2 = false;
        SkillEffect[1].gameObject.SetActive(false);
    }

    // 문자로 이루어진 값을 bool값으로 전환 ex) "1" => true, "0" => false
    private bool SkillUnlockParsing(string unlock)
    {
        return unlock.Equals("0") ? false : true;
    }

    [PunRPC]
    private void SpawnDamageText(Vector3 position, float damage)
    {
        GameObject damageText = Instantiate(_damageText);
        position.y += 1.0f;
        damageText.transform.position = position;
        damageText.GetComponent<DamageText>().damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.tag == "MonsterAttack" && !isRoll)
        {
            float modifiedDamage = Random.Range(other.GetComponentInParent<Monster>().damage * 0.85f, other.GetComponentInParent<Monster>().damage * 1.15f);
            modifiedDamage = Mathf.RoundToInt(modifiedDamage);

            photonView.RPC("SpawnDamageText", RpcTarget.AllBuffered, transform.position, modifiedDamage);
            stat.currHp -= (int)modifiedDamage;
        }
        else if (other.tag == "BossAttack" && !isRoll)
        {
            float modifiedDamage = Random.Range(other.GetComponentInParent<Boss>().realDmg * 0.85f, other.GetComponentInParent<Boss>().realDmg * 1.15f);
            modifiedDamage = Mathf.RoundToInt(modifiedDamage);

            photonView.RPC("SpawnDamageText", RpcTarget.AllBuffered, transform.position, modifiedDamage);
            stat.currHp -= (int)modifiedDamage;
        }
        else if (other.tag == "SafeZone")
        {
            CancelInvoke("DecreaseHealth");
        }
        else if (other.tag == "SavePoint")
        {
            scanObject = other.gameObject;
            isSavepoint = true;
            IngameManager.instance.uiManager.restUI.ToggleRestButton();
        }
        else if (other.tag == "Npc")
        {
            scanObject = other.gameObject;
            isNearbyNpc = true;
        }
        else if (other.tag == "Shop")
        {
            scanObject = other.gameObject;
            isNearbyShop = true;
        }
        else if (other.tag == "Boat")
        {
            scanObject = other.gameObject;
            isNearbyBoat = true;
        }
        else if (other.tag == "Car")
        {
            scanObject = other.gameObject;
            isNearbyCar = true;
        }
        else if (other.tag == "Water")
        {
            stat.currHp = 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!photonView.IsMine) return;

        if (other.tag == "SafeZone")
        {
            InvokeRepeating("DecreaseHealth", 1, 1);
        }
        else if (other.tag == "SavePoint")
        {
            isSavepoint = false;
            scanObject = null;
            IngameManager.instance.uiManager.restUI.ToggleRestButton();
        }
        else if (other.tag == "Npc")
        {
            isNearbyNpc = false;
            scanObject = null;
            IngameManager.instance.uiManager.talkUI.ShowText(scanObject);   // 대화 중 범위 벗어날 경우 대화 종료
        }
        else if (other.tag == "Shop")
        {
            isNearbyShop = false;
            scanObject = null;
            IngameManager.instance.uiManager.shopUI.CloseShop();
        }
        else if (other.tag == "Boat")
        {
            isNearbyBoat = false;
            scanObject = null;
        }
        else if (other.tag == "Car")
        {
            isNearbyCar = false;
            scanObject = null;
        }
    }

    private void DecreaseHealth() // 체력 감소
    {
        // 체력이 0 이상인 경우에만 체력 감소
        if (stat.currHp > 0)
        {
            stat.currHp--;
        }
    }

    private void CheckHp()
    {
        if (stat.currHp <= 0 && !isDead)
        {
            CancelInvoke("DecreaseHealth");
            StartCoroutine(DeathAndRespawn());
        }
    }

    [PunRPC]
    public void Teleport(Vector3 point)
    {
        // transform.position을 이용하기 위해서는 CharacterController 비활성화 해야 함
        characterController.enabled = false;
        transform.position = point;
        characterController.enabled = true;
    }

    private IEnumerator DeathAndRespawn()
    {
        /* Death */
        isRoll = false;
        isDead = true;

        // 죽는 모션 동기화
        photonView.RPC("AnimTriggerRpc", RpcTarget.All, "doDie");

        yield return new WaitForSeconds(2f);

        // 델리게이트 실행
        if (playerDeathEvent != null)
        {
            playerDeathEvent();
        }

        /* Respawn */
        yield return new WaitForSeconds(2f);
        Teleport(IngameManager.instance.uiManager.savepointUI.savepointVectors[stat.lastUseSavepoint]);
        stat.currHp = stat.maxHp;
        stat.currMp = stat.maxMp;
        isDead = false;
        isAttack = false;

        yield return new WaitForSeconds(1f);

        // 델리게이트 실행
        if (playerRespawnEvent != null)
        {
            playerRespawnEvent();
        }
    }

    // 메뉴창 열기
    public void OpenMenu()
    {
        canMove = false; // 캐릭터 움직임 멈춤
        cameraMovement.MoveControl(false);  // 카메라 움직임 멈춤
        cameraMovement.RotationControl(false);  // 카메라 회전 멈춤
        // 애니메이션 초기화
        animator.SetFloat("RunBlend", 0f);
        animator.SetBool("isJump", false);

        Cursor.visible = true;                     //마우스 커서가 보이게
        Cursor.lockState = CursorLockMode.None;   //마우스 커서를 고정 해제
    }

    // 메뉴창 닫기
    public void CloseMenu()
    {
        canMove = true; // 캐릭터 움직임 다시 시작
        cameraMovement.MoveControl(true);  // 카메라 움직임 다시 시작
        cameraMovement.RotationControl(true);  // 카메라 회전 다시 시작

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 인벤토리 열기 or 닫기
    private void OpenInven()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            IngameManager.instance.uiManager.inventoryUI.OpenAndCloseInven();
        }
    }

    // 장비창 열기 or 닫기
    private void OpenEquipment()
    {
        if (Input.GetButtonDown("Equipment"))
        {
            IngameManager.instance.uiManager.equipmentUI.OpenAndCloseEquipment();
        }
    }

    private void OpenExitUI()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (IngameManager.instance.uiManager.exitUI.exitPanel.activeSelf)
            { 
                IngameManager.instance.uiManager.exitUI.OpenAndCloseExit();
                Debug.Log("실행");
            }
            else if (IngameManager.instance.uiManager.equipmentUI.panel.activeSelf)
            {
                IngameManager.instance.uiManager.equipmentUI.OpenAndCloseEquipment();
            }
            else if (IngameManager.instance.uiManager.inventoryUI.inventoryPanel.activeSelf)
            {
                IngameManager.instance.uiManager.inventoryUI.OpenAndCloseInven();
            }
            else
            {
                IngameManager.instance.uiManager.exitUI.OpenAndCloseExit();
            }
        }
    }

    // F키로 상호작용
    private void Interact()
    {
        if (Input.GetButtonDown("Interaction") && isGround)
        {
            // 휴식(세이브 포인트) 
            if (isSavepoint)
            {
                int savepointNum = scanObject.GetComponent<SavePoint>().GetSavepointNum();

                // 휴식 UI 활성화
                IngameManager.instance.uiManager.restUI.OpenMenu();

                // 현재 세이브 포인트 지점 번호 저장
                stat.lastUseSavepoint = savepointNum;

                // 체력 마나 회복
                stat.currHp = stat.maxHp;
                stat.currMp = stat.maxMp;

                // 세이브 포인트 활성화
                stat.savepoint_unlock[savepointNum] = "1";
            }
            // Npc와 대화
            else if (isNearbyNpc)
            {
                IngameManager.instance.uiManager.talkUI.ShowText(scanObject);
            }
            // Shop
            else if (isNearbyShop)
            {
                IngameManager.instance.uiManager.shopUI.OpenShop(scanObject.GetComponent<ShopNpc>().itemCode);
            }
            // 보트 타기
            else if (isNearbyBoat)
            {
                IngameManager.instance.uiManager.savepointUI.ClickTeleport(3);
            }
            // 차 타기
            else if (isNearbyCar)
            {
                IngameManager.instance.uiManager.savepointUI.ClickTeleport(8);
            }
        }
    }

    public void LevelUp()
    {
        StartCoroutine(IngameManager.instance.uiManager.levelUpUI.LevelUp());
        photonView.RPC("LevelUpParticleRPC", RpcTarget.All);
    }

    [PunRPC]
    protected void LevelUpParticleRPC()
    {
        foreach (ParticleSystem particle in levelupParticle)
        {
            particle.Play();
        }
    }
}
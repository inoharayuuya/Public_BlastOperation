using System.Collections;
using Unity.VisualScripting;
using UnityEngine;



public class HomeCharaManager : MonoBehaviour
{
    // �X�N���[���̃X�s�[�h�f�t�H���g�l�A�ŏ��l�A�����̒l
    private const float DEF_SPEED = 1.5f;
    private const float MIN_SPEED = 0.5f;
    private const float SLOW_SPEED = 0.97f;
    // startPos���X�V����Ԋu
    private const float START_POS_TIME = 0.1f;

    // ��]�����E��������������
    private const int DIR_RIGHT = -1;
    private const int DIR_LEFT = 1;

    // ���K������x���W�̒��S
    private const float NORMALIZED_X_CENTER = 0.5f;

    // �X�N���[���̃X�s�[�h
    [SerializeField] private float speed;

    // �^�b�v���̃|�W�V����
    private Vector3 startPos;
    // �w�𗣂����Ƃ��̃|�W�V����
    private Vector3 endPos;

    // �^�b�v���Ă��邩�ǂ����̃t���O
    private bool isTap;
    // �������Ă��邩�̃t���O
    private bool isMove;

    // startPos���X�V����܂ł̎���
    private float startPosTime;

    // ���΃x�N�^�[
    private float direction;


    // �L�����̃I�u�W�F�N�g
    [SerializeField] private GameObject[] charas = new GameObject[CHARA_NUMBER];

    private int dir;
    private bool isDirDecition;
    private float tmp = 300f;
    private int index;

    private readonly int[] charaAngles = { 0,120,-120};
    private bool isCenter;

    // �^�b�v�̈��G���Ă��邩�ǂ���
    private bool isTapArea;

    #region �O�o�[�W�����̉�]����

    // �z�[���ɔz�u����L�����̐�
    private const int CHARA_NUMBER = 3;

    // �^�b�v���̍��W
    public Vector3 sMousePos;
    // �^�b�v�I�����̍��W
    public Vector3 eMousePos;

    // �h���b�O���n�߂����ǂ����̃t���O
    public bool isDrag;
    // �w�������ꂽ(�X�g�b�v)���ǂ����̃t���O
    private bool isStop;

    private int objNum = 0;

    // �X���C�v�����臒l
    [SerializeField] private float tapJudge;


    /// <summary>
    /// �w�𓮂��������ǂ����𔻒肷��
    /// �n�߂Ƀ^�b�v�������W���瓮���Ă����isMove���I���ňړ�������
    /// </summary>
    private void MoveJudge()
    {
        if (isDrag)
        {
            // �X�N���[�����W���琢�E���W�ɕϊ����Ă���
            //eMousePos =Camera.main.ScreenToWorldPoint(Input.mousePosition);
            eMousePos = Input.mousePosition;

            var directionX = eMousePos.x - sMousePos.x;

            // ��]���� �E
            if (directionX > tapJudge)
            {

                isMove = true;

                //dir = LEFT;
                isDrag = false;
            }
            else if (directionX < -tapJudge)
            {

                isMove = true;

                //dir = RIGHT;
                isDrag = false;
            }
        }

        if (isStop)
        {
            if (charas[objNum].transform.position.z > -299.9f || charas[objNum].transform.position.z <= -300.0f)
            {


                Debug.Log("a");
                isMove = true;
            }
            else
            {
                isMove = false;
                isStop = false;
            }
        }
    }

    IEnumerator StartPosUpdate()
    {

        yield return new WaitForSeconds(0.5f);
        sMousePos = Input.mousePosition;
        //isMove = false;
    }

    /// <summary>
    /// �C�x���g�g���K�[��PointerDown�̎��ɌĂяo��
    /// �h���b�O�̃t���O���I���ɂ��Ĕ�����J�n
    /// </summary>
    public void PointerDown()
    {
        // �X�N���[�����W���琢�E���W�ɕϊ����Ă���
        //sMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        sMousePos = Input.mousePosition;

        //.Log("s " +  sMousePos.x);

        isDrag = true;
    }

    /// <summary>
    /// �C�x���g�g���K�[��PointerUp�̎��ɌĂяo��
    /// �t���O���I�t�ɂ��Ĉړ����~
    /// </summary>
    public void PointerUp()
    {
        isDrag = false;
        var tmp = new Vector3(0, 0, 300);

        // �w�𗣂����Ƃ��Ɉ�ԑO�ɂ���L�����𒆐S�ɐݒ�
        for (int i = 0; i < charas.Length; i++)
        {
            var vec = charas[i].transform.position;

            if (vec.z < tmp.z)
            {
                tmp.z = vec.z;
                objNum = i;
            }
        }

        isMove = false;
        isStop = true;

    }

    /// <summary>
    /// �e�̉�]��ύX�����鏈��
    /// </summary>
    private void RotationChange()
    {
        // transform���擾
        Transform myTransform = this.transform;

        // ���[���h���W����ɁA��]���擾
        Vector3 worldAngle = myTransform.eulerAngles;
        worldAngle.y += 0.1f; // ���[���h���W����ɁAy�������ɂ�����]��10�x�ɕύX
        myTransform.eulerAngles = worldAngle; // ��]�p�x��ݒ�
    }

    #endregion

    private void Awake()
    {
        // �V�[�����Ƃɏ��������Ƃ�(�ǉ�����Ȃ�)
        Application.targetFrameRate = 60;

    }

    /// <summary>
    /// �L����3�̂̍��W���v�Z���Ďw�肷�鏈��
    /// </summary>
    private void SetPosition()
    {
        // �L����3�̂̍��W���w�肷�鏈��
        for (int i = 1; i < 3; i++)
        {
            // �x���@��120�x�A240�x 90�����Ă�͎̂n�_�̒���
            var deg = (120 * i) - 90;

            // 120�x�A240�x���ʓx�@�ɕϊ�
            var rad = Mathf.Deg2Rad * deg;

            // ���S���W�Ɗ���W�̃x�N�g���擾�p
            Vector3 center = transform.position;
            Vector3 standard = charas[0].transform.position;

            // �L����2,3�̖ڂ̍��W���w��
            var pos = charas[i].transform.position;

            var cVec = center - standard;

            // ���a�擾
            var r = cVec.z;

            pos.x = center.x + r * Mathf.Cos(rad);
            pos.z = center.z + r * Mathf.Sin(rad);
            charas[i].transform.position = pos;
        }
    }

    /// <summary>
    /// ��ʂ��^�b�v���Ă���Ƃ��A�������Ƃ��̏���
    /// </summary>
    private void TapCheck()
    {
        // �^�b�v�̈��G���Ă����
        if (isTapArea)
        {
            // ��ʂ��^�b�v���ꂽ�Ƃ�
            if (Input.GetMouseButtonDown(0))
            {
                // startPos�ɉ�ʂ������ꂽ���W��������
                startPos = Input.mousePosition;

                // ��ʂ�������Ă���̂�true�ɂ���
                isTap = true;

                // ������悤�ɂȂ�̂�true
                isMove = true;
            }

            // ��ʂ��^�b�v����Ă����
            if (Input.GetMouseButton(0))
            {
                // endPos�ɂ͖��t���[����ʂ�������Ă�����W��������
                endPos = Input.mousePosition;

                // �X�V���Ԃ�����܂�deltaTime�Ōv�Z
                startPosTime -= Time.deltaTime;

                // �X�s�[�h�͉�����邽�тɏ���������
                speed = DEF_SPEED;

                isCenter = false;

                // startPosTime��0�ɂȂ邽�т�startPos�����݂�endPos�ŏ�����
                if (startPosTime <= 0)
                {
                    // startPosTime�̏�����
                    startPosTime = START_POS_TIME;
                    startPos = endPos;
                }
            }
        }

        // ��ʂ���w�������ꂽ�Ƃ�
        if (Input.GetMouseButtonUp(0))
        {
            // �w�������ꂽ�̂�false
            isTap = false;
            isTapArea = false;
        }
    }

    /// <summary>
    /// �L�����𓮂�������
    /// </summary>
    private void CharaMove()
    {
        // ��ʂ�������Ă��Ȃ��Ԃ͌�����������
        if (!isTap && !isCenter)
        {
            // ����
            speed *= SLOW_SPEED;

            // �~�߂鏈��
            if (speed < MIN_SPEED)
            {
                // �ʏ�̈ړ������������̂�false
                isMove = false;


                // �����ɒ��S�ɂ��鏈��
                // ��Ԑ��ʂɋ߂��I�u�W�F�N�g��T��
                for (int i = 0; i < charas.Length; i++)
                {
                    if (charas[i].transform.position.z < tmp)
                    {
                        tmp = charas[i].transform.position.z;
                        index = i;
                    }

                    Debug.Log("�i���o�[�@" + index);
                }

                // ��Ԑ��ʂɋ߂������I�u�W�F�N�g�́A���K������z���W���擾
                var obj = charas[index].GetComponent<HomeCharaMove>();

                // ���K���������W�擾
                var normalizedZ = obj.GetNormalizedZ(charas[index]);
                var normalizedX = obj.GetNormalizedX(charas[index]);

                // ���ʂ̊p�x��ݒ�
                var frontAngle = 0;

                //Debug.Log("norX " + normalizedX);

                // ��x��������
                //if (speed == 0)
                // ��]��������(��x����)
                if (!isDirDecition)
                {
                    // ��]�̕���������
                    if (normalizedX > NORMALIZED_X_CENTER)
                    {
                        dir = DIR_LEFT;
                    }
                    else
                    {
                        dir = DIR_RIGHT;
                    }
                    
                    isDirDecition = true;

                }

                // transform���擾
                Transform myTransform = this.transform;
                // ���[���h���W����ɁA��]���擾
                Vector3 worldAngle = myTransform.eulerAngles;

                // TODO �����̒l������������Ȃ���
                // ���ʂɗ���܂ňړ�������
                if (dir == DIR_LEFT)
                {
                    if ( normalizedX > NORMALIZED_X_CENTER && !isCenter)
                    {
                        // y�������ɂ�����]��ύX
                        worldAngle.y += (100f * dir) * Time.deltaTime;
                    }
                    else
                    {
                        isCenter = true;
                        speed = 0;
                        worldAngle.y = charaAngles[index];
                        myTransform.eulerAngles = worldAngle;
                    }
                }
                else
                {
                    if ( normalizedX < NORMALIZED_X_CENTER)
                    {
                        // y�������ɂ�����]��ύX
                        worldAngle.y += (100f * dir) * Time.deltaTime;

                    }
                    else
                    {
                        isCenter = true;
                        speed = 0;
                        worldAngle.y = charaAngles[index];
                        myTransform.eulerAngles = worldAngle;
                    }
                }

                // ��]�p�x��ݒ�
                myTransform.eulerAngles = worldAngle;

            }
            else if(speed == 0)
            {
                // transform���擾
                Transform myTransform = this.transform;
                // ���[���h���W����ɁA��]���擾
                Vector3 worldAngle = myTransform.eulerAngles;

                
            }
        }
        else
        {
            tmp = 300;
            isDirDecition = false;
            
        }

        // startPos��endPos���v�Z���ċ��������߂�
        direction = (endPos.x - startPos.x) * speed;

        if (isMove)
        {
            // transform���擾
            Transform myTransform = this.transform;
            // ���[���h���W����ɁA��]���擾
            Vector3 worldAngle = myTransform.eulerAngles;

            // �v�Z���ʂ��v���X�̏ꍇ(0���܂܂Ȃ�)
            if (direction > 0)
            {
                // y�������ɂ�����]��ύX
                worldAngle.y += (-direction * speed) * Time.deltaTime;

                //dir = DIR_RIGHT;
            }
            else
            {
                // y�������ɂ�����]��ύX
                worldAngle.y += (-direction * speed) * Time.deltaTime;
                //dir = DIR_LEFT;
            }

            // ��]�p�x��ݒ�
            myTransform.eulerAngles = worldAngle;

        }

    }

    /// <summary>
    /// �^�b�v�̈��G���Ă����Ԃ̎��Ƀt���O���I��
    /// </summary>
    public void TapArea()
    {
        isTapArea = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = DEF_SPEED;

        // �L����3�̂̍��W���w�肷�鏈��
        SetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        // ��Ԑ��ʂɋ߂������I�u�W�F�N�g�́A���K������z���W���擾
        var obj = charas[0].GetComponent<HomeCharaMove>();

        // ���K���������W�擾
        var normalizedX = obj.GetNormalizedX(charas[0]);


        // �^�b�v���Ă��邩�A������Ă��邩����
        TapCheck();

        // �L�����𓮂�������
        CharaMove();

    }
}

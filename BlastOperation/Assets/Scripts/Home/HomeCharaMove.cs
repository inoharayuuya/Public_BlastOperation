using Const;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HomeCharaMove : MonoBehaviour
{
    // z���W�̍ŏ��l�A�ő�l
    private const float MIN_POS_Z = 300;
    private const float MAX_POS_Z = -300;
    // x���W�̍ő�l�A�ŏ��l
    private const float MIN_POS_X = 60;
    private const float MAX_POS_X = 660;
    // �Â��̍ő�l
    private const float MAX_DARK = 0.5f;

    // �F�ύX�p
    private float col;

    #region �O�o�[�W�����̏���

    // ��]�̃N�H�[�^�j�I��
    Quaternion angleAxis;

    // �~�^���̒��S�_
    [SerializeField] private Vector3 center;
    // ��]��
    [SerializeField] private Vector3 axis = Vector3.up;
    // ���]�܂ł̉~�^������
    [SerializeField] private float period;

    // ��x�������s�p
    bool isOnce;

    // �}�l�[�W���[�擾
    HomeCharaManager hm;

    /// <summary>
    /// �摜�̈ړ�����
    /// </summary>
    private void Move()
    {
        // ���݂̃|�W�V����
        /*var pos = transform.position;

        // z���W�𐳋K��
        normalizedZ = Common.NormalizedFunc(pos.z, MIN_POS_Z, MAX_POS_Z, 0, 1.0f);

        // �t���O���I���̎��Ɉړ�
        if (hm.isMove)
        {
            // �~�^���̈ʒu�v�Z
            pos -= center;
            pos = angleAxis * pos;
            pos += center;
        }

        transform.position = pos;

        // ��]�̃N�H�[�^�j�I���쐬
        angleAxis = Quaternion.AngleAxis(((360 / period) * hm.dir) * Time.deltaTime, axis);
        //angleAxis = Quaternion.AngleAxis(((360 * hm.dir) / period) * Time.deltaTime, axis);
        */
    }

    #endregion

    /// <summary>
    /// ����������
    /// </summary>
    private void Init()
    {
        // �J���[�͍ő�l(��)
        col = 1f;
    }

    /// <summary>
    /// �L�����摜�̖��邳��z���W�ɂ��ύX���鏈��
    /// </summary>
    private void ChangeColor()
    {
        // �J���[��z���W�ɂ���ĕύX
        col = GetNormalizedZ(this.gameObject);

        // �Â������ɒB���Ă���΂���ȏ�Â����Ȃ�
        if (col < MAX_DARK)
        {
            col = MAX_DARK;
        }

        // �C���[�W�R���|�[�l���g�̃J���[�擾�A�J���[����
        this.gameObject.GetComponent<Image>().color = new Color(col, col, col);
    }
    /// <summary>
    /// �摜�̉�]�͂����A��ɐ��ʂ��������鏈��
    /// </summary>
    private void MatchTheAngle()
    {
        // transform���擾
        Transform myTransform = this.transform;

        // ���[���h���W����ɁA��]���擾
        Vector3 worldAngle = myTransform.eulerAngles;

        // y���̉�]��0�ŌŒ�(���ʂ�����)
        worldAngle.y = 0;

        // ��]�p�x��ݒ�
        myTransform.eulerAngles = worldAngle;
    }

    /// <summary>
    /// ���K������z���W�̒l��Ԃ�����
    /// </summary>
    /// <returns>���K������z���W</returns>
    public float GetNormalizedZ(GameObject _obj)
    {
        // �x�N�^�[�擾
        var pos = _obj.transform.position;
        //Debug.Log("posx = " + pos.x);

        // z���W�𐳋K��
        var normalizedZ = Common.NormalizedFunc(pos.z, MIN_POS_Z, MAX_POS_Z, 0, 1.0f);

        // ���K������z���W��Ԃ�
        return normalizedZ;
    }

    /// <summary>
    /// ���K������x���W�̒l��Ԃ�����
    /// </summary>
    /// <returns>���K������z���W</returns>
    public float GetNormalizedX(GameObject _obj)
    {
        // �x�N�^�[�擾
        var pos = _obj.transform.position;

        // z���W�𐳋K��
        var normalizedX = Common.NormalizedFunc(pos.x, MIN_POS_X, MAX_POS_X, 0, 1.0f);

        // ���K������z���W��Ԃ�
        return normalizedX;
    }

    // Start is called before the first frame update
    void Start()
    {
        // ����������
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        var normalizedX = GetNormalizedX(this.gameObject);
        //.Log("norX " + normalizedX);


        // �p�x�ύX����
        MatchTheAngle();

        // �F�̕ύX����
        ChangeColor();
    }
}

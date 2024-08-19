using Const;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // �ȉ~�^�̒e


    [Tooltip("�ړ��x�N�g��")]
    private Vector3 moveVec;
    [Tooltip("�ړ����x")]
    private float moveSpeed;
    [Tooltip("�����̃T�C�Y���"), SerializeField]
    private Vector3 size;
    [Tooltip("�O�[�̉~")]
    private Transform forCirTr;
    [Tooltip("��[�̉~")]
    private Transform behCirTr;

    [Tooltip("�v���C���[�̓����蔻������I�u�W�F�N�g")]
    private Player player;

    [Tooltip("�U����")]
    private float atk;
    [Tooltip("�U������")]
    private string attr;

    [Tooltip("��ʉE��̍��W")]
    private Vector2 windowTopRightPos;
    [Tooltip("��ʍ����̍��W")]
    private Vector2 windowBottomLeftPos;

    /// <summary>
    /// 
    /// </summary>
    public void Main()
    {

    }

    /// <summary>
    /// ����������
    /// </summary>
    public void Init(Vector3 _vec, float _ang, float _speed, float _atk, string _attr)
    {
        // �����̎󂯎��
        //moveVec =  Quaternion.Euler(0, 0, _ang) * _vec.normalized;
        moveVec = _vec;
        moveSpeed = _speed;
        atk = _atk;
        attr = _attr;
        //print("�󂯎�����A���O���F" + _ang);
        transform.rotation = Quaternion.Euler(0, 0, _ang);

        // �e�ϐ��̏�����
        player = GameObject.Find("PlayerBall").GetComponent<Player>();
        size = transform.GetChild(0).localScale * transform.localScale.x;
        forCirTr = transform.GetChild(1).GetChild(0);
        behCirTr = transform.GetChild(1).GetChild(1);

        // ��ʃT�C�Y�̎擾
        windowBottomLeftPos = Camera.main.ViewportToWorldPoint(Vector2.zero);
        windowTopRightPos = Camera.main.ViewportToWorldPoint(Vector2.one);
    }

    // Update is called once per frame
    protected void Update()
    {
        // �ړ�����
        Move();

        // �����蔻�菈��
        if (Collision())
        {
            player.Damage(atk, attr);
            Destroy(gameObject);
            return;
        }

        // ��ʓ�����
        JudgeDisplay();

    }

    /// <summary>
    /// �ړ������A�Ǐ]�e���h���N���X�ł��邽�߁A���z�֐�
    /// </summary>
    protected virtual void Move()
    {
        transform.position += moveVec * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �e�̓����蔻���
    /// <para>���Ƃ̋���+�~��őȉ~���ߎ�</para>
    /// <para>1.�S�̂̉~�ŉ��ߎ��A�����蔻��</para>
    /// <para>2.�~�̂ǂ��炩�Ɠ������Ă��邩</para>
    /// <para>3.���Ɠ����蔻��</para>
    /// </summary>
    protected bool Collision()
    {

        // ���̓����蔻��
        var dummyDif = ((Vector2)(player.transform.position - transform.position)).magnitude;
        //print("dif:" + dif);
        // ��܂��ȓ����蔻��ŏ������ȗ�
        if (dummyDif < size.y / 2 + Common.SIZE_PLAYER / 2)
        {
            // ������\������
            //print("�߂�");
            // �e�̒����ɓ������Ă��邩
            var for2Player = player.transform.position - forCirTr.position;
            var beh2Player = player.transform.position - behCirTr.position;


            var for2beh = forCirTr.position - behCirTr.position;

            // �O�όv�Z�ɕK�v�ȃx�N�g�����Z�o
            var cross = Mathf.Abs(Vector3.Cross(for2Player, for2beh.normalized).z);
            // �e�̒��S�̐�����v���C���[�̋���
            if (cross < Common.SIZE_PLAYER / 2)
            {
                //print("�����Ƀq�b�g");
                return true;
            }



            // �e�̑O�[����[�ɓ������Ă��邩�ǂ���
            var forDif = for2Player.magnitude;
            var behDif = beh2Player.magnitude;

            if (
                forDif < forCirTr.transform.localScale.x / 2 * transform.localScale.x + Common.SIZE_PLAYER / 2 ||
                behDif < behCirTr.transform.localScale.x / 2 * transform.localScale.y + Common.SIZE_PLAYER / 2
                )
            {
                print("�O����Ƀq�b�g");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ��ʓ��ɂ��邩�ǂ�������
    /// </summary>
    private void JudgeDisplay()
    {
        if(
            transform.position.y < windowBottomLeftPos.y ||
            transform.position.x < windowBottomLeftPos.x ||
            transform.position.y > windowTopRightPos.y   ||
            transform.position.x > windowTopRightPos.x
            )
        {
            Destroy(gameObject);
        }
    }
}

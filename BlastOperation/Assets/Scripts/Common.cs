using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Const
{
    #region �񋓑�
    /// <summary>
    /// �U���p�^�[���̗�
    /// </summary>
    public enum EnemyAttackPattern
    {
        BULLET_WATER_SPREAD = 1,
        SLASH_AREA,
        KNIFE_THROW,
        BREATH_FIRE_SMALL,
        TALE_AREA_SMALL,
        BREATH_FIRE_SWEEP_BIG,
        BREATH_FIRE_AROUND_BIG,
        TALE_AREA_BIG,
        HEAL_SINGLE,
        HEAL_MULTI,
        BULLET_MAGIC_FOLLOW
    }

    #endregion

    #region �\����

    /// <summary>
    /// �G�������ɕK�v�ȏ��̍\���́A��������Init�̈����œn��
    /// </summary>
    public struct EnemyInitDatas
    {
        /// <summary>�̗�</summary>
        public int hp;
        /// <summary>�U����</summary>
        public float atk;
        /// <summary>�U���p�^�[��</summary>
        public int[] atk_ptns;
        /// <summary>�o���ʒu</summary>
        public Vector3 pos;
        /// <summary>�傫��</summary>
        public float size;
        /// <summary>��_</summary>
        public string wk;
        /// <summary>�{�X�t���O</summary>
        public bool isB;
        /// <summary>�摜�̃p�X</summary>
        public string path;
    }

    /// <summary>
    /// �U���A�_���[�W����Ɏg�����̍\���́A�_���[�W�C���^�[�t�F�[�X�̈����œn��
    /// </summary>
    public struct CharaData
    {
        public int atk;
        public string attr;
        public float speed;
    }

    #endregion


    /// <summary>
    /// �ėp�N���X
    /// </summary>
    public class Common
    {
    	// �{�[���̑�����L�����Z������Ƃ��̒���
        public const float TAP_VECTOR_LIMIT = 0.5f;
        // �Ґ��ɓ������L�����N�^�[�̍ő吔
        public const int PARTY_LIMIT = 4;
        // �V�[�����萔
        public const string SCENE_NAME_TITLE = "Title";
        public const string SCENE_NAME_QUEST = "Quest";
        public const string SCENE_NAME_MAIN = "Main";

        // �U������
        public const string ATTR_PHYSICAL = "����";
        public const string ATTR_MAGICAL = "���@";
        public const string ATTR_HEAL = "��";
        public const float ATTR_WEAK_RATE = 2f;

        // �P�L�����̕���̐�
        public const int WEAPON_LIMIT = 2;
        public const int MAX_CHARA_NUMBER = 10;
        public const int MAX_HAS_CHARA_NUMBER = 30;

        // �K�`��
        public const int MAX_GACHA_CHARA_NUMBER = 10;

        // �X�e�[�W��
        public const int STAGE_NUMBER = 4;
        // �Ґ���
        public const int ORG_NUMBER = 2;

        // �v���C���[�̃{�[���̃T�C�Y�i�j
        public const float SIZE_PLAYER = 0.65f;

        // API�ʐM�֘A
        public static string apiUrl = "";  // API�̃����N

        public const string SUCCES = "OK";
        public const string FAILED = "NG";

        public const string EP_CREATE_ACCOUNT   = "create_account";    // �A�J�E���g�V�K�o�^API�̃G���h�|�C���g
        public const string EP_LOGIN            = "login";             // ���O�C��API�̃G���h�|�C���g
        public const string EP_LOGOUT           = "logout";            // ���O�A�E�gAPI�̃G���h�|�C���g
        public const string EP_GET_MAIN_DATA    = "get_main_data";     // ���C���f�[�^�擾API�̃G���h�|�C���g
        public const string EP_SET_ACCOUNT_INFO = "set_account_info";  // �A�J�E���g���o�^API�̃G���h�|�C���g
        public const string EP_SET_IN_GAME      = "set_in_game";       // �Q�[�����f�[�^�ۑ�API�̃G���h�|�C���g
        public const string EP_GACHA            = "gacha";             // �K�`�����sAPI�̃G���h�|�C���g
        public const string EP_CHARA            = "get_character_info";// �L�������擾API�̃G���h�|�C���g
        public const string EP_GET_STAGE        = "get_stage_info";    //�X�e�[�W���̎擾 
        public const string EP_RESULT           = "result";            // ���U���g�̒ʐM


        public const string KEY_ACCOUNT_NAME = "account_name";  // �A�J�E���g�����i�[���Ă���Prefs�̃L�[��
        public const string KEY_USER_ID      = "user_id";       // ���[�U�[ID���i�[���Ă���Prefs�̃L�[��
        public const string KEY_STAGE_ID     = "stage_id";       // ���[�U�[ID���i�[���Ă���Prefs�̃L�[��
        public const string KEY_PARTY_ID     = "party_id";       // ���[�U�[ID���i�[���Ă���Prefs�̃L�[��

        
        public static void LoadScene(string _sceneName)
        {
            SceneManager.LoadScene(_sceneName);
        }


        /// <summary>
        /// �w��t���[����Ɏw��̊֐����Ăяo��
        /// </summary>
        /// <param name="_delayFrame">�x���t���[����</param>
        /// <param name="_action">�Ăяo���֐�</param>
        /// <returns></returns>
        public static IEnumerator Delay(int _delayFrame, Action _action)
        {
            for (var i = 0; i < _delayFrame; i++)
            {
                yield return null;
            }
            _action();
        }

        /// <summary>
        /// �w��b��Ɏw��̊֐����Ăяo��
        /// </summary>
        /// <param name="_waitTime">�x���b��</param>
        /// <param name="_action">�Ăяo���֐�</param>
        /// <returns></returns>
        public static IEnumerator Delay(float _waitTime, Action _action)
        {
            yield return new WaitForSeconds(_waitTime);
            _action();
        }


        /// <summary>
        /// 0�`1�ɐ��K�����ꂽ�l(_val)��_min�`_max�ɐ��K�����ĕԂ��֐�
        /// </summary>
        /// <param name="_val">0�`1�ɐ��K�����ꂽ�l</param>
        /// <param name="_min">���K������l�̍ŏ��l</param>
        /// <param name="_max">���K������l�̍ő�l</param>
        /// <returns></returns>
        public static float NormalizedFunc(float _val, float _min, float _max)
        {
            return _val * (_max - _min) + _min;
        }

        /// <summary>
        /// _min1�`_max1�ɐ��K�����ꂽ�l(_val)��_min2�`_max2�ɐ��K�����ĕԂ��֐�
        /// </summary>
        /// <param name="_val">���K���������l</param>
        /// <param name="_min1">_val���Ƃ�l�͈̔͂̍ŏ��l</param>
        /// <param name="_max1">_val���Ƃ�l�͈̔͂̍ő�l</param>
        /// <param name="_min2">���K������l�̍ŏ��l</param>
        /// <param name="_max2">���K������l�̍ő�l</param>
        /// <returns></returns>
        public static float NormalizedFunc(float _val, float _min1, float _max1, float _min2, float _max2)
        {
            // _val��0�`1�Ő��K��
            _val = (_val - _min1) / (_max1 - _min1);
            //_min2�`_max2�Ő��K�����ꂽ�l��Ԃ�
            return NormalizedFunc(_val, _min2, _max2);
        }

        /// <summary>
        /// �o���f�[�V����
        /// ���͂��ꂽ�����̃`�F�b�N�ƕϊ�
        /// ���{��Ɖp�����ȊO���͂ł��Ȃ�
        /// </summary>
        public static char OnValidateInput(string text, int index, char addedChar)
        {
            //�u���v�`�u���v�܂łƁA�u�[�v�u�_�u���n�C�t���v���Ђ炪�ȂƂ���
            //�u�_�u���n�C�t���v����u�R�g�v�܂łƁA�J�^�J�i�t���K�i�g���ƁA
            //���_�Ɣ����_��S�p�J�^�J�i�Ƃ���
            //���_�ƒ����L�����܂�
            //CJK���������ACJK�݊������ACJK���������g��A�͈̔͂ɂ��邩���ׂ�
            if (
                '\u3041' <= addedChar && addedChar <= '\u309F' ||
                '\u30A0' <= addedChar && addedChar <= '\u30FF' ||
                '\u31F0' <= addedChar && addedChar <= '\u31FF' ||
                '\u3099' <= addedChar && addedChar <= '\u309C' ||
                '\u4E00' <= addedChar && addedChar <= '\u9FCF' ||
                '\uF900' <= addedChar && addedChar <= '\uFAFF' ||
                '\u3400' <= addedChar && addedChar <= '\u4DBF' ||
                addedChar == '\u30FC' || addedChar == '\u30A0'
               )
            {
                // ���{��̏ꍇ�A���͂��ꂽ���������̂܂ܕԂ�
                return addedChar;
            }
            else
            {
                // �A���t�@�x�b�g�܂��͐����łȂ���Γ��͂��Ȃ�
                if (char.IsLetterOrDigit(addedChar))
                {
                    // �p�����̏ꍇ�A���͂��ꂽ���������̂܂ܕԂ�
                    return addedChar;
                }
                else
                {
                    // ����ȊO�̏ꍇ�ANULL��Ԃ�
                    return '\0';
                }
            }
        }
    }

    /// <summary>
    /// �ʐM����
    /// </summary>
    public static class HTTPManager
    {
        /// <summary>
        /// GET�ʐM����
        /// </summary>
        public static IEnumerator GetCommunication(string url)
        {
            using (var req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();
                switch (req.result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log("�ʐM����");
                        yield return req.downloadHandler.text;
                        break;

                    case UnityWebRequest.Result.ConnectionError:
                        Debug.Log("�ʐM�G���[");
                        yield return req.downloadHandler.text;
                        break;

                    case UnityWebRequest.Result.ProtocolError:
                        Debug.Log("�v���g�R���G���[");
                        Debug.Log(req.error);
                        yield return req.downloadHandler.text;
                        break;

                    default:
                        Debug.Log("�G���[");
                        Debug.Log(req.error);
                        break;
                }
            }
        }

        /// <summary>
        /// POST�ʐM����
        /// </summary>
        public static IEnumerator PostCommunication(string url, WWWForm form)
        {
            using (var req = UnityWebRequest.Post(url, form))
            {
                yield return req.SendWebRequest();
                switch (req.result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log("�ʐM����");
                        yield return req.downloadHandler.text;
                        break;

                    case UnityWebRequest.Result.ConnectionError:
                        Debug.Log("�ʐM�G���[");
                        yield return req.downloadHandler.text;
                        break;

                    case UnityWebRequest.Result.ProtocolError:
                        Debug.Log("�v���g�R���G���[");
                        Debug.Log(req.error);
                        yield return req.downloadHandler.text;
                        break;

                    default:
                        Debug.Log("�G���[");
                        Debug.Log(req.error);
                        break;
                }
            }
        }
    }
}




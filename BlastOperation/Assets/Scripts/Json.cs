using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using Const;

namespace Json
{
    #region �A�J�E���g�쐬API�p
    /// <summary>
    /// �A�J�E���g�쐬API�̃��X�|���X����p
    /// </summary>
    [Serializable]
    public class JsonCreateAccunt
    {
        public string result;    // �ʐM����
        public string message;   // �G���[���o�����Ȃǂ̃��b�Z�[�W
        public string password;  // �p�X���[�h
    }
    #endregion

    #region ���O�C��API�p
    /// <summary>
    /// ���O�C��API�̃��X�|���X����p
    /// </summary>
    [Serializable]
    public class JsonLogin
    {
        public string result;   // �ʐM����
        public string message;  // ���b�Z�[�W
        public string user_id;  // ���[�U�[ID
        public string rank;     // �����N
        public int    icon_id;  // �A�C�R��ID
    }
    #endregion

    #region ���O�A�E�gAPI�p
    /// <summary>
    /// ���O�A�E�gAPI�̃��X�|���X����p
    /// </summary>
    [Serializable]
    public class JsonLogout
    {
        public string result;   // �ʐM����
        public string message;  // ���b�Z�[�W
        public string user_id;  // ���[�U�[ID
    }
    #endregion

    #region ���C���f�[�^�擾API�p
    /// <summary>
    /// ���C���f�[�^
    /// </summary>
    [Serializable]
    public class JsonMainData
    {
        public string result;
        public JsonPlayerInfo player_info;
        public JsonCharacterMaster[] character_masters;
        public JsonStageMaster[] stage_masters;

        public JsonMainData()
        {
            player_info = new JsonPlayerInfo();

            character_masters = new JsonCharacterMaster[Common.PARTY_LIMIT];
            stage_masters = new JsonStageMaster[Common.STAGE_NUMBER];
        }

    }

    /// <summary>
    /// �v���C���[���
    /// </summary>
    [Serializable]
    public class JsonPlayerInfo
    {
        public string name;
        public string rank;
        public int energy;
        public int crystal;
        public int coin;
        public int total_rank_point;
        public int next_rank_point;
        public string comment;
        public int icon_id;
        public int party_id;
        public int[] has_character_ids;
        public JsonPartyInfo[] party_infos;
        public JsonProgressRate[] progress_rates;

        public JsonPlayerInfo()
        {
            has_character_ids = new int[Common.MAX_HAS_CHARA_NUMBER];
            party_infos = new JsonPartyInfo[Common.ORG_NUMBER];
            progress_rates = new JsonProgressRate[Common.STAGE_NUMBER];
        }
    }

    /// <summary>
    /// �Ґ���̏��.�e�v���C���[�͕Ґ���2�܂Ŏ��Ă�
    /// </summary>
    [Serializable]
    public class JsonPartyInfo
    {
        public int[] box_ids;

        public JsonPartyInfo()
        {
            box_ids = new int[Common.PARTY_LIMIT];
        }
    }

    /// <summary>
    /// �v���C���[�̃Q�[���i�s�x
    /// </summary>
    [Serializable]
    public class JsonProgressRate
    {
        public int stage_id;
        public bool is_clear;
        public bool is_order;
    }

    #endregion

    #region �K�`���f�[�^

    [Serializable]
    public class JsonGachaData
    {
        // ���U���g��OK�̎��̂ݏ����i�߂�
        public string result;
        public int[] character_ids;
        public JsonPlayerInfo player_info;
        public JsonCharacterMaster[] character_masters;
        public JsonWeaponMaster[] weapon_masters;

        public JsonGachaData()
        {
            player_info = new JsonPlayerInfo();

            character_ids = new int[Common.MAX_GACHA_CHARA_NUMBER];
            character_masters = new JsonCharacterMaster[Common.MAX_CHARA_NUMBER];
            weapon_masters = new JsonWeaponMaster[Common.MAX_CHARA_NUMBER * 2];  // ����̓L����1�̂ɂ�2�Ȃ̂�20
        }
    }


    #endregion

    #region �L�����ꗗ�f�[�^

    [Serializable]
    public class JsonCharaData
    {
        public string result;
        public string message;
        public int[] character_ids;
        public JsonCharacterMaster[] character_masters;
        public JsonWeaponMaster[] weapon_masters;

        JsonCharaData()
        {
            character_ids = new int[Common.MAX_HAS_CHARA_NUMBER];
            character_masters = new JsonCharacterMaster[Common.MAX_HAS_CHARA_NUMBER];
            weapon_masters = new JsonWeaponMaster[Common.MAX_HAS_CHARA_NUMBER * 2];
        }
    }

    #endregion

    #region �N�G�X�g�f�[�^

    [Serializable]
    public class JsonQuestData
    {
        public string result;
        public string message;
        public JsonStageMaster stage_master;
        public JsonEnemyMaster[] enemy_masters;
        public JsonEnemyAttackMaster[] enemy_atk_masters;
        public JsonCharacterMaster[] character_masters;
        public JsonWeaponMaster[] weapon_masters;

        public JsonQuestData()
        {
            stage_master = new JsonStageMaster();
            enemy_masters = new JsonEnemyMaster[6];
            enemy_atk_masters = new JsonEnemyAttackMaster[6];
            character_masters = new JsonCharacterMaster[4];
            weapon_masters = new JsonWeaponMaster[8];
        }
    }

    #endregion

    #region ���U���g�f�[�^

    [Serializable]
    public class JsonResultData
    {
        public string result;
        public string message;
        public int[] limit_rank_points;
        public JsonPlayerInfo player_info;

        public JsonResultData()
        {
            limit_rank_points = new int[7];
            player_info = new JsonPlayerInfo();
        }
    }

    #endregion


    #region �}�X�^�[�f�[�^

    [Serializable]
    public class JsonCharacterMaster
    {
        public int id;
        public string rank;
        public string name;
        public string name_id;
        public int hp;
        public float atk;
        public float speed;
        public float physical_def;
        public float magical_def;
        public int[] weapon_ids;


        public JsonCharacterMaster()
        {
            weapon_ids = new int[Common.WEAPON_LIMIT];
        }
    }


    [Serializable]
    public class JsonEnemyMaster
    {
        public int id;
        public string name;
        public string name_id;
        public float hp;
        public float atk;
        public float def;
        public int[] atk_pattern_ids;
        public string weak;
        public JsonEnemyMaster()
        {
            atk_pattern_ids = new int[6];
        }
    }



    [Serializable]
    public class JsonStageMaster
    {
        public int id;
        public string name;
        public string note;
        public int use_energy;
        public int coin;
        public int rank_point;
        public int crystal;
        public JsonEnemyInfo[] enemy_infos;
        public int max_floor;
    }

    [Serializable]
    public class JsonEnemyInfo
    {
        public int id;
        public Vector3 position;
        public int floor;
        public bool is_boss;
        public float atk_rate;
        public float hp_rate;
        public float size;
    }

    [Serializable]
    public class JsonEnemyAttackMaster
    {
        public int atk_id;
        public int atk_area_id;
        public float atk_rate;
        public string atk_attr;
    }



    [Serializable]
    public class JsonWeaponMaster
    {
        public int id;
        public string name;
        public string name_id;
        public string attr;
        public float atk_rate;
        public float speed_rate;
        public string range;
        public int special_effect_id;

    }

    #endregion



}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// �v���C���[����G�ւ̃_���[�W����
    /// </summary>
    /// <param name="charaData"></param>
    void Damage(float _atk, string _attr);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// プレイヤーから敵へのダメージ処理
    /// </summary>
    /// <param name="charaData"></param>
    void Damage(float _atk, string _attr);
}

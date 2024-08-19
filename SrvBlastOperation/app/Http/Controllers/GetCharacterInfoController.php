<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Account;
use App\Models\CharacterMaster;
use App\Models\Member;
use App\Models\ProgressRate;
use App\Models\HasCharacter;
use App\Models\PartyInfo;
use App\Models\WeaponMaster;
use App\Models\EnemyMaster;
use App\Models\StageMaster;
use App\Models\StageInEnemy;
use App\Models\EnemyAtkMaster;
use App\Models\EnemyAtkPatternMaster;

class GetCharacterInfoController extends Controller
{
    // Post通信処理
    public function store(Request $request)
    {
        $account = null;
        try{
            // 同じIDのデータがあるか確認
            $account = Account::where('user_id', $request->user_id)->first();
        }
        catch (Exception $ex) {
            // エラーの場合
            return response()->json([
                'result'  => 'NG',
                'message' => $ex->getMessage(),
            ],400);
        }

        if ($account == null)
        {
            // 同じuser_idのデータが無かった場合
            return response()->json([
                'result' => 'NG',
            ],400);
        }
        else
        {
            // 所持キャラクターの情報を取得
            try{
                $tmp = json_decode($request->player_info);
                foreach ($tmp->has_character_ids as $key => $has_character_id)
                {
                    $has_characters[] = HasCharacter::where('id', $has_character_id)->first();
                    $character_ids[] = $has_characters[$key]['character_id'];
                }
            }
            catch (Exception $ex) {
                // エラーの場合
                return response()->json([
                    'result'  => 'NG',
                    'message' => $ex->getMessage(),
                ],400);
            }

            $character_masters = $this->GetCharacterMaster($account, $character_ids);

            foreach ($character_masters as $key => $character_master)
            {
                // $res_character_ids[] = $character_master['id'];
                foreach ($character_master['weapon_ids'] as $key => $weapon_id)
                {
                    $res_weapon_ids[] = $weapon_id;
                }
            }

            // $character_ids = array_unique($res_character_ids);

            // 被ったweapon_idを取り除く
            $weapon_ids = array_unique($res_weapon_ids);

            //パーティーに編成しているキャラクターの武器分ループ(被りなし)
            foreach ($weapon_ids as $key => $weapon_id)
            {
                try{
                    // weapon_idが同じデータを取り出す
                    $tmp_weapon_master = WeaponMaster::where('id', $weapon_id)->first();
                }
                catch (Exception $ex) {
                    // エラーの場合
                    return response()->json([
                        'result'  => 'NG',
                        'message' => $ex->getMessage(),
                    ],400);
                }

                // weapon_mastersの組み立て
                $weapon_masters[] = [
                    'id'                => $tmp_weapon_master->id,
                    'name'              => $tmp_weapon_master->name,
                    'name_id'           => $tmp_weapon_master->name_id,
                    'attr'              => $tmp_weapon_master->attr,
                    'atk_rate'          => $tmp_weapon_master->atk_rate,
                    'speed_rate'        => $tmp_weapon_master->speed_rate,
                    'range'             => $tmp_weapon_master->range,
                    'special_effect_id' => $tmp_weapon_master->special_effect_id,
                ];
            }

            // レスポンス
            return response()->json([
                'result'            => 'OK',
                'character_ids'     => $character_ids,
                'character_masters' => $character_masters,
                'weapon_masters'    => $weapon_masters,
                // 'test' => $character_ids,
            ],200);
        }
    }

    // 所持キャラクターのステータス情報取得
    public function GetCharacterMaster($account, $character_ids)
    {
        // 被ったcharacter_idを取り除く
        $character_ids = array_unique($character_ids);

        foreach ($character_ids as $key => $character_id)
        {
            // character_mastersテーブルの情報取得
            $has_character_masters[] = CharacterMaster::where('id', $character_id)->first();
        }

        // レスポンス用に所持キャラクターのステータス情報を加工
        foreach ($has_character_masters as $key => $has_character_master)
        {
            // 一番目の武器IDと二番目の武器IDを配列に格納
            $weapon_ids = [
                $has_character_master->first_weapon_id,
                $has_character_master->second_weapon_id,
            ];

            // レスポンスで使うデータを組み合わせる
            $characters[] = [
                'id'           => $has_character_master->id,
                'rank'         => $has_character_master->rank,
                'name'         => $has_character_master->name,
                'name_id'      => $has_character_master->name_id,
                'hp'           => $has_character_master->hp,
                'atk'          => $has_character_master->atk,
                'speed'        => $has_character_master->speed,
                'physical_def' => $has_character_master->physical_def,
                'magical_def'  => $has_character_master->magical_def,
                'weapon_ids'   => $weapon_ids,
            ];
        }

        return $characters;
    }
}

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

class GetStageInfoController extends Controller
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
            $tmp_stage_master  = null;
            $tmp_stage_enemies = null;
            try{
                // ステージのマスターデータを取得
                $tmp_stage_master = StageMaster::where('id', $request->stage_id)->first();
                // 取得したデータが空の場合はエラーとみなす
                if (empty($tmp_stage_master))
                {
                    return response()->json([
                        'result'  => 'NG',
                        'message' => '取得したデータが空です。stage_idが間違っている可能性があります。',
                    ],400);
                }

                $index = StageInEnemy::where('stage_id', $tmp_stage_master->id)->count();
                // 取得したデータが空の場合はエラーとみなす
                if (empty($index))
                {
                    return response()->json([
                        'result'  => 'NG',
                        'message' => '取得したデータが空です。データベースにマスターデータが登録されていない可能性があります。',
                    ],400);
                }
                $tmp_stage_enemies = StageInEnemy::where('stage_id', $tmp_stage_master->id)->get();
            }
            catch (Exception $ex) {
                // エラーの場合
                return response()->json([
                    'result'  => 'NG',
                    'message' => $ex->getMessage(),
                ],400);
            }

            // ステージ内に出てくる敵データの分解
            foreach ($tmp_stage_enemies as $key => $tmp_stage_enemy)
            {
                $enemy_infos[] = [
                    'id'       => $tmp_stage_enemy->enemy_id,
                    'position' => [
                        'x' => $tmp_stage_enemy->x,
                        'y' => $tmp_stage_enemy->y,
                        'z' => $tmp_stage_enemy->z,
                    ],
                    'floor'    => $tmp_stage_enemy->floor,
                    'is_boss'  => $tmp_stage_enemy->is_boss,
                    'atk_rate' => $tmp_stage_enemy->atk_rate,
                    'hp_rate'  => $tmp_stage_enemy->hp_rate,
                    'size'     => $tmp_stage_enemy->size,
                ];
            }

            // ステージのマスターデータ
            $stage_master = [
                'id'          => $tmp_stage_master->id,
                'name'        => $tmp_stage_master->name,
                'note'        => $tmp_stage_master->note,
                'use_energy'  => $tmp_stage_master->use_energy,
                'rank_point'  => $tmp_stage_master->rank_point,
                'coin'        => $tmp_stage_master->coin,
                'crystal'     => $tmp_stage_master->crystal,
                'enemy_infos' => $enemy_infos,
                'max_floor'   => $tmp_stage_master->max_floor,
            ];

            // ステージに挑戦した時点でスタミナを消費する
            $account->energy -= $tmp_stage_master->use_energy;

            if ($account->energy < 0)
            {
                // エラーの場合
                return response()->json([
                    'result'  => 'NG',
                    'message' => 'スタミナが足りません。',
                ],400);
            }

            // プレイヤー情報を保存
            $account->save();

            // 敵のマスターデータを全取得
            foreach ($enemy_infos as $key => $enemy_info)
            {
                $tmp_enemy_ids[] = $tmp_stage_enemies[$key]['enemy_id'];
            }

            // 重複しているIDを削除
            $enemy_ids = array_unique($tmp_enemy_ids);

            // 取得した敵のIDに紐づいたデータを取得する
            foreach ($enemy_ids as $key => $enemy_id)
            {
                $tmp_enemy_atk_patterns = null;
                $tmp_enemy_master       = null;
                try{
                    // enemy_idが同じデータを取り出す
                    $tmp_enemy_atk_patterns = EnemyAtkPatternMaster::where('enemy_id', $enemy_id)->get();
                    $tmp_enemy_master       = EnemyMaster::where('id', $enemy_id)->first();
                }
                catch (Exception $ex) {
                    // エラーの場合
                    return response()->json([
                        'result'  => 'NG',
                        'message' => $ex->getMessage(),
                    ],400);
                }

                // アタックパターンの分解
                foreach ($tmp_enemy_atk_patterns as $key => $enemy_atk_pattern)
                {
                    $enemy_atk_patterns[] = $enemy_atk_pattern->atk_id;
                    $total_atk_patterns[] = $enemy_atk_pattern->atk_id;
                }

                // enemy_mastersの組み立て
                $enemy_masters[] = [
                    'id'              => $tmp_enemy_master->id,
                    'name'            => $tmp_enemy_master->name,
                    'name_id'         => $tmp_enemy_master->name_id,
                    'hp'              => $tmp_enemy_master->hp,
                    'atk'             => $tmp_enemy_master->atk,
                    'def'             => $tmp_enemy_master->def,
                    'atk_pattern_ids' => $enemy_atk_patterns,
                    'weak'            => $tmp_enemy_master->weak,
                ];
                $enemy_atk_patterns = null;  // 次のループに備えnullで初期化
            }

            // 被った攻撃パターンを取り除く
            $total_atk_patterns = array_unique($total_atk_patterns);

            // 登場する全敵の攻撃パターン分ループ(被りなし)
            foreach ($total_atk_patterns as $key => $total_atk_pattern)
            {
                try{
                    // atk_patternが同じデータを取り出す
                    $tmp_enemy_atk_master = EnemyAtkMaster::where('id', $total_atk_pattern)->first();
                }
                catch (Exception $ex) {
                    // エラーの場合
                    return response()->json([
                        'result'  => 'NG',
                        'message' => $ex->getMessage(),
                    ],400);
                }

                // enemy_atk_mastersの組み立て
                $enemy_atk_masters[] = [
                    'atk_rate'    => $tmp_enemy_atk_master->atk_rate,
                    'atk_attr'    => $tmp_enemy_atk_master->atk_attr,
                    'atk_area_id' => $tmp_enemy_atk_master->atk_area_id,
                    'atk_id'      => $tmp_enemy_atk_master->atk_id,
                ];
            }

            // パーティー情報を取得
            try{
                $members = Member::where('user_id', $account->user_id)->where('party_id', $request->party_id)->get();
            }
            catch (Exception $ex) {
                // エラーの場合
                return response()->json([
                    'result'  => 'NG',
                    'message' => $ex->getMessage(),
                ],400);
            }

            $player_info = $this->GetPlayerInfo($account);
            $character_masters = $this->GetCharacterMaster($account, $player_info);

            foreach ($character_masters as $key => $character_master)
            {
                foreach ($character_master["weapon_ids"] as $key => $weapon_id)
                {
                    $res_weapon_ids[] = $weapon_id;
                }
            }

            // JSONをオブジェクト型に変換、被ったweapon_idを取り除く
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
                'stage_master'      => $stage_master,
                'enemy_masters'     => $enemy_masters,
                'enemy_atk_masters' => $enemy_atk_masters,
                'character_masters' => $character_masters,
                'weapon_masters'    => $weapon_masters,
            ],200);
        }
    }

    // プレイヤー情報取得
    public function GetPlayerInfo($account)
    {
        $party_infos    = PartyInfo::where('user_id', $account->user_id)->get();     // party_infosテーブルの情報取得
        $has_characters = HasCharacter::where('user_id', $account->user_id)->limit(30)->get();  // has_charactersテーブルの情報取得
        $tmp_progress_rates = ProgressRate::where('user_id', $account->user_id)->get();

        // パーティー情報を分解
        foreach ($party_infos as $key => $party_info)
        {
            // membersテーブルの情報取得
            $members[] = Member::where('user_id', $account->user_id)->where('party_id', $party_info->party_id)->get();
        }

        // メンバー情報を分解
        foreach ($members as $key => $member)
        {
            // さらに分解
            foreach($member as $key => $tmp)
            {
                // レスポンス用にメンバー情報を分解
                $box_ids[] = $member[$key]->has_character_id;
            }
            
            // レスポンス用にパーティー情報を加工
            $res_party_infos[] = [
                'box_ids' => $box_ids,
            ];
            $box_ids = null;
        }

        // レスポンス用に所持キャラクター情報を加工
        foreach ($has_characters as $key => $has_character)
        {
            $character_ids[] = $has_character->id;
        }

        foreach ($tmp_progress_rates as $key => $progress_rate)
        {
            $res_progress_rates[] = [
                'stage_id' => $progress_rate['stage_id'],
                'is_clear' => $progress_rate['is_clear'],
                'is_order' => $progress_rate['is_order'],
            ];
        }

        // レスポンスで使うデータを組み合わせる
        $player_info = [
            'name'              => $account->name,
            'rank'              => $account->rank,
            'energy'            => $account->energy,
            'crystal'           => $account->crystal,
            'coin'              => $account->coin,
            'total_rank_point'  => $account->total_rank_point,
            'next_rank_point'   => $account->next_rank_point,
            'comment'           => $account->comment,
            'icon_id'           => $account->icon_id,
            'party_id'          => $account->party_id,
            'has_character_ids' => $character_ids,
            'party_infos'       => $res_party_infos,
            'progress_rates'    => $res_progress_rates,
        ];

        return $player_info;
    }

    // 所持キャラクターのステータス情報取得
    public function GetCharacterMaster($account, $player_info)
    {
        $party_infos = PartyInfo::where('user_id', $account->user_id)->get();     // party_infosテーブルの情報取得

        // return $player_info["party_infos"];

        // プレイヤーが編成しているキャラクターだけ取得する
        for ($i = 0; $i < count($player_info["party_infos"][$player_info["party_id"]]["box_ids"]); $i++)
        {
            // has_charactersテーブルの情報取得
            $has_characters[] = HasCharacter::where('user_id', $account->user_id)
            ->where('box_id', $player_info["party_infos"][$player_info["party_id"]]["box_ids"][$i])
            ->first();
        }

        // return $has_characters;

        foreach ($has_characters as $key => $has_character)
        {
            // character_mastersテーブルの情報取得
            $has_character_masters[] = CharacterMaster::where('id', $has_character["character_id"])->first();
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

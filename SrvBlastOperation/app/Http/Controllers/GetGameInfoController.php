<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Account;
use App\Models\Member;
use App\Models\PartyInfo;
use App\Models\ProgressRate;
use App\Models\HasCharacter;
use App\Models\CharacterMaster;
use App\Models\WeaponMaster;
use App\Models\EnemyMaster;
use App\Models\StageMaster;
use App\Models\StageInEnemy;
use App\Http\Requests\GetGameInfoRequest;

class GetGameInfoController extends Controller
{
    // Post通信処理
    public function store(GetGameInfoRequest $request)
    {
        // 同じIDのデータがあるか確認
        $account = Account::where('user_id', $request->user_id)->first();
        
        if ($account == null)
        {
            // 同じuser_idのデータが無かった場合
            return response()->json([
                'result' => 'NG',
                'message' => 'アカウントが存在しません。',
            ],400);
        }
        else
        {
            // 初期データが存在するかを確認する
            $tmp = PartyInfo::where('user_id', $request->user_id)->first();

            if ($tmp == null)
            {
                // 同じuser_idのデータが無かった場合
                // プレイヤー情報を作成
                $this->CreatePlayerInfo($request);
                
                // プレイヤー情報を取得
                $player_info = $this->GetPlayerInfo($account);

                // 所持キャラクターのステータス情報を取得
                $character_masters = $this->GetCharacterMaster($request);

                // 武器の情報を取得
                $weapon_masters = $this->GetWeaponMaster($request);

                // 敵の情報を取得
                $enemy_masters = $this->GetEnemyMaster($request);
            
                // ステージの情報を取得
                $stage_masters = $this->GetStageMaster($request);

                // レスポンス
                return response()->json([
                    'result' => 'OK',
                    'player_info' => $player_info,
                    'character_masters' => $character_masters,
                    'weapon_masters' => $weapon_masters,
                    'enemy_masters' => $enemy_masters,
                    'stage_masters' => $stage_masters,
                ],200);
            }
            else
            {
                // プレイヤー情報を取得
                $player_info = $this->GetPlayerInfo($account);

                // 所持キャラクターのステータス情報を取得
                $character_masters = $this->GetCharacterMaster($request);

                // 武器の情報を取得
                $weapon_masters = $this->GetWeaponMaster($request);

                // 敵の情報を取得
                $enemy_masters = $this->GetEnemyMaster($request);
            
                // ステージの情報を取得
                $stage_masters = $this->GetStageMaster($request);

                // レスポンス
                return response()->json([
                    'result' => 'OK',
                    'player_info' => $player_info,
                    'character_masters' => $character_masters,
                    'weapon_masters' => $weapon_masters,
                    'enemy_masters' => $enemy_masters,
                    'stage_masters' => $stage_masters,
                ],200);
            }
        }
    }

    // プレイヤー情報作成
    public function CreatePlayerInfo($request)
    {
        // 登録データをセット
        // party_infos
        for ($i = 0; $i < 2; $i++)
        {
            $party_infos = new PartyInfo();
            $party_infos->user_id = $request->user_id;
            $party_infos->party_id = $i + 1;

            // データベースに登録
            $party_infos->save();
        }

        // members
        for ($i = 0; $i < 2; $i++)
        {
            for ($j = 0; $j < 4; $j++)
            {
                $members = new Member();
                $members->user_id = $request->user_id;
                $members->party_id = $i + 1;
                $members->box_id = $j + 1;

                // データベースに登録
                $members->save();
            }
        }

        // progress_rates
        $tmp = StageMaster::get()->count();
        for ($i = 0; $i < $tmp; $i++)
        {
            $progress_rates = new ProgressRate();
            $progress_rates->user_id = $request->user_id;
            $progress_rates->stage_id = $i + 1;
            $progress_rates->is_clear = 0;
            $progress_rates->is_order = 1;

            // データベースに登録
            $progress_rates->save();
        }

        // has_characters
        $character_masters = CharacterMaster::whereBetween('id', [1, 4])->get();
        foreach ($character_masters as $key => $character_master)
        {
            $has_characters = new HasCharacter();
            $has_characters->user_id = $request->user_id;
            $has_characters->box_id = $character_master->id;
            $has_characters->character_id = $character_master->id;
                
            // データベースに登録
            $has_characters->save();
        }

        // return null;
    }

    // プレイヤー情報取得
    public function GetPlayerInfo($account)
    {
        // 同じuser_idのデータがあった場合
        $party_infos = PartyInfo::where('user_id', $account->user_id)->get();        // party_infosテーブルの情報取得
        $progress_rates = ProgressRate::where('user_id', $account->user_id)->get();  // progress_ratesテーブルの情報取得
        $has_characters = HasCharacter::where('user_id', $account->user_id)->get();   // has_charactersテーブルの情報取得

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
                $box_ids[] = $member[$key]->box_id;
            }
        }

        // レスポンス用にパーティー情報を加工
        $party_infos = [
            'box_id' => $box_ids,
        ];

        // レスポンス用にステージ進行情報を加工
        foreach ($progress_rates as $key => $progress_rate)
        {
            $stages[] = [
                'stage_id' => $progress_rates[$key]->stage_id,
                'is_clear' => $progress_rates[$key]->is_clear,
                'is_order' => $progress_rates[$key]->is_order,
            ];
        }

        // レスポンス用に所持キャラクター情報を加工
        foreach ($has_characters as $key => $has_character)
        {
            $characters[] = [
                'box_id' => $has_characters[$key]->box_id,
                'id' => $has_characters[$key]->character_id,
            ];
        }

        // レスポンスで使うデータを組み合わせる
        $player_info = [
            'name'             => $account->name,
            'rank'             => $account->rank,
            'energy'           => $account->energy,
            'crystal'          => $account->crystal,
            'coin'             => $account->coin,
            'total_rank_point' => $account->total_rank_point,
            'next_rank_point'  => $account->next_rank_point,
            'comment'          => $account->comment,
            'icon_id'          => $account->icon_id,
            'party_infos'      => $party_infos,
            'progress_rates'   => $progress_rates,
            'has_characters'   => $characters,
        ];

        return $player_info;
    }

    // 所持キャラクターのステータス情報取得
    public function GetCharacterMaster($request)
    {
        $character_masters = CharacterMaster::get();

        // レスポンス用に所持キャラクターのステータス情報を加工
        foreach ($character_masters as $key => $character_master)
        {
            // 一番目の武器IDと二番目の武器IDを配列に格納
            $weapon_ids = [
                $character_master->first_weapon_id,
                $character_master->second_weapon_id,
            ];

            // レスポンスで使うデータを組み合わせる
            $characters[] = [
                'id'           => $character_master->id,
                'rank'         => $character_master->rank,
                'name'         => $character_master->name,
                'hp'           => $character_master->hp,
                'atk_rate'     => $character_master->atk_rate,
                'physical_def' => $character_master->physical_def,
                'magical_def'  => $character_master->magical_def,
                'weapon_ids'   => $weapon_ids,
            ];
        }

        return $characters;
    }

    public function GetWeaponMaster($request)
    {
        $weapon_masters = WeaponMaster::get();

        // レスポンス用に所持キャラクターのステータス情報を加工
        foreach ($weapon_masters as $key => $weapon_master)
        {
            // レスポンスで使うデータを組み合わせる
            $weapons[] = [
                'id'                => $weapon_master->id,
                'name'              => $weapon_master->name,
                'attr'              => $weapon_master->attr,
                'atk'               => $weapon_master->atk,
                'speed'             => $weapon_master->speed,
                'special_effect_id' => $weapon_master->special_effect_id,
            ];
        }

        return $weapons;
    }

    public function GetEnemyMaster($request)
    {
        $enemy_masters = EnemyMaster::get();

        // レスポンス用に所持キャラクターのステータス情報を加工
        foreach ($enemy_masters as $key => $enemy_master)
        {
            // 攻撃パターン
            $atk_pattern = [
                'atk' => $enemy_master->atk,
                'attr' => $enemy_master->attr,
                'atk_area' => $enemy_master->atk_area,
                'atk_id' => $enemy_master->atk_id,
            ];

            // レスポンスで使うデータを組み合わせる
            $enemies[] = [
                'id'          => $enemy_master->id,
                'name'        => $enemy_master->name,
                'hp'          => $enemy_master->hp,
                'atk_pattern' => $atk_pattern,
                'weak'        => $enemy_master->weak,
            ];
        }

        return $enemies;
    }

    public function GetStageMaster($request)
    {
        $stage_masters = StageMaster::get();

        // レスポンス用に所持キャラクターのステータス情報を加工
        foreach ($stage_masters as $key => $stage_master)
        {
            $stage_in_enemies = StageInEnemy::where('stage_id', $stage_master->id)->get();

            foreach ($stage_in_enemies as $key => $stage_in_enemy)
            {
                // 敵の座標
                $position = [
                    'x' => $stage_in_enemy->x,
                    'y' => $stage_in_enemy->y,
                    'z' => $stage_in_enemy->z,
                ];

                // 攻撃パターン
                $enemies[] = [
                    'id'       => $stage_in_enemy->enemy_id,
                    'position' => $position,
                    'floor'    => $stage_in_enemy->floor,
                    'is_boss'  => $stage_in_enemy->is_boss,
                    'atk_rate' => $stage_in_enemy->atk_rate,
                    'hp_rate'  => $stage_in_enemy->hp_rate,
                    'size'     => $stage_in_enemy->size,
                ];
            }
            
            // レスポンスで使うデータを組み合わせる
            $stages[] = [
                'id'          => $stage_master->id,
                'name'        => $stage_master->name,
                'note'        => $stage_master->note,
                'use_energy'  => $stage_master->use_energy,
                'rank_point'  => $stage_master->rank_point,
                'coin'        => $stage_master->coin,
                'enemy_infos' => $enemies,
                'max_floor'   => $stage_master->max_floor,
            ];
        }

        return $stages;
    }
}

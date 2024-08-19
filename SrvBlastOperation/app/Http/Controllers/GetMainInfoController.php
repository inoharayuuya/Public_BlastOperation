<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Http\Requests\GetMainInfoRequest;
use App\Models\Account;
use App\Models\PartyInfo;
use App\Models\Member;
use App\Models\HasCharacter;
use App\Models\ProgressRate;
use App\Models\CharacterMaster;
use App\Models\StageMaster;

class GetMainInfoController extends Controller
{
    // Post通信処理
    public function store(GetMainInfoRequest $request)
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
            // 同じuser_idのデータがあった場合　データ更新
            // プレイヤー情報を取得
            $player_info = $this->GetPlayerInfo($account);

            // 所持キャラクターのステータス情報を取得
            $character_masters = $this->GetCharacterMaster($account, $player_info);

            // ステージの情報を取得
            $stage_masters = $this->GetStageMaster();

            // レスポンス
            return response()->json([
                'result' => 'OK',
                'player_info' => $player_info,
                'character_masters' => $character_masters,
                'stage_masters' => $stage_masters,
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

    public function GetStageMaster()
    {
        $stage_masters = StageMaster::get();

        // レスポンス用に所持キャラクターのステータス情報を加工
        foreach ($stage_masters as $key => $stage_master)
        {
            // レスポンスで使うデータを組み合わせる
            $stages[] = [
                'id'          => $stage_master->id,
                'name'        => $stage_master->name,
                'note'        => $stage_master->note,
                'use_energy'  => $stage_master->use_energy,
                'coin'        => $stage_master->coin,
            ];
        }

        return $stages;
    }
}

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
use App\Http\Requests\GetGameInfoRequest;

class SetPlayerInfoController extends Controller
{
    const MAX_PARTY_INDEX = 2;
    const MAX_MENBER_INDEX = 4;
    // Post通信処理
    public function store(Request $request)
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
            // アカウント情報を分解
            $player_info = json_decode($request->player_info, false);

            // プレイヤー情報をリクエストパラメータで更新
            $this->UpdatePlayerInfo($player_info, $account);

            // レスポンス
            return response()->json([
                'result' => 'OK',
            ],200);
        }
    }

    // プレイヤー情報作成
    public function UpdatePlayerInfo($player_info, $account)
    {
        // 登録データをセット
        // accounts
        $account->name = $player_info->name;
        $account->rank = $player_info->rank;
        $account->energy = $player_info->energy;
        $account->crystal = $player_info->crystal;
        $account->coin = $player_info->coin;
        $account->total_rank_point = $player_info->total_rank_point;
        $account->next_rank_point = $player_info->next_rank_point;
        $account->comment = $player_info->comment;
        $account->icon_id = $player_info->icon_id;
        $account->party_id = $player_info->party_id;
        $account->save();

        // has_characters
        for ($i = 0; $i < count($player_info->has_character_ids); $i++)
        {
            $has_characters = HasCharacter::where('user_id', $player_info->user_id)->first();
            // $has_characters->;
        }

        // party_infos
        for ($i = 0; $i < MAX_PARTY_INDEX; $i++)
        {
            $party_infos = PartyInfo::where('user_id', $player_info->user_id)->first();
            $party_infos->party_id = $i + 1;
            $party_infos->save();
        }

        // members
        $members = Member::where('user_id', $account->user_id)->get();
        for ($i = 0; $i < MAX_PARTY_INDEX; $i++)
        {
            for ($j = 0; $j < MAX_MENBER_INDEX; $j++)
            {
                $members[($i * MAX_MENBER_INDEX) + $j]->party_id = $player_info->party_id;
                $members[($i * MAX_MENBER_INDEX) + $j]->box_id = $j + 1;
                $members->save();
            }
        }

        // progress_rates

        // has_characters
        
        return;
    }
}

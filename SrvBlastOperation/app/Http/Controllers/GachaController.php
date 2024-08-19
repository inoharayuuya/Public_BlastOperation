<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Account;
use App\Models\HasCharacter;
use App\Models\CharacterMaster;
use App\Models\WeaponMaster;

use App\Libraries\Common;

class GachaController extends Controller
{
    const RARITY_INFO = [
        [74,  0],
        [15, 89],
        [10, 10],
        [ 1,  1],
    ];

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
            ],400);
        }
        else
        {
            // 所持石がマイナスになる時はリクエスト失敗
            if ($account->crystal - 50 < 0)
            {
                return response()->json([
                    'result' => 'NG',
                ],400);
            }

            // 同じuser_idのデータがある場合
            $roll_num = 0;
            $lottery_type = $request->is_single;//0=1連,1=10連(1/10回)

            // ガチャ石の個数を減らしてデータベースを更新
            $crystal = $this->UpdateAccount($account);

            // 所持キャラクターを追加
            $has_character_ids = $this->SetHasCharacter($request);

            // レスポンス用
            $player_info = [
                'crystal' => $crystal,
                'has_character_ids' => $has_character_ids,
            ];

            $tmp_id = 0;
            $character_masters = [];
            for ($i = 0; $i < count(Common::CHARACTER_IDS); $i++)
            {
                if ($tmp_id != Common::CHARACTER_IDS[$i])
                {
                    $tmp = CharacterMaster::where('id', Common::CHARACTER_IDS[$i])->first();
                    $character_masters[] = [
                        'id' => $tmp->id,
                        'rank' => $tmp->rank,
                        'name' => $tmp->name,
                        'name_id' => $tmp->name_id,
                        'hp' => $tmp->hp,
                        'atk' => $tmp->atk,
                        'speed' => $tmp->speed,
                        'physical_def' => $tmp->physical_def,
                        'magical_def' => $tmp->magical_def,
                        'weapon_ids' => [
                            $tmp->first_weapon_id,
                            $tmp->second_weapon_id,
                        ],
                    ];
                    $weapon_ids[] = $tmp->first_weapon_id;
                    $weapon_ids[] = $tmp->second_weapon_id;
                }
                $tmp_id = Common::CHARACTER_IDS[$i];
            }

            foreach ($weapon_ids as $key => $weapon_id)
            {
                $tmp = WeaponMaster::where('id', $weapon_id)->first();
                $weapon_masters[] = [
                    'id' => $tmp->id,
                    'name' => $tmp->name,
                    'attr' => $tmp->attr,
                    'atk_rate' => $tmp->atk_rate,
                    'speed_rate' => $tmp->speed_rate,
                    'range' => $tmp->range,
                    'special_effect_id' => $tmp->special_effect_id,
                ];
            }

            // レスポンス
            return response()->json([
                'result' => 'OK',
                'character_ids' => Common::CHARACTER_IDS,
                'player_info' => $player_info,
                'character_masters' => $character_masters,
                'weapon_masters' => $weapon_masters,
            ],200);
        }
    }

    private function UpdateAccount($account)
    {
        $account->crystal = $account->crystal - 50;
        $account->save();

        return $account->crystal;
    }

    private function SetHasCharacter($request)
    {
        for ($i = 0; $i < count(Common::CHARACTER_IDS); $i++)
        {
            $has_character = new HasCharacter;
            $has_character->user_id = $request->user_id;
            $has_character->box_id = HasCharacter::where('user_id', $request->user_id)->max('box_id') + 1;
            $has_character->character_id = Common::CHARACTER_IDS[$i];
            $has_character->save();
            $characters[] = $has_character->id;
        }

        return $characters;
    }

    private function StartGacha()
    {
        for ($i = 0; $i < $roll_num; $i++)
        {
            if ($i == 9)//最後の1回
                $lottery_type = 1;
            else
                $lottery_type = 0;
            $this->GetDropItem();
        }
        $roll_numll = 0;
    }

    private function GetDropItem()
    {
        //レア度の抽選
        $item_rarity = ChooseRarity() + 3;
    }

    // private function ChooseRarity()
    // {
    //     //nextFloatでは0から1までのfloat値を返すので
    //     //そこにドロップ率の合計を掛ける

    //     $randomizer = new \Random\Randomizer();

    //     $randomPoint = \Random\Randomizer::nextFloat();

    //     //確率の合計値を格納
    //     $total = 0;

    //     //確率を合計する
    //     for ($i = 0; $i < count(Gacha::RARITY_INFO); $i++)
    //     {
    //         $total += Gacha::RARITY_INFO[$i][$lottery_type];
    //     }

    //     //randomPointの位置に該当するキーを返す
    //     for ($i = 0; $i < $RarityInfo.GetLength(0); $i++)
    //     {
    //         if ($randomPoint < $RarityInfo[$i, $lotteryType])
    //         {
    //             return $i;
    //         }
    //         else
    //         {
    //             randomPoint -= RarityInfo[i, lotteryType];
    //         }
    //     }
    //     return 0;
    // }
}

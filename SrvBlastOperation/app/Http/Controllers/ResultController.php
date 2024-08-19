<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Libraries\Common;
use App\Libraries\Rank;
use App\Models\Account;
use App\Models\StageMaster;
use App\Models\ProgressRate;

class ResultController extends Controller
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
            // 各ランク幅
            foreach (Common::NEXT_RANK_POINTS as $key => $rank_point)
            {
                $limit_rank_points[] = $rank_point;
            }

            // player_infoをオブジェクトに変換
            $player_info = json_decode($request->player_info);
            
            // 現在のランクでインデックスを付ける
            switch ($account->rank)
            {
                case Rank::RANK_F:
                    $rp_index = 0;
                    break;
                case Rank::RANK_E:
                    $rp_index = 1;
                    break;    
                case Rank::RANK_D:
                    $rp_index = 2;
                    break;
                case Rank::RANK_C:
                    $rp_index = 3;
                    break;
                case Rank::RANK_B:
                    $rp_index = 4;
                    break;
                case Rank::RANK_A:
                    $rp_index = 5;
                    break;
                case Rank::RANK_S:
                    $rp_index = 6;
                    break;
                default:
                    $rp_index = 6;
                    break;
            }

            // 挑戦したステージ情報を取得(連続ランクアップがある可能性があるのでループで処理)
            while (1)
            {
                try{
                    // 同じstage_idのステージデータを取得する
                    $tmp_stage_master = StageMaster::where('id', $request->stage_id)->first();
    
                    // player_infoに上書き
                    if ($account->total_rank_point + $tmp_stage_master->rank_point >= Common::NEXT_RANK_POINTS[$rp_index])
                    {
                        $account->total_rank_point -= Common::NEXT_RANK_POINTS[$rp_index];
                        $rp_index++;
                        continue;
                    }
    
                    // インデックスをランクに直す
                    switch ($rp_index)
                    {
                        case 0:
                            $account->rank = 'F';
                            break;
                        case 1:
                            $account->rank = 'E';
                            break;    
                        case 2:
                            $account->rank = 'D';
                            break;
                        case 3:
                            $account->rank = 'C';
                            break;
                        case 4:
                            $account->rank = 'B';
                            break;
                        case 5:
                            $account->rank = 'A';
                            break;
                        case 6:
                            $account->rank = 'S';
                            break;
                    }

                    $progress_rates = ProgressRate::where('user_id', $request->user_id)->get();

                    if ($progress_rates[$request->stage_id - 1]['is_clear'] == 0)
                    {
                        $account->crystal += $tmp_stage_master->crystal;
                    }

                    // アカウント情報書き換え
                    $account->energy            = $player_info->energy;
                    $account->coin              = $player_info->coin;
                    $account->total_rank_point += $tmp_stage_master->rank_point;
                    $account->next_rank_point   = Common::NEXT_RANK_POINTS[$rp_index] - $account->total_rank_point;
                    $account->save();

                    break;
                }
                catch (Exception $ex) {
                    // エラーの場合
                    return response()->json([
                        'result' => 'NG',
                    ],400);
                }
            }

            // 進行情報書き換え
            $progress_rates[$request->stage_id - 1]['is_clear'] = 1;
            $progress_rates[$request->stage_id - 1]->save();

            foreach ($progress_rates as $key => $progress_rate)
            {
                $res_progress_rates[] = [
                    'stage_id' => $progress_rate['stage_id'],
                    'is_clear' => $progress_rate['is_clear'],
                    'is_order' => $progress_rate['is_order'],
                ];
            }

            $player_info = [
                'rank'             => $account->rank,
                'energy'           => $account->energy,
                'total_rank_point' => $account->total_rank_point,
                'next_rank_point'  => $account->next_rank_point,
                'coin'             => $account->coin,
                'crystal'          => $account->crystal,
                'progress_rates'   => $res_progress_rates,
            ];

            // レスポンス
            return response()->json([
                'result'            => 'OK',
                'limit_rank_points' => $limit_rank_points,
                'player_info'       => $player_info,
            ],200);
        }
    }
}

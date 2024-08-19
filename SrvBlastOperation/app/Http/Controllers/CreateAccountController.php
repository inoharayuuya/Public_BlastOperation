<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use App\Models\Account;
use App\Models\PartyInfo;
use App\Models\Member;
use App\Models\HasCharacter;
use App\Models\ProgressRate;
use App\Models\CharacterMaster;
use App\Models\StageMaster;
use App\Http\Requests\CreateAccountRequest;

class CreateAccountController extends Controller
{
    // Post通信処理
    public function store(CreateAccountRequest $request)
    {
        // パスワード自動生成モードではないかつパスワードが入力されていないとき
        if (!$request->auto_flg && !isset($request->password))
        {
            // レスポンス
            return response()->json([
                'result'  => 'NG',
                'message' => 'パスワードを入力するか、オートパスワードモードを選択してください。',
            ],400);
        }
        else if ($request->auto_flg && isset($request->password))
        {
            // レスポンス
            return response()->json([
                'result'  => 'NG',
                'message' => 'パスワードを入力した状態で、オートパスワードモードを選択しないでください。',
            ],400);
        }
        
        // 新しいデータを作成する
        $account = $this->CreateUser($request);

        // プレイヤー情報を作成
        $this->CreatePlayerInfo($account);

        // パスワード自動生成されたらパスワードを返す
        if ($request->auto_flg)
        {
            // レスポンス
            return response()->json([
                'result'  => 'OK',
                'password' => $account->password,
            ],200);
        }
    }

    public function CreateUser($request)
    {
        // パスワード自動生成モードか確認
        if ($request->auto_flg)
        {
            // ハッシュ化する
            $max_id = Account::latest('id')->select('id')->first();
            $password = hash('sha256', $max_id->id . strtotime('now'));  // name + タイムスタンプ
        }
        else
        {
            // 受け取ったパスワードをそのまま格納する
            $password = $request->password;
        }

        // 登録データをセット
        $account = new Account();

        $account->user_id = hash('sha256', $request->name . strtotime('now'));
        $account->is_login = false;
        $account->auto_flg = $request->auto_flg;
        $account->name = $request->name;
        $account->password = $password;
        $account->rank = "F";
        $account->energy = 50;
        $account->crystal = 0;
        $account->coin = 0;
        $account->total_rank_point = 0;
        $account->next_rank_point = 10;
        $account->comment = "よろしくお願いします！";
        $account->icon_id = 0;
        $account->party_id = 0;

        // データベースに登録
        $account->save();

        return $account;
    }

    // プレイヤー情報作成
    public function CreatePlayerInfo($account)
    {
        // 登録データをセット
        // party_infos
        for ($i = 0; $i < 2; $i++)
        {
            $party_infos = new PartyInfo();
            $party_infos->user_id = $account->user_id;
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
                $members->user_id = $account->user_id;
                $members->party_id = $i + 1;
                $members->has_character_id = $j + 1;

                // データベースに登録
                $members->save();
            }
        }
        
        // progress_rates
        $tmp = StageMaster::get()->count();        
        for ($i = 0; $i < $tmp; $i++)
        {
            $progress_rates = new ProgressRate();
            $progress_rates->user_id = $account->user_id;
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
            $has_characters->user_id = $account->user_id;
            $has_characters->box_id = $character_master->id;
            $has_characters->character_id = $character_master->id;

            // データベースに登録
            $has_characters->save();
        }

        return;
    }
}

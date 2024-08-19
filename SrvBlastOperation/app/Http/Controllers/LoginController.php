<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Account;
use App\Http\Requests\LoginRequest;

class LoginController extends Controller
{
    // Post通信処理
    public function store(LoginRequest $request)
    {
        // 同じIDのデータがあるか確認
        $accounts = Account::where('name', $request->name)
        ->where('password', $request->password)
        ->first();

        if ($accounts == null)
        {
            // 同じ名前のデータが無かった場合
            // レスポンス
            return response()->json([
                'result'  => 'NG',
                'message' => 'アカウントが存在しません。',
            ],400);
        }
        else
        {
            // 同じIDのデータがあった場合
            // そのアカウントが別の端末でログインしていないかを確認
            if ($accounts->is_login == false)
            {
                // ログインが可能なのでis_loginをtrueにして他の端末でログインできないようにする
                $account = \DB::table('accounts')
                ->where('name', $request->name)
                ->update(['is_login' => 1]);

                // レスポンス
                return response()->json([
                    'result'  => 'OK',
                    'message' => 'ログインしました。',
                    'user_id' => $accounts->user_id,
                    'rank'    => $accounts->rank,
                    'icon_id' => $accounts->icon_id,
                ],200);
            }
            else
            {
                // レスポンス
                return response()->json([
                    'result'  => 'NG',
                    'message' => 'このアカウントは別のデバイスでログインしています。ログアウトしてからもう一度ログインし直してください。',
                ],400);
            }
        }
    }
}

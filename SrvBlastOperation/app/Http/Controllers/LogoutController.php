<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use App\Models\Account;
use App\Http\Requests\LogoutRequest;

class LogoutController extends Controller
{
    // Post通信処理
    public function store(LogoutRequest $request)
    {
        // 同じIDのデータがあるか確認
        $accounts = Account::where('user_id', $request->user_id)
        ->first();

        if ($accounts != null)
        {
            // 同じIDのデータがあった場合
            if ($accounts->is_login == true)
            {
                // ログアウトが可能なのでis_loginをfalseにして他の端末でログインできるようにする
                $account = \DB::table('accounts')
                ->where('user_id', $request->user_id)
                ->update(['is_login' => 0]);

                // レスポンス
                return response()->json([
                    'result'  => 'OK',
                    'message' => 'ログアウトしました。',
                    'user_id' => $accounts->user_id,
                ],200);
            }
            else
            {
                // レスポンス
                return response()->json([
                    'result'  => 'NG',
                    'message' => 'ログインされていません。',
                ],400);
            }
        }

        // レスポンス
        return response()->json([
            'result'  => 'NG',
            'message' => 'アカウントが存在しません。',
        ],400);
    }
}

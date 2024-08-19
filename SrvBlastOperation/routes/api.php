<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\CreateAccountController;
use App\Http\Controllers\LoginController;
use App\Http\Controllers\LogoutController;
use App\Http\Controllers\GetMainInfoController;
// use App\Http\Controllers\SetPlayerInfoController;
use App\Http\Controllers\GetCharacterInfoController;
use App\Http\Controllers\GetStageInfoController;
use App\Http\Controllers\ResultController;
use App\Http\Controllers\GachaController;

// アカウント作成API
Route::apiResource('create_account', CreateAccountController::class);

// ログインAPI
Route::apiResource('login', LoginController::class);

// ログアウトAPI
Route::apiResource('logout', LogoutController::class);

// ゲーム情報取得API
// Route::apiResource('get_game_info', GetGameInfoController::class);

// メインデータ取得API
Route::apiResource('get_main_data', GetMainInfoController::class);

// プレイヤー基本情報登録API
// Route::apiResource('set_player_info', SetPlayerInfoController::class);

// キャラクター情報取得API
Route::apiResource('get_character_info', GetCharacterInfoController::class);

// ステージ情報取得API
Route::apiResource('get_stage_info', GetStageInfoController::class);

// リザルトAPI
Route::apiResource('result', ResultController::class);

// ガチャAPI
Route::apiResource('gacha', GachaController::class);

/*
|--------------------------------------------------------------------------
| API Routes
|--------------------------------------------------------------------------
|
| Here is where you can register API routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| is assigned the "api" middleware group. Enjoy building your API!
|
*/

Route::middleware('auth:sanctum')->get('/user', function (Request $request) {
    return $request->user();
});

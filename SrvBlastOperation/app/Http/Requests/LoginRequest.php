<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;
use Illuminate\Contracts\Validation\Validator;
use Illuminate\Http\Exceptions\HttpResponseException;

class LoginRequest extends FormRequest
{
    /**
     * Determine if the user is authorized to make this request.
     *
     * @return bool
     */
    public function authorize()
    {
        // API実行時の認証設定
        // ログインが不要の場合はTrueを返す
        return true;
    }

    /**
     * Get the validation rules that apply to the request.
     *
     * @return array
     */
    public function rules()
    {
        return [
            // バリデーションルールを設定
            'name' => 'required|string',
            'auto_flg' => 'required|boolean',
            'password' => 'required|string',
        ];
    }
    
    public function messages()
    {
        // バリデーションに引っかかった時のメッセージを設定
        return [
            'name.required' => 'nameは必須項目です。',
            'name.string' => 'nameには文字列を入れてください。',
            'auto_flg.required' => 'auto_flgは必須項目です。',
            'auto_flg.boolean' => 'auto_flgは0か1で入力してください。',
            'password.required' => 'passwordは必須項目です。',
            'password.string' => 'passwordには文字列を入れてください。',
        ];
    }

    protected function failedValidation(Validator $validator)
    {
        // バリデーションに引っかかった時の返却値の設定
        $res = response()->json([
            'result' => 'NG',
            'errors' => $validator->errors(),
        ], 400);
        throw new HttpResponseException($res);
    }
}

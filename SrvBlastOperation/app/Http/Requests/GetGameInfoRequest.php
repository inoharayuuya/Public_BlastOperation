<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;
use Illuminate\Contracts\Validation\Validator;
use Illuminate\Http\Exceptions\HttpResponseException;

class GetGameInfoRequest extends FormRequest
{
    /**
     * Determine if the user is authorized to make this request.
     *
     * @return bool
     */
    public function authorize()
    {
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
            'user_id' => 'required|string',
        ];
    }
    
    public function messages()
    {
        // バリデーションに引っかかった時のメッセージを設定
        return [
            'user_id.required' => 'user_idは必須項目です。',
            'user_id.string' => 'user_idには文字列を入れてください。',
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

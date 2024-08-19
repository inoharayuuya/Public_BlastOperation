<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class StageMasterSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        DB::table('stage_masters')->insert([
            [
                'name' => "始まりの台地",
                'note' => "ギルド協会\n" . 
                          "「緊急！緊急！魔法使いがスライムの軍隊を率いて、この街を襲いに来ました！\n" . 
                          "至急冒険者は退治にとりかかってください！\n" . 
                          "ちょっと待って！！あー！！私の家ぇぇぇぇ！」",
                'use_energy' => 0,
                'rank_point' => 10,
                'coin' => 0,
                'crystal' => 10,
                'max_floor' => 4,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "無法地帯入口",
                'note' => "未熟な戦士\n" . 
                          "「なんか入口付近に物騒なやつがいてなぁ、そいつらに急に襲われたんだよ！\n" . 
                          "あいつらに目に物を見せてやってはくれねぇか？\n" . 
                          "なぁ、頼むよ！この通りだ！」",
                'use_energy' => 0,
                'rank_point' => 50,
                'coin' => 0,
                'crystal' => 20,
                'max_floor' => 5,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "竜の住処",
                'note' => "駆け出し冒険者\n" . 
                          "「へへ、ここが竜の住処か...\n" . 
                          "この竜の卵を売れば俺は億万長者だぜー！\n" . 
                          "うわぁぁぁぁぁぁ！！\n" . 
                          "誰か助けてくれぇぇぇぇぇぇ！！」",
                'use_energy' => 0,
                'rank_point' => 100,
                'coin' => 0,
                'crystal' => 20,
                'max_floor' => 3,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ]
        ]);
    }
}

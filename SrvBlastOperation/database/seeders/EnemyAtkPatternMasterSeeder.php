<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class EnemyAtkPatternMasterSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        DB::table('enemy_atk_pattern_masters')->insert([
            [
                'enemy_id' => 1,
                'atk_id' => 1,
                'enemy_name' => 'スライム',
                'atk_name' => '水拡散',
            ],
            [
                'enemy_id' => 2,
                'atk_id' => 1,
                'enemy_name' => '魔法使い',
                'atk_name' => 'ヒール(単体)',
            ],
            [
                'enemy_id' => 2,
                'atk_id' => 1,
                'enemy_name' => '魔法使い',
                'atk_name' => 'ヒール(全体)',
            ],
            [
                'enemy_id' => 2,
                'atk_id' => 1,
                'enemy_name' => '魔法使い',
                'atk_name' => 'ホーミング魔法弾',
            ],
            [
                'enemy_id' => 3,
                'atk_id' => 1,
                'enemy_name' => '盗賊',
                'atk_name' => '周囲斬撃',
            ],
            [
                'enemy_id' => 3,
                'atk_id' => 1,
                'enemy_name' => '盗賊',
                'atk_name' => 'ナイフ投げ',
            ],
            [
                'enemy_id' => 4,
                'atk_id' => 1,
                'enemy_name' => 'ボス盗賊',
                'atk_name' => '周囲斬撃',
            ],
            [
                'enemy_id' => 4,
                'atk_id' => 1,
                'enemy_name' => 'ボス盗賊',
                'atk_name' => 'ナイフ投げ',
            ],
            [
                'enemy_id' => 5,
                'atk_id' => 1,
                'enemy_name' => '子ドラゴン',
                'atk_name' => '火を噴く(小)',
            ],
            [
                'enemy_id' => 5,
                'atk_id' => 1,
                'enemy_name' => '子ドラゴン',
                'atk_name' => '周囲しっぽ(小)',
            ],
            [
                'enemy_id' => 6,
                'atk_id' => 1,
                'enemy_name' => 'ドラゴン',
                'atk_name' => '火を噴く(大、直線扇移動)',
            ],
            [
                'enemy_id' => 6,
                'atk_id' => 1,
                'enemy_name' => 'ドラゴン',
                'atk_name' => '火を噴く(大、円)',
            ],
            [
                'enemy_id' => 6,
                'atk_id' => 1,
                'enemy_name' => 'ドラゴン',
                'atk_name' => '周囲しっぽ(大)',
            ],
        ]);
    }
}

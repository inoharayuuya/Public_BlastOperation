<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class EnemyAtkMasterSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        DB::table('enemy_atk_masters')->insert([
            [
                'name' => '水拡散',
                'atk_rate' => 1.1,
                'atk_attr' => '魔法',
                'atk_area_id' => 1,
                'atk_id' => 1,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => '周囲斬撃',
                'atk_rate' => 1.85,
                'atk_attr' => '物理',
                'atk_area_id' => 2,
                'atk_id' => 2,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => 'ナイフ投げ',
                'atk_rate' => 1.35,
                'atk_attr' => '物理',
                'atk_area_id' => 3,
                'atk_id' => 3,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => '火を噴く(小)',
                'atk_rate' => 1.2,
                'atk_attr' => '魔法',
                'atk_area_id' => 1,
                'atk_id' => 4,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => '周囲しっぽ(小)',
                'atk_rate' => 1.15,
                'atk_attr' => '物理',
                'atk_area_id' => 2,
                'atk_id' => 5,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => '火を噴く(大、直線扇移動)',
                'atk_rate' => 2.5,
                'atk_attr' => '魔法',
                'atk_area_id' => 1,
                'atk_id' => 6,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => '火を噴く(大、円)',
                'atk_rate' => 2.1,
                'atk_attr' => '魔法',
                'atk_area_id' => 2,
                'atk_id' => 7,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => '周囲しっぽ(大)',
                'atk_rate' => 2.2,
                'atk_attr' => '物理',
                'atk_area_id' => 2,
                'atk_id' => 8,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => 'ヒール(単体)',
                'atk_rate' => 1.35,
                'atk_attr' => '回復',
                'atk_area_id' => 0,
                'atk_id' => 9,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => 'ヒール(全体)',
                'atk_rate' => 1.15,
                'atk_attr' => '回復',
                'atk_area_id' => 0,
                'atk_id' => 10,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => 'ホーミング魔法弾',
                'atk_rate' => 1.55,
                'atk_attr' => '魔法',
                'atk_area_id' => 4,
                'atk_id' => 11,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
        ]);
    }
}

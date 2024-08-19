<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class EnemyMasterSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        DB::table('enemy_masters')->insert([
            [
                'name' => "スライム",
                'name_id' => "slime",
                'hp' => 100.0,
                'atk' => 5.0,
                'def' => 1.0,
                'weak' => '魔法',
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' =>"魔法使い",
                'name_id' => "witch",
                'hp' => 200.0,
                'atk' => 10.0,
                'def' => 5.0,
                'weak' => '物理',
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "盗賊",
                'name_id' => "thief",
                'hp' => 150.0,
                'atk' => 5.0,
                'def' => 2.5,
                'weak' => '魔法',
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' =>"ボス盗賊",
                'name_id' => "boss_thief",
                'hp' => 500.0,
                'atk' => 15.0,
                'def' => 5.0,
                'weak' => '魔法',
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "子ドラゴン",
                'name_id' => "child_dragon",
                'hp' => 150.0,
                'atk' => 10.0,
                'def' => 5.0,
                'weak' => '物理',
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' =>"親ドラゴン",
                'name_id' => "parent_dragon",
                'hp' => 750.0,
                'atk' => 20.0,
                'def' => 10.0,
                'weak' => '物理',
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
        ]);
    }
}

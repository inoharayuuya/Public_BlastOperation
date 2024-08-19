<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class WeaponMasterSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        DB::table('weapon_masters')->insert([
            [
                'name' => "剣1",
                'name_id' => "sword_1",
                'attr' => "物理",
                'atk_rate' => 1.05,
                'speed_rate' => 1.05,
                'range' => "近距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "剣2",
                'name_id' => "sword_2",
                'attr' => "物理",
                'atk_rate' => 1.075,
                'speed_rate' => 0.925,
                'range' => "近距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "斧",
                'name_id' => "axe_1",
                'attr' => "物理",
                'atk_rate' => 1.1,
                'speed_rate' => 0.85,
                'range' => "近距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "杖1",
                'name_id' => "rod_1",
                'attr' => "魔法",
                'atk_rate' => 1.025,
                'speed_rate' => 1.0,
                'range' => "遠距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "杖2",
                'name_id' => "rod_2",
                'attr' => "魔法",
                'atk_rate' => 1.125,
                'speed_rate' => 0.9,
                'range' => "遠距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "本",
                'name_id' => "grimoire_1",
                'attr' => "回復",
                'atk_rate' => 5.0,
                'speed_rate' => 0.85,
                'range' => "遠距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "スライム：突進",
                'name_id' => "slime_1",
                'attr' => "物理",
                'atk_rate' => 1.025,
                'speed_rate' => 0.95,
                'range' => "近距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "子ドラゴン：炎",
                'name_id' => "child_dragon_1",
                'attr' => "魔法",
                'atk_rate' => 1.15,
                'speed_rate' => 0.825,
                'range' => "遠距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "太刀",
                'name_id' => "long_sword_1",
                'attr' => "物理",
                'atk_rate' => 1.075,
                'speed_rate' => 0.95,
                'range' => "近距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "短刀",
                'name_id' => "short_sword_1",
                'attr' => "物理",
                'atk_rate' => 1.0,
                'speed_rate' => 1.1,
                'range' => "近距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "ゴーレム：突進",
                'name_id' => "golem_1",
                'attr' => "物理",
                'atk_rate' => 1.225,
                'speed_rate' => 0.8,
                'range' => "近距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "杖3",
                'name_id' => "rod_3",
                'attr' => "魔法",
                'atk_rate' => 1.2,
                'speed_rate' => 0.925,
                'range' => "遠距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "アスカロン",
                'name_id' => "ascalon",
                'attr' => "物理",
                'atk_rate' => 1.7,
                'speed_rate' => 0.875,
                'range' => "近距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "グラム",
                'name_id' => "gram",
                'attr' => "魔法",
                'atk_rate' => 1.525,
                'speed_rate' => 1.05,
                'range' => "近距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "爪",
                'name_id' => "claw_1",
                'attr' => "物理",
                'atk_rate' => 1.3,
                'speed_rate' => 1.0,
                'range' => "近距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'name' => "炎",
                'name_id' => "flame_1",
                'attr' => "魔法",
                'atk_rate' => 1.4,
                'speed_rate' => 0.925,
                'range' => "遠距離",
                'special_effect_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
        ]);
    }
}

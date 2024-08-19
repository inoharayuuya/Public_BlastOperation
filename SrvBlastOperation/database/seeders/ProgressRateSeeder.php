<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class ProgressRateSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        DB::table('progress_rates')->insert([
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'stage_id' => 1,
                'is_clear' => 0,
                'is_order' => 1,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'stage_id' => 2,
                'is_clear' => 0,
                'is_order' => 1,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'stage_id' => 3,
                'is_clear' => 0,
                'is_order' => 1,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'stage_id' => 1,
                'is_clear' => 0,
                'is_order' => 1,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'stage_id' => 2,
                'is_clear' => 0,
                'is_order' => 1,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'stage_id' => 3,
                'is_clear' => 0,
                'is_order' => 1,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
        ]);
    }
}

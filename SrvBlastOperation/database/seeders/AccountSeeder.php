<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class AccountSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        DB::table('accounts')->insert([
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'is_login' => 0,
                'auto_flg' => 0,
                'name' => "admin",
                'password' => "admin",
                'rank' => "F",
                'energy' => 50,
                'crystal' => 0,
                'coin' => 0,
                'total_rank_point' => 0,
                'next_rank_point' => 10,
                'comment' => 'よろしくお願いします！',
                'icon_id' => 0,
                'party_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'is_login' => 0,
                'auto_flg' => 0,
                'name' => "test_user1",
                'password' => "test_user1",
                'rank' => "F",
                'energy' => 50,
                'crystal' => 0,
                'coin' => 0,
                'total_rank_point' => 0,
                'next_rank_point' => 10,
                'comment' => 'よろしくお願いします！',
                'icon_id' => 0,
                'party_id' => 0,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
        ]);
    }
}

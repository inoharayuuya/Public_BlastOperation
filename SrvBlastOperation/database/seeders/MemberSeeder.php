<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class MemberSeeder extends Seeder
{
    /**
     * Run the database seeds.
     *
     * @return void
     */
    public function run()
    {
        DB::table('members')->insert([
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'party_id' => 1,
                'has_character_id' => 1,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'party_id' => 1,
                'has_character_id' => 2,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'party_id' => 1,
                'has_character_id' => 3,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'party_id' => 1,
                'has_character_id' => 4,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'party_id' => 2,
                'has_character_id' => 1,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'party_id' => 2,
                'has_character_id' => 2,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'party_id' => 2,
                'has_character_id' => 3,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                'party_id' => 2,
                'has_character_id' => 4,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'party_id' => 1,
                'has_character_id' => 5,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'party_id' => 1,
                'has_character_id' => 6,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'party_id' => 1,
                'has_character_id' => 7,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'party_id' => 1,
                'has_character_id' => 8,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'party_id' => 2,
                'has_character_id' => 5,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'party_id' => 2,
                'has_character_id' => 6,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'party_id' => 2,
                'has_character_id' => 7,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
            [
                'user_id' => "9f77e4bd38f2520954cef862f98c9780c987af4708bb1131c0f2d719ea23d581",
                'party_id' => 2,
                'has_character_id' => 8,
                'created_at' => DB::raw('NOW()'),
                'updated_at' => DB::raw('NOW()'),
            ],
        ]);
    }
}

<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Database\Seeders\AccountSeeder;
use Database\Seeders\CharacterMasterSeeder;
use Database\Seeders\WeaponMasterSeeder;
use Database\Seeders\EnemyMasterSeeder;
use Database\Seeders\StageMasterSeeder;
use Database\Seeders\PartyInfoSeeder;
use Database\Seeders\MemberSeeder;
use Database\Seeders\HasCharacterSeeder;
use Database\Seeders\ProgressRateSeeder;
use Database\Seeders\StageInEnemySeeder;
use Database\Seeders\EnemyAtkMasterSeeder;
use Database\Seeders\EnemyAtkPatternMasterSeeder;

class DatabaseSeeder extends Seeder
{
    /**
     * Seed the application's database.
     *
     * @return void
     */
    public function run()
    {
        // \App\Models\User::factory(10)->create();
        $this->call([
            AccountSeeder::class,
            CharacterMasterSeeder::class,
            WeaponMasterSeeder::class,
            EnemyMasterSeeder::class,
            StageMasterSeeder::class,
            PartyInfoSeeder::class,
            MemberSeeder::class,
            HasCharacterSeeder::class,
            ProgressRateSeeder::class,
            StageInEnemySeeder::class,
            EnemyAtkMasterSeeder::class,
            EnemyAtkPatternMasterSeeder::class,
        ]);
    }
}

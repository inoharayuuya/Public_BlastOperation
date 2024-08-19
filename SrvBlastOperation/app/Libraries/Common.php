<?php

namespace App\Libraries;

class Common{
    public const RANK = [
        'RANK_F' => 1,
        'RANK_E' => 2,
        'RANK_D' => 3,
        'RANK_C' => 4,
        'RANK_B' => 5,
        'RANK_A' => 6,
        'RANK_S' => 7,
    ];

    // ランクアップに必要なポイント数
    public const NEXT_RANK_POINTS = [
        10,    // Eランク達成に必要なポイント数
        100,   // Dランク達成に必要なポイント数
        500,   // Cランク達成に必要なポイント数
        1000,  // Bランク達成に必要なポイント数
        2000,  // Aランク達成に必要なポイント数
        10000, // Sランク達成に必要なポイント数
        0,
    ];

    public const CHARACTER_IDS = [
        5,5,6,6,7,7,8,8,9,10
    ];
}

class Rank{
    public const RANK_F = 'F';
    public const RANK_E = 'E';
    public const RANK_D = 'D';
    public const RANK_C = 'C';
    public const RANK_B = 'B';
    public const RANK_A = 'A';
    public const RANK_S = 'S';
}

?>
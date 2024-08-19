<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateEnemyAtkPatternMastersTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('enemy_atk_pattern_masters', function (Blueprint $table) {
            $table->id();
            $table->integer('enemy_id');
            $table->integer('atk_id');
            $table->string('enemy_name');
            $table->string('atk_name');
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('enemy_atk_pattern_masters');
    }
}

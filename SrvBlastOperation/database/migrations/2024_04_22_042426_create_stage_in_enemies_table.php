<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateStageInEnemiesTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('stage_in_enemies', function (Blueprint $table) {
            $table->id();
            $table->integer('stage_id');
            $table->integer('enemy_id');
            $table->decimal('x');
            $table->decimal('y');
            $table->decimal('z');
            $table->integer('floor');
            $table->boolean('is_boss');
            $table->decimal('atk_rate');
            $table->decimal('hp_rate');
            $table->decimal('size');
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
        Schema::dropIfExists('stage_in_enemies');
    }
}

<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateEnemyAtkMastersTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('enemy_atk_masters', function (Blueprint $table) {
            $table->id();
            $table->string('name');
            $table->decimal('atk_rate');
            $table->string('atk_attr');
            $table->integer('atk_area_id');
            $table->integer('atk_id');
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
        Schema::dropIfExists('enemy_atk_masters');
    }
}

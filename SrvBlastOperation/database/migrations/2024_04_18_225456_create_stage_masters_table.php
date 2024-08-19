<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateStageMastersTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('stage_masters', function (Blueprint $table) {
            $table->id();
            $table->string('name');
            $table->string('note');
            $table->integer('use_energy');
            $table->integer('rank_point');
            $table->integer('coin');
            $table->integer('crystal');
            $table->integer('max_floor');
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
        Schema::dropIfExists('stage_masters');
    }
}

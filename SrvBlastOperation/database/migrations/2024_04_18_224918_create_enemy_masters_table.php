<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateEnemyMastersTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('enemy_masters', function (Blueprint $table) {
            $table->id();
            $table->string('name');
            $table->string('name_id');
            $table->decimal('hp');
            $table->decimal('atk');
            $table->decimal('def');
            $table->string('weak');
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
        Schema::dropIfExists('enemy_masters');
    }
}

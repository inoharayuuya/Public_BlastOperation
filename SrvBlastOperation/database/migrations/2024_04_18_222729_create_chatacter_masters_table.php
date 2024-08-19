<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateChatacterMastersTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('character_masters', function (Blueprint $table) {
            $table->id();
            $table->string('rank');
            $table->string('name');
            $table->string('name_id');
            $table->integer('hp');
            $table->decimal('atk');
            $table->decimal('speed');
            $table->integer('physical_def');
            $table->integer('magical_def');
            $table->integer('first_weapon_id');
            $table->integer('second_weapon_id');
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
        Schema::dropIfExists('character_masters');
    }
}

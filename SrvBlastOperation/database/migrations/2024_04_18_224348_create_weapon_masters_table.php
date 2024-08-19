<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateWeaponMastersTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('weapon_masters', function (Blueprint $table) {
            $table->id();
            $table->string('name');
            $table->string('name_id');
            $table->string('attr');
            $table->decimal('atk_rate');
            $table->decimal('speed_rate');
            $table->string('range');
            $table->integer('special_effect_id');
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
        Schema::dropIfExists('weapon_masters');
    }
}

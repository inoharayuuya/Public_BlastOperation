<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateAccountsTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('accounts', function (Blueprint $table) {
            $table->id();
            $table->string('user_id');
            $table->boolean('is_login');
            $table->boolean('auto_flg');
            $table->string('name');
            $table->string('password');
            $table->string('rank');
            $table->integer('energy');
            $table->integer('crystal');
            $table->integer('coin');
            $table->integer('total_rank_point');
            $table->integer('next_rank_point');
            $table->string('comment');
            $table->integer('icon_id');
            $table->integer('party_id');
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
        Schema::dropIfExists('accounts');
    }
}

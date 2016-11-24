class CreateWatermarks < ActiveRecord::Migration
  def change
    create_table :watermarks do |t|
      t.string :watermark
      t.string :original_image

      t.timestamps null: false
    end
  end
end

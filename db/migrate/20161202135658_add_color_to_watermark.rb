class AddColorToWatermark < ActiveRecord::Migration
  def change
    add_column :watermarks, :color, :boolean, default: false
  end
end

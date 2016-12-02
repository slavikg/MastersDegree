class AddPsnrAndSsimToWatermark < ActiveRecord::Migration
  def change
    add_column :watermarks, :origin_psnr, :string
    add_column :watermarks, :origin_ssim, :string
    add_column :watermarks, :watermark_psnr, :string
    add_column :watermarks, :watermark_ssim, :string
  end
end

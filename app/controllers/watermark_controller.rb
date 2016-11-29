class WatermarkController < ApplicationController
  require 'open3'
  before_action :watermark, only: [:show, :encrypt, :decrypt]

  def index
  end

  def show
    methods = ImageAttack.methods - Object.methods
    @new_image_paths = methods.map { |method| {name: method, path: ImageAttack.send(method, @watermark.original_image, @watermark.id)} }
  end

  def encrypt
    dir = path_to_result.join("#{watermark.id}")
    Dir.mkdir(dir) unless File.directory?(dir)
    @name_action = 'encrypt'
    @original_image_path = watermark.original_image.path
    @watermark_path = watermark.watermark.path
    @key_path_with_name = dir.to_s + '/key.pac'
    @encrypt_image_path_with_name = dir.to_s + '/encrypt_image.bmp'
    @difference_image_between_original_image_and_result_image_path_with_name = dir.to_s + '/difference_original_image.bmp'
    @args = "'#{@name_action}' '#{@original_image_path}' '#{@watermark_path}' '#{@key_path_with_name}' '#{@encrypt_image_path_with_name}' '#{@difference_image_between_original_image_and_result_image_path_with_name}'"
    @psnr = @ssim = nil
    ::Open3.popen3({"MYVAR" => "a_value"},
                   "/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono ~/Desktop/test13.exe #{@args}") do |i, o, e, w|
      i.close
      data = o.read
      data.slice! 'Infinity'
      split_data = data.split
      @psnr = split_data[1]
      @ssim = split_data[3]
      o.close
      e.close
      w.value.exitstatus
    end
  end

  def decrypt
    dir = path_to_result.join("#{watermark.id}")
    Dir.mkdir(dir) unless File.directory?(dir)
    @name_action = 'decrypt'
    @encrypt_image_path_with_name = dir.to_s + '/encrypt_image.bmp'
    @watermark_path = watermark.watermark.path
    @key_path_with_name = dir.to_s + '/key.pac'
    @watermark_after_decrypt_path_with_name = dir.to_s + '/watermark_after_decrypt.bmp'
    @difference_image_between_original_watermark_and_result_watermark_path_with_name = dir.to_s + '/difference_watermark_image.bmp'
    @args = "'#{@name_action}' '#{@encrypt_image_path_with_name}' '#{@watermark_path}' '#{@key_path_with_name}' '#{@watermark_after_decrypt_path_with_name}' '#{@difference_image_between_original_watermark_and_result_watermark_path_with_name}'"
    ::Open3.popen3({"MYVAR" => "a_value"},
                   "/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono ~/Desktop/test13.exe #{@args}") do |i, o, e, w|
      i.close
      data = o.read
      split_data = data.split
      @psnr = split_data[1]
      @ssim = split_data[3]
      o.close
      e.close
      w.value.exitstatus
    end
  end

  def new
    @watermark = Watermark.new
  end

  def create
    watermark = Watermark.new watermark_params
    if watermark.save!
      redirect_to watermark_path(watermark)
    else
      render :new
    end
  end

  private

  def path_to_result
    @_path_to_result ||= Rails.root.join('public', 'result')
  end

  def watermark
    @watermark ||= Watermark.find params[:id]
  end

  def watermark_params
    params.require(:watermark).permit(:watermark, :original_image)
  end
end

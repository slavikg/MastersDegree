class WatermarkController < ApplicationController
  require 'open3'
  before_action :watermark, only: [:show, :encrypt, :decrypt, :decrypt_with_attack]
  before_action :dir, only: [:show, :encrypt, :decrypt, :decrypt_with_attack]

  def index
  end

  def show
  end

  def encrypt
    name_action('encrypt')
    @original_image_path = watermark.original_image.path
    @watermark_path = watermark.watermark.path
    @key_path_with_name = dir.to_s + '/key.pac'
    @encrypt_image_path_with_name = dir.to_s + '/encrypt_image.bmp'
    @difference_image_between_original_image_and_result_image_path_with_name = dir.to_s + '/difference_original_image.bmp'
    @args = "'#{@name_action}' '#{@original_image_path}' '#{@watermark_path}' '#{@key_path_with_name}' '#{@encrypt_image_path_with_name}' '#{@difference_image_between_original_image_and_result_image_path_with_name}'"
    @psnr = @ssim = nil
    ::Open3.popen3({"MYVAR" => "a_value"},
                   "/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono vendor/encrypt/test19.exe #{@args}") do |i, o, e, w|
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
    @watermark.update_attributes(origin_psnr: @psnr, origin_ssim: @ssim)
  end

  def decrypt
    name_action('decrypt')
    @encrypt_image_path_with_name = dir.to_s + '/encrypt_image.bmp'
    @watermark_path = watermark.watermark.path
    @key_path_with_name = dir.to_s + '/key.pac'
    @watermark_after_decrypt_path_with_name = dir.to_s + '/watermark_after_decrypt.bmp'
    @difference_image_between_original_watermark_and_result_watermark_path_with_name = dir.to_s + '/difference_watermark_image.bmp'
    @args = "'#{@name_action}' '#{@encrypt_image_path_with_name}' '#{@watermark_path}' '#{@key_path_with_name}' '#{@watermark_after_decrypt_path_with_name}' '#{@difference_image_between_original_watermark_and_result_watermark_path_with_name}'"
    ::Open3.popen3({"MYVAR" => "a_value"},
                   "/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono vendor/encrypt/test19.exe #{@args}") do |i, o, e, w|
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
    @watermark.update_attributes(watermark_psnr: @psnr, watermark_ssim: @ssim)
  end

  def decrypt_with_attack
    redirect_to root_path unless attack
    if attack_not_all?
      name_action('decrypt')
      @attack_name = attack_name
      @attacked_image_path = ImageAttack.send(attack, @watermark.original_image, @watermark.id)
      @encrypt_image_path_with_name = path_to_public.to_s + @attacked_image_path
      @watermark_path = watermark.watermark.path
      @key_path_with_name = dir.to_s + '/key.pac'
      watermark_after_decrypt_with_name = "/result/#{watermark.id}/watermark_after_decrypt_with_#{attack}_attack.bmp"
      @watermark_after_decrypt_path_with_name = path_to_public.to_s + watermark_after_decrypt_with_name
      difference_image_between_original_watermark_and_result_watermark_with_name = "/result/#{watermark.id}/difference_watermark_image_after_#{attack}_attack.bmp"
      @difference_image_between_original_watermark_and_result_watermark_path_with_name = path_to_public.to_s + difference_image_between_original_watermark_and_result_watermark_with_name
      @args = "'#{@name_action}' '#{@encrypt_image_path_with_name}' '#{@watermark_path}' '#{@key_path_with_name}' '#{@watermark_after_decrypt_path_with_name}' '#{@difference_image_between_original_watermark_and_result_watermark_path_with_name}'"
      psnr = ssim = nil
      ::Open3.popen3({"MYVAR" => "a_value"},
                     "/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono vendor/encrypt/test19.exe #{@args}") do |i, o, e, w|
        i.close
        data = o.read
        data.slice! 'Infinity'
        split_data = data.split
        psnr = split_data[1]
        ssim = split_data[3]
        o.close
        e.close
        w.value.exitstatus
      end
      @result = [
        {
          attack_name: @attack_name,
          attacked_image_path: @attacked_image_path,
          watermark_after_decrypt_with_name: watermark_after_decrypt_with_name,
          difference_image_between_original_watermark_and_result_watermark_with_name: difference_image_between_original_watermark_and_result_watermark_with_name,
          psnr: psnr,
          ssim: ssim
        }
      ]
    else
      name_action('decrypt')
      @attacked_image_pathes_with_attacks = ImageAttack.all(@watermark.original_image, @watermark.id)
      @watermark_path = watermark.watermark.path
      @key_path_with_name = dir.to_s + '/key.pac'
      @result = @attacked_image_pathes_with_attacks.map do |attacked_image_path_with_attack|
        attack = attacked_image_path_with_attack[:attack]
        attacked_image_path = attacked_image_path_with_attack[:path]
        @encrypt_image_path_with_name = path_to_public.to_s + attacked_image_path
        watermark_after_decrypt_with_name = "/result/#{watermark.id}/watermark_after_decrypt_with_#{attack}_attack.bmp"
        watermark_after_decrypt_path_with_name = path_to_public.to_s + watermark_after_decrypt_with_name
        difference_image_between_original_watermark_and_result_watermark_with_name = "/result/#{watermark.id}/difference_watermark_image_after_#{attack}_attack.bmp"
        difference_image_between_original_watermark_and_result_watermark_path_with_name = path_to_public.to_s + difference_image_between_original_watermark_and_result_watermark_with_name
        @args = "'#{@name_action}' '#{@encrypt_image_path_with_name}' '#{@watermark_path}' '#{@key_path_with_name}' '#{watermark_after_decrypt_path_with_name}' '#{difference_image_between_original_watermark_and_result_watermark_path_with_name}'"
        psnr = ssim = nil
        ::Open3.popen3({"MYVAR" => "a_value"},
                       "/Library/Frameworks/Mono.framework/Versions/Current/Commands/mono vendor/encrypt/test19.exe #{@args}") do |i, o, e, w|
          i.close
          data = o.read
          data.slice! 'Infinity'
          split_data = data.split
          psnr = split_data[1]
          ssim = split_data[3]
          o.close
          e.close
          w.value.exitstatus
        end
        {
          attack_name: attack_name(attack),
          attacked_image_path: attacked_image_path,
          watermark_after_decrypt_with_name: watermark_after_decrypt_with_name,
          difference_image_between_original_watermark_and_result_watermark_with_name: difference_image_between_original_watermark_and_result_watermark_with_name,
          psnr: psnr,
          ssim: ssim
        }
      end
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

  def name_action(action)
    @name_action = @watermark.color ? "color-#{action}" : action
  end

  def attack_not_all?
    @is_attack_not_all ||= attack != 'all'
  end

  def attack
    @_attack ||= params[:attack]
  end

  def attack_name(name = nil)
    new_attack_name = name || attack
    new_attack_name.to_s.tr('_', ' ').capitalize
  end

  def dir
    @_dir ||= begin
      dir = path_to_result.join("#{watermark.id}")
      Dir.mkdir(dir) unless File.directory?(dir)
      dir
    end
  end

  def path_to_result
    @_path_to_result ||= Rails.root.join('public', 'result')
  end

  def path_to_public
    @_path_to_public ||= Rails.root.join('public')
  end

  def watermark
    @watermark ||= Watermark.find params[:id]
  end

  def watermark_params
    params.require(:watermark).permit(:watermark, :original_image, :color)
  end
end

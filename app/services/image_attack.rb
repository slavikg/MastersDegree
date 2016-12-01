class ImageAttack
  class << self
    def crop_and_big_resize(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      old_height = img[:height]
      old_width = img[:width]
      img.crop("#{old_width - 1}x#{old_height - 1}+0+0")
      img.resize("#{old_width}x#{old_height}")
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'crop_and_big_resize.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/crop_and_big_resize.bmp"
    end

    def big_resize_and_crop(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      old_height = img[:height]
      old_width = img[:width]
      img.resize("#{old_width + 1}x#{old_height + 1}")
      img.crop("#{old_width}x#{old_height}+0+0")
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'big_resize_and_crop.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/big_resize_and_crop.bmp"
    end

    def small_resize_and_big_resize(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      old_height = img[:height]
      old_width = img[:width]
      img.resize("#{old_width - 1}x#{old_height - 1}")
      img.resize("#{old_width}x#{old_height}")
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'small_resize_and_big_resize.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/small_resize_and_big_resize.bmp"
    end

    def big_resize_and_small_resize(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      old_height = img[:height]
      old_width = img[:width]
      img.resize("#{old_width + 1}x#{old_height + 1}")
      img.resize("#{old_width}x#{old_height}")
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'big_resize_and_small_resize.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/big_resize_and_small_resize.bmp"
    end

    def flop(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      img.flop
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'flop.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/flop.bmp"
    end

    def rotate_to_right(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      img.rotate '90'
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'rotate_to_right.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/rotate_to_right.bmp"
    end

    def rotate_to_left(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      img.rotate '-90'
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'rotate_to_left.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/rotate_to_left.bmp"
    end

    def flip(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      img.flip
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'flip.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/flip.bmp"
    end

    def convert_from_bmp_to_jpg_and_back(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'convert_from_bmp_to_jpg_and_back.jpg')
      img.format 'jpg'
      img.write(new_image_path)
      img = MiniMagick::Image.open(new_image_path)
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'convert_from_bmp_to_jpg_and_back.bmp')
      img.format 'BMP3'
      img.write(new_image_path)
      "/result/#{element_id}/convert_from_bmp_to_jpg_and_back.bmp"
    end

    def change_depth_by_one(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      img.depth '99'
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'change_depth_by_one.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/change_depth_by_one.bmp"
    end

    def blur(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      img.blur '0x1'
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'blur.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/blur.bmp"
    end

    def sharpen(img, element_id, image_path = nil)
      dir = "#{path_to_result}/#{element_id}/encrypt_image.bmp"
      img = MiniMagick::Image.open(dir)
      img.sharpen '0x1'
      img.format 'BMP3'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'sharpen.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/sharpen.bmp"
    end

    def all(img, element_id, image_path = nil)
      attacks =  self.methods - Object.methods - [:all]
      attacks.map { |attack| { attack: attack, path: self.send(attack, img, element_id) } }
    end

    private

    def path_to_result
      @_path_to_result ||= Rails.root.join('public', 'result').to_s
    end
  end
end

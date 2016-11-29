class ImageAttack
  class << self
    def crop_and_big_resize(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      old_height = img[:height]
      old_width = img[:width]
      img.crop("#{old_width - 1}x#{old_height - 1}+0+0")
      img.resize("#{old_width}x#{old_height}")
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'crop_and_big_resize.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/crop_and_big_resize.bmp"
    end

    def big_resize_and_crop(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      old_height = img[:height]
      old_width = img[:width]
      img.resize("#{old_width + 1}x#{old_height + 1}")
      img.crop("#{old_width}x#{old_height}+0+0")
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'big_resize_and_crop.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/big_resize_and_crop.bmp"
    end

    def small_resize_and_big_resize(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      old_height = img[:height]
      old_width = img[:width]
      img.resize("#{old_width - 1}x#{old_height - 1}")
      img.resize("#{old_width}x#{old_height}")
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'small_resize_and_big_resize.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/small_resize_and_big_resize.bmp"
    end

    def big_resize_and_small_resize(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      old_height = img[:height]
      old_width = img[:width]
      img.resize("#{old_width + 1}x#{old_height + 1}")
      img.resize("#{old_width}x#{old_height}")
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'big_resize_and_small_resize.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/big_resize_and_small_resize.bmp"
    end

    def flop(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      img.flop
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'flop.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/flop.bmp"
    end

    def rotate_to_right(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      img.rotate '90'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'rotate_to_right.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/rotate_to_right.bmp"
    end

    def rotate_to_left(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      img.rotate '-90'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'rotate_to_left.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/rotate_to_left.bmp"
    end

    def flip(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      img.flip
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'flip.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/flip.bmp"
    end

    def convert_from_bmp_to_jpg_and_back(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'convert_from_bmp_to_jpg_and_back.jpg')
      img.format 'jpg'
      img.write(new_image_path)
      img = MiniMagick::Image.open(new_image_path)
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'convert_from_bmp_to_jpg_and_back.png')
      img.format 'png'
      img.write(new_image_path)
      "/result/#{element_id}/convert_from_bmp_to_jpg_and_back.png"
    end

    def change_depth_by_one(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      img.depth '99'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'change_depth_by_one.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/change_depth_by_one.bmp"
    end

    def blur(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      img.blur '0x1'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'blur.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/blur.bmp"
    end

    def sharpen(img, element_id, image_path = nil)
      img = MiniMagick::Image.open(img.path)
      img ||= MiniMagick::Image.open(image_path)
      img.sharpen '0x1'
      new_image_path = Rails.root.join('public', 'result', "#{element_id}", 'sharpen.bmp')
      img.write(new_image_path)
      "/result/#{element_id}/sharpen.bmp"
    end
  end
end

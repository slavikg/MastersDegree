class Watermark < ActiveRecord::Base
  mount_uploader :original_image, ImageUploader
  mount_uploader :watermark, ImageUploader
end

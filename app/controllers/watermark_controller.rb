class WatermarkController < ApplicationController
  before_action :watermark, only: :show

  def index
  end

  def show
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

  def watermark
    @watermark = Watermark.find params[:id]
  end

  def watermark_params
    params.require(:watermark).permit(:watermark, :original_image)
  end
end

using System;
using System.IO;
using ImageMagick;
using Naheulbook.Core.Configurations;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Utils;

public interface IMapImageUtil
{
    MapImageData SplitMapImage(Stream imageStream, int mapId);
}

public class MapImageUtil(MapImageConfiguration configuration) : IMapImageUtil
{
    public MapImageData SplitMapImage(Stream imageStream, int mapId)
    {
        using (var image = new MagickImage(imageStream))
        {
            var zoomCount = ComputeZoomCount(image);
            var zoomNumber = zoomCount + configuration.ExtraZoomCount;
            var sizePercentage = 100d * Math.Pow(2, configuration.ExtraZoomCount);
            var mapDirectoryPath = Path.Combine(configuration.OutputDirectory, mapId.ToString());
            Directory.CreateDirectory(mapDirectoryPath);

            while (zoomNumber >= 0)
            {
                var copy = image.Clone();
                copy.Resize((int) (image.Width * sizePercentage / 100), (int) (image.Height * sizePercentage / 100));
                var tiles = copy.CropToTiles(configuration.TilesSize, configuration.TilesSize);

                var zoomDirectoryPath = Path.Combine(mapDirectoryPath, zoomNumber.ToString());
                Directory.CreateDirectory(zoomDirectoryPath);

                foreach (var tile in tiles)
                {
                    var fileX = tile.Page.X / configuration.TilesSize;
                    var fileY = tile.Page.Y / configuration.TilesSize;
                    var filePath = Path.Combine(zoomDirectoryPath, fileX + "_" + fileY + ".png");
                    tile.BackgroundColor = MagickColors.Transparent;
                    tile.Extent(configuration.TilesSize, configuration.TilesSize, Gravity.Northwest);
                    tile.Write(filePath);
                }

                zoomNumber--;
                sizePercentage /= 2;
            }
            return new MapImageData
            {
                Width = image.Width,
                Height = image.Height,
                ZoomCount = zoomCount,
                ExtraZoomCount = configuration.ExtraZoomCount,
            };
        }
    }

    private int ComputeZoomCount(IMagickImage image)
    {
        var zoomCount = 0;
        var width = image.Width;
        while (width > configuration.MinZoomMapSize)
        {
            zoomCount++;
            width /= 2;
        }

        return zoomCount;
    }
}
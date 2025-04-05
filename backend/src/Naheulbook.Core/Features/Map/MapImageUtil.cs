using System;
using System.IO;
using ImageMagick;
using Microsoft.Extensions.Options;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Core.Features.Map;

public interface IMapImageUtil
{
    MapImageData SplitMapImage(Stream imageStream, int mapId);
}

public class MapImageUtil(IOptions<MapImageOptions> options) : IMapImageUtil
{
    public MapImageData SplitMapImage(Stream imageStream, int mapId)
    {
        using var image = new MagickImage(imageStream);
        var zoomCount = ComputeZoomCount(image);
        var zoomNumber = zoomCount + options.Value.ExtraZoomCount;
        var sizePercentage = 100d * Math.Pow(2, options.Value.ExtraZoomCount);
        var mapDirectoryPath = Path.Combine(options.Value.OutputDirectory, mapId.ToString());
        Directory.CreateDirectory(mapDirectoryPath);

        while (zoomNumber >= 0)
        {
            var copy = image.Clone();
            copy.Resize((uint)(image.Width * sizePercentage / 100), (uint)(image.Height * sizePercentage / 100));
            var tiles = copy.CropToTiles(options.Value.TilesSize, options.Value.TilesSize);

            var zoomDirectoryPath = Path.Combine(mapDirectoryPath, zoomNumber.ToString());
            Directory.CreateDirectory(zoomDirectoryPath);

            foreach (var tile in tiles)
            {
                var fileX = tile.Page.X / options.Value.TilesSize;
                var fileY = tile.Page.Y / options.Value.TilesSize;
                var filePath = Path.Combine(zoomDirectoryPath, fileX + "_" + fileY + ".png");
                tile.BackgroundColor = MagickColors.Transparent;
                tile.Extent(options.Value.TilesSize, options.Value.TilesSize, Gravity.Northwest);
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
            ExtraZoomCount = options.Value.ExtraZoomCount,
        };
    }

    private int ComputeZoomCount(IMagickImage image)
    {
        var zoomCount = 0;
        var width = image.Width;
        while (width > options.Value.MinZoomMapSize)
        {
            zoomCount++;
            width /= 2;
        }

        return zoomCount;
    }
}
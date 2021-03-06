// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using SixLabors.ImageSharp.Tests.TestUtilities.ImageComparison;

using Xunit;

// ReSharper disable InconsistentNaming
namespace SixLabors.ImageSharp.Tests.Processing.Processors.Binarization
{
    public class BinaryDitherTests
    {
        public static readonly string[] CommonTestImages =
            {
                TestImages.Png.CalliphoraPartial, TestImages.Png.Bike
            };

        public static readonly TheoryData<string, IOrderedDither> OrderedDitherers = new TheoryData<string, IOrderedDither>
        {
            { "Bayer8x8", KnownDitherers.BayerDither8x8 },
            { "Bayer4x4", KnownDitherers.BayerDither4x4 },
            { "Ordered3x3", KnownDitherers.OrderedDither3x3 },
            { "Bayer2x2", KnownDitherers.BayerDither2x2 }
        };

        public static readonly TheoryData<string, IErrorDiffuser> ErrorDiffusers = new TheoryData<string, IErrorDiffuser>
        {
            { "Atkinson", KnownDiffusers.Atkinson },
            { "Burks", KnownDiffusers.Burks },
            { "FloydSteinberg", KnownDiffusers.FloydSteinberg },
            { "JarvisJudiceNinke", KnownDiffusers.JarvisJudiceNinke },
            { "Sierra2", KnownDiffusers.Sierra2 },
            { "Sierra3", KnownDiffusers.Sierra3 },
            { "SierraLite", KnownDiffusers.SierraLite },
            { "StevensonArce", KnownDiffusers.StevensonArce },
            { "Stucki", KnownDiffusers.Stucki },
        };

        public const PixelTypes TestPixelTypes = PixelTypes.Rgba32 | PixelTypes.Bgra32 | PixelTypes.Rgb24;

        private static IOrderedDither DefaultDitherer => KnownDitherers.BayerDither4x4;

        private static IErrorDiffuser DefaultErrorDiffuser => KnownDiffusers.Atkinson;

        [Theory]
        [WithFileCollection(nameof(CommonTestImages), nameof(OrderedDitherers), PixelTypes.Rgba32)]
        [WithTestPatternImages(nameof(OrderedDitherers), 100, 100, PixelTypes.Rgba32)]
        public void BinaryDitherFilter_WorksWithAllDitherers<TPixel>(TestImageProvider<TPixel> provider, string name, IOrderedDither ditherer)
            where TPixel : struct, IPixel<TPixel>
        {
            using (Image<TPixel> image = provider.GetImage())
            {
                image.Mutate(x => x.BinaryDither(ditherer));
                image.DebugSave(provider, name);
            }
        }

        [Theory]
        [WithFileCollection(nameof(CommonTestImages), nameof(ErrorDiffusers), PixelTypes.Rgba32)]
        [WithTestPatternImages(nameof(ErrorDiffusers), 100, 100, PixelTypes.Rgba32)]
        public void DiffusionFilter_WorksWithAllErrorDiffusers<TPixel>(TestImageProvider<TPixel> provider, string name, IErrorDiffuser diffuser)
            where TPixel : struct, IPixel<TPixel>
        {
            using (Image<TPixel> image = provider.GetImage())
            {
                image.Mutate(x => x.BinaryDiffuse(diffuser, .5F));
                image.DebugSave(provider, name);
            }
        }

        [Theory]
        [WithFile(TestImages.Png.Bike, TestPixelTypes)]
        public void BinaryDitherFilter_ShouldNotDependOnSinglePixelType<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : struct, IPixel<TPixel>
        {
            using (Image<TPixel> image = provider.GetImage())
            {
                image.Mutate(x => x.BinaryDither(DefaultDitherer));
                image.DebugSave(provider);
            }
        }

        [Theory]
        [WithFile(TestImages.Png.Bike, TestPixelTypes)]
        public void DiffusionFilter_ShouldNotDependOnSinglePixelType<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : struct, IPixel<TPixel>
        {
            using (Image<TPixel> image = provider.GetImage())
            {
                image.Mutate(x => x.BinaryDiffuse(DefaultErrorDiffuser, 0.5f));
                image.DebugSave(provider);
            }
        }

        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, PixelTypes.Rgba32)]
        public void ApplyDitherFilterInBox<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : struct, IPixel<TPixel>
        {
            using (Image<TPixel> source = provider.GetImage())
            using (Image<TPixel> image = source.Clone())
            {
                var bounds = new Rectangle(10, 10, image.Width / 2, image.Height / 2);

                image.Mutate(x => x.BinaryDither(DefaultDitherer, bounds));
                image.DebugSave(provider);

                ImageComparer.Tolerant().VerifySimilarityIgnoreRegion(source, image, bounds);
            }
        }

        [Theory]
        [WithFile(TestImages.Png.CalliphoraPartial, PixelTypes.Rgba32)]
        public void ApplyDiffusionFilterInBox<TPixel>(TestImageProvider<TPixel> provider)
            where TPixel : struct, IPixel<TPixel>
        {
            using (Image<TPixel> source = provider.GetImage())
            using (Image<TPixel> image = source.Clone())
            {
                var bounds = new Rectangle(10, 10, image.Width / 2, image.Height / 2);

                image.Mutate(x => x.BinaryDiffuse(DefaultErrorDiffuser, .5F, bounds));
                image.DebugSave(provider);

                ImageComparer.Tolerant().VerifySimilarityIgnoreRegion(source, image, bounds);
            }
        }
    }
}

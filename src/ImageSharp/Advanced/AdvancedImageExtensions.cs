// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Runtime.InteropServices;

using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Advanced
{
    /// <summary>
    /// Extension methods over Image{TPixel}
    /// </summary>
    public static class AdvancedImageExtensions
    {
        /// <summary>
        /// Accepts a <see cref="IImageVisitor"/> to implement a double-dispatch pattern in order to
        /// apply pixel-specific operations on non-generic <see cref="Image"/> instances
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="visitor">The visitor.</param>
        public static void AcceptVisitor(this Image source, IImageVisitor visitor)
            => source.Accept(visitor);

        /// <summary>
        /// Gets the configuration for the image.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>Returns the configuration.</returns>
        public static Configuration GetConfiguration(this Image source)
            => GetConfiguration((IConfigurationProvider)source);

        /// <summary>
        /// Gets the configuration for the image frame.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>Returns the configuration.</returns>
        public static Configuration GetConfiguration(this ImageFrame source)
            => GetConfiguration((IConfigurationProvider)source);

        /// <summary>
        /// Gets the configuration .
        /// </summary>
        /// <param name="source">The source image</param>
        /// <returns>Returns the bounds of the image</returns>
        private static Configuration GetConfiguration(IConfigurationProvider source)
            => source?.Configuration ?? Configuration.Default;

        /// <summary>
        /// Gets the representation of the pixels as a <see cref="Span{T}"/> of contiguous memory in the source image's pixel format
        /// stored in row major order.
        /// </summary>
        /// <typeparam name="TPixel">The type of the pixel.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The <see cref="Span{TPixel}"/></returns>
        public static Span<TPixel> GetPixelSpan<TPixel>(this ImageFrame<TPixel> source)
            where TPixel : struct, IPixel<TPixel>
            => source.GetPixelMemory().Span;

        /// <summary>
        /// Gets the representation of the pixels as a <see cref="Span{T}"/> of contiguous memory in the source image's pixel format
        /// stored in row major order.
        /// </summary>
        /// <typeparam name="TPixel">The type of the pixel.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The <see cref="Span{TPixel}"/></returns>
        public static Span<TPixel> GetPixelSpan<TPixel>(this Image<TPixel> source)
            where TPixel : struct, IPixel<TPixel>
            => source.Frames.RootFrame.GetPixelSpan();

        /// <summary>
        /// Gets the representation of the pixels as a <see cref="Span{T}"/> of contiguous memory
        /// at row <paramref name="rowIndex"/> beginning from the the first pixel on that row.
        /// </summary>
        /// <typeparam name="TPixel">The type of the pixel.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="rowIndex">The row.</param>
        /// <returns>The <see cref="Span{TPixel}"/></returns>
        public static Span<TPixel> GetPixelRowSpan<TPixel>(this ImageFrame<TPixel> source, int rowIndex)
            where TPixel : struct, IPixel<TPixel>
            => source.PixelBuffer.GetRowSpan(rowIndex);

        /// <summary>
        /// Gets the representation of the pixels as <see cref="Span{T}"/> of of contiguous memory
        /// at row <paramref name="rowIndex"/> beginning from the the first pixel on that row.
        /// </summary>
        /// <typeparam name="TPixel">The type of the pixel.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="rowIndex">The row.</param>
        /// <returns>The <see cref="Span{TPixel}"/></returns>
        public static Span<TPixel> GetPixelRowSpan<TPixel>(this Image<TPixel> source, int rowIndex)
            where TPixel : struct, IPixel<TPixel>
            => source.Frames.RootFrame.GetPixelRowSpan(rowIndex);

        /// <summary>
        /// Returns a reference to the 0th element of the Pixel buffer,
        /// allowing direct manipulation of pixel data through unsafe operations.
        /// The pixel buffer is a contiguous memory area containing Width*Height TPixel elements laid out in row-major order.
        /// </summary>
        /// <typeparam name="TPixel">The Pixel format.</typeparam>
        /// <param name="source">The source image frame</param>
        /// <returns>A pinnable reference the first root of the pixel buffer.</returns>
        [Obsolete("This method will be removed in our next release! Please use MemoryMarshal.GetReference(source.GetPixelSpan())!")]
        public static ref TPixel DangerousGetPinnableReferenceToPixelBuffer<TPixel>(this ImageFrame<TPixel> source)
            where TPixel : struct, IPixel<TPixel>
            => ref DangerousGetPinnableReferenceToPixelBuffer((IPixelSource<TPixel>)source);

        /// <summary>
        /// Returns a reference to the 0th element of the Pixel buffer,
        /// allowing direct manipulation of pixel data through unsafe operations.
        /// The pixel buffer is a contiguous memory area containing Width*Height TPixel elements laid out in row-major order.
        /// </summary>
        /// <typeparam name="TPixel">The Pixel format.</typeparam>
        /// <param name="source">The source image</param>
        /// <returns>A pinnable reference the first root of the pixel buffer.</returns>
        [Obsolete("This method will be removed in our next release! Please use MemoryMarshal.GetReference(source.GetPixelSpan())!")]
        public static ref TPixel DangerousGetPinnableReferenceToPixelBuffer<TPixel>(this Image<TPixel> source)
            where TPixel : struct, IPixel<TPixel>
            => ref source.Frames.RootFrame.DangerousGetPinnableReferenceToPixelBuffer();

        /// <summary>
        /// Gets the representation of the pixels as a <see cref="Memory{T}"/> of contiguous memory in the source image's pixel format
        /// stored in row major order.
        /// </summary>
        /// <typeparam name="TPixel">The Pixel format.</typeparam>
        /// <param name="source">The source <see cref="ImageFrame{TPixel}"/></param>
        /// <returns>The <see cref="Memory{T}"/></returns>
        internal static Memory<TPixel> GetPixelMemory<TPixel>(this ImageFrame<TPixel> source)
            where TPixel : struct, IPixel<TPixel>
        {
            return source.PixelBuffer.MemorySource.Memory;
        }

        /// <summary>
        /// Gets the representation of the pixels as a <see cref="Memory{T}"/> of contiguous memory in the source image's pixel format
        /// stored in row major order.
        /// </summary>
        /// <typeparam name="TPixel">The Pixel format.</typeparam>
        /// <param name="source">The source <see cref="Image{TPixel}"/></param>
        /// <returns>The <see cref="Memory{T}"/></returns>
        internal static Memory<TPixel> GetPixelMemory<TPixel>(this Image<TPixel> source)
            where TPixel : struct, IPixel<TPixel>
        {
            return source.Frames.RootFrame.GetPixelMemory();
        }

        /// <summary>
        /// Gets the representation of the pixels as a <see cref="Span{T}"/> of contiguous memory
        /// at row <paramref name="rowIndex"/> beginning from the the first pixel on that row.
        /// </summary>
        /// <typeparam name="TPixel">The type of the pixel.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="rowIndex">The row.</param>
        /// <returns>The <see cref="Span{TPixel}"/></returns>
        internal static Memory<TPixel> GetPixelRowMemory<TPixel>(this ImageFrame<TPixel> source, int rowIndex)
            where TPixel : struct, IPixel<TPixel>
            => source.PixelBuffer.GetRowMemory(rowIndex);

        /// <summary>
        /// Gets the representation of the pixels as <see cref="Span{T}"/> of of contiguous memory
        /// at row <paramref name="rowIndex"/> beginning from the the first pixel on that row.
        /// </summary>
        /// <typeparam name="TPixel">The type of the pixel.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="rowIndex">The row.</param>
        /// <returns>The <see cref="Span{TPixel}"/></returns>
        internal static Memory<TPixel> GetPixelRowMemory<TPixel>(this Image<TPixel> source, int rowIndex)
            where TPixel : struct, IPixel<TPixel>
            => source.Frames.RootFrame.GetPixelRowMemory(rowIndex);

        /// <summary>
        /// Gets the <see cref="MemoryAllocator"/> assigned to 'source'.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>Returns the configuration.</returns>
        internal static MemoryAllocator GetMemoryAllocator(this IConfigurationProvider source)
            => GetConfiguration(source).MemoryAllocator;

        /// <summary>
        /// Returns a reference to the 0th element of the Pixel buffer.
        /// Such a reference can be used for pinning but must never be dereferenced.
        /// </summary>
        /// <param name="source">The source image frame</param>
        /// <returns>A reference to the element.</returns>
        private static ref TPixel DangerousGetPinnableReferenceToPixelBuffer<TPixel>(IPixelSource<TPixel> source)
            where TPixel : struct, IPixel<TPixel>
            => ref MemoryMarshal.GetReference(source.PixelBuffer.GetSpan());
    }
}

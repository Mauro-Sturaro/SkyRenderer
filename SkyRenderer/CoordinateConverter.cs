﻿using System;

namespace SkyRenderer
{
    /// <summary>
    /// Converts between celestial coordinates (RA/Dec) and image coordinates (X/Y)
    /// using stereographic projection with rotation support
    /// </summary>
    public class CoordinateConverter
    {
        private readonly double raCenter;
        private readonly double decCenter;
        private readonly double width;
        private readonly double height;
        private readonly double scale;
        private readonly double fieldWidth;
        private readonly double fieldHeight;
        private readonly double rotationAngle; // Rotation angle in degrees

        private const double ArcsecondsPerDegree = 3600.0;
        private const double DegreesToRadians = Math.PI / 180.0;
        private const double RadiansToDegrees = 180.0 / Math.PI;
        private const double MaxFisheyeAngle = 90.0;

        /// <summary>
        /// Initializes a new coordinate converter
        /// </summary>
        /// <param name="raCenterDeg">Right Ascension of the image center in degrees</param>
        /// <param name="decCenterDeg">Declination of the image center in degrees</param>
        /// <param name="imageWidth">Image width in pixels</param>
        /// <param name="imageHeight">Image height in pixels</param>
        /// <param name="imageScale">Image scale in arcseconds per pixel</param>
        /// <param name="rotationDeg">Image rotation in degrees</param>
        public CoordinateConverter(double raCenterDeg, double decCenterDeg,
                                 double imageWidth, double imageHeight,
                                 double imageScale, double rotationDeg = 0.0)
        {
            raCenter = raCenterDeg;
            decCenter = decCenterDeg;
            width = imageWidth;
            height = imageHeight;
            scale = imageScale;
            rotationAngle = -rotationDeg; // image rotation is opposite to camera rotation
            fieldWidth = (width * scale) / ArcsecondsPerDegree;
            fieldHeight = (height * scale) / ArcsecondsPerDegree;
        }

        /// <summary>
        /// Converts celestial coordinates to image coordinates using stereographic projection
        /// </summary>
        /// <param name="raStar">Right Ascension of the star in degrees</param>
        /// <param name="decStar">Declination of the star in degrees</param>
        /// <returns>A tuple containing (x, y) pixel coordinates. Returns (NaN, NaN) if the point is outside the valid field</returns>
        public (double x, double y) ConvertRaDecToXY(double raStar, double decStar)
        {
            if (fieldWidth > MaxFisheyeAngle)
            {
                double angularDist = GetAngularDistance(raStar, decStar);
                if (angularDist > MaxFisheyeAngle)
                {
                    return (Double.NaN, Double.NaN);
                }
            }

            double ra1 = raStar * DegreesToRadians;
            double dec1 = decStar * DegreesToRadians;
            double ra0 = raCenter * DegreesToRadians;
            double dec0 = decCenter * DegreesToRadians;

            double sinDec0 = Math.Sin(dec0);
            double cosDec0 = Math.Cos(dec0);
            double sinDec1 = Math.Sin(dec1);
            double cosDec1 = Math.Cos(dec1);
            double cosRA = Math.Cos(ra1 - ra0);

            double cosDist = sinDec0 * sinDec1 + cosDec0 * cosDec1 * cosRA;
            double k = 2 / (1 + cosDist);

            // Calculate unrotated coordinates
            double x = k * cosDec1 * Math.Sin(ra1 - ra0);
            double y = k * (cosDec0 * sinDec1 - sinDec0 * cosDec1 * cosRA);

            // Apply rotation
            double rotRad = rotationAngle * DegreesToRadians;
            double cosRot = Math.Cos(rotRad);
            double sinRot = Math.Sin(rotRad);
            double xRot = x * cosRot - y * sinRot;
            double yRot = x * sinRot + y * cosRot;

            // Convert to arcseconds
            xRot *= RadiansToDegrees * ArcsecondsPerDegree;
            yRot *= RadiansToDegrees * ArcsecondsPerDegree;

            // Apply scale and center in image
            xRot = (-xRot / scale) + width / 2.0;
            yRot = (-yRot / scale) + height / 2.0;

            if (fieldWidth <= MaxFisheyeAngle)
            {
                if (xRot < 0 || xRot >= width || yRot < 0 || yRot >= height)
                {
                    return (Double.NaN, Double.NaN);
                }
            }

            return (xRot, yRot);
        }

        /// <summary>
        /// Calculates the angular distance between the center and a given point
        /// </summary>
        /// <param name="raStar">Right Ascension of the point in degrees</param>
        /// <param name="decStar">Declination of the point in degrees</param>
        /// <returns>Angular distance in degrees</returns>
        private double GetAngularDistance(double raStar, double decStar)
        {
            double ra1 = raStar * DegreesToRadians;
            double dec1 = decStar * DegreesToRadians;
            double ra0 = raCenter * DegreesToRadians;
            double dec0 = decCenter * DegreesToRadians;

            double cosDist = Math.Sin(dec0) * Math.Sin(dec1) +
                           Math.Cos(dec0) * Math.Cos(dec1) * Math.Cos(ra1 - ra0);

            return Math.Acos(Math.Max(-1.0, Math.Min(1.0, cosDist))) * RadiansToDegrees;
        }
    }
}
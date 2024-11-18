using System;
using System.Collections.Generic;
using System.Linq;

namespace SkyRenderer
{
    /// <summary>
    /// Calculates the sky region boundaries covered by an image based on its center position and size
    /// </summary>
    public class SkyImageBounds
    {
        // Threshold in degrees for special handling of polar regions
        private const double PolarThreshold = 88.0;

        /// <summary>
        /// Calculates the boundaries of the sky region covered by an image
        /// </summary>
        /// <param name="centerRA">Right Ascension of image center in degrees</param>
        /// <param name="centerDec">Declination of image center in degrees</param>
        /// <param name="scaleArcsecPerPixel">Image scale in arcseconds per pixel</param>
        /// <param name="widthPixels">Image width in pixels</param>
        /// <param name="heightPixels">Image height in pixels</param>
        /// <param name="rotationAngleDegrees">Image rotation in degrees</param>
        /// <returns>A BoundingBox defining the region covered by the image</returns>
        public static BoundingBox CalculateBounds(
            double centerRA,
            double centerDec,
            double scaleArcsecPerPixel,
            int widthPixels,
            int heightPixels,
            double rotationAngleDegrees)
        {
            // Convert dimensions to degrees
            double widthDegrees = (widthPixels * scaleArcsecPerPixel) / 3600.0;
            double heightDegrees = (heightPixels * scaleArcsecPerPixel) / 3600.0;

            // Check if we're near a pole
            bool nearPole = Math.Abs(centerDec) + Math.Max(widthDegrees, heightDegrees) / 2.0 > PolarThreshold;

            if (nearPole)
            {
                return CalculatePolarBounds(centerRA, centerDec, widthDegrees, heightDegrees, rotationAngleDegrees);
            }

            double rotationRad = rotationAngleDegrees * Math.PI / 180.0;
            double centerDecRad = centerDec * Math.PI / 180.0;

            double halfWidth = widthDegrees / 2.0;
            double halfHeight = heightDegrees / 2.0;

            var corners = new List<(double dRa, double dDec)>
            {
                CalculateOffset(-halfWidth, -halfHeight, rotationRad),
                CalculateOffset(-halfWidth, halfHeight, rotationRad),
                CalculateOffset(halfWidth, -halfHeight, rotationRad),
                CalculateOffset(halfWidth, halfHeight, rotationRad)
            };

            var raOffsets = new List<double>();
            var decOffsets = new List<double>();

            foreach (var (dRa, dDec) in corners)
            {
                double decAtCorner = centerDec + dDec;
                if (Math.Abs(decAtCorner) > 89.0)
                {
                    // If a corner reaches a pole, the field is circumpolar
                    return CalculatePolarBounds(centerRA, centerDec, widthDegrees, heightDegrees, rotationAngleDegrees);
                }

                // Use a more stable cosine correction
                double cosDecFactor = Math.Cos((centerDecRad + dDec * Math.PI / 180.0));
                if (Math.Abs(cosDecFactor) < 0.001)
                {
                    cosDecFactor = Math.Sign(cosDecFactor) * 0.001;
                }
                double raOffset = dRa / cosDecFactor;

                raOffsets.Add(raOffset);
                decOffsets.Add(dDec);
            }

            double decMin = Math.Max(-90.0, centerDec + decOffsets.Min());
            double decMax = Math.Min(90.0, centerDec + decOffsets.Max());

            double minRaOffset = raOffsets.Min();
            double maxRaOffset = raOffsets.Max();

            double raMin = (centerRA + minRaOffset + 360.0) % 360.0;
            double raMax = (centerRA + maxRaOffset + 360.0) % 360.0;

            bool crossesRA360 = Math.Abs(maxRaOffset - minRaOffset) > 180 || raMax < raMin;

            return new BoundingBox {
                RaMin = raMin,
                RaMax = raMax,
                DecMin = decMin,
                DecMax = decMax,
                CrossesRA360 = crossesRA360,
                IsCircumpolar = false
            };
        }

        /// <summary>
        /// Calculates bounds for fields near the celestial poles
        /// </summary>
        private static BoundingBox CalculatePolarBounds(
            double centerRA,
            double centerDec,
            double widthDegrees,
            double heightDegrees,
            double rotationAngleDegrees)
        {
            // For fields near poles, cover the entire RA range
            // and calculate only Dec limits
            double maxExtent = Math.Max(widthDegrees, heightDegrees) / 2.0;

            if (centerDec > 0)  // North Pole
            {
                return new BoundingBox {
                    RaMin = 0,
                    RaMax = 360,
                    DecMin = Math.Max(-90, centerDec - maxExtent),
                    DecMax = 90,
                    CrossesRA360 = true,
                    IsCircumpolar = true
                };
            }
            else  // South Pole
            {
                return new BoundingBox {
                    RaMin = 0,
                    RaMax = 360,
                    DecMin = -90,
                    DecMax = Math.Min(90, centerDec + maxExtent),
                    CrossesRA360 = true,
                    IsCircumpolar = true
                };
            }
        }

        /// <summary>
        /// Calculates coordinate offsets with rotation
        /// </summary>
        private static (double dRa, double dDec) CalculateOffset(
            double x,
            double y,
            double rotationRad)
        {
            double dRa = x * Math.Cos(rotationRad) - y * Math.Sin(rotationRad);
            double dDec = x * Math.Sin(rotationRad) + y * Math.Cos(rotationRad);
            return (dRa, dDec);
        }
    }
}
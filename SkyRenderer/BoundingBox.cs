namespace SkyRenderer
{
    /// <summary>
    /// Represents a rectangular region of the sky defined by RA and Dec coordinates
    /// </summary>
    public class BoundingBox
    {
        /// <summary>
        /// Gets or sets the minimum Right Ascension in degrees (0-360)
        /// </summary>
        public double RaMin { get; set; }

        /// <summary>
        /// Gets or sets the maximum Right Ascension in degrees (0-360)
        /// </summary>
        public double RaMax { get; set; }

        /// <summary>
        /// Gets or sets the minimum Declination in degrees (-90 to +90)
        /// </summary>
        public double DecMin { get; set; }

        /// <summary>
        /// Gets or sets the maximum Declination in degrees (-90 to +90)
        /// </summary>
        public double DecMax { get; set; }

        /// <summary>
        /// Gets or sets whether the region crosses the RA=0/360 boundary
        /// </summary>
        public bool CrossesRA360 { get; set; }

        /// <summary>
        /// Gets or sets whether the region includes a celestial pole
        /// </summary>
        public bool IsCircumpolar { get; set; }
    }
}
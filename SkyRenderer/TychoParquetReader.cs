using Parquet;
using Parquet.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SkyRenderer
{
    /// <summary>
    /// Reads star data from a Parquet file containing Tycho-2 catalog data.
    /// Provides filtered access to star records based on RA/Dec coordinates.
    /// </summary>
    public class TychoParquetReader : IDisposable
    {
        private readonly string parquetFilePath;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the TychoParquetReader.
        /// </summary>
        /// <param name="filePath">Path to the Parquet file containing Tycho-2 data</param>
        /// <exception cref="ArgumentNullException">Thrown when filePath is null or empty</exception>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist</exception>
        public TychoParquetReader(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Tycho database file not found", filePath);

            parquetFilePath = filePath;
        }

        /// <summary>
        /// Reads star records within the specified RA/Dec range.
        /// </summary>
        /// <param name="minRA">Minimum Right Ascension in degrees (0-360)</param>
        /// <param name="maxRA">Maximum Right Ascension in degrees (0-360)</param>
        /// <param name="minDEC">Minimum Declination in degrees (-90 to +90)</param>
        /// <param name="maxDEC">Maximum Declination in degrees (-90 to +90)</param>
        /// <returns>An async enumerable of StarRecords within the specified range</returns>
        public async IAsyncEnumerable<StarRecord> ReadFilteredData(
            double minRA,
            double maxRA,
            double minDEC,
            double maxDEC)
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(TychoParquetReader));

            using Stream fileStream = File.OpenRead(parquetFilePath);
            using var parquetReader = await ParquetReader.CreateAsync(fileStream);

            DataField[] dataFields = parquetReader.Schema.GetDataFields();
            if (dataFields.Length < 4)
                throw new InvalidDataException("Invalid Parquet file format: insufficient columns");

            var raField = dataFields[1];
            var deField = dataFields[2];

            for (int currentRowGroup = 0; currentRowGroup < parquetReader.RowGroupCount; currentRowGroup++)
            {
                await foreach (var record in ReadFromRowGroupAsync(
                    parquetReader, dataFields, currentRowGroup,
                    minRA, maxRA, minDEC, maxDEC))
                {
                    yield return record;
                }
            }
        }

        private async IAsyncEnumerable<StarRecord> ReadFromRowGroupAsync(
            ParquetReader parquetReader,
            DataField[] dataFields,
            int currentRowGroup,
            double minRA,
            double maxRA,
            double minDEC,
            double maxDEC)
        {

            using ParquetRowGroupReader groupReader = parquetReader.OpenRowGroupReader(currentRowGroup);

            var raStat = groupReader.GetStatistics(dataFields[0]);
            var deStat = groupReader.GetStatistics(dataFields[1]);

            if (raStat == null || deStat == null)
                yield break;

            var raMinValue = (double?)raStat.MinValue;
            var raMaxValue = (double?)raStat.MaxValue;
            var deMinValue = (double?)deStat.MinValue;
            var deMaxValue = (double?)deStat.MaxValue;

            if (!raMinValue.HasValue || !raMaxValue.HasValue ||
                !deMinValue.HasValue || !deMaxValue.HasValue)
                yield break;

            if (!Intersect(raMinValue.Value, raMaxValue.Value, minRA, maxRA) ||
                !Intersect(deMinValue.Value, deMaxValue.Value, minDEC, maxDEC))
                yield break;

            var raCol = await groupReader.ReadColumnAsync(dataFields[0]);
            var deCol = await groupReader.ReadColumnAsync(dataFields[1]);
            var btCol = await groupReader.ReadColumnAsync(dataFields[2]);
            var vtCol = await groupReader.ReadColumnAsync(dataFields[3]);

            var raData = (double?[])raCol.Data;
            var deData = (double?[])deCol.Data;
            var btData = (double?[])btCol.Data;
            var vtData = (double?[])vtCol.Data;

            for (int i = 0; i < raCol.NumValues; i++)
            {
               
                double ra = raData[i].Value;
                double de = deData[i].Value;

                if (Inside(ra, minRA, maxRA) && Inside(de, minDEC, maxDEC))
                {
                    // Use VT magnitude if available, otherwise BT magnitude, or 999.0 as fallback
                    var mag = vtData[i] ?? btData[i] ?? 999.0;
                    // Calculate B-V color index if both magnitudes are available
                    double bv = btData[i].HasValue && vtData[i].HasValue
                        ? btData[i]!.Value - vtData[i]!.Value
                        : 0;

                    yield return new StarRecord {
                        RAdeg = ra,
                        DEdeg = de,
                        Vmag = mag,
                        BV = bv
                    };
                }
            }
        }

        /// <summary>
        /// Checks if a given RA value is inside the specified range, handling 0/360 degree boundary.
        /// </summary>
        private static bool Inside(double ra, double minRA, double maxRA)
        {
            // Normalize values to 0-360 range
            ra = ((ra % 360) + 360) % 360;
            minRA = ((minRA % 360) + 360) % 360;
            maxRA = ((maxRA % 360) + 360) % 360;

            if (minRA == 0 && maxRA == 0)
                maxRA = 360;

            // Check if the range crosses the 0/360 meridian
            if (minRA > maxRA)
            {
                // If crossing meridian, point is inside if >= min OR <= max
                return ra >= minRA || ra <= maxRA;
            }
            else
            {
                // Normal case: point must be between min and max
                return ra >= minRA && ra <= maxRA;
            }
        }

        /// <summary>
        /// Checks if two ranges of coordinates intersect, handling 0/360 degree boundary.
        /// </summary>
        private static bool Intersect(double minValue, double maxValue, double minFilter, double maxFilter)
        {
            // Normalize values to 0-360 range
            double min = ((minValue % 360) + 360) % 360;
            double max = ((maxValue % 360) + 360) % 360;
            minFilter = ((minFilter % 360) + 360) % 360;
            maxFilter = ((maxFilter % 360) + 360) % 360;

            if (minFilter == 0 && maxFilter == 0)
                maxFilter = 360;

            bool rangeCrosses360 = min > max;
            bool filterCrosses360 = minFilter > maxFilter;

            if (filterCrosses360)
            {
                if (rangeCrosses360)
                {
                    // Both ranges cross 360
                    return true;
                }
                // Only filter crosses 360
                return (min <= maxFilter || max >= minFilter);
            }
            else if (rangeCrosses360)
            {
                // Only range crosses 360
                return (minFilter <= max || maxFilter >= min);
            }
            else
            {
                // Neither crosses 360
                return (max >= minFilter && min <= maxFilter);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
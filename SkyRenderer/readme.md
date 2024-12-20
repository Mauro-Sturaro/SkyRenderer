# SkyRenderer

SkyRenderer is a .NET library for generating synthetic astronomical images based on the Tycho-2 star catalog.  
It creates realistic star field images from a star database, offering a variety of configurable parameters.

The library also includes a class to retrieve real sky background images (via the CDS Hips2Fits service), primarily to support validation of the renderer's results.

For usage see project website

**Note:** The rendered images contain stars only; galaxies and nebulae are not included.

---

## Features

- Generate synthetic star field images with accurate star positions from the Tycho-2 catalog.
- Support for real sky background images from DSS2 via the CDS Hips2Fits service.
- Configurable image parameters:
  - Field of view and image scale
  - Image dimensions
  - Field rotation
  - Star rendering options (simple or Moffat profile)
  - Star coloring based on B-V index
- High-performance data filtering for large catalogs.
- Supports both synthetic and real background images.
- Image rendering times range from 50 to 800 ms, depending on FOV, image size, and hardware.
- Low memory footprint.
- Rendered images are compatible with plate-solving tools (verified with Astrometry.net and ASTAP).

---

### Dependencies

- `Parquet.Net` (5.0.2)
- `SixLabors.ImageSharp` (3.1.5)
- `SixLabors.ImageSharp.Drawing` (2.1.4)

---

## Star Data

The library includes a star catalog in Parquet format derived from the Tycho-2 catalog (I/259).  
It is a merged dataset combining the Tycho-2 catalog with its two supplements and contains star positions and magnitudes in the B and V channels.

The original data source is available at: [https://cdsarc.cds.unistra.fr/viz-bin/cat/I/259](https://cdsarc.cds.unistra.fr/viz-bin/cat/I/259)

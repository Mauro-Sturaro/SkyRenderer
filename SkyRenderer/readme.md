# SkyRenderer

SkyRenderer is a .NET library for generating synthetic astronomical images based on the Tycho-2 star catalog.  
It creates realistic star field images from a star database, offering a variety of configurable parameters.

The library also includes a class to retrieve real sky background images (via the CDS Hips2Fits service), primarily to support validation of the renderer's results.

The included `SkyRendererTest` application is a WinForms-based tool designed to test the library and demonstrate its usage and capabilities.

For usage see project website

**Note:** The rendered images contain stars only; galaxies and nebulae are not included.


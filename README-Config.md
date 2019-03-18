# Configuration file
You can use the `config.ini` file located at the root folder of the project / release folder to configure and tweak your game experience.
The first step is to rename the `config.ini.dist` file to `config.ini`.


| Parameter | Values |
|-----------|---------|
| **Global** | |
| PlayMusic  | `True` or `False` |
| MorrowindPath | The Morrowind's `Data Files` path |
| CellRadius | How number of cell to load |
| CellDetailRadius | Detail Cell Radius |
| CellRadiusOnLoad | How many cell load on load |
| **Lighting**| |
| AnimateLights  | `True` or `False` |
| SunShadows  | `True` or `False` |
| LightShadows  | `True` or `False` |
| RenderExteriorCellLights | `True` or `False` |
| DayNightCycle | `True` or `False` |
| GenerateNormalMap | `True` or `False` |
| NormalGeneratorIntensity | A value from 0.1 to 1.0 |
| **Effects** | |
| AntiAliasing |  A value from 0 to 3 (0 is disabled) | 
| PostProcessQuality | A value from 0 to 3 (0 is disabled) | 
| WaterBackSideTransparent | `True` or `False` |
| **Rendering** | |
| Shader  | `PBR` or `Simple` or `Unlit` or `Default` |
| RenderPath  | `Forward` / `Deferred` / `Lightwight` |
| CameraFarClip | a value from 10 to 10000 |
| WaterQuality | a value from 0 to 2 |
| SRPQuality | a value from 0 to 2 |
| RenderScale | A value from 0.1 to 2.0 | 
| **VR** | |
| FollowHeadDirection | `True` or `False` |
| RoomScale | `True` or `False` |
| ForceControllers | `True` or `False` |
| XRVignette | `True` or `False` |
| **Debug** | |
| CreaturesEnabled | `True` or `False` |